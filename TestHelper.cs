using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using MinhaEmpresa.Models;
using MinhaEmpresa.DAO;

namespace MinhaEmpresa
{
    public static class TestHelper
    {
        // Lista de cargos para teste
        private static readonly List<Cargo> Cargos = new List<Cargo>
        {
            new Cargo { Id = 1, Nome = "Analista", Nivel = "Junior" },
            new Cargo { Id = 2, Nome = "Desenvolvedor", Nivel = "Pleno" },
            new Cargo { Id = 3, Nome = "Gerente", Nivel = "Senior" },
            new Cargo { Id = 4, Nome = "Coordenador", Nivel = "Senior" },
            new Cargo { Id = 5, Nome = "Assistente", Nivel = "Junior" }
        };

        // Lista de departamentos para teste
        private static readonly List<Departamento> Departamentos = new List<Departamento>
        {
            new Departamento { Id = 1, Nome = "TI", DataCriacao = DateTime.Now },
            new Departamento { Id = 2, Nome = "RH", DataCriacao = DateTime.Now },
            new Departamento { Id = 3, Nome = "Financeiro", DataCriacao = DateTime.Now },
            new Departamento { Id = 4, Nome = "Administrativo", DataCriacao = DateTime.Now },
            new Departamento { Id = 5, Nome = "Comercial", DataCriacao = DateTime.Now }
        };

        public static void TestRoleAssignment()
        {
            try
            {
                // Testar combinações específicas que eram problemáticas antes
                TestSpecificCombination(3, 3); // Gerente no Financeiro
                TestSpecificCombination(3, 2); // Gerente no RH
                TestSpecificCombination(4, 3); // Coordenador no Financeiro

                MessageBox.Show("Testes de combinações específicas concluídos com sucesso!", 
                               "Testes Concluídos", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao executar testes de combinações específicas: {ex.Message}", 
                               "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void TestAllCombinations()
        {
            try
            {
                StringBuilder results = new StringBuilder();
                int successCount = 0;
                int failCount = 0;
                List<string> failures = new List<string>();

                // Testar todas as combinações possíveis de cargo e departamento
                foreach (var cargo in Cargos)
                {
                    foreach (var departamento in Departamentos)
                    {
                        try
                        {
                            // Criar um funcionário com esta combinação
                            var funcionario = new Funcionario
                            {
                                Nome = $"Teste {cargo.Nome} {departamento.Nome}",
                                Email = $"teste.{cargo.Nome.ToLower()}@{departamento.Nome.ToLower()}.com",
                                Telefone = "(00) 00000-0000",
                                CargoId = cargo.Id,
                                Salario = 5000m,
                                DepartamentoId = departamento.Id,
                                DataContratacao = DateTime.Now,
                                DataNascimento = DateTime.Now.AddYears(-30),
                                Status = StatusFuncionario.Ativo,
                                Observacoes = $"Funcionário de teste para combinação {cargo.Nome} no {departamento.Nome}"
                            };

                            // Inserir no banco de dados
                            var dao = new FuncionarioDAO();
                            dao.InserirFuncionario(funcionario);
                            successCount++;
                            results.AppendLine($"Sucesso: {cargo.Nome} no {departamento.Nome}");
                        }
                        catch (Exception ex)
                        {
                            failCount++;
                            string error = $"Falha: {cargo.Nome} no {departamento.Nome} - Erro: {ex.Message}";
                            failures.Add(error);
                            results.AppendLine(error);
                        }
                    }
                }

                // Exibir resultados
                StringBuilder messageBuilder = new StringBuilder();
                messageBuilder.AppendLine("Testes concluídos!");
                messageBuilder.AppendLine();
                messageBuilder.AppendLine($"Sucesso: {successCount}");
                messageBuilder.AppendLine($"Falhas: {failCount}");
                messageBuilder.AppendLine();
                
                if (failCount > 0)
                {
                    messageBuilder.AppendLine("Falhas encontradas:");
                    foreach (var failure in failures)
                    {
                        messageBuilder.AppendLine(failure);
                    }
                }
                else
                {
                    messageBuilder.AppendLine("Todas as combinações de cargo e departamento foram bem-sucedidas!");
                }
                
                string message = messageBuilder.ToString();

                MessageBox.Show(message, "Resultado dos Testes", MessageBoxButtons.OK, 
                    failCount > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao executar testes de todas as combinações: {ex.Message}", 
                               "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void TestSpecificCombination(int cargoId, int departamentoId)
        {
            // Obter informações do cargo e departamento
            var cargoDAO = new CargoDAO();
            var departamentoDAO = new DepartamentoDAO();
            var cargo = cargoDAO.ObterCargoPorId(cargoId);
            var departamento = departamentoDAO.ObterDepartamentoPorId(departamentoId);

            if (cargo == null || departamento == null)
            {
                throw new Exception($"Cargo ID {cargoId} ou Departamento ID {departamentoId} não encontrado.");
            }

            // Criar um funcionário com esta combinação
            var funcionario = new Funcionario
            {
                Nome = $"Teste {cargo.Nome} {departamento.Nome}",
                Email = $"teste.{cargo.Nome.ToLower()}@{departamento.Nome.ToLower()}.com",
                Telefone = "(00) 00000-0000",
                CargoId = cargoId,
                Salario = 10000m,
                DepartamentoId = departamentoId,
                DataContratacao = DateTime.Now,
                DataNascimento = DateTime.Now.AddYears(-35),
                Status = StatusFuncionario.Ativo,
                Observacoes = $"Funcionário de teste para verificar a atribuição de {cargo.Nome} ao departamento {departamento.Nome}"
            };

            // Inserir no banco de dados
            var dao = new FuncionarioDAO();
            dao.InserirFuncionario(funcionario);

            Console.WriteLine($"Funcionário de teste criado com sucesso! Cargo: {cargo.Nome}, Departamento: {departamento.Nome}");
        }
    }
}
