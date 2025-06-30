using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Polly;
using Polly.Retry;

Console.WriteLine("==== Running Semantic Kernel Models Using Github Models ========");

#region Configuring the AppSettings to load
string path = "D:\\AIML_Tutorial\\SemanticKernelTut\\SemanticKernelGithubModels\\appsettings.json";
IConfiguration config = new ConfigurationBuilder().AddJsonFile(path).Build();
#endregion

# region Creating Semantic Kernel Building
IConfigurationSection section = config.GetRequiredSection("GithubModels");
IKernelBuilder kernelbuilder = Kernel.CreateBuilder();

kernelbuilder.AddOpenAIChatCompletion(
    modelId: section.GetRequiredSection("ModelName").Value.ToString(),
    endpoint: new Uri(section.GetRequiredSection("URI").Value.ToString()),
    apiKey: section.GetRequiredSection("GHToken").Value.ToString()
);

Kernel kernel = kernelbuilder.Build();
#endregion

#region Processing the Request with GithubModels
FunctionResult result = await kernel.InvokePromptAsync("Hello How are you?");
Console.WriteLine(result);
#endregion

#region Using Polly for Retrying if Fails
ResiliencePipeline pipeline = new ResiliencePipelineBuilder()
    .AddRetry(new RetryStrategyOptions()
    {
        Delay = TimeSpan.FromMinutes(1),
        MaxRetryAttempts = 2,
        OnRetry = static args =>
        {
            Console.WriteLine("OnRetry, Attempt: {0}", args.AttemptNumber);
            return default;
        }
    })
    .Build();

await pipeline.ExecuteAsync(async (token) =>
{
    FunctionResult result2 = await kernel.InvokePromptAsync("What is Your name and What is Your Model");
    Console.WriteLine(result2);
});
#endregion