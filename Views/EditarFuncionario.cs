using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using MyEmployeeProject.Models;
using MyEmployeeProject.DAO;

namespace MyEmployeeProject.Views
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
            
            // Carregar cargos do banco de dados
            CargoDAO cargoDAO = new CargoDAO();
            var cargos = cargoDAO.ListarCargos();
            
            // Configurar o ComboBox de cargos
            cmbCargo.DataSource = null; // Limpar qualquer binding anterior
            cmbCargo.Items.Clear();
            cmbCargo.DataSource = new BindingSource(cargos, null);
            cmbCargo.DisplayMember = "Nome";
            cmbCargo.ValueMember = "Id";
            
            // Log para debug
            Console.WriteLine("Cargos carregados do banco de dados:");
            foreach (var cargo in cargos)
            {
                Console.WriteLine($"ID: {cargo.Id}, Nome: {cargo.Nome}, Nivel: {cargo.Nivel}");
            }

            txtSalario = new TextBox { Location = new System.Drawing.Point(150, 140), Width = 300 };
            
            cmbDepartamento = new ComboBox 
            { 
                Location = new System.Drawing.Point(150, 200), 
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList
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
            // Definir o nome
            txtNome.Text = funcionario.Nome;
            
            // Obter o cargo e departamento atuais do banco de dados
            CargoDAO cargoDAO = new CargoDAO();
            DepartamentoDAO departamentoDAO = new DepartamentoDAO();
            
            var cargoAtual = cargoDAO.ObterCargoPorId(funcionario.CargoId);
            var departamentoAtual = departamentoDAO.ObterDepartamentoPorId(funcionario.DepartamentoId);
            
            Console.WriteLine("===== INFORMAÇÕES DO FUNCIONÁRIO NO BANCO DE DADOS =====");
            Console.WriteLine($"ID: {funcionario.Id}, Nome: {funcionario.Nome}");
            Console.WriteLine($"CargoId: {funcionario.CargoId}, Cargo: {cargoAtual?.Nome ?? "Não encontrado"}");
            Console.WriteLine($"DepartamentoId: {funcionario.DepartamentoId}, Departamento: {departamentoAtual?.Nome ?? "Não encontrado"}");
            Console.WriteLine("================================================");
            
            // Resetar os ComboBoxes
            cmbCargo.SelectedIndex = -1;
            cmbDepartamento.SelectedIndex = -1;
            
            // Selecionar o cargo correto
            for (int i = 0; i < cmbCargo.Items.Count; i++)
            {
                if (cmbCargo.Items[i] is Cargo cargo && cargo.Id == funcionario.CargoId)
                {
                    cmbCargo.SelectedIndex = i;
                    Console.WriteLine($"Cargo selecionado no ComboBox: {cargo.Nome} (ID: {cargo.Id})");
                    break;
                }
            }
            
            // Verificar se o cargo foi selecionado
            if (cmbCargo.SelectedIndex == -1)
            {
                Console.WriteLine($"ERRO: Não foi possível selecionar o cargo ID {funcionario.CargoId}");
                // Tentar selecionar pelo índice 0 como fallback
                if (cmbCargo.Items.Count > 0)
                {
                    cmbCargo.SelectedIndex = 0;
                    Console.WriteLine("Selecionado o primeiro cargo da lista como fallback");
                }
            }
            
            // Definir o salário
            txtSalario.Text = funcionario.Salario.ToString("F2");
            
            // Selecionar o departamento correto
            for (int i = 0; i < cmbDepartamento.Items.Count; i++)
            {
                if (cmbDepartamento.Items[i] is Departamento departamento && departamento.Id == funcionario.DepartamentoId)
                {
                    cmbDepartamento.SelectedIndex = i;
                    Console.WriteLine($"Departamento selecionado no ComboBox: {departamento.Nome} (ID: {departamento.Id})");
                    break;
                }
            }
            
            // Verificar se o departamento foi selecionado
            if (cmbDepartamento.SelectedIndex == -1)
            {
                Console.WriteLine($"ERRO: Não foi possível selecionar o departamento ID {funcionario.DepartamentoId}");
                // Tentar selecionar pelo índice 0 como fallback
                if (cmbDepartamento.Items.Count > 0)
                {
                    cmbDepartamento.SelectedIndex = 0;
                    Console.WriteLine("Selecionado o primeiro departamento da lista como fallback");
                }
            }
            
            // Definir a data de contratação
            dtpDataContratacao.Value = funcionario.DataContratacao;
            
            // Verificar os valores selecionados
            Console.WriteLine("===== VALORES FINAIS SELECIONADOS =====");
            Console.WriteLine($"Cargo selecionado: {(cmbCargo.SelectedItem as Cargo)?.Nome ?? "Nenhum"} (ID: {(cmbCargo.SelectedItem as Cargo)?.Id ?? 0})");
            Console.WriteLine($"Departamento selecionado: {(cmbDepartamento.SelectedItem as Departamento)?.Nome ?? "Nenhum"} (ID: {(cmbDepartamento.SelectedItem as Departamento)?.Id ?? 0})");
        }

        private void BtnSalvar_Click(object? sender, EventArgs e)
        {
            try
            {
                if (ValidarCampos())
                {
                    funcionario.Nome = txtNome.Text;
                    
                    // Obter o cargo diretamente do banco de dados para garantir consistu00eancia
                    CargoDAO cargoDAO = new CargoDAO();
                    int cargoId = 0;
                    
                    if (cmbCargo.SelectedItem is Cargo selectedCargo)
                    {
                        cargoId = selectedCargo.Id;
                        Console.WriteLine($"Cargo selecionado no ComboBox: ID {selectedCargo.Id}, Nome: {selectedCargo.Nome}");
                    }
                    else if (cmbCargo.SelectedIndex >= 0)
                    {
                        // Tentar obter o ID do cargo pelo u00edndice selecionado
                        var item = cmbCargo.Items[cmbCargo.SelectedIndex];
                        if (item is Cargo cargo)
                        {
                            cargoId = cargo.Id;
                            Console.WriteLine($"Cargo obtido pelo u00edndice {cmbCargo.SelectedIndex}: ID {cargo.Id}, Nome: {cargo.Nome}");
                        }
                    }
                    
                    if (cargoId <= 0)
                    {
                        MessageBox.Show("Erro: Nenhum cargo selecionado ou cargo invu00e1lido.", "Erro",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    
                    // Obter o cargo completo do banco de dados
                    var cargoBD = cargoDAO.ObterCargoPorId(cargoId);
                    if (cargoBD == null)
                    {
                        MessageBox.Show($"Erro: Cargo com ID {cargoId} nu00e3o encontrado no banco de dados.", "Erro",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    
                    // Atualizar o funcionu00e1rio com o cargo correto
                    funcionario.CargoId = cargoBD.Id;
                    funcionario.Cargo = cargoBD;
                    Console.WriteLine($"Cargo definido para atualizau00e7u00e3o: ID {cargoBD.Id}, Nome: {cargoBD.Nome}");
                    
                    // Obter o departamento diretamente do banco de dados para garantir consistu00eancia
                    DepartamentoDAO departamentoDAO = new DepartamentoDAO();
                    int departamentoId = 0;
                    
                    if (cmbDepartamento.SelectedItem is Departamento selectedDepartamento)
                    {
                        departamentoId = selectedDepartamento.Id;
                        Console.WriteLine($"Departamento selecionado no ComboBox: ID {selectedDepartamento.Id}, Nome: {selectedDepartamento.Nome}");
                    }
                    else if (cmbDepartamento.SelectedIndex >= 0)
                    {
                        // Tentar obter o ID do departamento pelo u00edndice selecionado
                        var item = cmbDepartamento.Items[cmbDepartamento.SelectedIndex];
                        if (item is Departamento departamento)
                        {
                            departamentoId = departamento.Id;
                            Console.WriteLine($"Departamento obtido pelo u00edndice {cmbDepartamento.SelectedIndex}: ID {departamento.Id}, Nome: {departamento.Nome}");
                        }
                    }
                    
                    if (departamentoId <= 0)
                    {
                        MessageBox.Show("Erro: Nenhum departamento selecionado ou departamento invu00e1lido.", "Erro",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    
                    // Obter o departamento completo do banco de dados
                    var departamentoBD = departamentoDAO.ObterDepartamentoPorId(departamentoId);
                    if (departamentoBD == null)
                    {
                        MessageBox.Show($"Erro: Departamento com ID {departamentoId} nu00e3o encontrado no banco de dados.", "Erro",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    
                    // Atualizar o funcionu00e1rio com o departamento correto
                    funcionario.DepartamentoId = departamentoBD.Id;
                    funcionario.Departamento = departamentoBD;
                    Console.WriteLine($"Departamento definido para atualizau00e7u00e3o: ID {departamentoBD.Id}, Nome: {departamentoBD.Nome}");
                    
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
