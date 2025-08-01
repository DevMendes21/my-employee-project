using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using MyEmployeeProject.Models;
using MyEmployeeProject.Conexao;

namespace MyEmployeeProject.DAO
{
    public class DepartamentoDAO
    {
        public List<Departamento> ListarDepartamentos()
        {
            List<Departamento> departamentos = new List<Departamento>();
            string sql = "SELECT DISTINCT id, nome, data_criacao FROM departamentos ORDER BY nome";

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
                                Departamento departamento = new Departamento
                                {
                                    Id = Convert.ToInt32(reader["id"]),
                                    Nome = reader["nome"].ToString() ?? string.Empty,
                                    DataCriacao = reader["data_criacao"] != DBNull.Value ? Convert.ToDateTime(reader["data_criacao"]) : DateTime.Now
                                };
                                departamentos.Add(departamento);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao listar departamentos: " + ex.Message);
                }
            }
            var result = departamentos.GroupBy(d => d.Nome)
                               .Select(g => g.First())
                               .OrderBy(d => d.Nome)
                               .ToList();
            
            // Verificar se o departamento de TI está na lista
            if (!result.Any(d => d.Nome == "TI"))
            {
                Console.WriteLine("Departamento de TI não encontrado no banco de dados. Adicionando manualmente.");
                
                // Tentar obter o ID do departamento de TI diretamente do banco
                int tiId = 0;
                try
                {
                    using (MySqlConnection conn = ConexaoUniversal.GetConnection())
                    {
                        string insertSql = "INSERT INTO departamentos (nome, descricao) VALUES ('TI', 'Tecnologia da Informação'); SELECT LAST_INSERT_ID();";
                        using (MySqlCommand cmd = new MySqlCommand(insertSql, conn))
                        {
                            tiId = Convert.ToInt32(cmd.ExecuteScalar());
                            Console.WriteLine($"Departamento de TI criado com ID: {tiId}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao criar departamento de TI: {ex.Message}");
                    // Se não conseguir criar, usar um ID temporário
                    tiId = -1;
                }
                
                // Adicionar o departamento de TI à lista
                result.Add(new Departamento
                {
                    Id = tiId,
                    Nome = "TI",
                    DataCriacao = DateTime.Now
                });
                
                // Reordenar a lista
                result = result.OrderBy(d => d.Nome).ToList();
            }
            
            return result;
        }

        public Departamento? ObterDepartamentoPorId(int id)
        {
            string sql = "SELECT * FROM departamentos WHERE id = @id";
            Departamento? departamento = null;

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
                                departamento = new Departamento
                                {
                                    Id = Convert.ToInt32(reader["id"]),
                                    Nome = reader["nome"].ToString() ?? string.Empty,
                                    DataCriacao = reader["data_criacao"] != DBNull.Value ? Convert.ToDateTime(reader["data_criacao"]) : DateTime.Now
                                };
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao obter departamento: " + ex.Message);
                }
            }
            return departamento;
        }
    }
}
