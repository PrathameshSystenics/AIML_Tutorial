using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.Google;
using ModelContextProtocol.Client;
using ModelContextProtocol.Configuration;
using ModelContextProtocol.Protocol.Transport;
using ModelContextProtocol.Protocol.Types;
using System.Text;

string settingspath = "D:\\Training\\AIML\\MCPTut\\MCPTutorial.SemanticKernelIntegrations\\appsettings.json";
IConfiguration config = new ConfigurationManager().AddJsonFile(settingspath).Build();


#region MCP: Client
HttpClient httpClient = new()
{
    BaseAddress = new Uri(config["MCPServerUrl"]!)
};

McpServerConfig serverconfig = new()
{
    Id = "Sample",
    Name = "SampleSSEServer",
    TransportType = TransportTypes.Sse,
    Location = httpClient.BaseAddress.ToString()
};

McpClientOptions clientoptions = new()
{
    ClientInfo = new Implementation() { Name = "Tool Client", Version = "1.0.0" }
};

IMcpClient mcpclient = McpClientFactory.CreateAsync(serverconfig, clientoptions).ConfigureAwait(ConfigureAwaitOptions.ContinueOnCapturedContext).GetAwaiter().GetResult();
#endregion


#region Kernel: Connector Gemini Model And Azure OpenAI Model
IKernelBuilder kernelbuilder = Kernel.CreateBuilder();
kernelbuilder.AddGoogleAIGeminiChatCompletion(
    modelId: config["GeminiModel:Model"]!,
    apiKey: config["GeminiModel:ApiKey"]!,
    serviceId: "geminimodel"
);

kernelbuilder.AddAzureOpenAIChatCompletion(
    deploymentName: config["AzureOpenAI:ModelName"]!,
    endpoint: config["AzureOpenAI:Endpoint"]!,
    apiKey: config["AzureOpenAI:ApiKey"]!,
    modelId: config["AzureOpenAI:ModelVersion"]!,
    serviceId: "azureopenaimodel"
);

#region Tools from the MCP Server
IList<AIFunction> aifunctions = await mcpclient.GetAIFunctionsAsync(new CancellationTokenSource().Token);
kernelbuilder.Plugins.AddFromFunctions("MCPTools", aifunctions.Select(func => func.AsKernelFunction()));
#endregion

Kernel kernel = kernelbuilder.Build();

string systemprompt = @"""You are a helpful assistant bot. 
    Helps the user to solve the problem.
    Keep The conversation short and precise.
    Don't use any vulgar language or simple chit chat.
    """;

ChatHistory chathistory = new ChatHistory(systemprompt);
Console.WriteLine("Helpful Assistant");
while (true)
{
    IChatCompletionService chatcompletionservice = kernel.GetRequiredService<IChatCompletionService>(serviceKey: "azureopenaimodel");

    Console.Write("User : ");
    string userinput = Console.ReadLine()!;
    chathistory.AddUserMessage(userinput);

    /*    GeminiPromptExecutionSettings promptexecutionsettings = new GeminiPromptExecutionSettings()
        {
            ToolCallBehavior = GeminiToolCallBehavior.AutoInvokeKernelFunctions,
            FunctionChoiceBehavior=FunctionChoiceBehavior.Auto(),
            ServiceId = "geminimodel"
        };*/

    AzureOpenAIPromptExecutionSettings promptexecutionsettings = new AzureOpenAIPromptExecutionSettings()
    {
        ToolCallBehavior = Microsoft.SemanticKernel.Connectors.OpenAI.ToolCallBehavior.AutoInvokeKernelFunctions,
        ServiceId = "azureopenaimodel"
    };


    StringBuilder stringbuilder = new StringBuilder();
    Console.Write("Assistant : ");
    await foreach (StreamingChatMessageContent chatcontent in chatcompletionservice.GetStreamingChatMessageContentsAsync(chathistory, promptexecutionsettings, kernel))
    {
        Console.Write(chatcontent.Content);
        stringbuilder.Append(chatcontent.Content);

    }
    Console.WriteLine();
    chathistory.AddAssistantMessage(stringbuilder.ToString());
}

#endregion