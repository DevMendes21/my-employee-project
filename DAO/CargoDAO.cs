using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using MinhaEmpresa.Models;
using MinhaEmpresa.Conexao;

namespace MinhaEmpresa.DAO
{
    public class CargoDAO
    {
        public List<Cargo> ListarCargos()
        {
            List<Cargo> cargos = new List<Cargo>();
            string sql = "SELECT DISTINCT id, nome, nivel FROM cargos ORDER BY nome";

            using (MySqlConnection conn = MinhaEmpresa.Conexao.Conexao.GetConnection())
            {
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Cargo cargo = new Cargo
                                {
                                    Id = Convert.ToInt32(reader["id"]),
                                    Nome = reader["nome"].ToString() ?? string.Empty,
                                    Nivel = reader["nivel"].ToString() ?? string.Empty
                                };
                                cargos.Add(cargo);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao listar cargos: " + ex.Message);
                }
            }
            return cargos.GroupBy(c => c.Nome)
                         .Select(g => g.First())
                         .OrderBy(c => c.Nome)
                         .ToList();
        }

        public Cargo? ObterCargoPorId(int id)
        {
            string sql = "SELECT * FROM cargos WHERE id = @id";
            Cargo? cargo = null;

            using (MySqlConnection conn = MinhaEmpresa.Conexao.Conexao.GetConnection())
            {
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                cargo = new Cargo
                                {
                                    Id = Convert.ToInt32(reader["id"]),
                                    Nome = reader["nome"].ToString() ?? string.Empty,
                                    Nivel = reader["nivel"].ToString() ?? string.Empty
                                };
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao obter cargo: " + ex.Message);
                }
            }
            return cargo;
        }
    }
}
