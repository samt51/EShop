using System.Diagnostics;
using Catalog.Application;
using Catalog.Application.Middleware.Exceptions;
using Catalog.Persistence;
using Catalog.Persistence.Context;
using Catalog.Persistence.Extensions;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using Serilog.Debugging;
using Serilog.Sinks.PostgreSQL;
using NpgsqlTypes;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables(); //

Console.WriteLine($"RabbitMq:Host = {builder.Configuration["RabbitMq:Host"]}");
Console.WriteLine($"RabbitMQ:Host = {builder.Configuration["RabbitMQ:Host"]}");

builder.Services.AddOutputCache(o =>
{
    o.AddPolicy("Departments", p => p
        .Expire(TimeSpan.FromMinutes(30))
        .SetVaryByHeader("Accept-Language"));
});

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
    .AddEnvironmentVariables(); 


builder.Services
    .AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "live" });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Catalog API",
        Version = "v1",
        Description = "Catalog microservice"
    });
});

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("GatewayOnly", p => 
        p.WithOrigins("http://localhost:5187")
            .AllowAnyHeader()
            .AllowAnyMethod());
});



builder.Services.AddAutoMapper(_ => { }, AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddApplication(builder.Configuration);
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddAuthorization();



var app = builder.Build();


    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog API v1");
  
    });

    await app.MigrateDevAndSeedAsync<AppDbContext>(async (db, sp) =>
    {
        await HostingExtensions.DevSeeder.SeedAsync(db);
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
    app.UseCors("GatewayOnly");
    app.UseOutputCache();
    app.MapControllers();
    app.MapGet("/health", () => Results.Text("Healthy")).WithName("Health").WithOpenApi();
    app.MapHealthChecks("/live",  new HealthCheckOptions { Predicate = r => r.Name == "self" });
    app.MapHealthChecks("/ready", new HealthCheckOptions { Predicate = _ => true });
    app.MapHealthChecks("/healthz", new HealthCheckOptions { ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse });
    app.Run("http://0.0.0.0:5018");
