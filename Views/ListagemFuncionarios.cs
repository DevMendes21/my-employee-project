using System;
using System.Windows.Forms;
using MinhaEmpresa.Models;
using MinhaEmpresa.DAO;

namespace MinhaEmpresa.Views
{
    public partial class ListagemFuncionarios : Form
    {
        private DataGridView dgvFuncionarios;
        private Button btnEditar;
        private Button btnRemover;
        private Button btnAtualizar;
        private FuncionarioDAO funcionarioDAO;

        public ListagemFuncionarios()
        {
            InitializeComponent();
            InitializeCustomComponents();
            funcionarioDAO = new FuncionarioDAO();
            CarregarFuncionarios();
        }

        private void InitializeComponent()
        {
            this.Text = "Listagem de Funcionários";
            this.Width = 800;
            this.Height = 600;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
        }

        private void InitializeCustomComponents()
        {
            // DataGridView
            dgvFuncionarios = new DataGridView
            {
                Location = new System.Drawing.Point(20, 20),
                Width = 740,
                Height = 450,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            // Configurar colunas
            dgvFuncionarios.AutoGenerateColumns = false;
            
            dgvFuncionarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Id",
                HeaderText = "ID",
                Width = 50
            });
            
            dgvFuncionarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Nome",
                HeaderText = "Nome",
                Width = 150
            });
            
            dgvFuncionarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Email",
                HeaderText = "Email",
                Width = 150
            });
            
            dgvFuncionarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Telefone",
                HeaderText = "Telefone",
                Width = 100
            });
            
            dgvFuncionarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Cargo.Nome",
                HeaderText = "Cargo",
                Width = 100
            });
            
            dgvFuncionarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Salario",
                HeaderText = "Salário",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
            });
            
            dgvFuncionarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Departamento.Nome",
                HeaderText = "Departamento",
                Width = 120
            });
            
            dgvFuncionarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Status",
                HeaderText = "Status",
                Width = 80
            });

            // Buttons
            btnEditar = new Button
            {
                Text = "Editar",
                Location = new System.Drawing.Point(20, 490),
                Width = 100,
                Height = 30,
                BackColor = System.Drawing.Color.FromArgb(0, 120, 212),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnEditar.Click += BtnEditar_Click;

            btnRemover = new Button
            {
                Text = "Remover",
                Location = new System.Drawing.Point(140, 490),
                Width = 100,
                Height = 30,
                BackColor = System.Drawing.Color.FromArgb(220, 53, 69),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRemover.Click += BtnRemover_Click;

            btnAtualizar = new Button
            {
                Text = "Atualizar",
                Location = new System.Drawing.Point(260, 490),
                Width = 100,
                Height = 30,
                BackColor = System.Drawing.Color.FromArgb(40, 167, 69),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAtualizar.Click += (s, e) => CarregarFuncionarios();

            // Adicionar controles ao formulário
            this.Controls.AddRange(new Control[] { dgvFuncionarios, btnEditar, btnRemover, btnAtualizar });
        }

        private void CarregarFuncionarios()
        {
            try
            {
                var funcionarios = funcionarioDAO.ListarFuncionarios();
                dgvFuncionarios.DataSource = funcionarios;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar funcionários: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEditar_Click(object sender, EventArgs e)
        {
            if (dgvFuncionarios.CurrentRow != null)
            {
                var funcionario = (Funcionario)dgvFuncionarios.CurrentRow.DataBoundItem;
                var formEditar = new CadastroFuncionario(funcionario);
                formEditar.ShowDialog();
                CarregarFuncionarios(); // Recarrega a lista após edição
            }
            else
            {
                MessageBox.Show("Selecione um funcionário para editar!", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnRemover_Click(object sender, EventArgs e)
        {
            if (dgvFuncionarios.CurrentRow != null)
            {
                var funcionario = (Funcionario)dgvFuncionarios.CurrentRow.DataBoundItem;
                var result = MessageBox.Show($"Deseja realmente remover o funcionário {funcionario.Nome}?",
                    "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        funcionarioDAO.RemoverFuncionario(funcionario.Id);
                        MessageBox.Show("Funcionário removido com sucesso!", "Sucesso",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        CarregarFuncionarios();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao remover funcionário: {ex.Message}", "Erro",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Selecione um funcionário para remover!", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
