using Microsoft.EntityFrameworkCore;
using Products.Data;
using Products.Endpoints;


var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();


// Add services to the container.
builder.Services.AddSingleton<RandomFailureMiddleware>();

// Give the Name for adding the support of the Postgres
builder.AddNpgsqlDbContext<ProductDataContext>("productcontext");

// Add services to the container.
var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseMiddleware<RandomFailureMiddleware>();

app.MapProductEndpoints();

app.UseStaticFiles();

app.Run();


public class RandomFailureMiddleware : IMiddleware
{
    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var path = context.Request.Path.Value;

        if (path is null || !path.Contains("api/Product", StringComparison.InvariantCultureIgnoreCase))
            return next(context);

        if (Random.Shared.NextDouble() >= 0.6)
        {
            throw new Exception("Computer says no.");
        }
        return next(context);
    }
}
