using System;
using System.Windows.Forms;
using MinhaEmpresa.Models;
using MinhaEmpresa.DAO;

namespace MinhaEmpresa.Views
{
    public partial class ListagemFuncionarios : Form
    {
        private DataGridView dgvFuncionarios = new();
        private Button btnEditar = new();
        private Button btnRemover = new();
        private Button btnAtualizar = new();
        private readonly FuncionarioDAO funcionarioDAO = new();

        public ListagemFuncionarios()
        {
            InitializeComponent();
            InitializeCustomComponents();
            CarregarFuncionarios();
        }

        private void InitializeComponent()
        {
            this.Text = "Listagem de Funcionários";
            this.Width = 1000;
            this.Height = 600;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = System.Drawing.Color.White;
        }

        private void InitializeCustomComponents()
        {
            // DataGridView
            dgvFuncionarios = new DataGridView
            {
                Location = new System.Drawing.Point(20, 20),
                Width = 940,
                Height = 450,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                BackgroundColor = System.Drawing.Color.White,
                BorderStyle = BorderStyle.None,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = System.Drawing.Color.FromArgb(0, 120, 212),
                    ForeColor = System.Drawing.Color.White,
                    Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold),
                    Padding = new Padding(10, 5, 10, 5)
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new System.Drawing.Font("Segoe UI", 9.75F),
                    Padding = new Padding(5),
                    SelectionBackColor = System.Drawing.Color.FromArgb(0, 120, 212),
                    SelectionForeColor = System.Drawing.Color.White
                },
                RowHeadersVisible = false,
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = System.Drawing.Color.FromArgb(240, 240, 240)
                },
                EnableHeadersVisualStyles = false
            };

            // Configurar colunas
            dgvFuncionarios.AutoGenerateColumns = false;
            
            // Configurar colunas do DataGridView
            var colunas = new[]
            {
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Id",
                    HeaderText = "ID",
                    Width = 50
                },
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Nome",
                    HeaderText = "Nome",
                    Width = 200
                },
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Cargo.Nome",
                    HeaderText = "Cargo",
                    Width = 120
                },
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Departamento.Nome",
                    HeaderText = "Departamento",
                    Width = 120
                },
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Salario",
                    HeaderText = "Salário",
                    Width = 100,
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        Format = "C2",
                        Alignment = DataGridViewContentAlignment.MiddleRight
                    }
                },
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Email",
                    HeaderText = "Email",
                    Width = 180
                },
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Telefone",
                    HeaderText = "Telefone",
                    Width = 100
                },
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Status",
                    HeaderText = "Status",
                    Width = 70
                }
            };

            // Add all columns at once
            dgvFuncionarios.Columns.AddRange(colunas);
            
            dgvFuncionarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "DataContratacao",
                HeaderText = "Data Contratação",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }
            });

            dgvFuncionarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "DataNascimento",
                HeaderText = "Data Nascimento",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy", NullValue = "" }
            });
            
            // Add age column using the Idade property
            dgvFuncionarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Idade",
                HeaderText = "Idade",
                Width = 60,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
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

        private void BtnEditar_Click(object? sender, EventArgs e)
        {
            if (dgvFuncionarios.CurrentRow != null)
            {
                var funcionario = (Funcionario)dgvFuncionarios.CurrentRow.DataBoundItem;
                var formEditar = new CadastroFuncionario(funcionario);
                formEditar.FormClosed += (s, args) => CarregarFuncionarios(); // Recarrega após fechar o form
                formEditar.Show(); // Usar Show() em vez de ShowDialog() para melhor experiência
            }
            else
            {
                MessageBox.Show("Selecione um funcionário para editar!", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnRemover_Click(object? sender, EventArgs e)
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
