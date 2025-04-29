using DaprTutorial.Hubs;
using Microsoft.SemanticKernel;
using ProcessFramework.Steps;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddMvc().AddDapr();
builder.Services.AddSignalR();

builder.Services.AddKernel()
                .AddGoogleAIGeminiChatCompletion(
                     modelId: builder.Configuration["GeminiModel:ModelName"]!,
                     apiKey: builder.Configuration["GeminiModel:ApiKey"]!
                );

// Adding the Steps in the Services for accessing the DaprClient
builder.Services.AddTransient<GatherInformationStep>();
builder.Services.AddTransient<GenerateDocumentationStep>();
builder.Services.AddTransient<PublishDocumentationStep>();

// Adding the Process Steps into the Dapr Actors
builder.Services.AddActors(static configure =>
{
    configure.AddProcessActors();
});

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
}

app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllers();
app.MapActorsHandlers();
app.MapHub<StepDataHub>("/stepdatahub");

app.Run();
