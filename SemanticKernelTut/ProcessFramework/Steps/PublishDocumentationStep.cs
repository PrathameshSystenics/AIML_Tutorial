using Microsoft.SemanticKernel;

namespace ProcessFramework.Steps
{
    // The Last Step
    public class PublishDocumentationStep : KernelProcessStep
    {
        [KernelFunction]
        public void PublishDocumentation(string docs)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine($"[{nameof(PublishDocumentationStep)}]:Publishing the Documentation");
            Console.ResetColor();

            Console.WriteLine(docs);
        }
    }
}
