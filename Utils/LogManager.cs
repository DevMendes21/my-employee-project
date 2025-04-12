using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MinhaEmpresa.Utils
{
    /// <summary>
    /// Gerencia o sistema de logs da aplicação
    /// </summary>
    public static class LogManager
    {
        private static readonly string LogDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "MinhaEmpresa",
            "Logs"
        );

        private static readonly object _lockObject = new object();

        /// <summary>
        /// Níveis de log disponíveis
        /// </summary>
        public enum LogLevel
        {
            Debug,
            Info,
            Warning,
            Error,
            Critical
        }

        /// <summary>
        /// Inicializa o sistema de logs
        /// </summary>
        static LogManager()
        {
            try
            {
                // Garantir que o diretório de logs exista
                if (!Directory.Exists(LogDirectory))
                {
                    try
                    {
                        Directory.CreateDirectory(LogDirectory);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao criar diretório de logs: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro na inicialização do LogManager: {ex.Message}");
            }
        }

        /// <summary>
        /// Registra uma mensagem de log
        /// </summary>
        /// <param name="level">Nível do log</param>
        /// <param name="message">Mensagem a ser registrada</param>
        /// <param name="source">Fonte/origem do log (classe, método)</param>
        public static void Log(LogLevel level, string message, string source = "")
        {
            string logFileName = Path.Combine(LogDirectory, $"log_{DateTime.Now:yyyyMMdd}.txt");
            string logEntry = FormatLogEntry(level, message, source);

            try
            {
                // Usar lock para evitar problemas de concorrência ao escrever no arquivo
                lock (_lockObject)
                {
                    File.AppendAllText(logFileName, logEntry + Environment.NewLine, Encoding.UTF8);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao escrever log: {ex.Message}");
            }
        }

        /// <summary>
        /// Registra uma mensagem de log de forma assíncrona
        /// </summary>
        /// <param name="level">Nível do log</param>
        /// <param name="message">Mensagem a ser registrada</param>
        /// <param name="source">Fonte/origem do log (classe, método)</param>
        public static async Task LogAsync(LogLevel level, string message, string source = "")
        {
            string logFileName = Path.Combine(LogDirectory, $"log_{DateTime.Now:yyyyMMdd}.txt");
            string logEntry = FormatLogEntry(level, message, source);

            try
            {
                await File.AppendAllTextAsync(logFileName, logEntry + Environment.NewLine, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao escrever log assíncrono: {ex.Message}");
            }
        }

        /// <summary>
        /// Registra uma mensagem de log de depuração
        /// </summary>
        /// <param name="message">Mensagem a ser registrada</param>
        /// <param name="source">Fonte/origem do log</param>
        public static void Debug(string message, string source = "") => Log(LogLevel.Debug, message, source);

        /// <summary>
        /// Registra uma mensagem de log informativa
        /// </summary>
        /// <param name="message">Mensagem a ser registrada</param>
        /// <param name="source">Fonte/origem do log</param>
        public static void Info(string message, string source = "") => Log(LogLevel.Info, message, source);

        /// <summary>
        /// Registra uma mensagem de log de aviso
        /// </summary>
        /// <param name="message">Mensagem a ser registrada</param>
        /// <param name="source">Fonte/origem do log</param>
        public static void Warning(string message, string source = "") => Log(LogLevel.Warning, message, source);

        /// <summary>
        /// Registra uma mensagem de log de erro
        /// </summary>
        /// <param name="message">Mensagem a ser registrada</param>
        /// <param name="source">Fonte/origem do log</param>
        public static void Error(string message, string source = "") => Log(LogLevel.Error, message, source);

        /// <summary>
        /// Registra uma mensagem de log de erro crítico
        /// </summary>
        /// <param name="message">Mensagem a ser registrada</param>
        /// <param name="source">Fonte/origem do log</param>
        public static void Critical(string message, string source = "") => Log(LogLevel.Critical, message, source);

        /// <summary>
        /// Registra uma exceção como erro
        /// </summary>
        /// <param name="ex">Exceção a ser registrada</param>
        /// <param name="message">Mensagem adicional</param>
        /// <param name="source">Fonte/origem do log</param>
        public static void Exception(Exception ex, string message = "", string source = "")
        {
            string exceptionMessage = string.IsNullOrEmpty(message)
                ? $"Exceção: {ex.Message}"
                : $"{message} - Exceção: {ex.Message}";

            string fullMessage = $"{exceptionMessage}\nStackTrace: {ex.StackTrace}";

            if (ex.InnerException != null)
            {
                fullMessage += $"\nInnerException: {ex.InnerException.Message}";
            }

            Log(LogLevel.Error, fullMessage, source);
        }

        /// <summary>
        /// Formata uma entrada de log
        /// </summary>
        /// <param name="level">Nível do log</param>
        /// <param name="message">Mensagem a ser registrada</param>
        /// <param name="source">Fonte/origem do log</param>
        /// <returns>Entrada de log formatada</returns>
        private static string FormatLogEntry(LogLevel level, string message, string source)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string sourceInfo = string.IsNullOrEmpty(source) ? "" : $" [{source}]";
            return $"[{timestamp}] [{level}]{sourceInfo}: {message}";
        }

        /// <summary>
        /// Limpa logs antigos (mais de 30 dias)
        /// </summary>
        public static void LimparLogsAntigos(int diasParaReter = 30)
        {
            try
            {
                string[] arquivos = Directory.GetFiles(LogDirectory, "log_*.txt");
                DateTime dataLimite = DateTime.Now.AddDays(-diasParaReter);

                foreach (string arquivo in arquivos)
                {
                    FileInfo info = new FileInfo(arquivo);
                    if (info.CreationTime < dataLimite)
                    {
                        File.Delete(arquivo);
                        Debug($"Log antigo excluído: {info.Name}", "LogManager.LimparLogsAntigos");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao limpar logs antigos: {ex.Message}");
            }
        }
    }
}
