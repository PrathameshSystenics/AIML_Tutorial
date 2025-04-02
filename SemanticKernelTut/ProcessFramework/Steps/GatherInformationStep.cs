using Microsoft.SemanticKernel;

namespace ProcessFramework.Steps
{
    // Create the Step Class by inheriting the KernelProcessStep Class
    public class GatherInformationStep : KernelProcessStep
    {
        // process step to gather the product information.
        // Create the kernel Function
        [KernelFunction]
        public string GatherProductInformation(string productname)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine($"[GatherInformationStep]: Gathering Product Information for {productname}");
            Console.ResetColor();
            return $"""
                Gather the Information related to the {productname}

                ## Product Description:
                1. Description 1
                2. Description 2
                """;
        }
    }
}
