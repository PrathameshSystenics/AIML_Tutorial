namespace ProductClassification.SemanticKernel
{
    public class LLMConnectionConfig
    {
        public string Url { get; set; } = string.Empty;
        public string ModelName { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
    }

    public class AzureOpenAIConnectionConfig : LLMConnectionConfig
    {
        public string ModelVersion { get; set; } = string.Empty;
    }

    


}
