using System.Diagnostics;
using System.Text.Json;
using Basket.Application;
using Basket.Application.Middleware.Exceptions;
using Basket.Persistence;
using HealthChecks.UI.Client;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Serilog.Context;
using Serilog.Events;
using Serilog.Debugging;
using Serilog.Sinks.PostgreSQL;
using NpgsqlTypes;
using Serilog;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;


var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables(); 



builder.Services.AddHealthChecks()
    .AddCheck("basket-self", () => HealthCheckResult.Healthy());


if (builder.Environment.IsDevelopment())
{
    SelfLog.Enable(Console.Error);
    AppDomain.CurrentDomain.ProcessExit += (_, __) => SelfLog.Disable();
}

var columns = new Dictionary<string, ColumnWriterBase>
{
    ["timestamp"]        = new TimestampColumnWriter(),
    ["level"]            = new LevelColumnWriter(renderAsText: true, NpgsqlDbType.Varchar),
    ["message"]          = new RenderedMessageColumnWriter(),
    ["message_template"] = new MessageTemplateColumnWriter(),
    ["exception"]        = new ExceptionColumnWriter(),
    ["properties"]       = new LogEventSerializedColumnWriter(NpgsqlDbType.Jsonb),


    ["source_context"]   = new SinglePropertyColumnWriter("SourceContext", PropertyWriteMethod.ToString, NpgsqlDbType.Text),
    ["trace_id"]         = new SinglePropertyColumnWriter("TraceId",      PropertyWriteMethod.ToString, NpgsqlDbType.Text),
    ["user_id"]          = new SinglePropertyColumnWriter("UserId",       PropertyWriteMethod.ToString, NpgsqlDbType.Text),
};

// Serilog host entegrasyonu
builder.Host.UseSerilog((ctx, sp, cfg) =>
{
    cfg.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
      .Enrich.FromLogContext()
      .Enrich.WithProperty("Application", ctx.HostingEnvironment.ApplicationName)
      .Enrich.WithProperty("Environment", ctx.HostingEnvironment.EnvironmentName)
      .WriteTo.Console();

    var logsCs = ctx.Configuration.GetConnectionString("LogsDb");
    if (!string.IsNullOrWhiteSpace(logsCs))
    {
        cfg.WriteTo.Async(a => a.PostgreSQL(
            connectionString: logsCs,
            tableName: "logs",
            columnOptions: columns,
            needAutoCreateTable: true
        ));
    }
});

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables(); // <-- ÖNEMLİ: RabbitMQ__Host -> RabbitMQ:Host

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Basket API",
        Version = "v1",
        Description = "Basket microservice"
    });
});

if (builder.Configuration is IConfigurationRoot root)
{
    Console.WriteLine("=== CONFIG DEBUG VIEW ===");
    Console.WriteLine(root.GetDebugView()); // .NET 6+ mevcut
    Console.WriteLine("=========================");
}

builder.Services.AddAutoMapper(_ => { }, AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddApplication(builder.Configuration);
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddAuthorization();

var cfgRabbit = builder.Configuration.GetSection("RabbitMQ");
Console.WriteLine($"RabbitMQ Host={cfgRabbit["Host"]}, Port={cfgRabbit["Port"]}, User={cfgRabbit["Username"]}, VHost={cfgRabbit["VirtualHost"]}");

var app = builder.Build();



app.UseSwagger();
app.UseSwaggerUI(o =>
{
    o.SwaggerEndpoint("/swagger/v1/swagger.json", "Basket API v1");
    o.RoutePrefix = "swagger";
});

app.UseSerilogRequestLogging(opts =>
{
    opts.EnrichDiagnosticContext = (diag, http) =>
    {
        var traceId = Activity.Current?.Id ?? http.TraceIdentifier;
        diag.Set("TraceId", traceId);
        var userId = http.User?.FindFirst("Id")?.Value;
        if (!string.IsNullOrWhiteSpace(userId)) diag.Set("UserId", userId);
    };
});





app.ConfigureExceptionHandlingMiddleware();

app.UseHttpsRedirection(); 
app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    var claim = context.User?.Identities?.Select(x => x.FindFirst("Id"))?.FirstOrDefault();
    if (claim is not null)
    {
        using (LogContext.PushProperty("UserId", claim.Value))
            await next();
    }
    else
    {
        await next();
    }
});

app.MapGet("/", () => Results.NoContent());

app.MapControllers();

app.MapHealthChecks("/health/self", new HealthCheckOptions
{
    Predicate = r => r.Name == "basket-self",
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status200OK
    },
    ResponseWriter = async (ctx, rpt) =>
    {
        ctx.Response.ContentType = "text/plain";
        ctx.Response.Headers["Cache-Control"] = "no-store, no-cache";
        await ctx.Response.WriteAsync("Healthy");
    }
});


app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status503ServiceUnavailable,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    },
    ResponseWriter = async (ctx, report) =>
    {
        ctx.Response.ContentType = "application/json";
        ctx.Response.Headers["Cache-Control"] = "no-store, no-cache";

        var payload = new
        {
            status = report.Status.ToString(),
            entries = report.Entries.ToDictionary(
                e => e.Key,
                e => new
                {
                    status = e.Value.Status.ToString(),
                    description = e.Value.Description,
                    data = e.Value.Data,
                    error = e.Value.Exception?.Message
                })
        };

        await ctx.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }
});
app.MapHealthChecks("/hc", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.Run();
