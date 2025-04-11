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
        private bool isFirstLoad = true;

        public ListagemFuncionarios()
        {
            InitializeComponent();
            InitializeCustomComponents();
            // Initial load will happen in the Shown event
            this.Shown += ListagemFuncionarios_Shown;
        }
        
        private void ListagemFuncionarios_Shown(object? sender, EventArgs e)
        {
            // Only reload on first show or when explicitly requested
            if (isFirstLoad)
            {
                CarregarFuncionarios();
                isFirstLoad = false;
            }
        }

        private void InitializeComponent()
        {
            this.Text = "Listagem de Funcionários";
            this.Width = 1000;
            this.Height = 650; // Aumentar a altura do formulário
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
                Height = 500, // Aumentar a altura do DataGridView
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells,
                BackgroundColor = System.Drawing.Color.White,
                BorderStyle = BorderStyle.None,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = System.Drawing.Color.FromArgb(0, 120, 212),
                    ForeColor = System.Drawing.Color.White,
                    Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold),
                    Padding = new Padding(10, 8, 10, 8) // Aumentar o padding vertical dos cabeçalhos
                },
                ColumnHeadersHeight = 40, // Aumentar a altura dos cabeçalhos
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new System.Drawing.Font("Segoe UI", 9.75F),
                    Padding = new Padding(5, 10, 5, 10), // Aumentar o padding vertical
                    SelectionBackColor = System.Drawing.Color.FromArgb(0, 120, 212),
                    SelectionForeColor = System.Drawing.Color.White
                },
                RowTemplate = new DataGridViewRow
                {
                    Height = 40 // Definir altura fixa para todas as linhas
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
            
            // Configurar colunas do DataGridView na sequência solicitada
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
                    Name = "ColCargo",
                    HeaderText = "Cargo",
                    Width = 120
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "ColDepartamento",
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
                // Adicionar coluna de Salário Anual
                new DataGridViewTextBoxColumn
                {
                    Name = "ColSalarioAnual",
                    HeaderText = "Salário Anual",
                    Width = 120,
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        Format = "C2",
                        Alignment = DataGridViewContentAlignment.MiddleRight
                    }
                },
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Status",
                    HeaderText = "Status",
                    Width = 80
                },
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Telefone",
                    HeaderText = "Telefone",
                    Width = 100
                },
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Email",
                    HeaderText = "Email",
                    Width = 180
                },
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Idade",
                    HeaderText = "Idade",
                    Width = 60,
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        Alignment = DataGridViewContentAlignment.MiddleCenter
                    }
                },
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "DataNascimento",
                    HeaderText = "Data Nascimento",
                    Width = 120,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy", NullValue = "" }
                },
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "DataContratacao",
                    HeaderText = "Data Contratação",
                    Width = 120,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }
                }
            };

            // Adicionar todas as colunas de uma vez
            dgvFuncionarios.Columns.AddRange(colunas);

            // Buttons
            btnEditar = new Button
            {
                Text = "Editar",
                Location = new System.Drawing.Point(20, 540), // Ajustar posiu00e7u00e3o para baixo
                Width = 100,
                Height = 35, // Aumentar altura do botu00e3o
                BackColor = System.Drawing.Color.FromArgb(0, 120, 212),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold)
            };
            btnEditar.Click += BtnEditar_Click;

            btnRemover = new Button
            {
                Text = "Remover",
                Location = new System.Drawing.Point(140, 540), // Ajustar posiu00e7u00e3o para baixo
                Width = 100,
                Height = 35, // Aumentar altura do botu00e3o
                BackColor = System.Drawing.Color.FromArgb(220, 53, 69),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold)
            };
            btnRemover.Click += BtnRemover_Click;

            // Adicionar botu00e3o para atualizar
            btnAtualizar = new Button
            {
                Text = "Atualizar",
                Location = new System.Drawing.Point(260, 540), // Ajustar posição para baixo
                Width = 100,
                Height = 35, // Aumentar altura do botão
                BackColor = System.Drawing.Color.FromArgb(40, 167, 69),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold)
            };
            btnAtualizar.Click += (s, e) => CarregarFuncionarios();

            // Adicionar controles ao formulário
            this.Controls.AddRange(new Control[] { dgvFuncionarios, btnEditar, btnRemover, btnAtualizar });
        }

        public void CarregarFuncionarios()
        {
            try
            {
                // Criar uma nova instância do DAO para garantir que não há cache
                var funcionarioDAOFresh = new FuncionarioDAO();
                var funcionarios = funcionarioDAOFresh.ListarFuncionarios();
                
                // Limpar completamente o DataGridView
                dgvFuncionarios.DataSource = null;
                dgvFuncionarios.Rows.Clear();
                dgvFuncionarios.Refresh();
                
                // Configurar nova fonte de dados
                dgvFuncionarios.DataSource = funcionarios;
                
                // Format the nested properties manually
                FormatarPropriedadesAninhadas();
                
                // Configurar para que a tabela se expanda conforme necessário
                dgvFuncionarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
                
                // Ajustar o tamanho da tabela para mostrar todas as colunas
                int totalWidth = 0;
                foreach (DataGridViewColumn col in dgvFuncionarios.Columns)
                {
                    totalWidth += col.Width;
                }
                
                // Garantir que a tabela tenha largura suficiente para mostrar todas as colunas
                dgvFuncionarios.Width = Math.Min(this.ClientSize.Width - 40, totalWidth + 20);
                
                // Forçar atualização visual
                dgvFuncionarios.Refresh();
                this.Refresh();
                
                Console.WriteLine($"Listagem atualizada: {funcionarios.Count} funcionários carregados");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar funcionários: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void FormatarPropriedadesAninhadas()
        {
            // Garantir que todas as linhas tenham a altura adequada
            foreach (DataGridViewRow row in dgvFuncionarios.Rows)
            {
                row.Height = 40; // Definir altura fixa para cada linha
            }
            
            // Handle nested properties manually
            foreach (DataGridViewRow row in dgvFuncionarios.Rows)
            {
                if (row.DataBoundItem is Funcionario funcionario)
                {
                    try
                    {
                        // Set Cargo column value
                        if (funcionario.Cargo != null && !string.IsNullOrEmpty(funcionario.Cargo.Nome))
                        {
                            row.Cells["ColCargo"].Value = funcionario.Cargo.Nome;
                        }
                        else
                        {
                            // Try to get cargo name directly from database if needed
                            row.Cells["ColCargo"].Value = "N/A";
                        }
                        
                        // Set Departamento column value
                        if (funcionario.Departamento != null && !string.IsNullOrEmpty(funcionario.Departamento.Nome))
                        {
                            row.Cells["ColDepartamento"].Value = funcionario.Departamento.Nome;
                        }
                        else
                        {
                            // Try to get departamento name directly from database if needed
                            row.Cells["ColDepartamento"].Value = "N/A";
                        }
                        
                        // Calcular e definir o Salário Anual (salário mensal * 13.33 para incluir 13º e férias)
                        decimal salarioAnual = funcionario.Salario * 13.33m;
                        row.Cells["ColSalarioAnual"].Value = salarioAnual;
                    }
                    catch (Exception ex)
                    {
                        // Log error but continue processing other rows
                        Console.WriteLine($"Erro ao formatar linha: {ex.Message}");
                    }
                }
            }
        }

        private void BtnEditar_Click(object? sender, EventArgs e)
        {
            if (dgvFuncionarios.CurrentRow != null)
            {
                var funcionario = (Funcionario)dgvFuncionarios.CurrentRow.DataBoundItem;
                var formEditar = new CadastroFuncionario(funcionario);
                formEditar.FormClosed += (s, args) => 
                {
                    // Always reload after editing
                    CarregarFuncionarios();
                }; 
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
