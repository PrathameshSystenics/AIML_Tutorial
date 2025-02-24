using Aspire.Hosting.Lifecycle;
using Humanizer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OllamaSharp;
using OllamaSharp.Models;


namespace ProductClassification.AppHost
{
    public class OllamaContainerLifecycleHook : IDistributedApplicationLifecycleHook
    {
        private ResourceNotificationService _notificationservice;
        private ILogger _logger;
        private readonly IConfiguration _configuration;

        public OllamaContainerLifecycleHook(ResourceNotificationService notificationservice, ILogger<OllamaContainerLifecycleHook> logger, IConfiguration config)
        {
            _logger = logger;
            _notificationservice = notificationservice;
            _configuration = config;
        }

        public async Task AfterResourcesCreatedAsync(DistributedApplicationModel appModel, CancellationToken cancellationToken = default)
        {
            try
            {
                IResource containerresource = appModel.GetContainerResources().Where(resource => resource.Name.Equals("ollama")).FirstOrDefault()!;

                EndpointAnnotation containerendpoint = containerresource.Annotations.OfType<EndpointAnnotation>().Where(endpoint => endpoint.Name.Equals("ollamaendpoint")).FirstOrDefault()!;

                string connectionstring = containerendpoint.AllocatedEndpoint!.UriString;

                await DownloadModelInOllama(connectionstring, containerresource);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return;
            }

        }

        private async Task<bool> HasModelAsync(OllamaApiClient ollamaapiclient, string modelname)
        {
            try
            {
                var availablemodels = await ollamaapiclient.ListLocalModelsAsync();
                return availablemodels.Any(model => model.Name.Equals(modelname));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        private async Task PullModel(OllamaApiClient ollamaapiclient, string modelname, IResource resource)
        {
            try
            {
                IAsyncEnumerable<PullModelResponse> modelresponse = ollamaapiclient.PullModelAsync(modelname)!;

                await foreach (PullModelResponse response in modelresponse)
                {
                    await _notificationservice.PublishUpdateAsync(resource, state => state with
                    {
                        State = new ResourceStateSnapshot($"Downloading {modelname} Model: {response.Percent.ToMetric(decimals: 0)}% ", KnownResourceStateStyles.Info)
                    });
                }

                await _notificationservice.PublishUpdateAsync(resource, state => state with
                {
                    State = new ResourceStateSnapshot(KnownResourceStates.Running, KnownResourceStateStyles.Success)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return;
            }
        }


        private async Task DownloadModelInOllama(string connectionstring, IResource resource)
        {
            try
            {
                OllamaApiClient ollamaapiclient = new OllamaApiClient(connectionstring);

                string[] ollamamodelstodownload = _configuration.GetSection("OllamaModels").Get<string[]>()!;

                foreach (string model in ollamamodelstodownload)
                {
                    bool hasmodel = await HasModelAsync(ollamaapiclient, model);
                    if (!hasmodel)
                    {
                        await PullModel(ollamaapiclient, model, resource);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
