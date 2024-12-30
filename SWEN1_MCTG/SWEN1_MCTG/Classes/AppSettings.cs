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
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("C:\\Users\\samue\\Documents\\Studium\\Semester 3 - WS2024\\SWEN1\\SWEN1_MCTG\\SWEN1_MCTG\\appsettings.json", optional: false, reloadOnChange: true);

            configuration = builder.Build();
        }

        public static string GetConnectionString(string name)
        {
            return configuration.GetConnectionString(name);
        }
    }
}