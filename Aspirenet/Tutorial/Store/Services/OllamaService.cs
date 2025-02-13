using DataEntities;
using System.Text.Json;

namespace Store.Services
{
    public class OllamaService
    {
        HttpClient httpClient;
        public OllamaService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }
        public async Task<object> CallOllamaMessage()
        {
            var response = await httpClient.GetAsync("/api/version");


            return response;
        }
    }
}
