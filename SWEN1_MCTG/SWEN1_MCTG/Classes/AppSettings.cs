using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace SWEN1_MCTG.Classes
{
    public class AppSettings
    {
        private static IConfigurationRoot configuration;

        static AppSettings()
        {
            // Get the base directory of the application
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Navigate to the correct location of the appsettings.json file
            string projectRoot = Path.Combine(baseDirectory, "..", "..", "..", "..", "SWEN1_MCTG");
            string configFilePath = Path.Combine(projectRoot, "appsettings.json");

            // Ensure the path is normalized
            string normalizedPath = Path.GetFullPath(configFilePath);

            var builder = new ConfigurationBuilder()
                .SetBasePath(projectRoot)
                .AddJsonFile(normalizedPath, optional: false, reloadOnChange: true);

            configuration = builder.Build();
        }

        public static string GetConnectionString(string name)
        {
            return configuration.GetConnectionString(name);
        }
    }
}