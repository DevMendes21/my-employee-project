using System;
using System.Windows.Forms;
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
        private int? funcionarioId;

        public CadastroFuncionario(Funcionario? funcionario = null)
        {
            InitializeComponent();
            InitializeCustomComponents();
            funcionarioDAO = new FuncionarioDAO();

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
            this.Width = 500;
            this.Height = 650;
            this.AutoScroll = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
        }

        private void InitializeCustomComponents()
        {
            // Labels
            Label lblNome = new Label { Text = "Nome:", Location = new System.Drawing.Point(20, 20) };
            Label lblEmail = new Label { Text = "Email:", Location = new System.Drawing.Point(20, 60) };
            Label lblTelefone = new Label { Text = "Telefone:", Location = new System.Drawing.Point(20, 100) };
            Label lblCargo = new Label { Text = "Cargo:", Location = new System.Drawing.Point(20, 140) };
            Label lblSalario = new Label { Text = "Salário:", Location = new System.Drawing.Point(20, 180) };
            Label lblDepartamento = new Label { Text = "Departamento:", Location = new System.Drawing.Point(20, 220) };
            Label lblDataContratacao = new Label { Text = "Data de Contratação:", Location = new System.Drawing.Point(20, 260), AutoSize = true, Width = 120 };
            Label lblDataNascimento = new Label { Text = "Data de Nascimento:", Location = new System.Drawing.Point(20, 300), AutoSize = true, Width = 120 };
            Label lblStatus = new Label { Text = "Status:", Location = new System.Drawing.Point(20, 340) };
            Label lblObservacoes = new Label { Text = "Observações:", Location = new System.Drawing.Point(20, 380) };

            // TextBoxes e ComboBoxes
            txtNome = new TextBox { Location = new System.Drawing.Point(150, 20), Width = 300 };
            txtEmail = new TextBox { Location = new System.Drawing.Point(150, 60), Width = 300 };
            txtTelefone = new TextBox { Location = new System.Drawing.Point(150, 100), Width = 300 };
            
            cmbCargo = new ComboBox 
            { 
                Location = new System.Drawing.Point(150, 140), 
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
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

            txtSalario = new TextBox { Location = new System.Drawing.Point(150, 180), Width = 300 };
            
            cmbDepartamento = new ComboBox 
            { 
                Location = new System.Drawing.Point(150, 220), 
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            var departamentos = new List<Departamento>
            {
                new Departamento { Id = 1, Nome = "TI" },
                new Departamento { Id = 2, Nome = "RH" },
                new Departamento { Id = 3, Nome = "Financeiro" },
                new Departamento { Id = 4, Nome = "Administrativo" },
                new Departamento { Id = 5, Nome = "Comercial" }
            };
            cmbDepartamento.DataSource = departamentos;
            cmbDepartamento.DisplayMember = "Nome";
            cmbDepartamento.ValueMember = "Id";

            dtpDataContratacao = new DateTimePicker 
            { 
                Location = new System.Drawing.Point(150, 260), 
                Width = 300,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy",
                Value = DateTime.Now
            };

            dtpDataNascimento = new DateTimePicker 
            { 
                Location = new System.Drawing.Point(150, 300), 
                Width = 300,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy",
                Value = DateTime.Now
            };

            cmbStatus = new ComboBox
            {
                Location = new System.Drawing.Point(150, 340),
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbStatus.Items.AddRange(Enum.GetValues(typeof(StatusFuncionario))
                                      .Cast<StatusFuncionario>()
                                      .Select(s => s.ToString())
                                      .ToArray());
            cmbStatus.SelectedIndex = 0; // Ativo por padrão

            txtObservacoes = new TextBox 
            { 
                Location = new System.Drawing.Point(150, 380), 
                Width = 300,
                Height = 80,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

            // Buttons
            btnSalvar = new Button
            {
                Text = "Salvar",
                Location = new System.Drawing.Point(150, 480),
                Width = 100,
                Height = 30,
                BackColor = System.Drawing.Color.FromArgb(0, 120, 212),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSalvar.Click += BtnSalvar_Click;

            btnVoltar = new Button
            {
                Text = "Voltar",
                Location = new System.Drawing.Point(350, 480),
                Width = 100,
                Height = 30,
                BackColor = System.Drawing.Color.FromArgb(234, 234, 234),
                FlatStyle = FlatStyle.Flat
            };
            btnVoltar.Click += (s, e) => this.Close();

            // Adicionar controles ao formulário
            this.Controls.AddRange(new Control[] 
            { 
                lblNome, lblEmail, lblTelefone, lblCargo, lblSalario, lblDepartamento, 
                lblDataContratacao, lblDataNascimento, lblStatus, lblObservacoes,
                txtNome, txtEmail, txtTelefone, cmbCargo, txtSalario, cmbDepartamento, 
                dtpDataContratacao, dtpDataNascimento, cmbStatus, txtObservacoes,
                btnSalvar, btnVoltar
            });
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
