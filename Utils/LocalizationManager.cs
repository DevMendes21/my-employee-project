using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Threading;

namespace MinhaEmpresa.Utils
{
    /// <summary>
    /// Gerencia a localizau00e7u00e3o e internacionalizau00e7u00e3o do sistema
    /// </summary>
    public static class LocalizationManager
    {
        private static readonly string LocalizationDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "MinhaEmpresa",
            "Localization"
        );

        private static Dictionary<string, Dictionary<string, string>> _translations = new Dictionary<string, Dictionary<string, string>>();
        private static string _currentLanguage = "pt-BR"; // Idioma padru00e3o

        /// <summary>
        /// Idioma atual do sistema
        /// </summary>
        public static string CurrentLanguage
        {
            get { return _currentLanguage; }
            set
            {
                if (_currentLanguage != value)
                {
                    _currentLanguage = value;
                    LoadTranslations(value);
                    
                    // Salvar a configurau00e7u00e3o
                    var config = ConfigManager.Config;
                    ConfigManager.DefinirConfiguracao("CurrentLanguage", value);
                    ConfigManager.SalvarConfiguracoes(config);
                    
                    // Atualizar a cultura da thread atual
                    try
                    {
                        Thread.CurrentThread.CurrentUICulture = new CultureInfo(value);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao definir cultura: {ex.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// Idiomas disponu00edveis no sistema
        /// </summary>
        public static List<string> AvailableLanguages { get; private set; } = new List<string> { "pt-BR", "en-US" };

        /// <summary>
        /// Inicializa o sistema de localizau00e7u00e3o
        /// </summary>
        static LocalizationManager()
        {
            try
            {
                // Garantir que o diretu00f3rio de localizau00e7u00e3o exista
                if (!Directory.Exists(LocalizationDirectory))
                {
                    try
                    {
                        Directory.CreateDirectory(LocalizationDirectory);
                        // Criar arquivos de traduu00e7u00e3o padru00e3o se nu00e3o existirem
                        CreateDefaultTranslationFiles();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao criar diretu00f3rio de localizau00e7u00e3o: {ex.Message}");
                    }
                }

                try
                {
                    // Carregar idioma salvo nas configurau00e7u00f5es
                    string savedLanguage = ConfigManager.ObterConfiguracao<string>("CurrentLanguage", "pt-BR");
                    _currentLanguage = savedLanguage;
                    LoadTranslations(savedLanguage);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao carregar idioma: {ex.Message}");
                    // Em caso de erro, usar o idioma padru00e3o
                    _currentLanguage = "pt-BR";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro na inicializau00e7u00e3o do LocalizationManager: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtu00e9m uma string traduzida
        /// </summary>
        /// <param name="key">Chave da string</param>
        /// <param name="defaultValue">Valor padru00e3o caso a traduu00e7u00e3o nu00e3o seja encontrada</param>
        /// <returns>String traduzida</returns>
        public static string GetString(string key, string? defaultValue = null)
        {
            if (_translations.TryGetValue(_currentLanguage, out var languageDict) && 
                languageDict.TryGetValue(key, out var translation))
            {
                return translation;
            }

            // Se nu00e3o encontrou, tenta no idioma padru00e3o (pt-BR)
            if (_currentLanguage != "pt-BR" && 
                _translations.TryGetValue("pt-BR", out var defaultDict) && 
                defaultDict.TryGetValue(key, out var defaultTranslation))
            {
                return defaultTranslation;
            }

            // Se ainda nu00e3o encontrou, retorna o valor padru00e3o ou a pru00f3pria chave
            return defaultValue ?? key;
        }

        /// <summary>
        /// Carrega as traduu00e7u00f5es para um idioma especu00edfico
        /// </summary>
        /// <param name="language">Cu00f3digo do idioma</param>
        private static void LoadTranslations(string language)
        {
            string filePath = Path.Combine(LocalizationDirectory, $"{language}.json");

            try
            {
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    var translations = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

                    if (translations != null)
                    {
                        if (_translations.ContainsKey(language))
                        {
                            _translations[language] = translations;
                        }
                        else
                        {
                            _translations.Add(language, translations);
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Arquivo de traduu00e7u00e3o nu00e3o encontrado: {filePath}");
                    // Criar arquivo de traduu00e7u00e3o padru00e3o
                    CreateDefaultTranslationFile(language);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar traduu00e7u00f5es: {ex.Message}");
            }
        }

        /// <summary>
        /// Cria arquivos de traduu00e7u00e3o padru00e3o para os idiomas suportados
        /// </summary>
        private static void CreateDefaultTranslationFiles()
        {
            foreach (string language in AvailableLanguages)
            {
                CreateDefaultTranslationFile(language);
            }
        }

        /// <summary>
        /// Cria um arquivo de traduu00e7u00e3o padru00e3o para um idioma especu00edfico
        /// </summary>
        /// <param name="language">Cu00f3digo do idioma</param>
        private static void CreateDefaultTranslationFile(string language)
        {
            string filePath = Path.Combine(LocalizationDirectory, $"{language}.json");

            // Verificar se o arquivo ju00e1 existe
            if (File.Exists(filePath))
                return;

            try
            {
                Dictionary<string, string> defaultTranslations = new Dictionary<string, string>();

                // Adicionar traduu00e7u00f5es padru00e3o com base no idioma
                switch (language)
                {
                    case "pt-BR":
                        defaultTranslations = GetDefaultPortugueseTranslations();
                        break;
                    case "en-US":
                        defaultTranslations = GetDefaultEnglishTranslations();
                        break;
                    default:
                        // Para outros idiomas, usar inglu00eas como base
                        defaultTranslations = GetDefaultEnglishTranslations();
                        break;
                }

                // Salvar o arquivo
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(defaultTranslations, options);
                File.WriteAllText(filePath, json);

                // Adicionar ao dicionu00e1rio em memu00f3ria
                if (_translations.ContainsKey(language))
                {
                    _translations[language] = defaultTranslations;
                }
                else
                {
                    _translations.Add(language, defaultTranslations);
                }

                Console.WriteLine($"Arquivo de traduu00e7u00e3o criado: {language}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar arquivo de traduu00e7u00e3o: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtu00e9m as traduu00e7u00f5es padru00e3o em portuguu00eas
        /// </summary>
        private static Dictionary<string, string> GetDefaultPortugueseTranslations()
        {
            return new Dictionary<string, string>
            {
                // Menu
                { "menu.dashboard", "Dashboard" },
                { "menu.employees", "Funcionu00e1rios" },
                { "menu.new_employee", "Novo Funcionu00e1rio" },
                { "menu.reports", "Relatu00f3rios" },
                { "menu.settings", "Configurau00e7u00f5es" },
                
                // Botu00f5es
                { "button.save", "Salvar" },
                { "button.cancel", "Cancelar" },
                { "button.delete", "Excluir" },
                { "button.edit", "Editar" },
                { "button.add", "Adicionar" },
                { "button.close", "Fechar" },
                { "button.export", "Exportar" },
                { "button.import", "Importar" },
                
                // Configurau00e7u00f5es
                { "settings.title", "Configurau00e7u00f5es do Sistema" },
                { "settings.theme", "Tema" },
                { "settings.dark_theme", "Tema Escuro" },
                { "settings.high_contrast", "Alto Contraste" },
                { "settings.auto_update", "Atualizau00e7u00e3o Automu00e1tica" },
                { "settings.language", "Idioma" },
                { "settings.export_format", "Formato de Exportau00e7u00e3o" },
                { "settings.chart_order", "Ordenau00e7u00e3o do Gru00e1fico" },
                { "settings.show_values", "Mostrar Valores" },
                
                // Mensagens
                { "message.saved", "Configurau00e7u00f5es salvas com sucesso!" },
                { "message.error", "Ocorreu um erro!" },
                { "message.confirm_delete", "Tem certeza que deseja excluir?" },
                { "message.data_updated", "Dados atualizados com sucesso!" },
                { "message.high_contrast_on", "Modo de alto contraste ativado" },
                { "message.high_contrast_off", "Modo de alto contraste desativado" },
                
                // Funcionu00e1rios
                { "employee.name", "Nome" },
                { "employee.position", "Cargo" },
                { "employee.department", "Departamento" },
                { "employee.salary", "Salu00e1rio" },
                { "employee.hire_date", "Data de Admissu00e3o" },
                { "employee.details", "Detalhes do Funcionu00e1rio" },
                
                // Atalhos
                { "shortcut.help", "Ajuda - Teclas de Atalho" },
                { "shortcut.help_text", "Teclas de atalho disponu00edveis:\n\nF1 - Exibir esta ajuda\nF5 - Atualizar dados\nCtrl+C - Abrir configurau00e7u00f5es\nCtrl+F - Abrir lista de funcionu00e1rios\nCtrl+N - Novo funcionu00e1rio\nAlt+H - Alternar modo de alto contraste" }
            };
        }

        /// <summary>
        /// Obtu00e9m as traduu00e7u00f5es padru00e3o em inglu00eas
        /// </summary>
        private static Dictionary<string, string> GetDefaultEnglishTranslations()
        {
            return new Dictionary<string, string>
            {
                // Menu
                { "menu.dashboard", "Dashboard" },
                { "menu.employees", "Employees" },
                { "menu.new_employee", "New Employee" },
                { "menu.reports", "Reports" },
                { "menu.settings", "Settings" },
                
                // Buttons
                { "button.save", "Save" },
                { "button.cancel", "Cancel" },
                { "button.delete", "Delete" },
                { "button.edit", "Edit" },
                { "button.add", "Add" },
                { "button.close", "Close" },
                { "button.export", "Export" },
                { "button.import", "Import" },
                
                // Settings
                { "settings.title", "System Settings" },
                { "settings.theme", "Theme" },
                { "settings.dark_theme", "Dark Theme" },
                { "settings.high_contrast", "High Contrast" },
                { "settings.auto_update", "Auto Update" },
                { "settings.language", "Language" },
                { "settings.export_format", "Export Format" },
                { "settings.chart_order", "Chart Order" },
                { "settings.show_values", "Show Values" },
                
                // Messages
                { "message.saved", "Settings saved successfully!" },
                { "message.error", "An error occurred!" },
                { "message.confirm_delete", "Are you sure you want to delete?" },
                { "message.data_updated", "Data updated successfully!" },
                { "message.high_contrast_on", "High contrast mode enabled" },
                { "message.high_contrast_off", "High contrast mode disabled" },
                
                // Employees
                { "employee.name", "Name" },
                { "employee.position", "Position" },
                { "employee.department", "Department" },
                { "employee.salary", "Salary" },
                { "employee.hire_date", "Hire Date" },
                { "employee.details", "Employee Details" },
                
                // Shortcuts
                { "shortcut.help", "Help - Keyboard Shortcuts" },
                { "shortcut.help_text", "Available keyboard shortcuts:\n\nF1 - Show this help\nF5 - Update data\nCtrl+C - Open settings\nCtrl+F - Open employee list\nCtrl+N - New employee\nAlt+H - Toggle high contrast mode" }
            };
        }

        /// <summary>
        /// Adiciona ou atualiza uma traduu00e7u00e3o
        /// </summary>
        /// <param name="language">Cu00f3digo do idioma</param>
        /// <param name="key">Chave da string</param>
        /// <param name="value">Valor traduzido</param>
        public static void SetTranslation(string language, string key, string value)
        {
            try
            {
                // Verificar se o idioma existe no dicionu00e1rio
                if (!_translations.ContainsKey(language))
                {
                    _translations.Add(language, new Dictionary<string, string>());
                }

                // Adicionar ou atualizar a traduu00e7u00e3o
                if (_translations[language].ContainsKey(key))
                {
                    _translations[language][key] = value;
                }
                else
                {
                    _translations[language].Add(key, value);
                }

                // Salvar as alterau00e7u00f5es no arquivo
                SaveTranslations(language);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao definir traduu00e7u00e3o: {ex.Message}");
            }
        }

        /// <summary>
        /// Salva as traduu00e7u00f5es de um idioma no arquivo
        /// </summary>
        /// <param name="language">Cu00f3digo do idioma</param>
        private static void SaveTranslations(string language)
        {
            string filePath = Path.Combine(LocalizationDirectory, $"{language}.json");

            try
            {
                if (_translations.TryGetValue(language, out var translations))
                {
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    string json = JsonSerializer.Serialize(translations, options);
                    File.WriteAllText(filePath, json);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar traduu00e7u00f5es: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtu00e9m o nome do idioma a partir do cu00f3digo
        /// </summary>
        /// <param name="languageCode">Cu00f3digo do idioma</param>
        /// <returns>Nome do idioma</returns>
        public static string GetLanguageName(string languageCode)
        {
            try
            {
                CultureInfo culture = new CultureInfo(languageCode);
                return culture.NativeName;
            }
            catch
            {
                return languageCode;
            }
        }
    }
}
