using MySql.Data.MySqlClient;
using System;
using System.Configuration;

namespace MyEmployeeProject.Conexao
{
    using global::MySql.Data.MySqlClient;
    using global::System.Configuration;

    public class Conexao
    {
        private static readonly string connectionString = 
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public static MySqlConnection GetConnection()
        {
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
