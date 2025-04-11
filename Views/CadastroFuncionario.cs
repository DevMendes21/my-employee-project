using System;
using System.Windows.Forms;
using MinhaEmpresa.Models;
using MinhaEmpresa.DAO;

namespace MinhaEmpresa.Views
{
    public partial class CadastroFuncionario : Form
    {
        private TextBox txtNome;
        private ComboBox cmbCargo;
        private TextBox txtSalario;
        private ComboBox cmbDepartamento;
        private DateTimePicker dtpDataContratacao;
        private Button btnSalvar;
        private Button btnVoltar;
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
            this.Height = 400;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
        }

        private void InitializeCustomComponents()
        {
            // Labels
            Label lblNome = new Label { Text = "Nome:", Location = new System.Drawing.Point(20, 20) };
            Label lblCargo = new Label { Text = "Cargo:", Location = new System.Drawing.Point(20, 80) };
            Label lblSalario = new Label { Text = "Salário:", Location = new System.Drawing.Point(20, 140) };
            Label lblDepartamento = new Label { Text = "Departamento:", Location = new System.Drawing.Point(20, 200) };
            Label lblDataContratacao = new Label { Text = "Data de Contratação:", Location = new System.Drawing.Point(20, 260) };

            // TextBoxes e ComboBoxes
            txtNome = new TextBox { Location = new System.Drawing.Point(150, 20), Width = 300 };
            
            cmbCargo = new ComboBox 
            { 
                Location = new System.Drawing.Point(150, 80), 
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

            txtSalario = new TextBox { Location = new System.Drawing.Point(150, 140), Width = 300 };
            
            cmbDepartamento = new ComboBox 
            { 
                Location = new System.Drawing.Point(150, 200), 
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
                lblNome, lblCargo, lblSalario, lblDepartamento, lblDataContratacao,
                txtNome, cmbCargo, txtSalario, cmbDepartamento, dtpDataContratacao,
                btnSalvar, btnVoltar
            });
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidarCampos())
                {
                    Funcionario funcionario = new Funcionario
                    {
                        Nome = txtNome.Text,
                        CargoId = ((Cargo)cmbCargo.SelectedItem).Id,
                        Cargo = (Cargo)cmbCargo.SelectedItem,
                        Salario = Convert.ToDecimal(txtSalario.Text),
                        DepartamentoId = ((Departamento)cmbDepartamento.SelectedItem).Id,
                        Departamento = (Departamento)cmbDepartamento.SelectedItem,
                        DataContratacao = dtpDataContratacao.Value
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
            cmbCargo.SelectedIndex = -1;
            txtSalario.Clear();
            cmbDepartamento.SelectedIndex = -1;
            dtpDataContratacao.Value = DateTime.Now;
        }
    }
}
