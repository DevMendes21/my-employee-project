using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using MyEmployeeProject.Models;
using MyEmployeeProject.Conexao;

namespace MyEmployeeProject.DAO
{
    public class FuncionarioDAO
    {
        public void InserirFuncionario(Funcionario funcionario)
        {
            // Garantir que o funcionário tenha um cargo e departamento válidos
            if (funcionario.CargoId <= 0)
            {
                throw new Exception("Cargo inválido. Selecione um cargo válido para o funcionário.");
            }

            if (funcionario.DepartamentoId <= 0)
            {
                throw new Exception("Departamento inválido. Selecione um departamento válido para o funcionário.");
            }

            // Verificar se o cargo existe
            CargoDAO cargoDAO = new CargoDAO();
            var cargo = cargoDAO.ObterCargoPorId(funcionario.CargoId);
            if (cargo == null)
            {
                throw new Exception($"Cargo com ID {funcionario.CargoId} não encontrado.");
            }

            // Verificar se o departamento existe
            DepartamentoDAO departamentoDAO = new DepartamentoDAO();
            var departamento = departamentoDAO.ObterDepartamentoPorId(funcionario.DepartamentoId);
            if (departamento == null)
            {
                throw new Exception($"Departamento com ID {funcionario.DepartamentoId} não encontrado.");
            }

            // Log para debug
            Console.WriteLine($"Inserindo funcionário {funcionario.Nome} com cargo {cargo.Nome} (ID: {funcionario.CargoId}) no departamento {departamento.Nome} (ID: {funcionario.DepartamentoId})");

            string sql = "INSERT INTO funcionarios (nome, email, telefone, cargo_id, departamento_id, salario, data_contratacao, data_nascimento, observacoes, status) " +
                        "VALUES (@nome, @email, @telefone, @cargoId, @departamentoId, @salario, @dataContratacao, @dataNascimento, @observacoes, @status)";

            using (MySqlConnection conn = MyEmployeeProject.Conexao.Conexao.GetConnection())
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
                        cmd.Parameters.AddWithValue("@status", funcionario.Status.ToString());
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (MySqlException ex)
                {
                    throw new Exception($"Erro ao inserir funcionário no banco de dados: {ex.Message}. Cargo ID: {funcionario.CargoId}, Departamento ID: {funcionario.DepartamentoId}");
                }
                catch (Exception ex)
                {
                    throw new Exception($"Erro ao inserir funcionário: {ex.Message}");
                }
            }
        }

        public List<Funcionario> ListarFuncionarios()
        {
            List<Funcionario> funcionarios = new List<Funcionario>();
            string sql = @"SELECT f.*, 
                        c.nome as cargo_nome,
                        d.nome as departamento_nome,
                        TIMESTAMPDIFF(YEAR, f.data_nascimento, CURDATE()) as idade,
                        f.status
                        FROM funcionarios f
                        LEFT JOIN cargos c ON f.cargo_id = c.id
                        LEFT JOIN departamentos d ON f.departamento_id = d.id";

            using (MySqlConnection conn = MyEmployeeProject.Conexao.Conexao.GetConnection())
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
                                    Email = reader["email"] != DBNull.Value ? reader["email"].ToString() ?? string.Empty : string.Empty,
                                    Telefone = reader["telefone"] != DBNull.Value ? reader["telefone"].ToString() ?? string.Empty : string.Empty,
                                    CargoId = Convert.ToInt32(reader["cargo_id"]),
                                    Cargo = new Cargo
                                    {
                                        Id = Convert.ToInt32(reader["cargo_id"]),
                                        Nome = reader["cargo_nome"].ToString() ?? string.Empty,
                                        Nivel = "N/A" // Setting a default value for required field
                                    },
                                    DepartamentoId = Convert.ToInt32(reader["departamento_id"]),
                                    Departamento = new Departamento
                                    {
                                        Id = Convert.ToInt32(reader["departamento_id"]),
                                        Nome = reader["departamento_nome"].ToString() ?? string.Empty,
                                        DataCriacao = DateTime.Now // Setting a default value
                                    },
                                    Salario = Convert.ToDecimal(reader["salario"]),
                                    DataContratacao = Convert.ToDateTime(reader["data_contratacao"]),
                                    DataNascimento = reader["data_nascimento"] != DBNull.Value ? Convert.ToDateTime(reader["data_nascimento"]) : null,
                                    Observacoes = reader["observacoes"] != DBNull.Value ? reader["observacoes"].ToString() : null,
                                    Status = Enum.TryParse<StatusFuncionario>(reader["status"].ToString(), out var status) ? status : StatusFuncionario.Ativo
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
            // Garantir que o funcionário tenha um cargo e departamento válidos
            if (funcionario.CargoId <= 0)
            {
                throw new Exception("Cargo inválido. Selecione um cargo válido para o funcionário.");
            }

            if (funcionario.DepartamentoId <= 0)
            {
                throw new Exception("Departamento inválido. Selecione um departamento válido para o funcionário.");
            }

            // Verificar se o cargo existe
            CargoDAO cargoDAO = new CargoDAO();
            var cargo = cargoDAO.ObterCargoPorId(funcionario.CargoId);
            if (cargo == null)
            {
                throw new Exception($"Cargo com ID {funcionario.CargoId} não encontrado.");
            }

            // Verificar se o departamento existe
            DepartamentoDAO departamentoDAO = new DepartamentoDAO();
            var departamento = departamentoDAO.ObterDepartamentoPorId(funcionario.DepartamentoId);
            if (departamento == null)
            {
                throw new Exception($"Departamento com ID {funcionario.DepartamentoId} não encontrado.");
            }

            // Log para debug
            Console.WriteLine($"Atualizando funcionário {funcionario.Nome} (ID: {funcionario.Id}) com cargo {cargo.Nome} (ID: {funcionario.CargoId}) no departamento {departamento.Nome} (ID: {funcionario.DepartamentoId})");

            string sql = @"UPDATE funcionarios 
                         SET nome = @nome, 
                             email = @email,
                             telefone = @telefone,
                             cargo_id = @cargoId, 
                             departamento_id = @departamentoId, 
                             salario = @salario, 
                             data_contratacao = @dataContratacao,
                             data_nascimento = @dataNascimento,
                             status = @status,
                             observacoes = @observacoes
                         WHERE id = @id";

            using (MySqlConnection conn = MyEmployeeProject.Conexao.Conexao.GetConnection())
            {
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", funcionario.Id);
                        cmd.Parameters.AddWithValue("@nome", funcionario.Nome);
                        cmd.Parameters.AddWithValue("@email", funcionario.Email ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@telefone", funcionario.Telefone ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@cargoId", funcionario.CargoId);
                        cmd.Parameters.AddWithValue("@departamentoId", funcionario.DepartamentoId);
                        cmd.Parameters.AddWithValue("@salario", funcionario.Salario);
                        cmd.Parameters.AddWithValue("@dataContratacao", funcionario.DataContratacao);
                        cmd.Parameters.AddWithValue("@dataNascimento", funcionario.DataNascimento.HasValue ? (object)funcionario.DataNascimento.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@status", funcionario.Status.ToString());
                        cmd.Parameters.AddWithValue("@observacoes", funcionario.Observacoes ?? (object)DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (MySqlException ex)
                {
                    throw new Exception($"Erro ao atualizar funcionário no banco de dados: {ex.Message}. Cargo ID: {funcionario.CargoId}, Departamento ID: {funcionario.DepartamentoId}");
                }
                catch (Exception ex)
                {
                    throw new Exception($"Erro ao atualizar funcionário: {ex.Message}");
                }
            }
        }

        public void RemoverFuncionario(int id)
        {
            string sql = "DELETE FROM funcionarios WHERE id = @id";

            using (MySqlConnection conn = MyEmployeeProject.Conexao.Conexao.GetConnection())
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

        public void ExcluirFuncionario(int id)
        {
            RemoverFuncionario(id);
        }

        public int ContarFuncionarios()
        {
            string sql = "SELECT COUNT(*) FROM funcionarios";
            
            using (MySqlConnection conn = MyEmployeeProject.Conexao.Conexao.GetConnection())
            {
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao contar funcionários: " + ex.Message);
                }
            }
        }

        public Funcionario? ObterFuncionarioPorId(int id)
        {
            string sql = @"SELECT f.*, 
                        c.nome as cargo_nome, c.nivel as cargo_nivel, c.salario_base,
                        d.nome as departamento_nome, d.descricao as departamento_descricao
                        FROM funcionarios f
                        LEFT JOIN cargos c ON f.cargo_id = c.id
                        LEFT JOIN departamentos d ON f.departamento_id = d.id
                        WHERE f.id = @id";

            using (MySqlConnection conn = MyEmployeeProject.Conexao.Conexao.GetConnection())
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
                                return new Funcionario
                                {
                                    Id = reader.GetInt32("id"),
                                    Nome = reader.GetString("nome"),
                                    Email = reader.IsDBNull("email") ? string.Empty : reader.GetString("email"),
                                    Telefone = reader.IsDBNull("telefone") ? string.Empty : reader.GetString("telefone"),
                                    CargoId = reader.GetInt32("cargo_id"),
                                    DepartamentoId = reader.GetInt32("departamento_id"),
                                    Salario = reader.GetDecimal("salario"),
                                    DataContratacao = reader.GetDateTime("data_contratacao"),
                                    DataNascimento = reader.IsDBNull("data_nascimento") ? null : reader.GetDateTime("data_nascimento"),
                                    Status = Enum.Parse<StatusFuncionario>(reader.GetString("status")),
                                    Observacoes = reader.IsDBNull("observacoes") ? null : reader.GetString("observacoes"),
                                    DataCriacao = reader.GetDateTime("data_criacao"),
                                    DataAtualizacao = reader.GetDateTime("data_atualizacao"),
                                    Cargo = reader.IsDBNull("cargo_nome") ? null : new Cargo
                                    {
                                        Id = reader.GetInt32("cargo_id"),
                                        Nome = reader.GetString("cargo_nome"),
                                        Nivel = reader.IsDBNull("cargo_nivel") ? string.Empty : reader.GetString("cargo_nivel"),
                                        SalarioBase = reader.IsDBNull("salario_base") ? 0 : reader.GetDecimal("salario_base")
                                    },
                                    Departamento = reader.IsDBNull("departamento_nome") ? null : new Departamento
                                    {
                                        Id = reader.GetInt32("departamento_id"),
                                        Nome = reader.GetString("departamento_nome"),
                                        Descricao = reader.IsDBNull("departamento_descricao") ? null : reader.GetString("departamento_descricao")
                                    }
                                };
                            }
                            return null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao buscar funcionário: " + ex.Message);
                }
            }
        }
    }
}
