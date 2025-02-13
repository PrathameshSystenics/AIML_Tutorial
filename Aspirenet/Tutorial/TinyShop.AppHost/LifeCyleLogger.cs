

using Aspire.Hosting.Lifecycle;
using Microsoft.Extensions.Logging;

namespace Store
{
    // Implemented all the lifecycle hooks
    public class LifeCyleLogger(ILogger<LifeCyleLogger> logger): IDistributedApplicationLifecycleHook
    {
        public Task BeforeStartAsync(
        DistributedApplicationModel appModel, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("BeforeStartAsync");
            return Task.CompletedTask;
        }

        public Task AfterEndpointsAllocatedAsync(
            DistributedApplicationModel appModel, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("AfterEndpointsAllocatedAsync");
            return Task.CompletedTask;
        }

        public Task AfterResourcesCreatedAsync(
            DistributedApplicationModel appModel, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("AfterResourcesCreatedAsync");
            return Task.CompletedTask;
        }
    }
}
