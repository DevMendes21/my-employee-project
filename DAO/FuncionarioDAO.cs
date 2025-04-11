using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using MinhaEmpresa.Models;
using MinhaEmpresa.Conexao;

namespace MinhaEmpresa.DAO
{
    public class FuncionarioDAO
    {
        public void InserirFuncionario(Funcionario funcionario)
        {
            string sql = "INSERT INTO funcionarios (nome, email, telefone, cargo_id, departamento_id, salario, data_contratacao, data_nascimento, observacoes) " +
                        "VALUES (@nome, @email, @telefone, @cargoId, @departamentoId, @salario, @dataContratacao, @dataNascimento, @observacoes)";

            using (MySqlConnection conn = MinhaEmpresa.Conexao.Conexao.GetConnection())
            {
                try
                {
                    // A conexão já é aberta no GetConnection()
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@nome", funcionario.Nome);
                        cmd.Parameters.AddWithValue("@email", funcionario.Email ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@telefone", funcionario.Telefone ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@cargoId", funcionario.CargoId);
                        cmd.Parameters.AddWithValue("@departamentoId", funcionario.DepartamentoId);
                        cmd.Parameters.AddWithValue("@salario", funcionario.Salario);
                        cmd.Parameters.AddWithValue("@dataContratacao", funcionario.DataContratacao);
                        cmd.Parameters.AddWithValue("@dataNascimento", funcionario.DataNascimento.HasValue ? (object)funcionario.DataNascimento.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@observacoes", funcionario.Observacoes ?? (object)DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao inserir funcionário: " + ex.Message);
                }
            }
        }

        public List<Funcionario> ListarFuncionarios()
        {
            List<Funcionario> funcionarios = new List<Funcionario>();
            string sql = @"SELECT f.*, 
                        c.nome as cargo_nome,
                        d.nome as departamento_nome,
                        TIMESTAMPDIFF(YEAR, f.data_nascimento, CURDATE()) as idade
                        FROM funcionarios f
                        INNER JOIN cargos c ON f.cargo_id = c.id
                        INNER JOIN departamentos d ON f.departamento_id = d.id";

            using (MySqlConnection conn = MinhaEmpresa.Conexao.Conexao.GetConnection())
            {
                try
                {
                    // A conexão já é aberta no GetConnection()
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Funcionario func = new Funcionario
                                {
                                    Id = Convert.ToInt32(reader["id"]),
                                    Nome = reader["nome"].ToString() ?? string.Empty,
                                    Email = reader["email"] != DBNull.Value ? reader["email"].ToString() : null,
                                    Telefone = reader["telefone"] != DBNull.Value ? reader["telefone"].ToString() : null,
                                    CargoId = Convert.ToInt32(reader["cargo_id"]),
                                    Cargo = new Cargo
                                    {
                                        Id = Convert.ToInt32(reader["cargo_id"]),
                                        Nome = reader["cargo_nome"].ToString() ?? string.Empty
                                    },
                                    DepartamentoId = Convert.ToInt32(reader["departamento_id"]),
                                    Departamento = new Departamento
                                    {
                                        Id = Convert.ToInt32(reader["departamento_id"]),
                                        Nome = reader["departamento_nome"].ToString() ?? string.Empty
                                    },
                                    Salario = Convert.ToDecimal(reader["salario"]),
                                    DataContratacao = Convert.ToDateTime(reader["data_contratacao"]),
                                    DataNascimento = reader["data_nascimento"] != DBNull.Value ? Convert.ToDateTime(reader["data_nascimento"]) : null,
                                    Observacoes = reader["observacoes"] != DBNull.Value ? reader["observacoes"].ToString() : null
                                };
                                funcionarios.Add(func);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao listar funcionários: " + ex.Message);
                }
            }
            return funcionarios;
        }

        public void AtualizarFuncionario(Funcionario funcionario)
        {
            string sql = "UPDATE funcionarios SET nome = @nome, cargo_id = @cargoId, departamento_id = @departamentoId, " +
                        "salario = @salario, data_contratacao = @dataContratacao WHERE id = @id";

            using (MySqlConnection conn = MinhaEmpresa.Conexao.Conexao.GetConnection())
            {
                try
                {
                    // A conexão já é aberta no GetConnection()
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", funcionario.Id);
                        cmd.Parameters.AddWithValue("@nome", funcionario.Nome);
                        cmd.Parameters.AddWithValue("@cargoId", funcionario.CargoId);
                        cmd.Parameters.AddWithValue("@departamentoId", funcionario.DepartamentoId);
                        cmd.Parameters.AddWithValue("@salario", funcionario.Salario);
                        cmd.Parameters.AddWithValue("@dataContratacao", funcionario.DataContratacao);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao atualizar funcionário: " + ex.Message);
                }
            }
        }

        public void RemoverFuncionario(int id)
        {
            string sql = "DELETE FROM funcionarios WHERE id = @id";

            using (MySqlConnection conn = MinhaEmpresa.Conexao.Conexao.GetConnection())
            {
                try
                {
                    // A conexão já é aberta no GetConnection()
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao remover funcionário: " + ex.Message);
                }
            }
        }
    }
}
