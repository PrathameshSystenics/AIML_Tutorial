using ProductClassification.Postgres;
using ProductClassification.Data;
using ProductClassification.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

// Adding the Console Logging.
builder.Logging.AddConsole();

// Adding the Session 
builder.Services.AddMvc();
builder.Services.AddSession();

// Adding the DB Support
builder.AddNpgsqlDbContext<ApplicationDBContext>("promptevaldb", configureDbContextOptions: (options) =>
{
    options.UseNpgsql(sqloptions =>
    {
        sqloptions.MigrationsAssembly("ProductClassification.Postgres");
    });
});
builder.AddServicesRequiredForAI();


// Injecting the Required Service
builder.Services.AddScoped<EvaluationDataRepository>();
builder.AddVectorDbSupport(builder.Configuration);
builder.AddKernelPlugins();

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

//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();