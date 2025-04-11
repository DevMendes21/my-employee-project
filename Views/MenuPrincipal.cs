using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using MinhaEmpresa.DAO;

namespace MinhaEmpresa.Views
{
    public class MenuPrincipal : Form
    {
        private readonly FuncionarioDAO funcionarioDAO = new();
        private Panel sideMenu;
        private Panel dashboardPanel;
        private TableLayoutPanel indicadoresPanel;
        private ListView listSalariosPorDepartamento = new();
        private ListView listFuncionariosPorCargo = new();
        private Label lblTotalFuncionarios = new();
        private Label lblCustoTotal = new();
        private Label lblMediaSalarial = new();

        public MenuPrincipal()
        {
            sideMenu = new Panel();
            dashboardPanel = new Panel();
            indicadoresPanel = new TableLayoutPanel();
            InitializeComponent();
            CarregarDashboard();
        }

        private void InitializeComponent()
        {
            this.Text = "Sistema de Gestão de Funcionários";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(240, 240, 240);

            // Side Menu
            sideMenu = new Panel
            {
                BackColor = Color.FromArgb(51, 51, 76),
                Location = new Point(0, 0),
                Width = 220,
                Dock = DockStyle.Left
            };

            var btnFuncionarios = new Button
            {
                Text = "Funcionários",
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.Gainsboro,
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                Dock = DockStyle.Top,
                Height = 60,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0),
                FlatAppearance = { BorderSize = 0 }
            };
            btnFuncionarios.Click += (s, e) => new ListagemFuncionarios().Show();

            var btnCargos = new Button
            {
                Text = "Cargos",
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.Gainsboro,
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                Dock = DockStyle.Top,
                Height = 60,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0),
                FlatAppearance = { BorderSize = 0 }
            };

            var btnDepartamentos = new Button
            {
                Text = "Departamentos",
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.Gainsboro,
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                Dock = DockStyle.Top,
                Height = 60,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0),
                FlatAppearance = { BorderSize = 0 }
            };

            // Logo Panel
            var logoPanel = new Panel
            {
                BackColor = Color.FromArgb(39, 39, 58),
                Dock = DockStyle.Top,
                Height = 80
            };

            var lblLogo = new Label
            {
                Text = "Gestão RH",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 15F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            logoPanel.Controls.Add(lblLogo);

            sideMenu.Controls.AddRange(new Control[] { btnDepartamentos, btnCargos, btnFuncionarios, logoPanel });

            // Dashboard Panel
            dashboardPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            // Indicadores
            indicadoresPanel.Dock = DockStyle.Top;
            indicadoresPanel.Height = 120;
            indicadoresPanel.Padding = new Padding(10);
            indicadoresPanel.ColumnCount = 4;
            indicadoresPanel.RowCount = 1;
            indicadoresPanel.BackColor = Color.White;
            indicadoresPanel.Margin = new Padding(0, 0, 0, 20);
            
            for (int i = 0; i < 4; i++)
            {
                indicadoresPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            }


            // Total de Funcionários
            var card1 = CriarCardIndicador("Total de Funcionários", "0", "person");
            lblTotalFuncionarios = (Label)card1.Controls[0].Controls[0];
            indicadoresPanel.Controls.Add(card1, 0, 0);

            // Custo Total
            var card2 = CriarCardIndicador("Custo Total Mensal", "R$ 0,00", "money");
            lblCustoTotal = (Label)card2.Controls[0].Controls[0];
            indicadoresPanel.Controls.Add(card2, 1, 0);

            // Média Salarial
            var card3 = CriarCardIndicador("Média Salarial", "R$ 0,00", "chart-line");
            lblMediaSalarial = (Label)card3.Controls[0].Controls[0];
            indicadoresPanel.Controls.Add(card3, 2, 0);

            // Maior Salário
            var card4 = CriarCardIndicador("Maior Salário", "R$ 0,00", "trophy");
            indicadoresPanel.Controls.Add(card4, 3, 0);

            // Painéis de Informações
            var graficosPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                ColumnCount = 2,
                RowCount = 1
            };
            graficosPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            graficosPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            // Lista de Salários por Departamento
            var panelSalarios = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                BackColor = Color.White
            };

            var lblSalarios = new Label
            {
                Text = "Salários por Departamento",
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 51, 76),
                TextAlign = ContentAlignment.MiddleLeft,
                Height = 30
            };

            listSalariosPorDepartamento = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.75F)
            };
            listSalariosPorDepartamento.Columns.Add("Departamento", 200);
            listSalariosPorDepartamento.Columns.Add("Total", 150);

            panelSalarios.Controls.AddRange(new Control[] { listSalariosPorDepartamento, lblSalarios });

            // Lista de Funcionários por Cargo
            var panelCargos = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                BackColor = Color.White
            };

            var lblCargos = new Label
            {
                Text = "Funcionários por Cargo",
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 51, 76),
                TextAlign = ContentAlignment.MiddleLeft,
                Height = 30
            };

            listFuncionariosPorCargo = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.75F)
            };
            listFuncionariosPorCargo.Columns.Add("Cargo", 200);
            listFuncionariosPorCargo.Columns.Add("Total", 150);

            panelCargos.Controls.AddRange(new Control[] { listFuncionariosPorCargo, lblCargos });

            graficosPanel.Controls.Add(panelSalarios, 0, 0);
            graficosPanel.Controls.Add(panelCargos, 1, 0);

            dashboardPanel.Controls.Add(graficosPanel);
            dashboardPanel.Controls.Add(indicadoresPanel);

            this.Controls.AddRange(new Control[] { dashboardPanel, sideMenu });
        }

        private Panel CriarCardIndicador(string titulo, string valor, string icone)
        {
            var card = new Panel
            {
                BackColor = Color.White,
                Margin = new Padding(10),
                Dock = DockStyle.Fill,
                Padding = new Padding(15)
            };

            var lblIcone = new Label
            {
                Text = ObterIcone(icone),
                Font = new Font("Segoe UI Symbol", 24F, FontStyle.Regular),
                ForeColor = Color.FromArgb(51, 51, 76),
                Dock = DockStyle.Left,
                Width = 40,
                TextAlign = ContentAlignment.MiddleCenter
            };

            var conteudoPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10, 0, 0, 0)
            };

            var lblTitulo = new Label
            {
                Text = titulo,
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = Color.Gray,
                Dock = DockStyle.Top,
                Height = 25
            };

            var lblValor = new Label
            {
                Text = valor,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 51, 76),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            conteudoPanel.Controls.AddRange(new Control[] { lblValor, lblTitulo });
            card.Controls.AddRange(new Control[] { conteudoPanel, lblIcone });
            return card;
        }

        private string ObterIcone(string nome)
        {
            return nome switch
            {
                "person" => "὆4",     // 👤
                "money" => "Ὃ0",     // 💰
                "chart-line" => "Ὄ8", // 📈
                "trophy" => "Ἴ6",     // 🏆
                _ => "⭐"              // ⭐
            };
        }

        private void CarregarDashboard()
        {
            try
            {
                var funcionarios = funcionarioDAO.ListarFuncionarios();

                // Atualizar indicadores
                var totalFuncionarios = funcionarios.Count;
                var custoTotal = funcionarios.Sum(f => f.Salario);
                var mediaSalarial = funcionarios.Any() ? funcionarios.Average(f => f.Salario) : 0;
                var maiorSalario = funcionarios.Any() ? funcionarios.Max(f => f.Salario) : 0;

                indicadoresPanel.Controls.Clear();
                indicadoresPanel.Controls.Add(CriarCardIndicador("Total de Funcionários", totalFuncionarios.ToString(), "person"), 0, 0);
                indicadoresPanel.Controls.Add(CriarCardIndicador("Custo Total", custoTotal.ToString("C2"), "money"), 1, 0);
                indicadoresPanel.Controls.Add(CriarCardIndicador("Média Salarial", mediaSalarial.ToString("C2"), "chart-line"), 2, 0);
                indicadoresPanel.Controls.Add(CriarCardIndicador("Maior Salário", maiorSalario.ToString("C2"), "trophy"), 3, 0);

                // Atualizar lista de salários por departamento
                var salariosPorDepartamento = funcionarios
                    .GroupBy(f => f.Departamento?.Nome ?? "Sem Departamento")
                    .Select(g => new { Departamento = g.Key, TotalSalarios = g.Sum(f => f.Salario) });

                listSalariosPorDepartamento.Items.Clear();
                foreach (var item in salariosPorDepartamento)
                {
                    var listItem = new ListViewItem(item.Departamento);
                    listItem.SubItems.Add(item.TotalSalarios.ToString("C2"));
                    listSalariosPorDepartamento.Items.Add(listItem);
                }

                // Atualizar lista de funcionários por cargo
                var funcionariosPorCargo = funcionarios
                    .GroupBy(f => f.Cargo?.Nome ?? "Sem Cargo")
                    .Select(g => new { Cargo = g.Key, Total = g.Count() });

                listFuncionariosPorCargo.Items.Clear();
                foreach (var item in funcionariosPorCargo)
                {
                    var listItem = new ListViewItem(item.Cargo);
                    listItem.SubItems.Add($"{item.Total} ({(double)item.Total / funcionarios.Count:P0})");
                    listFuncionariosPorCargo.Items.Add(listItem);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar dashboard: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
