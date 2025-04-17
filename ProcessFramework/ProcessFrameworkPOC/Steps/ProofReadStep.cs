using Microsoft.SemanticKernel;
using ProcessFrameworkPOC.Models;
using ProcessFrameworkPOC.Utilities;

namespace ProcessFrameworkPOC.Steps
{
    public class ProofReadStep : KernelProcessStep
    {
        [KernelFunction]
        public async Task ProofReadByUser(Kernel _kernel, KernelProcessStepContext context, UserInputs inputs)
        {
            PrettyPrint.Print("Proof Read By User", ConsoleColor.DarkMagenta, ConsoleColor.White);
            Console.WriteLine("Read the Blog If Improvements Needed Give with Response Y or N ");
            string response = Console.ReadLine()!;
            if (response.Equals("Y"))
            {
                Console.WriteLine("Suggest the Improvements Need to Be Done in the Blog : ");
                string suggestions = Console.ReadLine()!;
                inputs.ModificationsNeeded = suggestions;
                await context.EmitEventAsync("ImprovementByUser", inputs);
            }
            await context.EmitEventAsync("NoImprovementsNeeded", inputs);
        }
    }
}
