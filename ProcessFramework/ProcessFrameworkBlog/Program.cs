using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;

// A Process Step to Gather the Information Related to Product based on its Product name
public class ResearchStep : KernelProcessStep
{
    [KernelFunction]
    public string ResearchRegardingProduct(string productname)
    {
        Console.WriteLine($"[{nameof(ResearchStep)}]:\tGathering product information for product named {productname}");

        // Here we have a hardcoded response for the product information. You can use the AI Agent to actually do these.
        return
            """
            Product Description:
            GlowBrew is a revolutionary AI driven coffee machine with industry leading number of LEDs and programmable light shows. The machine is also capable of brewing coffee and has a built in grinder.

            Product Features:
            1. **Luminous Brew Technology**: Customize your morning ambiance with programmable LED lights that sync with your brewing process.
            2. **AI Taste Assistant**: Learns your taste preferences over time and suggests new brew combinations to explore.
            3. **Gourmet Aroma Diffusion**: Built-in aroma diffusers enhance your coffee's scent profile, energizing your senses before the first sip.

            Troubleshooting:
            - **Issue**: LED Lights Malfunctioning
                - **Solution**: Reset the lighting settings via the app. Ensure the LED connections inside the GlowBrew are secure. Perform a factory reset if necessary.
            """;
    }
}

// A process Step to generate the blog based on the information gathered while preserving the State.
public class ContentGenerationStep : KernelProcessStep<GenerateDocumentState>
{
    // Initializing the State for the First time
    private GenerateDocumentState _state = new();

    private string SystemPrompt = """
        Your job is to write high quality and engaging user blog. You will be provide with information about the product in the form of internal documentation, specs, and troubleshooting guides and you must use this information and nothing else to generate the blog. If suggestions are provided on the blog you create, take the suggestions into account rewrite the blob. Make sure the product sounds amazing.
        """;

    // Called by the process runtime when the step instance is activated. Use this to load state that may be persisted from previous activations.
    public override ValueTask ActivateAsync(KernelProcessStepState<GenerateDocumentState> state)
    {
        this._state = state.State!;
        this._state.ChatHistory ??= new ChatHistory(SystemPrompt);
        return base.ActivateAsync(state);
    }

    [KernelFunction]
    public async Task GenerateContentAsync(string productinfo, Kernel kernel, KernelProcessStepContext context)
    {
        Console.WriteLine($"[{nameof(ContentGenerationStep)}]:\tGenerating blog for provided productInfo...");

        // Add the new product info to the chat history
        this._state.ChatHistory!.AddUserMessage($"Product Info:\n{productinfo}");

        // Get a response from the LLM
        IChatCompletionService chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

        GeminiPromptExecutionSettings promptexec = new GeminiPromptExecutionSettings()
        {
            MaxTokens = 5000
        };

        var generatedDocumentationResponse = await chatCompletionService.GetChatMessageContentAsync(this._state.ChatHistory!,promptexec);

        DocumentInfo generatedContent = new()
        {
            Id = Guid.NewGuid().ToString(),
            Title = $"Generated document",
            Content = generatedDocumentationResponse.Content!,
        };

        // Adding the Message to the State
        this._state!.LastGeneratedDocument = generatedContent;

        // Emiting the Event
        await context.EmitEventAsync("ContentGenerated", generatedContent);
    }
}

// A State to maintain in the Step
public class GenerateDocumentState
{
    public ChatHistory ChatHistory { get; set; }
    public DocumentInfo? LastGeneratedDocument { get; set; }
}

public class DocumentInfo
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

// A process step to publish the generated blog
public class PublishStep : KernelProcessStep
{
    [KernelFunction]
    public DocumentInfo PublishDocs(DocumentInfo documentinfo)
    {
        // For example purposes we just write the generated docs to the console
        Console.WriteLine($"[{nameof(PublishStep)}]:\tPublishing Blog");
        Console.WriteLine(documentinfo.Content);
        return documentinfo;
    }
}

public partial class Program
{
    public async static Task Main(string[] args)
    {
        IConfiguration config = new ConfigurationManager().AddJsonFile("D:\\Training\\AIML\\ProcessFramework\\ProcessFrameworkBlog\\appsettings.json").Build();

        // Create a New Kernel Instance
        Kernel kernel = Kernel.CreateBuilder().AddGoogleAIGeminiChatCompletion(
                   apiKey: config["GeminiModel:ApiKey"]!,
                   modelId: config["GeminiModel:ModelName"]!
                   ).Build();

        // Create the Process Builder
        ProcessBuilder processbuilder = new ProcessBuilder("automaticblog");

        // Add the Steps in the Process builder
        ProcessStepBuilder researchstep = processbuilder.AddStepFromType<ResearchStep>();
        ProcessStepBuilder contentgenerationstep = processbuilder.AddStepFromType<ContentGenerationStep>();
        ProcessStepBuilder publishstep = processbuilder.AddStepFromType<PublishStep>();

        // Add the Flow to the Steps in the Process builder
        processbuilder.OnInputEvent("start").SendEventTo(new ProcessFunctionTargetBuilder(researchstep));
        researchstep.OnFunctionResult().SendEventTo(new ProcessFunctionTargetBuilder(contentgenerationstep));
        contentgenerationstep.OnEvent("ContentGenerated").SendEventTo(new ProcessFunctionTargetBuilder(publishstep));
        publishstep.OnFunctionResult().StopProcess();

        // Building the Process Builder
        KernelProcess process = processbuilder.Build();

        // Starting the Process
        await process.StartAsync(kernel, new KernelProcessEvent() { Data = "GlowBrew", Id = "start" });

    }
}

