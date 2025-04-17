using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessFramework.Steps
{
    public class StatefulStep:KernelProcessStep<GeneratedDocumentationState>
    {
        private GeneratedDocumentationState _state = new();

        public override ValueTask ActivateAsync(KernelProcessStepState<GeneratedDocumentationState> state)
        {
            this._state = state.State!;
            return base.ActivateAsync(state);
        }

        [KernelFunction]
        public async Task LoggingData(Kernel _kernel,KernelProcessStepContext context,string docs)
        {
            Console.WriteLine($"The Title is {this._state.Title}");
            await context.EmitEventAsync("GenerateMore", docs);

        }
    }
}
