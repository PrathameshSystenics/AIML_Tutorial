using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace ProcessFramework.Steps
{
    // Step can be Stateful or stateless
    public class GenerateDocumentationStep : KernelProcessStep<GeneratedDocumentationState>
    {
        private GeneratedDocumentationState _state = new();

        private string systemPrompt =
                """
            Your job is to write high quality and engaging customer facing documentation for a new product from Contoso. You will be provide with information
            about the product in the form of internal documentation, specs, and troubleshooting guides and you must use this information and
            nothing else to generate the documentation. If suggestions are provided on the documentation you create, take the suggestions into account and
            rewrite the documentation. Make sure the product sounds amazing.
            """;

        // Called by the process runtime when the step instance is activated. Use this to load state that may be persisted from previous activations.

        public override ValueTask ActivateAsync(KernelProcessStepState<GeneratedDocumentationState> state)
        {
            this._state = state.State!;
            this._state.ChatHistory ??= new ChatHistory(systemPrompt);

            return base.ActivateAsync(state);
        }

        [KernelFunction]
        public async Task GenerateDocumentationAsync(Kernel kernel, KernelProcessStepContext context, string productInfo)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine($"[{nameof(GenerateDocumentationStep)}]: Generating the Documentation");
            Console.ResetColor();

            // Add the new product info to the chat history
            this._state.ChatHistory!.AddUserMessage($"Product Info:\n\n{productInfo}");

            // Get a response from the LLM
            IChatCompletionService chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

            var generatedDocumentationResponse = await chatCompletionService.GetChatMessageContentAsync(this._state.ChatHistory!);

            // Emmitting the event
            await context.EmitEventAsync("DocumentationGenerated", generatedDocumentationResponse.Content!.ToString());
        }


    }
    // Stateful state
    public class GeneratedDocumentationState
    {
        public ChatHistory? ChatHistory { get; set; }
    }
}
