using Microsoft.EntityFrameworkCore;
using ProductClassification.Data;
using ProductClassification.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Adding the Console Logging.
builder.Logging.AddConsole();

// Adding the Session 
builder.Services.AddMvc();
builder.Services.AddSession();

// Injecting the Required Service
builder.Services.AddSingleton<AIConnectorService>();
builder.Services.AddSingleton<ClassificationService>();

// Adding the DB Support
builder.Services.AddDbContext<ApplicationDBContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<EvaluationDataRepository>();
builder.Services.AddScoped<EvaluationService>();

var app = builder.Build();

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

using (var scope = app.Services.CreateScope())
{
    var dbcontext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
    // seed the data
    DBInitializer.Initialize(dbcontext);
}


app.Run();