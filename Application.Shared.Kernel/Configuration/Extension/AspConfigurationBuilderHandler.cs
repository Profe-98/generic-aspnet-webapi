using Microsoft.Extensions.Configuration;
using Application.Shared.Kernel.Configuration.Model.Abstraction;
using Application.Shared.Kernel.Data.Format.Json;

namespace Application.Shared.Kernel.Configuration.Extension
{
    public static class AspConfigurationBuilderHandler
    {
        public static IConfigurationBuilder AddCustomWebApiConfig<T>(this IConfigurationBuilder configurationBuilder, string fileRootDir, T configurationToAppend = null)
            where T : AbstractConfigurationModel
        {
            T tmp = null;
            if (configurationToAppend == null)
            {
                tmp = Activator.CreateInstance<T>();
            }
            string instanceName = configurationToAppend != null ?
                configurationToAppend.GetType().Name : tmp.GetType().Name;

            string fileName = instanceName.ToLower().Replace("model", "") + ".json";
            string path = Path.Combine(fileRootDir, fileName);
            if (!File.Exists(path) && configurationToAppend != null)
            {
                string content = null;
                using (JsonHandler jsonHandler = new JsonHandler())
                {
                    content = jsonHandler.JsonSerialize(configurationToAppend);
                }
                File.WriteAllText(path, content);
            }
            configurationBuilder.AddJsonFile(path, optional: false, reloadOnChange: true);
            return configurationBuilder;
        }
    }
    

}
