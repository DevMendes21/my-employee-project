using MySql.Data.MySqlClient;
using System;

namespace MyEmployeeProject.Conexao
{
    public class ConexaoUniversal
    {
        private static string? connectionString;

        public static void SetConnectionString(string connString)
        {
            connectionString = connString;
        }

        public static string GetConnectionString()
        {
            if (!string.IsNullOrEmpty(connectionString))
            {
                return connectionString;
            }

            // Tentar diferentes fontes de configuração
            // 1. Variável de ambiente
            var envConnectionString = Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING");
            if (!string.IsNullOrEmpty(envConnectionString))
            {
                return envConnectionString;
            }

            // 2. Configuração padrão para desenvolvimento local
            return "server=localhost;user=root;database=EmpresaDB;password=;port=3306;";
        }

        public static MySqlConnection GetConnection()
        {
            try
            {
                var connString = GetConnectionString();
                var connection = new MySqlConnection(connString);
                connection.Open();
                return connection;
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        throw new Exception("Não foi possível conectar ao servidor MySQL. Verifique se o MySQL está rodando.", ex);
                    case 1045:
                        throw new Exception("Usuário/senha inválidos. Verifique suas credenciais no arquivo de configuração.", ex);
                    case 1049:
                        throw new Exception("Banco de dados 'EmpresaDB' não encontrado. Execute o script EmpresaDB.sql primeiro.", ex);
                    default:
                        throw new Exception($"Erro ao conectar ao banco de dados MySQL: {ex.Message}", ex);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro inesperado ao conectar ao banco: {ex.Message}", ex);
            }
        }

        public static bool TestConnection()
        {
            try
            {
                using (var conn = GetConnection())
                {
                    return conn.State == System.Data.ConnectionState.Open;
                }
            }
            catch
            {
                return false;
            }
        }

        public static string GetDatabaseInfo()
        {
            try
            {
                using (var conn = GetConnection())
                {
                    using (var cmd = new MySqlCommand("SELECT VERSION()", conn))
                    {
                        var version = cmd.ExecuteScalar()?.ToString();
                        return $"MySQL {version} - Conectado com sucesso!";
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Erro de conexão: {ex.Message}";
            }
        }
    }
}
