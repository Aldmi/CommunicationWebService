using System.IO;
using Microsoft.Extensions.Configuration;

namespace Shared.ForConfigFiles
{
    public static class JsonConfigLib
    {
        public static IConfigurationRoot GetConfiguration(string basePath = null, string fileName = "appsettings.json")
        {
            if (string.IsNullOrEmpty(basePath))
            {
                basePath = Directory.GetCurrentDirectory();
            }

            var builder = new ConfigurationBuilder();
            // установка пути к текущему каталогу
            builder.SetBasePath(basePath);
            // получаем конфигурацию из файла appsettings.json
            builder.AddJsonFile("appsettings.json");
            // создаем конфигурацию
            var config = builder.Build();

            return config;
        }
    }
}