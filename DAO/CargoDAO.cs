using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using MyEmployeeProject.Models;
using MyEmployeeProject.Conexao;

namespace MyEmployeeProject.DAO
{
    public class CargoDAO
    {
        public List<Cargo> ListarCargos()
        {
            List<Cargo> cargos = new List<Cargo>();
            string sql = "SELECT DISTINCT id, nome, nivel, salario_base FROM cargos ORDER BY nome";

            using (MySqlConnection conn = ConexaoUniversal.GetConnection())
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
                                    Nivel = reader["nivel"].ToString() ?? string.Empty,
                                    SalarioBase = Convert.ToDecimal(reader["salario_base"])
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

            using (MySqlConnection conn = ConexaoUniversal.GetConnection())
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
                                    Nivel = reader["nivel"].ToString() ?? string.Empty,
                                    SalarioBase = Convert.ToDecimal(reader["salario_base"])
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

        public bool InserirCargo(Cargo cargo)
        {
            string sql = "INSERT INTO cargos (nome, nivel, salario_base, data_criacao) VALUES (@nome, @nivel, @salario_base, @data_criacao)";

            using (MySqlConnection conn = ConexaoUniversal.GetConnection())
            {
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@nome", cargo.Nome);
                        cmd.Parameters.AddWithValue("@nivel", cargo.Nivel);
                        cmd.Parameters.AddWithValue("@salario_base", cargo.SalarioBase);
                        cmd.Parameters.AddWithValue("@data_criacao", DateTime.Now);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao inserir cargo: " + ex.Message);
                }
            }
        }

        public bool AtualizarCargo(Cargo cargo)
        {
            string sql = "UPDATE cargos SET nome = @nome, nivel = @nivel, salario_base = @salario_base WHERE id = @id";

            using (MySqlConnection conn = ConexaoUniversal.GetConnection())
            {
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", cargo.Id);
                        cmd.Parameters.AddWithValue("@nome", cargo.Nome);
                        cmd.Parameters.AddWithValue("@nivel", cargo.Nivel);
                        cmd.Parameters.AddWithValue("@salario_base", cargo.SalarioBase);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao atualizar cargo: " + ex.Message);
                }
            }
        }

        public bool ExcluirCargo(int id)
        {
            string sql = "DELETE FROM cargos WHERE id = @id";

            using (MySqlConnection conn = ConexaoUniversal.GetConnection())
            {
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao excluir cargo: " + ex.Message);
                }
            }
        }
    }
}
