namespace ProductClassification.Extensions
{
    /// <summary>
    /// Maps the Configuration Section present in the <see cref="IConfiguration"/> to the Specified <see cref="T"/> Object, using the Key. 
    /// </summary>
    public static class ConfigurationExtension
    {

        /// <summary>
        /// Maps the Data Present in appsettings.json to specified Class. 
        /// </summary>
        /// <typeparam name="T">The Object to Map the Configuration</typeparam>
        /// <param name="config">Configuration</param>
        /// <param name="key">Represent the Configuration Section. </param>
        /// <returns> Maps the Configuration Section to the <typeparamref name="T"/></returns>
        /// <exception cref="KeyNotFoundException">Throws if the <paramref name="key"/> is not Found.</exception>
        public static T MapConfigurationToClass<T>(this IConfiguration config, string key)
        {
            T t = config.GetSection(key).Get<T>()!;
            return t == null ? throw new KeyNotFoundException($"The {key} requested is not Present in the appsettings.json") : t;
        }

    }
}
