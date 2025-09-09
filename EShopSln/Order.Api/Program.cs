using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using NpgsqlTypes;
using Order.Application;
using Order.Application.Middleware.Exceptions;
using Order.Infrastructure;
using Order.Infrastructure.Context;
using Order.Infrastructure.Extensions;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Order API", Version = "v1" }); // Payment'ta "Payment API"
    // 500 alan projelerde en çok çakışma buradan geliyor:
    c.CustomSchemaIds(t => t.FullName); // Aynı isimli tip çakışmalarını önler
    // Eğer XML yorumlarını ekliyorsan dosya yoksa 500 verir; emin değilsen ekleme.
    // var xml = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
    // c.IncludeXmlComments(xml, includeControllerXmlComments: true);
});

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

builder.Services.AddTransient<ExceptionMiddleware>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Order API", Version = "v1" });
    c.CustomSchemaIds(t => t.FullName);   // <— ÇAKIŞMAYI KESER
    // Eğer XML comments ekliyorsan dosya yoksa 500 verir; emin değilsen kaldır:
    // var xml = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
    // c.IncludeXmlComments(xml, true);
});

var env = builder.Environment;

builder.Configuration
    .SetBasePath(env.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

builder.Services.AddAutoMapper(_ => { }, AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "live" });
builder.Services.AddAuthorization();


var app = builder.Build();

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


app.MapHealthChecks("/health/live",  new HealthCheckOptions { Predicate = _ => false });
app.MapHealthChecks("/health/ready");
app.MapHealthChecks("/health");


app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order API v1");
});


app.ConfigureExceptionHandlingMiddleware();
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
app.MapGet("/health", () => Results.Text("Healthy"))
    .WithName("Health")
    .WithOpenApi(); 
app.MapHealthChecks("/live",  new() { Predicate = r => r.Tags.Contains("live") });
app.MapHealthChecks("/ready", new() { Predicate = r => r.Tags.Count == 0 || r.Tags.Contains("ready") });
app.MapControllers();
app.Run();
