using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Microsoft.OpenApi.Models;
using MMLib.SwaggerForOcelot;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "EShop Gateway", Version = "v1" });
});

builder.Services.AddOcelot(builder.Configuration);
builder.Services.AddSwaggerForOcelot(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Birleşik UI => http://localhost:5187/swagger
    app.UseSwaggerForOcelotUI(opt =>
    {
        opt.PathToSwaggerGenerator = "/swagger/docs";
        // İstersen: opt.RoutePrefix = "swagger"; // default zaten "swagger"
    });

    // Gateway’in kendi dokümanı => http://localhost:5187/gateway/swagger
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "EShop Gateway v1");
        c.RoutePrefix = "gateway/swagger";
    });

    app.UseDeveloperExceptionPage();
}

await app.UseOcelot();
app.Run();