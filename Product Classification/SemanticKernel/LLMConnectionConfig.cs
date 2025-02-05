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

    /// <summary>
    /// Maps the Configuration Section present in the <see cref="IConfiguration"/> to the Specified <see cref="T"/> Object, using the Key. 
    /// </summary>
    public static class ConfigurationMapper
    {
        public static T MapConfigurationToClass<T>(this IConfiguration config, string key)
        {
            T t = config.GetSection(key).Get<T>();
            return t == null ? throw new KeyNotFoundException($"The {key} requested is not Present in the appsettings.json") : t;
        }
    }


}
