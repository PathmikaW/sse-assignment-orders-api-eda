using FluentValidation;
using FluentValidation.AspNetCore;
using OrderManagement.Api.Middleware;
using OrderManagement.Application;
using OrderManagement.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services from Application and Infrastructure layers
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();

// Add controllers
builder.Services.AddControllers();

// Add FluentValidation auto-validation
builder.Services.AddFluentValidationAutoValidation();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Order Management API",
        Version = "v1",
        Description = "ASP.NET Core Web API for managing orders with event-driven architecture"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Order Management API v1");
    });
}

// Add global exception handling middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

// Make Program class accessible for integration tests
public partial class Program { }
