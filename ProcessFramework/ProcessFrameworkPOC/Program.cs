using Microsoft.Extensions.Configuration;
using ProcessFrameworkPOC;
using ProcessFrameworkPOC.Models;

#region Configuration Manager - JSON
string path = "D:\\Training\\AIML\\ProcessFramework\\ProcessFrameworkPOC\\appsettings.json";
IConfiguration config = new ConfigurationBuilder().AddJsonFile(path).Build();
#endregion

Console.WriteLine("Blog Writer Assistant");

//Console.WriteLine("Enter the Title of the Blog:");
//string? title = Console.ReadLine();

//Console.WriteLine("Enter the Summary/Description of the Blog:");
//string? desc = Console.ReadLine();

UserInputs inputs = new UserInputs()
{
    Title = "Getting Started with Semantic Kernel",
    Description = "How to use AzureOpenAI Connector with Semantic Kernel how to invoke the message give complete code",
    Tone = Tone.Informative,
    Keywords = new string[] { "Semantic Kernel", "LLM", "C#","AzureOpenAI" },
    Creativity = 1.5f,
    DesiredLength = DesiredLength.Medium
};

BlogGeneratorProcess blogGeneratorProcess = new BlogGeneratorProcess(config);
await blogGeneratorProcess.RunProcessAsync(inputs);