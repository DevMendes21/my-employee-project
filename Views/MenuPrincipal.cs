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
        private Panel sideMenu = new();
        private Panel dashboardPanel = new();
        private TableLayoutPanel indicadoresPanel = new();
        private TableLayoutPanel graficosPanel = new()
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 2,
            Padding = new Padding(0, 20, 0, 0)
        };
        private ListView listSalariosPorDepartamento = new();
        private ListView listFuncionariosPorCargo = new();
        private Label lblTotalFuncionarios = new();
        private Label lblCustoTotal = new();
        private Label lblMediaSalarial = new();

        public MenuPrincipal()
        {
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
            dashboardPanel.Dock = DockStyle.Fill;
            dashboardPanel.Padding = new Padding(20);

            // Configurar indicadores
            var indicadores = new[]
            {
                ("Total de Funcionários", "0", "person"),
                ("Custo Total Mensal", "R$ 0,00", "money"),
                ("Média Salarial", "R$ 0,00", "chart-line"),
                ("Maior Salário", "R$ 0,00", "trophy")
            };

            // Configurar painel de indicadores
            this.indicadoresPanel.Dock = DockStyle.Top;
            this.indicadoresPanel.Height = 120;
            this.indicadoresPanel.Padding = new Padding(10);
            this.indicadoresPanel.BackColor = Color.FromArgb(240, 240, 240);
            this.indicadoresPanel.ColumnCount = 4;
            this.indicadoresPanel.RowCount = 1;

            // Configurar colunas do painel de indicadores
            for (int i = 0; i < 4; i++)
            {
                this.indicadoresPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            }

            // Adicionar indicadores
            for (int i = 0; i < indicadores.Length; i++)
            {
                var (titulo, valor, icone) = indicadores[i];
                var card = CriarCardIndicador(titulo, valor, icone);
                var label = (Label)card.Controls[0].Controls[0];

                switch (i)
                {
                    case 0:
                        lblTotalFuncionarios = label;
                        break;
                    case 1:
                        lblCustoTotal = label;
                        break;
                    case 2:
                        lblMediaSalarial = label;
                        break;
                }

                this.indicadoresPanel.Controls.Add(card, i, 0);
            }

            // Configurar painéis de gráficos
            var paineis = new[]
            {
                ("Custos por Departamento", "money", listSalariosPorDepartamento, new[] { ("Departamento", 200), ("Total", 150), ("%", 70) }),
                ("Distribuição por Cargo", "person", listFuncionariosPorCargo, new[] { ("Cargo", 200), ("Total", 100), ("%", 70) }),
                ("Média Salarial por Cargo", "chart-line", new ListView(), new[] { ("Cargo", 200), ("Média", 150) }),
                ("Status dos Funcionários", "info", new ListView(), new[] { ("Status", 200), ("Total", 100), ("%", 70) })
            };

            for (int i = 0; i < paineis.Length; i++)
            {
                var (titulo, icone, listView, colunas) = paineis[i];
                var painel = CriarPainelGrafico(titulo, icone);
                ConfigurarListView(listView, colunas);
                painel.Controls[0].Controls.Add(listView);
                this.graficosPanel.Controls.Add(painel, i % 2, i / 2);
            }

            this.dashboardPanel.Controls.Add(this.indicadoresPanel);
            this.dashboardPanel.Controls.Add(this.graficosPanel);
            this.indicadoresPanel.BackColor = Color.White;
            this.indicadoresPanel.Margin = new Padding(0, 0, 0, 20);






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

        private Panel CriarPainelGrafico(string titulo, string icone)
        {
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                BackColor = Color.White,
                Margin = new Padding(10)
            };

            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40
            };

            var lblIcone = new Label
            {
                Text = ObterIcone(icone),
                Font = new Font("Segoe UI Symbol", 16F, FontStyle.Regular),
                ForeColor = Color.FromArgb(51, 51, 76),
                Dock = DockStyle.Left,
                Width = 30,
                TextAlign = ContentAlignment.MiddleCenter
            };

            var lblTitulo = new Label
            {
                Text = titulo,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 51, 76),
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill,
                Padding = new Padding(5, 0, 0, 0)
            };

            headerPanel.Controls.AddRange(new Control[] { lblTitulo, lblIcone });

            var contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 10, 0, 0)
            };

            mainPanel.Controls.Add(contentPanel);
            mainPanel.Controls.Add(headerPanel);

            return mainPanel;
        }

        private void ConfigurarListView(ListView listView, (string Nome, int Largura)[] colunas)
        {
            listView.View = View.Details;
            listView.FullRowSelect = true;
            listView.GridLines = true;
            listView.Dock = DockStyle.Fill;
            listView.Font = new Font("Segoe UI", 9.75F);
            listView.BackColor = Color.White;
            listView.BorderStyle = BorderStyle.None;

            foreach (var (Nome, Largura) in colunas)
            {
                listView.Columns.Add(Nome, Largura);
            }
        }

        private string ObterIcone(string nome)
        {
            return nome switch
            {
                "person" => "὆4",     // 👤
                "money" => "Ὃ0",     // 💰
                "chart-line" => "Ὄ8", // 📈
                "trophy" => "Ἴ6",     // 🏆
                "info" => "ℹ",       // ℹ
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
                    .Select(g => new
                    {
                        Departamento = g.Key,
                        TotalSalarios = g.Sum(f => f.Salario),
                        Percentual = (double)(g.Sum(f => f.Salario) / custoTotal)
                    })
                    .OrderByDescending(g => g.TotalSalarios);

                listSalariosPorDepartamento.Items.Clear();
                foreach (var item in salariosPorDepartamento)
                {
                    var listItem = new ListViewItem(item.Departamento);
                    listItem.SubItems.Add(item.TotalSalarios.ToString("C2"));
                    listItem.SubItems.Add(item.Percentual.ToString("P1"));
                    listSalariosPorDepartamento.Items.Add(listItem);
                }

                // Atualizar lista de funcionários por cargo
                var funcionariosPorCargo = funcionarios
                    .GroupBy(f => f.Cargo?.Nome ?? "Sem Cargo")
                    .Select(g => new
                    {
                        Cargo = g.Key,
                        Total = g.Count(),
                        Percentual = (double)g.Count() / totalFuncionarios
                    })
                    .OrderByDescending(g => g.Total);

                listFuncionariosPorCargo.Items.Clear();
                foreach (var item in funcionariosPorCargo)
                {
                    var listItem = new ListViewItem(item.Cargo);
                    listItem.SubItems.Add(item.Total.ToString());
                    listItem.SubItems.Add(item.Percentual.ToString("P1"));
                    listFuncionariosPorCargo.Items.Add(listItem);
                }

                // Atualizar lista de média salarial por cargo
                var mediaSalarialPorCargo = funcionarios
                    .GroupBy(f => f.Cargo?.Nome ?? "Sem Cargo")
                    .Select(g => new
                    {
                        Cargo = g.Key,
                        MediaSalarial = g.Average(f => f.Salario)
                    })
                    .OrderByDescending(g => g.MediaSalarial);

                var listMediaSalarial = graficosPanel.Controls
                    .OfType<Panel>()
                    .FirstOrDefault(p => p.Controls.OfType<Label>().Any(l => l.Text == "Média Salarial por Cargo"))
                    ?.Controls.OfType<Panel>()
                    .FirstOrDefault()
                    ?.Controls.OfType<ListView>()
                    .FirstOrDefault();

                if (listMediaSalarial != null)
                {
                    listMediaSalarial.Items.Clear();
                    foreach (var item in mediaSalarialPorCargo)
                    {
                        var listItem = new ListViewItem(item.Cargo);
                        listItem.SubItems.Add(item.MediaSalarial.ToString("C2"));
                        listMediaSalarial.Items.Add(listItem);
                    }
                }

                // Atualizar lista de status
                var statusFuncionarios = funcionarios
                    .GroupBy(f => f.Status)
                    .Select(g => new
                    {
                        Status = g.Key,
                        Total = g.Count(),
                        Percentual = (double)g.Count() / totalFuncionarios
                    })
                    .OrderByDescending(g => g.Total);

                var listStatus = graficosPanel.Controls
                    .OfType<Panel>()
                    .FirstOrDefault(p => p.Controls.OfType<Label>().Any(l => l.Text == "Status dos Funcionários"))
                    ?.Controls.OfType<Panel>()
                    .FirstOrDefault()
                    ?.Controls.OfType<ListView>()
                    .FirstOrDefault();

                if (listStatus != null)
                {
                    listStatus.Items.Clear();
                    foreach (var item in statusFuncionarios)
                    {
                        var listItem = new ListViewItem(item.Status.ToString());
                        listItem.SubItems.Add(item.Total.ToString());
                        listItem.SubItems.Add(item.Percentual.ToString("P1"));
                        listStatus.Items.Add(listItem);
                    }
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
