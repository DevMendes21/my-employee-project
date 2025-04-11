using MySql.Data.MySqlClient;
using System;

namespace MinhaEmpresa.Conexao
{
    public class Conexao
    {
        public static MySqlConnection GetConnection()
        {
            string connStr = "server=localhost;user=root;database=EmpresaDB;password=SUASENHA;";
            return new MySqlConnection(connStr);
        }
    }
}
