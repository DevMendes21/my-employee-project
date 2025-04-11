using System;
using System.Windows.Forms;
using System.Collections.Generic;
using MinhaEmpresa.Models;
using MinhaEmpresa.DAO;
using MinhaEmpresa.Conexao;

namespace MinhaEmpresa
{
    public class TestRoleAssignment : Form
    {
        private ComboBox cmbCargo = null!;
        private ComboBox cmbDepartamento = null!;
        private Button btnTestar = null!;
        private Button btnSalvar = null!;
        private Label lblResultado = null!;
        private TextBox txtNome = null!;

        public TestRoleAssignment()
        {
            InitializeComponents();
            LoadData();
        }

        private void InitializeComponents()
        {
            this.Text = "Teste de Atribuição de Cargos";
            this.Width = 500;
            this.Height = 300;
            this.StartPosition = FormStartPosition.CenterScreen;

            Label lblNome = new Label { Text = "Nome:", Location = new System.Drawing.Point(20, 20) };
            txtNome = new TextBox { Location = new System.Drawing.Point(150, 20), Width = 300 };

            Label lblCargo = new Label { Text = "Cargo:", Location = new System.Drawing.Point(20, 60) };
            cmbCargo = new ComboBox
            {
                Location = new System.Drawing.Point(150, 60),
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            Label lblDepartamento = new Label { Text = "Departamento:", Location = new System.Drawing.Point(20, 100) };
            cmbDepartamento = new ComboBox
            {
                Location = new System.Drawing.Point(150, 100),
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            btnTestar = new Button
            {
                Text = "Testar Combinação",
                Location = new System.Drawing.Point(150, 140),
                Width = 150
            };
            btnTestar.Click += BtnTestar_Click;

            btnSalvar = new Button
            {
                Text = "Salvar Funcionário",
                Location = new System.Drawing.Point(310, 140),
                Width = 140
            };
            btnSalvar.Click += BtnSalvar_Click;

            lblResultado = new Label
            {
                Text = "Resultado do teste aparecerá aqui",
                Location = new System.Drawing.Point(20, 180),
                Width = 450,
                Height = 60,
                BorderStyle = BorderStyle.FixedSingle
            };

            this.Controls.AddRange(new Control[]
            {
                lblNome, txtNome,
                lblCargo, cmbCargo,
                lblDepartamento, cmbDepartamento,
                btnTestar, btnSalvar, lblResultado
            });
        }

        private void LoadData()
        {
            // Carregar cargos manualmente para garantir que não haja duplicatas
            var cargos = new List<Cargo>
            {
                new Cargo { Id = 1, Nome = "Analista", Nivel = "Junior" },
                new Cargo { Id = 2, Nome = "Desenvolvedor", Nivel = "Pleno" },
                new Cargo { Id = 3, Nome = "Gerente", Nivel = "Senior" },
                new Cargo { Id = 4, Nome = "Coordenador", Nivel = "Senior" },
                new Cargo { Id = 5, Nome = "Assistente", Nivel = "Junior" }
            };
            cmbCargo.DataSource = cargos;
            cmbCargo.DisplayMember = "Nome";
            cmbCargo.ValueMember = "Id";

            // Carregar departamentos manualmente para garantir que não haja duplicatas
            var departamentos = new List<Departamento>
            {
                new Departamento { Id = 1, Nome = "TI", DataCriacao = DateTime.Now },
                new Departamento { Id = 2, Nome = "RH", DataCriacao = DateTime.Now },
                new Departamento { Id = 3, Nome = "Financeiro", DataCriacao = DateTime.Now },
                new Departamento { Id = 4, Nome = "Comercial", DataCriacao = DateTime.Now },
                new Departamento { Id = 5, Nome = "Administrativo", DataCriacao = DateTime.Now }
            };
            cmbDepartamento.DataSource = departamentos;
            cmbDepartamento.DisplayMember = "Nome";
            cmbDepartamento.ValueMember = "Id";
        }

        private void BtnTestar_Click(object? sender, EventArgs e)
        {
            try
            {
                var cargo = cmbCargo.SelectedItem as Cargo;
                var departamento = cmbDepartamento.SelectedItem as Departamento;

                if (cargo == null || departamento == null)
                {
                    lblResultado.Text = "Selecione um cargo e um departamento para testar.";
                    return;
                }

                // Verificar se existe alguma restrição no banco de dados
                using (var conn = MinhaEmpresa.Conexao.Conexao.GetConnection())
                {
                    string sql = "SELECT COUNT(*) FROM funcionarios WHERE cargo_id = @cargoId AND departamento_id = @deptoId";
                    using (var cmd = new MySql.Data.MySqlClient.MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@cargoId", cargo.Id);
                        cmd.Parameters.AddWithValue("@deptoId", departamento.Id);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        lblResultado.Text = $"Combinação testada: {cargo.Nome} no departamento {departamento.Nome}\n" +
                                           $"Resultado: {(count > 0 ? $"{count} funcionários já usam esta combinação" : "Nenhum funcionário usa esta combinação")}\n" +
                                           $"Esta combinação deve ser válida.";
                    }
                }
            }
            catch (Exception ex)
            {
                lblResultado.Text = $"Erro ao testar: {ex.Message}";
            }
        }

        private void BtnSalvar_Click(object? sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtNome.Text))
                {
                    MessageBox.Show("Digite um nome para o funcionário", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var cargo = cmbCargo.SelectedItem as Cargo;
                var departamento = cmbDepartamento.SelectedItem as Departamento;

                if (cargo == null || departamento == null)
                {
                    MessageBox.Show("Selecione um cargo e um departamento", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Criar um funcionário de teste com a combinação selecionada
                var funcionario = new Funcionario
                {
                    Nome = txtNome.Text,
                    Email = "teste@teste.com",
                    Telefone = "(00) 00000-0000",
                    CargoId = cargo.Id,
                    Cargo = cargo,
                    Salario = 1000m,
                    DepartamentoId = departamento.Id,
                    Departamento = departamento,
                    DataContratacao = DateTime.Now,
                    DataNascimento = DateTime.Now.AddYears(-30),
                    Status = StatusFuncionario.Ativo
                };

                // Salvar no banco de dados
                var dao = new FuncionarioDAO();
                dao.InserirFuncionario(funcionario);

                MessageBox.Show($"Funcionário {txtNome.Text} cadastrado com sucesso como {cargo.Nome} no departamento {departamento.Nome}!",
                    "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                lblResultado.Text = $"Funcionário cadastrado com sucesso!\n" +
                                   $"Cargo: {cargo.Nome} (ID: {cargo.Id})\n" +
                                   $"Departamento: {departamento.Nome} (ID: {departamento.Id})";
            }
            catch (Exception ex)
            {
                lblResultado.Text = $"Erro ao salvar: {ex.Message}";
                MessageBox.Show($"Erro ao cadastrar funcionário: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Main method removed to avoid multiple entry points
    }
}
