

using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using NpgsqlTypes;
using Payment.Application;
using Payment.Application.Middleware.Exceptions;
using Payment.Persistence;
using Payment.Persistence.Context;
using Payment.Persistence.Extensions;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;

var builder = WebApplication.CreateBuilder(args);


var env = builder.Environment;

builder.Configuration
    .SetBasePath(env.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

var connStr = builder.Configuration["ConnectionStrings:DefaultConnection"];
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(connStr, npg => npg.MigrationsAssembly("Payment.Persistence")));

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
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Payment API", Version = "v1" });
    c.CustomSchemaIds(t => t.FullName);
});

builder.Services.AddAutoMapper(_ => { }, AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "live" });
builder.Services.AddAuthorization();
builder.Host.UseSerilog(); 

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

// Configure the HTTP request pipeline.

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
app.UseSwagger();                                    
app.UseSwaggerUI(c =>                                
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Payment API v1");

});

app.ConfigureExceptionHandlingMiddleware();
app.MapHealthChecks("/live",  new() { Predicate = r => r.Tags.Contains("live") });
app.MapHealthChecks("/ready", new() { Predicate = r => r.Tags.Count == 0 || r.Tags.Contains("ready") });
app.MapGet("/health", () => Results.Text("Healthy")).WithName("Health").WithOpenApi();
app.MapControllers(); 
app.UseSerilogRequestLogging(); 
app.Run();