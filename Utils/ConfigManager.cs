using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

namespace MinhaEmpresa.Utils
{
    /// <summary>
    /// Gerencia a persistu00eancia e carregamento das configurau00e7u00f5es do sistema
    /// </summary>
    public static class ConfigManager
    {
        private static readonly string ConfigFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "MinhaEmpresa",
            "config.json"
        );
        
        /// <summary>
        /// Configurau00e7u00f5es do sistema
        /// </summary>
        public class AppConfig
        {
            public bool TemaEscuro { get; set; } = false;
            public bool AtualizacaoAutomatica { get; set; } = true;
            public string OrdenacaoGrafico { get; set; } = "Alfabu00e9tica";
            public bool MostrarValores { get; set; } = true;
            public Dictionary<string, object> ConfiguracoesAdicionais { get; set; } = new Dictionary<string, object>();
        }
        
        // Instu00e2ncia u00fanica das configurau00e7u00f5es
        private static AppConfig? _config;
        
        // Inicializador estático para garantir que as configurações sejam carregadas
        static ConfigManager()
        {
            try
            {
                _config = CarregarConfiguracoes();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inicializar ConfigManager: {ex.Message}");
                // Em caso de erro, criar uma configuração padrão
                _config = new AppConfig();
            }
        }
        
        /// <summary>
        /// Obtu00e9m as configurau00e7u00f5es atuais do sistema
        /// </summary>
        public static AppConfig Config
        {
            get
            {
                // Garantir que _config nunca seja nulo
                if (_config == null)
                {
                    _config = new AppConfig();
                }
                return _config;
            }
        }
        
        /// <summary>
        /// Carrega as configurau00e7u00f5es do arquivo
        /// </summary>
        private static AppConfig CarregarConfiguracoes()
        {
            try
            {
                // Verificar se o diretu00f3rio existe, caso contru00e1rio, criu00e1-lo
                string? configDir = Path.GetDirectoryName(ConfigFilePath);
                if (!string.IsNullOrEmpty(configDir) && !Directory.Exists(configDir))
                {
                    Directory.CreateDirectory(configDir);
                }
                
                // Verificar se o arquivo existe
                if (File.Exists(ConfigFilePath))
                {
                    string json = File.ReadAllText(ConfigFilePath);
                    var config = JsonSerializer.Deserialize<AppConfig>(json);
                    return config ?? new AppConfig();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar configurau00e7u00f5es: {ex.Message}");
                // Em caso de erro, retornar configurau00e7u00f5es padru00e3o
            }
            
            return new AppConfig();
        }
        
        /// <summary>
        /// Salva as configurau00e7u00f5es no arquivo
        /// </summary>
        public static void SalvarConfiguracoes(AppConfig config)
        {
            try
            {
                _config = config; // Atualiza a instu00e2ncia em memu00f3ria
                
                // Verificar se o diretu00f3rio existe, caso contru00e1rio, criu00e1-lo
                string? configDir = Path.GetDirectoryName(ConfigFilePath);
                if (!string.IsNullOrEmpty(configDir) && !Directory.Exists(configDir))
                {
                    Directory.CreateDirectory(configDir);
                }
                
                // Serializar e salvar
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(config, options);
                File.WriteAllText(ConfigFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar configurau00e7u00f5es: {ex.Message}");
                // Aqui poderia ser implementado um log de erros
            }
        }
        
        /// <summary>
        /// Define uma configurau00e7u00e3o adicional
        /// </summary>
        public static void DefinirConfiguracao(string chave, object valor)
        {
            if (Config.ConfiguracoesAdicionais.ContainsKey(chave))
            {
                Config.ConfiguracoesAdicionais[chave] = valor;
            }
            else
            {
                Config.ConfiguracoesAdicionais.Add(chave, valor);
            }
            
            SalvarConfiguracoes(Config);
        }
        
        /// <summary>
        /// Obtu00e9m uma configurau00e7u00e3o adicional
        /// </summary>
        public static T ObterConfiguracao<T>(string chave, T valorPadrao = default!)
        {
            if (Config.ConfiguracoesAdicionais.TryGetValue(chave, out object? valor) && valor != null)
            {
                try
                {
                    // Converter o valor para o tipo esperado
                    if (valor is T tipado)
                    {
                        return tipado;
                    }
                    else if (typeof(T).IsAssignableFrom(valor.GetType()))
                    {
                        return (T)valor;
                    }
                    else
                    {
                        // Tentar converter usando Convert se possível
                        return (T)Convert.ChangeType(valor, typeof(T));
                    }
                }
                catch
                {
                    // Se a conversão falhar, retornar o valor padrão
                    return valorPadrao;
                }
            }
            
            return valorPadrao;
        }
    }
}
