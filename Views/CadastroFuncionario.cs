using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using MinhaEmpresa.Models;
using MinhaEmpresa.DAO;

namespace MinhaEmpresa.Views
{
    public partial class CadastroFuncionario : Form
    {
        private TextBox txtNome = null!;
        private TextBox txtEmail = null!;
        private TextBox txtTelefone = null!;
        private ComboBox cmbCargo = null!;
        private TextBox txtSalario = null!;
        private ComboBox cmbDepartamento = null!;
        private DateTimePicker dtpDataContratacao = null!;
        private DateTimePicker dtpDataNascimento = null!;
        private ComboBox cmbStatus = null!;
        private TextBox txtObservacoes = null!;
        private Button btnSalvar = null!;
        private Button btnVoltar = null!;
        private FuncionarioDAO funcionarioDAO;
        private CargoDAO cargoDAO;
        private DepartamentoDAO departamentoDAO;
        private int? funcionarioId;

        public CadastroFuncionario(Funcionario? funcionario = null)
        {
            InitializeComponent();
            funcionarioDAO = new FuncionarioDAO();
            cargoDAO = new CargoDAO();
            departamentoDAO = new DepartamentoDAO();
            InitializeCustomComponents();

            if (funcionario != null)
            {
                funcionarioId = funcionario.Id;
                PreencherDadosFuncionario(funcionario);
                this.Text = "Editar Funcionário";
            }
        }

        private void InitializeComponent()
        {
            this.Text = "Cadastro de Funcionário";
            this.Width = 800;
            this.Height = 700;
            this.AutoScroll = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
        }

        // Método auxiliar para criar labels estilizadas
        private Label CreateStyledLabel(string text, int x, int y, System.Drawing.Font font, System.Drawing.Color color)
        {
            return new Label
            {
                Text = text,
                Location = new System.Drawing.Point(x, y),
                AutoSize = true,
                Font = font,
                ForeColor = color
            };
        }
        
        // Método auxiliar para criar textboxes estilizadas
        private TextBox CreateStyledTextBox(int x, int y, int width, int height)
        {
            return new TextBox
            {
                Location = new System.Drawing.Point(x, y),
                Width = width,
                Height = height,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new System.Drawing.Font("Segoe UI", 10),
                BackColor = System.Drawing.Color.White
            };
        }

        private void InitializeCustomComponents()
        {
            // Painel de cabeçalho
            Panel headerPanel = new Panel
            {
                BackColor = System.Drawing.Color.FromArgb(41, 128, 185),
                Location = new System.Drawing.Point(0, 0),
                Width = this.ClientSize.Width,
                Height = 80,
                Dock = DockStyle.Top
            };
            
            Label lblHeader = new Label
            {
                Text = funcionarioId.HasValue ? "Editar Funcionário" : "Novo Funcionário",
                ForeColor = System.Drawing.Color.White,
                Font = new System.Drawing.Font("Segoe UI", 18, System.Drawing.FontStyle.Bold),
                Location = new System.Drawing.Point(20, 20),
                AutoSize = true
            };
            
            headerPanel.Controls.Add(lblHeader);
            this.Controls.Add(headerPanel);
            
            // Painel principal
            Panel mainPanel = new Panel
            {
                BackColor = System.Drawing.Color.White,
                Location = new System.Drawing.Point(20, 100),
                Width = this.ClientSize.Width - 40,
                Height = this.ClientSize.Height - 120,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                BorderStyle = BorderStyle.None
            };
            
            // Estilo comum para labels
            System.Drawing.Font labelFont = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            System.Drawing.Color labelColor = System.Drawing.Color.FromArgb(70, 70, 70);
            
            // Labels com estilo moderno
            Label lblNome = CreateStyledLabel("Nome", 30, 30, labelFont, labelColor);
            Label lblEmail = CreateStyledLabel("Email", 30, 90, labelFont, labelColor);
            Label lblTelefone = CreateStyledLabel("Telefone", 30, 150, labelFont, labelColor);
            Label lblCargo = CreateStyledLabel("Cargo", 30, 210, labelFont, labelColor);
            Label lblSalario = CreateStyledLabel("Salário", 30, 270, labelFont, labelColor);
            Label lblDepartamento = CreateStyledLabel("Departamento", 400, 30, labelFont, labelColor);
            Label lblDataContratacao = CreateStyledLabel("Data de Contratação", 400, 90, labelFont, labelColor);
            Label lblDataNascimento = CreateStyledLabel("Data de Nascimento", 400, 150, labelFont, labelColor);
            Label lblStatus = CreateStyledLabel("Status", 400, 210, labelFont, labelColor);
            Label lblObservacoes = CreateStyledLabel("Observações", 30, 330, labelFont, labelColor);

            // TextBoxes e ComboBoxes com estilo moderno
            txtNome = CreateStyledTextBox(30, 55, 340, 30);
            txtEmail = CreateStyledTextBox(30, 115, 340, 30);
            txtTelefone = CreateStyledTextBox(30, 175, 340, 30);
            
            cmbCargo = new ComboBox 
            { 
                Location = new System.Drawing.Point(30, 235), 
                Width = 340,
                Height = 30,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new System.Drawing.Font("Segoe UI", 10),
                FlatStyle = FlatStyle.Flat
            };
            
            // Carregar cargos do banco de dados
            CargoDAO cargoDAO = new CargoDAO();
            var cargos = cargoDAO.ListarCargos();
            
            // Configurar o ComboBox de cargos com estilo moderno
            cmbCargo.DataSource = null; // Limpar qualquer binding anterior
            cmbCargo.Items.Clear();
            cmbCargo.BackColor = System.Drawing.Color.White;
            cmbCargo.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            cmbCargo.DataSource = new BindingSource(cargos, null);
            cmbCargo.DisplayMember = "Nome";
            cmbCargo.ValueMember = "Id";
            cmbCargo.SelectedIndex = -1; // Nenhum cargo selecionado inicialmente
            
            // Log para debug
            Console.WriteLine("Cargos carregados do banco de dados:");
            foreach (var cargo in cargos)
            {
                Console.WriteLine($"ID: {cargo.Id}, Nome: {cargo.Nome}, Nivel: {cargo.Nivel}");
            }

            txtSalario = CreateStyledTextBox(30, 295, 340, 30);
            
            cmbDepartamento = new ComboBox
            {
                Location = new System.Drawing.Point(400, 55),
                Width = 340,
                Height = 30,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new System.Drawing.Font("Segoe UI", 10),
                FlatStyle = FlatStyle.Flat,
                BackColor = System.Drawing.Color.White,
                ForeColor = System.Drawing.Color.FromArgb(50, 50, 50)
            };
            
            // Carregar departamentos do banco de dados
            DepartamentoDAO departamentoDAO = new DepartamentoDAO();
            var departamentos = departamentoDAO.ListarDepartamentos();
            
            // Configurar o ComboBox de departamentos
            cmbDepartamento.DataSource = null; // Limpar qualquer binding anterior
            cmbDepartamento.Items.Clear();
            cmbDepartamento.DataSource = new BindingSource(departamentos, null);
            cmbDepartamento.DisplayMember = "Nome";
            cmbDepartamento.ValueMember = "Id";
            
            // Log para debug
            Console.WriteLine("Departamentos carregados do banco de dados:");
            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"ID: {departamento.Id}, Nome: {departamento.Nome}");
            }

            dtpDataContratacao = new DateTimePicker 
            { 
                Location = new System.Drawing.Point(400, 115), 
                Width = 340,
                Height = 30,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy",
                Value = DateTime.Now,
                Font = new System.Drawing.Font("Segoe UI", 10),
                CalendarForeColor = System.Drawing.Color.FromArgb(50, 50, 50),
                CalendarTitleBackColor = System.Drawing.Color.FromArgb(41, 128, 185)
            };

            dtpDataNascimento = new DateTimePicker 
            { 
                Location = new System.Drawing.Point(400, 175), 
                Width = 340,
                Height = 30,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy",
                Value = DateTime.Now,
                Font = new System.Drawing.Font("Segoe UI", 10),
                CalendarForeColor = System.Drawing.Color.FromArgb(50, 50, 50),
                CalendarTitleBackColor = System.Drawing.Color.FromArgb(41, 128, 185)
            };

            cmbStatus = new ComboBox
            {
                Location = new System.Drawing.Point(400, 235),
                Width = 340,
                Height = 30,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new System.Drawing.Font("Segoe UI", 10),
                FlatStyle = FlatStyle.Flat,
                BackColor = System.Drawing.Color.White,
                ForeColor = System.Drawing.Color.FromArgb(50, 50, 50)
            };
            cmbStatus.Items.AddRange(Enum.GetValues(typeof(StatusFuncionario))
                                      .Cast<StatusFuncionario>()
                                      .Select(s => s.ToString())
                                      .ToArray());
            cmbStatus.SelectedIndex = 0; // Ativo por padrão

            txtObservacoes = new TextBox 
            { 
                Location = new System.Drawing.Point(30, 355), 
                Width = 710,
                Height = 120,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new System.Drawing.Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = System.Drawing.Color.White
            };

            // Buttons com estilo moderno
            btnSalvar = new Button
            {
                Text = "Salvar",
                Location = new System.Drawing.Point(550, 490),
                Width = 120,
                Height = 45,
                BackColor = System.Drawing.Color.FromArgb(41, 128, 185),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new System.Drawing.Font("Segoe UI", 11, System.Drawing.FontStyle.Bold),
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 0 }
            };
            btnSalvar.Click += BtnSalvar_Click;

            btnVoltar = new Button
            {
                Text = "Cancelar",
                Location = new System.Drawing.Point(410, 490),
                Width = 120,
                Height = 45,
                BackColor = System.Drawing.Color.FromArgb(220, 53, 69),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new System.Drawing.Font("Segoe UI", 11, System.Drawing.FontStyle.Bold),
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 0 }
            };
            btnVoltar.Click += (s, e) => this.Close();

            // Adicionar controles ao painel principal
            mainPanel.Controls.AddRange(new Control[] 
            { 
                lblNome, lblEmail, lblTelefone, lblCargo, lblSalario, lblDepartamento, 
                lblDataContratacao, lblDataNascimento, lblStatus, lblObservacoes,
                txtNome, txtEmail, txtTelefone, cmbCargo, txtSalario, cmbDepartamento, 
                dtpDataContratacao, dtpDataNascimento, cmbStatus, txtObservacoes,
                btnSalvar, btnVoltar
            });
            
            // Adicionar painel principal ao formulário
            this.Controls.Add(mainPanel);
        }

        private void BtnSalvar_Click(object? sender, EventArgs e)
        {
            try
            {
                if (ValidarCampos())
                {
                    var cargo = cmbCargo.SelectedItem as Cargo;
                    var departamento = cmbDepartamento.SelectedItem as Departamento;
                    
                    if (cargo == null || departamento == null)
                    {
                        MessageBox.Show("Selecione um cargo e departamento válidos.", "Aviso", 
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    Funcionario funcionario = new Funcionario
                    {
                        Id = funcionarioId ?? 0,
                        Nome = txtNome.Text,
                        Email = txtEmail.Text,
                        Telefone = txtTelefone.Text,
                        CargoId = cargo.Id,
                        Cargo = cargo,
                        Salario = Convert.ToDecimal(txtSalario.Text),
                        DepartamentoId = departamento.Id,
                        Departamento = departamento,
                        DataContratacao = dtpDataContratacao.Value,
                        DataNascimento = dtpDataNascimento.Value,
                        Status = cmbStatus.SelectedItem?.ToString() is string statusStr && Enum.TryParse<StatusFuncionario>(statusStr, out var status) ? 
                            status : StatusFuncionario.Ativo,
                        Observacoes = string.IsNullOrWhiteSpace(txtObservacoes.Text) ? null : txtObservacoes.Text
                    };

                    try
                    {
                        // Garantir que os IDs de cargo e departamento estejam corretamente definidos
                        funcionario.CargoId = cargo.Id;
                        funcionario.DepartamentoId = departamento.Id;
                        
                        Console.WriteLine($"Tentando salvar funcionário com cargo {cargo.Nome} (ID: {cargo.Id}) no departamento {departamento.Nome} (ID: {departamento.Id})");
                        
                        if (funcionarioId.HasValue)
                        {
                            funcionarioDAO.AtualizarFuncionario(funcionario);
                            MessageBox.Show("Funcionário atualizado com sucesso!", "Sucesso", 
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Close();
                        }
                        else
                        {
                            funcionarioDAO.InserirFuncionario(funcionario);
                            MessageBox.Show("Funcionário cadastrado com sucesso!", "Sucesso", 
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LimparCampos();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao salvar funcionário: {ex.Message} Cargo: {cargo.Nome} (ID: {cargo.Id}) Departamento: {departamento.Nome} (ID: {departamento.Id})", "Erro",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao cadastrar funcionário: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtNome.Text))
            {
                MessageBox.Show("O nome é obrigatório!", "Aviso", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cmbCargo.SelectedIndex == -1)
            {
                MessageBox.Show("Selecione um cargo!", "Aviso", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!decimal.TryParse(txtSalario.Text, out _))
            {
                MessageBox.Show("Salário inválido!", "Aviso", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cmbDepartamento.SelectedIndex == -1)
            {
                MessageBox.Show("Selecione um departamento!", "Aviso", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void LimparCampos()
        {
            txtNome.Clear();
            txtEmail.Clear();
            txtTelefone.Clear();
            cmbCargo.SelectedIndex = -1;
            txtSalario.Clear();
            cmbDepartamento.SelectedIndex = -1;
            dtpDataContratacao.Value = DateTime.Now;
            dtpDataNascimento.Value = DateTime.Now;
            cmbStatus.SelectedIndex = 0;
            txtObservacoes.Clear();
        }

        private void PreencherDadosFuncionario(Funcionario funcionario)
        {
            if (funcionario == null) return;

            txtNome.Text = funcionario.Nome;
            txtEmail.Text = funcionario.Email;
            txtTelefone.Text = funcionario.Telefone;
            
            // Selecionar o cargo
            for (int i = 0; i < cmbCargo.Items.Count; i++)
            {
                if (cmbCargo.Items[i] is Cargo cargo && cargo.Id == funcionario.CargoId)
                {
                    cmbCargo.SelectedIndex = i;
                    break;
                }
            }

            txtSalario.Text = funcionario.Salario.ToString();

            // Selecionar o departamento
            for (int i = 0; i < cmbDepartamento.Items.Count; i++)
            {
                if (cmbDepartamento.Items[i] is Departamento departamento && departamento.Id == funcionario.DepartamentoId)
                {
                    cmbDepartamento.SelectedIndex = i;
                    break;
                }
            }

            dtpDataContratacao.Value = funcionario.DataContratacao;
            dtpDataNascimento.Value = funcionario.DataNascimento ?? DateTime.Now;
            
            // Selecionar o status
            cmbStatus.SelectedItem = funcionario.Status.ToString();
            
            txtObservacoes.Text = funcionario.Observacoes ?? string.Empty;
        }
    }
}
