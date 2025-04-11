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
        private TextBox txtObservacoes = null!;
        private Button btnSalvar = null!;
        private Button btnVoltar = null!;
        private FuncionarioDAO funcionarioDAO;

        public CadastroFuncionario()
        {
            InitializeComponent();
            InitializeCustomComponents();
            funcionarioDAO = new FuncionarioDAO();
        }

        private void InitializeComponent()
        {
            this.Text = "Cadastro de Funcionário";
            this.Width = 500;
            this.Height = 550;
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
            Label lblDataContratacao = new Label { Text = "Data de Contratação:", Location = new System.Drawing.Point(20, 260) };
            Label lblDataNascimento = new Label { Text = "Data de Nascimento:", Location = new System.Drawing.Point(20, 300) };
            Label lblObservacoes = new Label { Text = "Observações:", Location = new System.Drawing.Point(20, 340) };

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
                new Cargo { Id = 1, Nome = "Analista" },
                new Cargo { Id = 2, Nome = "Desenvolvedor" },
                new Cargo { Id = 3, Nome = "Gerente" },
                new Cargo { Id = 4, Nome = "Coordenador" },
                new Cargo { Id = 5, Nome = "Assistente" }
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
                Format = DateTimePickerFormat.Short
            };

            dtpDataNascimento = new DateTimePicker 
            { 
                Location = new System.Drawing.Point(150, 300), 
                Width = 300,
                Format = DateTimePickerFormat.Short
            };

            txtObservacoes = new TextBox 
            { 
                Location = new System.Drawing.Point(150, 340), 
                Width = 300,
                Height = 60,
                Multiline = true
            };

            // Buttons
            btnSalvar = new Button
            {
                Text = "Salvar",
                Location = new System.Drawing.Point(150, 300),
                Width = 100
            };
            btnSalvar.Click += BtnSalvar_Click;

            btnVoltar = new Button
            {
                Text = "Voltar",
                Location = new System.Drawing.Point(350, 300),
                Width = 100
            };
            btnVoltar.Click += (s, e) => this.Close();

            // Adicionar controles ao formulário
            this.Controls.AddRange(new Control[] 
            { 
                lblNome, lblEmail, lblTelefone, lblCargo, lblSalario, lblDepartamento, 
                lblDataContratacao, lblDataNascimento, lblObservacoes,
                txtNome, txtEmail, txtTelefone, cmbCargo, txtSalario, cmbDepartamento, 
                dtpDataContratacao, dtpDataNascimento, txtObservacoes,
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
                        Nome = txtNome.Text,
                        Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text,
                        Telefone = string.IsNullOrWhiteSpace(txtTelefone.Text) ? null : txtTelefone.Text,
                        CargoId = cargo.Id,
                        Cargo = cargo,
                        Salario = Convert.ToDecimal(txtSalario.Text),
                        DepartamentoId = departamento.Id,
                        Departamento = departamento,
                        DataContratacao = dtpDataContratacao.Value,
                        DataNascimento = dtpDataNascimento.Value,
                        Observacoes = string.IsNullOrWhiteSpace(txtObservacoes.Text) ? null : txtObservacoes.Text
                    };

                    funcionarioDAO.InserirFuncionario(funcionario);
                    MessageBox.Show("Funcionário cadastrado com sucesso!", "Sucesso", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimparCampos();
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
            txtObservacoes.Clear();
        }
    }
}
