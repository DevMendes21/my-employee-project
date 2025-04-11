using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using MinhaEmpresa.Models;
using MinhaEmpresa.Conexao;

namespace MinhaEmpresa.DAO
{
    public class DepartamentoDAO
    {
        public List<Departamento> ListarDepartamentos()
        {
            List<Departamento> departamentos = new List<Departamento>();
            string sql = "SELECT DISTINCT id, nome, data_criacao FROM departamentos ORDER BY nome";

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
            return departamentos.GroupBy(d => d.Nome)
                               .Select(g => g.First())
                               .OrderBy(d => d.Nome)
                               .ToList();
        }

        public Departamento? ObterDepartamentoPorId(int id)
        {
            string sql = "SELECT * FROM departamentos WHERE id = @id";
            Departamento? departamento = null;

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
