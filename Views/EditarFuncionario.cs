using System;
using System.Windows.Forms;
using MinhaEmpresa.Models;
using MinhaEmpresa.DAO;

namespace MinhaEmpresa.Views
{
    public partial class EditarFuncionario : Form
    {
        private TextBox txtNome = null!;
        private ComboBox cmbCargo = null!;
        private TextBox txtSalario = null!;
        private ComboBox cmbDepartamento = null!;
        private DateTimePicker dtpDataContratacao = null!;
        private Button btnSalvar = null!;
        private Button btnCancelar = null!;
        private FuncionarioDAO funcionarioDAO = null!;
        private Funcionario funcionario = null!;

        public EditarFuncionario(Funcionario funcionario)
        {
            this.funcionario = funcionario;
            InitializeComponent();
            InitializeCustomComponents();
            funcionarioDAO = new FuncionarioDAO();
            PreencherCampos();
        }

        private void InitializeComponent()
        {
            this.Text = "Editar Funcionário";
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
                new Cargo { Id = 1, Nome = "Analista", Nivel = "Junior" },
                new Cargo { Id = 2, Nome = "Desenvolvedor", Nivel = "Pleno" },
                new Cargo { Id = 3, Nome = "Gerente", Nivel = "Senior" },
                new Cargo { Id = 4, Nome = "Coordenador", Nivel = "Senior" },
                new Cargo { Id = 5, Nome = "Assistente", Nivel = "Junior" }
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
                new Departamento { Id = 1, Nome = "TI", DataCriacao = DateTime.Now },
                new Departamento { Id = 2, Nome = "RH", DataCriacao = DateTime.Now },
                new Departamento { Id = 3, Nome = "Financeiro", DataCriacao = DateTime.Now },
                new Departamento { Id = 4, Nome = "Administrativo", DataCriacao = DateTime.Now },
                new Departamento { Id = 5, Nome = "Comercial", DataCriacao = DateTime.Now }
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

            btnCancelar = new Button
            {
                Text = "Cancelar",
                Location = new System.Drawing.Point(350, 300),
                Width = 100
            };
            btnCancelar.Click += (s, e) => this.Close();

            // Adicionar controles ao formulário
            this.Controls.AddRange(new Control[] 
            { 
                lblNome, lblCargo, lblSalario, lblDepartamento, lblDataContratacao,
                txtNome, cmbCargo, txtSalario, cmbDepartamento, dtpDataContratacao,
                btnSalvar, btnCancelar
            });
        }

        private void PreencherCampos()
        {
            txtNome.Text = funcionario.Nome;
            cmbCargo.SelectedValue = funcionario.CargoId;
            txtSalario.Text = funcionario.Salario.ToString("F2");
            cmbDepartamento.SelectedValue = funcionario.DepartamentoId;
            dtpDataContratacao.Value = funcionario.DataContratacao;
        }

        private void BtnSalvar_Click(object? sender, EventArgs e)
        {
            try
            {
                if (ValidarCampos())
                {
                    funcionario.Nome = txtNome.Text;
                    if (cmbCargo.SelectedItem is Cargo selectedCargo && cmbDepartamento.SelectedItem is Departamento selectedDepartamento)
                    {
                        funcionario.CargoId = selectedCargo.Id;
                        funcionario.Cargo = selectedCargo;
                        funcionario.Salario = Convert.ToDecimal(txtSalario.Text);
                        funcionario.DepartamentoId = selectedDepartamento.Id;
                        funcionario.Departamento = selectedDepartamento;
                    }
                    else
                    {
                        MessageBox.Show("Erro ao obter cargo ou departamento selecionado.", "Erro",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    funcionario.DataContratacao = dtpDataContratacao.Value;

                    funcionarioDAO.AtualizarFuncionario(funcionario);
                    MessageBox.Show("Funcionário atualizado com sucesso!", "Sucesso", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao atualizar funcionário: {ex.Message}", "Erro",
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
    }
}
