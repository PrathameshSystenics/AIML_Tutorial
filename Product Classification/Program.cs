using Microsoft.EntityFrameworkCore;
using ProductClassification.Data;
using ProductClassification.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Adding the Console Logging.
builder.Logging.AddConsole();

// Adding the Session 
builder.Services.AddMvc();
builder.Services.AddSession();

// Injecting the Required Service
builder.Services.AddScoped<AIConnectorService>();
builder.Services.AddScoped<ClassificationService>();

// Adding the DB Support
builder.AddNpgsqlDbContext<ApplicationDBContext>("promptevaldb");

builder.Services.AddScoped<EvaluationDataRepository>();
builder.Services.AddScoped<EvaluationService>();

var app = builder.Build();

app.MapDefaultEndpoints();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();

}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();