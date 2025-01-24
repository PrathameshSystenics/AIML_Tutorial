// reference the required package
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;

public class GettingStarted
{
    public async void GettingStarted_SemanticKernel()
    {
        #region Getting Started Usage - Creating the Kernel and invoking the prompt

        // create the Kernel
        IKernelBuilder kernelbuilder = Kernel.CreateBuilder();

        // Add the OpenAI Service provider to the Kernel
        kernelbuilder.AddOpenAIChatCompletion("modelId", "apikey", "orgid", "serviceid");

        // build the Kernel
        Kernel kernel = kernelbuilder.Build();

        // Invoke the Prompt 
        FunctionResult result = await kernel.InvokePromptAsync("Prompt Template");

        // Display the Result from the AI Model
        Console.WriteLine(result);

        // Invoke the prompt by the Template Prompt
        KernelArguments arguments = new KernelArguments() { { "topic", "Rabbit and Tortoise" } };
        FunctionResult result2 = await kernel.InvokePromptAsync("Give the Summary on the : {{$topic}}", arguments);

        // Invoke the Streaming prompt
        var result3 = kernel.InvokePromptStreamingAsync("Write the code to sort the numbers of an array in java");

        // looping through the content
        await foreach (StreamingKernelContent content in result3)
        {
            // Displaying the Content from the Stream.
            Console.WriteLine(content);
        }

        // retreive the chatcompletion service
        IChatCompletionService chatcompletionservice = kernel.GetRequiredService<IChatCompletionService>();

        #endregion

        #region Prompt Templates - Rendering the handlebar prompt & Kernel Template Prompt

        // using the input template variable
        KernelArguments arguments1 = new KernelArguments() { { "input", "Give me the code" } };

        // Passing the input template variable to the prompt by passing the kernel arguments. 
        var result4 = await kernel.InvokePromptAsync("<message role='user'>{{$input}}</message>", arguments1);

        // Handle bar templates

        // designing the template
        string template = """
    <message role="system">
        You are an assistant chatbot particularly focuses on the User health.
    </message>
    <message role="user">
        What is the BMI
    </message>
    """;

        // creating the template factory of the handle bar
        IPromptTemplateFactory handlebartemplatefactory = new HandlebarsPromptTemplateFactory();
        PromptTemplateConfig prompttemplateconfig = new PromptTemplateConfig()
        {
            Template = template,
            TemplateFormat = "handlebars",
            Name = "ChatPrompt"

        };

        // render the prompt
        var prompttemplate = handlebartemplatefactory.Create(prompttemplateconfig);
        var renderedprompt = await prompttemplate.RenderAsync(kernel);

        // display the prompt
        Console.WriteLine(renderedprompt);
        #endregion

        #region Execution Settings - Prompt Execution Settings

        // Invoking the prompt by the execution settings
        OpenAIPromptExecutionSettings promptExecutionSettings = new OpenAIPromptExecutionSettings()
        {
            Logprobs = true,
            Temperature = 0.9,
            MaxTokens = 200,
            ResponseFormat = "json"
        };

        // Creating the arguments by passing the execution settings in it.
        KernelArguments arg = new KernelArguments(promptExecutionSettings) { { "input", "How can we get the Help" } };

        // Passing the argument and execution settings while invoking the prompt
        FunctionResult executedresult = await kernel.InvokePromptAsync("<message role='user'>{{$input}}</message>", arg);

        // Display the result
        Console.WriteLine(executedresult);
        #endregion

        #region ChatCompletion Service - ChatHistory and ChatCompletion

        // retrieve the chatcompletion service from the kernel
        IChatCompletionService chatcompletion = kernel.GetRequiredService<IChatCompletionService>();

        // Chathistory object
        ChatHistory chathistory = new ChatHistory();

        // Adding the message into the object
        chathistory.Add(new ChatMessageContent(AuthorRole.System, "You are a health Assistant Chatbot"));

        // Adding the user Message
        chathistory.AddUserMessage("I have diabetes how to get the cure ?");

        // Passing the chat history and getting the content according to it
        ChatMessageContent contentgenerated = await chatcompletion.GetChatMessageContentAsync(chathistory, promptExecutionSettings, kernel);
        #endregion
    }
}