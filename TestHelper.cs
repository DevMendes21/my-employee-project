using System;
using System.Windows.Forms;
using MinhaEmpresa.Models;
using MinhaEmpresa.DAO;

namespace MinhaEmpresa
{
    public static class TestHelper
    {
        public static void TestRoleAssignment()
        {
            try
            {
                // Criar um funcionário com cargo de Gerente no departamento Financeiro
                var funcionario = new Funcionario
                {
                    Nome = "Teste Gerente Financeiro",
                    Email = "teste@financeiro.com",
                    Telefone = "(00) 00000-0000",
                    CargoId = 3, // ID do cargo Gerente
                    Salario = 12000m,
                    DepartamentoId = 3, // ID do departamento Financeiro
                    DataContratacao = DateTime.Now,
                    DataNascimento = DateTime.Now.AddYears(-35),
                    Status = StatusFuncionario.Ativo,
                    Observacoes = "Funcionário de teste para verificar a atribuição de Gerente ao departamento Financeiro"
                };

                // Inserir no banco de dados
                var dao = new FuncionarioDAO();
                dao.InserirFuncionario(funcionario);

                MessageBox.Show("Funcionário de teste criado com sucesso!\n" +
                               "Cargo: Gerente\n" +
                               "Departamento: Financeiro", 
                               "Teste Concluído", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao criar funcionário de teste: {ex.Message}", 
                               "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
