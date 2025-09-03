
using Catalog.Application;
using Catalog.Application.Middleware.Exceptions;
using Catalog.Persistence;
using Catalog.Persistence.Context;
using Catalog.Persistence.Extensions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

// Taranacak assembly'leri belirle
var mapperAssemblies = new[]
{
    typeof(Catalog.Persistence.Concrete.Repositories.CatalogProfile).Assembly,
    typeof(Program).Assembly
};

// Overload: (Action<IMapperConfigurationExpression>, params Assembly[])
builder.Services.AddAutoMapper(cfg => { /* opsiyonel global ayarlar */ }, mapperAssemblies);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddHealthChecks();  // <— BUNU EKLE
builder.Services.AddControllers();
var env = builder.Environment;

builder.Configuration
    .SetBasePath(env.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
var app = builder.Build();

// Dev'te otomatik migrate + dev-only seed
await app.MigrateDevAndSeedAsync<AppDbContext>(async (db, sp) =>
{
    await HostingExtensions.DevSeeder.SeedAsync(db);
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Health endpoints (mapping Program.cs’te)
app.MapHealthChecks("/health/live",  new HealthCheckOptions { Predicate = _ => false });
app.MapHealthChecks("/health/ready"); // AddNpgSql kayıtlıysa ready çalışır

app.ConfigureExceptionHandlingMiddleware();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();

