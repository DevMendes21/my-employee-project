using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace MyEmployeeProject.Utils
{
    public static class ConfigManagerCrossPlatform 
    {
        private static IConfiguration? _configuration;

        static ConfigManagerCrossPlatform()
        {
            InitializeConfiguration();
        }

        private static void InitializeConfiguration()
        {
            var builder = new ConfigurationBuilder();
            
            // Tentar usar appsettings.json primeiro (versão web)
            if (File.Exists("appsettings.json"))
            {
                builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            }
            // Fallback para App.config (versão Windows Forms)
            else if (File.Exists("App.config"))
            {
                // Para .NET Framework/Core, usar System.Configuration
                try
                {
                    _configuration = builder.Build();
                    return;
                }
                catch
                {
                    // Continuar para usar configuração manual
                }
            }

            _configuration = builder.Build();
        }

        public static string GetConnectionString(string name = "DefaultConnection")
        {
            if (_configuration != null)
            {
                var connectionString = _configuration.GetConnectionString(name);
                if (!string.IsNullOrEmpty(connectionString))
                {
                    return connectionString;
                }
            }

            // Fallback manual para desenvolvimento local
            return "server=localhost;user=root;database=EmpresaDB;password=;";
        }

        public static string GetSetting(string key, string defaultValue = "")
        {
            if (_configuration != null)
            {
                return _configuration[key] ?? defaultValue;
            }
            return defaultValue;
        }
    }
}
