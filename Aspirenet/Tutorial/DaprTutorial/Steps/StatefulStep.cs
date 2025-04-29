using DaprTutorial.Models;
using Microsoft.SemanticKernel;

namespace ProcessFramework.Steps
{
    public class StatefulStep : KernelProcessStep<GeneratedDocumentationState>
    {
        private GeneratedDocumentationState _state = new();

        public override ValueTask ActivateAsync(KernelProcessStepState<GeneratedDocumentationState> state)
        {
            this._state = state.State!;
            return base.ActivateAsync(state);
        }

        [KernelFunction]
        public async Task LoggingData(Kernel _kernel, KernelProcessStepContext context,UserInputs inputs)
        {
            Console.WriteLine($"The Title is {this._state.Title}");
            Console.WriteLine("StatefulSteps");
            await context.EmitEventAsync("PublishDocs", inputs);

        }
    }
}
