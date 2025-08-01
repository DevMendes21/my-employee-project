using MySql.Data.MySqlClient;
using System;
using Microsoft.Extensions.Configuration;

namespace MyEmployeeProject.Conexao
{
    public class ConexaoWeb
    {
        private static string? connectionString;

        public static void Initialize(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public static MySqlConnection GetConnection()
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("String de conexão não foi inicializada. Chame Initialize() primeiro.");
            }

            try
            {
                var connection = new MySqlConnection(connectionString);
                connection.Open();
                return connection;
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        throw new Exception("Não foi possível conectar ao servidor.", ex);
                    case 1045:
                        throw new Exception("Usuário/senha inválidos.", ex);
                    default:
                        throw new Exception("Erro ao conectar ao banco de dados.", ex);
                }
            }
        }

        public static bool TestConnection()
        {
            try
            {
                using (var conn = GetConnection())
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
