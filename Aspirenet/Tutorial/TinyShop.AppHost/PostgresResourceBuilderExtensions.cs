using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TinyShop.AppHost
{
    public static class PostgresResourceBuilderExtensions
    {
        // building the Custom Resource Commands code
        public static IResourceBuilder<PostgresDatabaseResource> WithClearDatabaseDataCommand(this IResourceBuilder<PostgresDatabaseResource> builder)
        {
            builder.WithCommand(
                name: "cleardatabasedata",
                displayName: "Clear Data",
                executeCommand: context => ExecuteClearDataFromDatabaseCommand(context, builder),
                displayDescription: "Clears all data from teh database",
                confirmationMessage: "Are you Sure to Clear the Data",
                iconName: "Delete",
                iconVariant: IconVariant.Filled
                );

            
            return builder;
        }

        public async static Task<ExecuteCommandResult> ExecuteClearDataFromDatabaseCommand(ExecuteCommandContext context, IResourceBuilder<PostgresDatabaseResource> builder)
        {

            string connectionstring = await builder.Resource.ConnectionStringExpression.GetValueAsync(context.CancellationToken);

            if (String.IsNullOrWhiteSpace(connectionstring))
            {
                throw new Exception("No Connection String is found");
            }
            context.ServiceProvider.GetRequiredService<ILogger<Program>>().LogInformation("Invoke Custom Command");

            return CommandResults.Success();
        }
    }
}
