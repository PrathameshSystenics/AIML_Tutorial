using Aspire.Hosting;

namespace ProductClassification.AppHost
{
    public static class Extensions
    {
        public static IResourceBuilder<ProjectResource> AddOllamaEndpointToEnvironmentVariables(this IResourceBuilder<ProjectResource> builder, EndpointReference endpoint)
        {
            builder.WithEnvironment("OllamaDeepSeek:Url", endpoint);
            builder.WithEnvironment("OllamaPhi3_8b:Url", endpoint);
            builder.WithEnvironment("OllamaQwen1_8b", endpoint);
            return builder;
        }

        public static IResourceBuilder<PostgresServerResource> AddCommandForClearEvaluationData(this IResourceBuilder<PostgresServerResource> builder)
        {
            builder.WithCommand(
                name: "clearevaldata",
                displayName: "Clear Evaluation Data",
                executeCommand: context => ClearEvaluationDataFromDatabase(context, builder),
                displayDescription: "Clears Evaluation Related Data",
                confirmationMessage: "Are You Sure You want To Delete Data",
                iconName: "Delete",
                iconVariant: IconVariant.Filled

                );
            return builder;
        }

        private static async Task<ExecuteCommandResult> ClearEvaluationDataFromDatabase(ExecuteCommandContext context, IResourceBuilder<PostgresServerResource> builder)
        {
            string connectionstring = await builder.Resource.GetConnectionStringAsync();


            
            return CommandResults.Success();
        }
    }
}
