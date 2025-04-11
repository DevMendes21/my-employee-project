using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
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
        private CargoDAO cargoDAO = null!;
        private DepartamentoDAO departamentoDAO = null!;
        private Funcionario funcionario = null!;

        public EditarFuncionario(Funcionario funcionario)
        {
            this.funcionario = funcionario;
            InitializeComponent();
            funcionarioDAO = new FuncionarioDAO();
            cargoDAO = new CargoDAO();
            departamentoDAO = new DepartamentoDAO();
            InitializeCustomComponents();
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
            
            // Definir cargos manualmente para evitar duplicações
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
            
            // Definir departamentos manualmente para garantir que todos estejam disponíveis
            var departamentos = new List<Departamento>
            {
                new Departamento { Id = 1, Nome = "TI", DataCriacao = DateTime.Now },
                new Departamento { Id = 2, Nome = "RH", DataCriacao = DateTime.Now },
                new Departamento { Id = 3, Nome = "Financeiro", DataCriacao = DateTime.Now },
                new Departamento { Id = 4, Nome = "Administrativo", DataCriacao = DateTime.Now },
                new Departamento { Id = 5, Nome = "Comercial", DataCriacao = DateTime.Now }
            };
            
            // Configurar o ComboBox de departamentos
            cmbDepartamento.DataSource = null; // Limpar qualquer binding anterior
            cmbDepartamento.Items.Clear();
            cmbDepartamento.DataSource = new BindingSource(departamentos, null);
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
            
            // Log para debug - antes de definir o cargo
            Console.WriteLine($"Carregando funcionário ID {funcionario.Id} - CargoId: {funcionario.CargoId}");
            foreach (var item in cmbCargo.Items)
            {
                if (item is Cargo cargo)
                {
                    Console.WriteLine($"Cargo disponível: ID {cargo.Id}, Nome: {cargo.Nome}");
                }
            }
            
            // Definir o cargo selecionado
            bool cargoEncontrado = false;
            foreach (var item in cmbCargo.Items)
            {
                if (item is Cargo cargo && cargo.Id == funcionario.CargoId)
                {
                    cmbCargo.SelectedItem = item;
                    cargoEncontrado = true;
                    Console.WriteLine($"Cargo selecionado: ID {cargo.Id}, Nome: {cargo.Nome}");
                    break;
                }
            }
            
            if (!cargoEncontrado)
            {
                Console.WriteLine($"ALERTA: Cargo ID {funcionario.CargoId} não encontrado na lista!");
            }
            
            txtSalario.Text = funcionario.Salario.ToString("F2");
            
            // Log para debug - antes de definir o departamento
            Console.WriteLine($"Carregando funcionário ID {funcionario.Id} - DepartamentoId: {funcionario.DepartamentoId}");
            foreach (var item in cmbDepartamento.Items)
            {
                if (item is Departamento departamento)
                {
                    Console.WriteLine($"Departamento disponível: ID {departamento.Id}, Nome: {departamento.Nome}");
                }
            }
            
            // Definir o departamento selecionado
            bool departamentoEncontrado = false;
            foreach (var item in cmbDepartamento.Items)
            {
                if (item is Departamento departamento && departamento.Id == funcionario.DepartamentoId)
                {
                    cmbDepartamento.SelectedItem = item;
                    departamentoEncontrado = true;
                    Console.WriteLine($"Departamento selecionado: ID {departamento.Id}, Nome: {departamento.Nome}");
                    break;
                }
            }
            
            if (!departamentoEncontrado)
            {
                Console.WriteLine($"ALERTA: Departamento ID {funcionario.DepartamentoId} não encontrado na lista!");
            }
            
            dtpDataContratacao.Value = funcionario.DataContratacao;
        }

        private void BtnSalvar_Click(object? sender, EventArgs e)
        {
            try
            {
                if (ValidarCampos())
                {
                    funcionario.Nome = txtNome.Text;
                    
                    // Verificar e obter o cargo selecionado
                    if (cmbCargo.SelectedItem is Cargo selectedCargo)
                    {
                        Console.WriteLine($"Cargo selecionado para atualização: ID {selectedCargo.Id}, Nome: {selectedCargo.Nome}");
                        funcionario.CargoId = selectedCargo.Id;
                        funcionario.Cargo = selectedCargo;
                    }
                    else
                    {
                        MessageBox.Show("Erro: Nenhum cargo selecionado.", "Erro",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    
                    // Verificar e obter o departamento selecionado
                    if (cmbDepartamento.SelectedItem is Departamento selectedDepartamento)
                    {
                        Console.WriteLine($"Departamento selecionado para atualização: ID {selectedDepartamento.Id}, Nome: {selectedDepartamento.Nome}");
                        funcionario.DepartamentoId = selectedDepartamento.Id;
                        funcionario.Departamento = selectedDepartamento;
                    }
                    else
                    {
                        MessageBox.Show("Erro: Nenhum departamento selecionado.", "Erro",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    
                    // Atualizar salário e data de contratação
                    funcionario.Salario = Convert.ToDecimal(txtSalario.Text);
                    funcionario.DataContratacao = dtpDataContratacao.Value;
                    
                    // Log para debug - valores finais antes de salvar
                    Console.WriteLine($"Valores finais para atualização do funcionário ID {funcionario.Id}:");
                    Console.WriteLine($"Nome: {funcionario.Nome}");
                    Console.WriteLine($"CargoId: {funcionario.CargoId}, Cargo: {funcionario.Cargo?.Nome}");
                    Console.WriteLine($"DepartamentoId: {funcionario.DepartamentoId}, Departamento: {funcionario.Departamento?.Nome}");
                    Console.WriteLine($"Salário: {funcionario.Salario}");

                    funcionarioDAO.AtualizarFuncionario(funcionario);
                    MessageBox.Show("Funcionário atualizado com sucesso!", "Sucesso", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Atualizar a listagem de funcionários se estiver aberta
                    foreach (Form form in Application.OpenForms)
                    {
                        if (form is ListagemFuncionarios listagemForm)
                        {
                            listagemForm.CarregarFuncionarios();
                            break;
                        }
                    }
                    
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
