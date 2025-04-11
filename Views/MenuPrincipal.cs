using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using MinhaEmpresa.DAO;
using MinhaEmpresa.Models;

namespace MinhaEmpresa.Views
{
    public partial class MenuPrincipal : Form
    {
        // UI Components
        private Panel sideMenu = null!;
        private Panel dashboardPanel = null!;
        private TableLayoutPanel indicadoresPanel = null!;
        private Panel graficosPanel = null!;
        private FuncionarioDAO funcionarioDAO = null!;
        
        // Dictionary to store all ListViews by their title
        private Dictionary<string, ListView> listViews = new Dictionary<string, ListView>();

        public MenuPrincipal()
        {
            InitializeComponent();
            InicializarComponentesUI();
            ConfigurarSideMenu();
            ConfigurarDashboard();
            ConfigurarListViews();
            CarregarDashboard();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MenuPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Name = "MenuPrincipal";
            this.Text = "Sistema de Gestão de Funcionários";
            this.WindowState = FormWindowState.Maximized;
            this.ResumeLayout(false);
        }

        private void InicializarComponentesUI()
        {
            // Initialize UI components
            sideMenu = new Panel();
            dashboardPanel = new Panel();
            indicadoresPanel = new TableLayoutPanel();
            graficosPanel = new Panel();
            funcionarioDAO = new FuncionarioDAO();
            
            // Configure form properties
            this.BackColor = Color.FromArgb(240, 240, 240);

            // Add main panels to form
            this.Controls.Add(dashboardPanel);
            this.Controls.Add(sideMenu);
        }

        private void ConfigurarSideMenu()
        {
            sideMenu.BackColor = Color.FromArgb(51, 51, 76);
            sideMenu.Location = new Point(0, 0);
            sideMenu.Width = 220;
            sideMenu.Dock = DockStyle.Left;

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
            
            // Dashboard button
            var btnDashboard = CriarBotaoMenu("Dashboard");
            btnDashboard.Image = ObterIcone("dashboard");
            btnDashboard.ImageAlign = ContentAlignment.MiddleLeft;
            btnDashboard.Padding = new Padding(10, 0, 0, 0);
            btnDashboard.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnDashboard.Click += (s, e) => MostrarDashboard();
            
            // Test buttons for role assignment
            var btnTesteRoles = CriarBotaoMenu("Teste Cargos Específicos");
            btnTesteRoles.Image = ObterIcone("chart");
            btnTesteRoles.ImageAlign = ContentAlignment.MiddleLeft;
            btnTesteRoles.Padding = new Padding(10, 0, 0, 0);
            btnTesteRoles.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnTesteRoles.Click += (s, e) => TestHelper.TestRoleAssignment();
            
            // Test button for all role-department combinations
            var btnTesteAllCombinations = CriarBotaoMenu("Teste Todas Combinações");
            btnTesteAllCombinations.Image = ObterIcone("chart");
            btnTesteAllCombinations.ImageAlign = ContentAlignment.MiddleLeft;
            btnTesteAllCombinations.Padding = new Padding(10, 0, 0, 0);
            btnTesteAllCombinations.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnTesteAllCombinations.Click += (s, e) => TestHelper.TestAllCombinations();
            
            // Funcionarios submenu
            var btnFuncionarios = CriarBotaoMenu("Funcionários");
            btnFuncionarios.Image = ObterIcone("user");
            btnFuncionarios.ImageAlign = ContentAlignment.MiddleLeft;
            btnFuncionarios.Padding = new Padding(10, 0, 0, 0);
            btnFuncionarios.TextImageRelation = TextImageRelation.ImageBeforeText;
            
            var panelSubMenuFuncionarios = new Panel
            {
                BackColor = Color.FromArgb(35, 35, 60),
                Dock = DockStyle.Top,
                Height = 0,  // Start collapsed
                Visible = false
            };
            
            var btnListarFuncionarios = CriarBotaoSubMenu("Listar Funcionários");
            btnListarFuncionarios.Click += (s, e) => new ListagemFuncionarios().Show();
            
            var btnNovoFuncionario = CriarBotaoSubMenu("Novo Funcionário");
            btnNovoFuncionario.Click += (s, e) => new CadastroFuncionario().Show();
            
            panelSubMenuFuncionarios.Controls.AddRange(new Control[] { btnNovoFuncionario, btnListarFuncionarios });
            
            btnFuncionarios.Click += (s, e) => ToggleSubMenu(panelSubMenuFuncionarios);
            
            // Cargos button
            var btnCargos = CriarBotaoMenu("Cargos");
            btnCargos.Image = ObterIcone("briefcase");
            btnCargos.ImageAlign = ContentAlignment.MiddleLeft;
            btnCargos.Padding = new Padding(10, 0, 0, 0);
            btnCargos.TextImageRelation = TextImageRelation.ImageBeforeText;
            
            // Departamentos button
            var btnDepartamentos = CriarBotaoMenu("Departamentos");
            btnDepartamentos.Image = ObterIcone("building");
            btnDepartamentos.ImageAlign = ContentAlignment.MiddleLeft;
            btnDepartamentos.Padding = new Padding(10, 0, 0, 0);
            btnDepartamentos.TextImageRelation = TextImageRelation.ImageBeforeText;
            
            // Relatórios button
            var btnRelatorios = CriarBotaoMenu("Relatórios");
            btnRelatorios.Image = ObterIcone("chart");
            btnRelatorios.ImageAlign = ContentAlignment.MiddleLeft;
            btnRelatorios.Padding = new Padding(10, 0, 0, 0);
            btnRelatorios.TextImageRelation = TextImageRelation.ImageBeforeText;
            
            // Add all controls to the side menu
            sideMenu.Controls.AddRange(new Control[] 
            { 
                btnRelatorios,
                btnDepartamentos, 
                btnCargos, 
                panelSubMenuFuncionarios,
                btnFuncionarios, 
                btnDashboard,
                logoPanel 
            });
        }

        private Button CriarBotaoMenu(string texto)
        {
            var btn = new Button
            {
                Text = texto,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.Gainsboro,
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                Dock = DockStyle.Top,
                Height = 60,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private void ConfigurarDashboard()
        {
            dashboardPanel.Dock = DockStyle.Fill;
            dashboardPanel.Padding = new Padding(20);

            ConfigurarPainelIndicadores();
            ConfigurarPainelGraficos();

            dashboardPanel.Controls.Add(graficosPanel);
            dashboardPanel.Controls.Add(indicadoresPanel);
        }

        private void ConfigurarPainelIndicadores()
        {
            indicadoresPanel.ColumnCount = 4;
            indicadoresPanel.RowCount = 1;
            indicadoresPanel.Dock = DockStyle.Top;
            indicadoresPanel.Height = 150;
            indicadoresPanel.BackColor = Color.FromArgb(240, 240, 240);
            indicadoresPanel.Padding = new Padding(20);
            indicadoresPanel.Margin = new Padding(0, 0, 0, 20); // Add margin at the bottom
            
            // Configure column and row styles
            for (int i = 0; i < indicadoresPanel.ColumnCount; i++)
            {
                indicadoresPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            }
            
            indicadoresPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        }

        private void ConfigurarPainelGraficos()
        {
            graficosPanel.Dock = DockStyle.Fill;
            graficosPanel.Padding = new Padding(10);
            graficosPanel.BackColor = Color.FromArgb(240, 240, 240);
            
            // Create a TableLayoutPanel for the charts
            var tableLayout = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 2,
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };
            
            // Configure column and row styles
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            
            // Add chart panels
            var salariosDepartamentoPanel = CriarPainelGrafico("Salários por Departamento", new[] { "Departamento", "Funcionários", "Total Salários" });
            var funcionariosCargoPanel = CriarPainelGrafico("Funcionários por Cargo", new[] { "Cargo", "Quantidade", "Percentual" });
            var mediaSalarialPanel = CriarPainelGrafico("Média Salarial por Cargo", new[] { "Cargo", "Média Salarial" });
            var statusPanel = CriarPainelGrafico("Status dos Funcionários", new[] { "Status", "Quantidade", "Percentual" });
            
            // Create chart visualizations
            AdicionarVisualizacaoGrafica(salariosDepartamentoPanel, "salarios");
            AdicionarVisualizacaoGrafica(funcionariosCargoPanel, "funcionarios");
            AdicionarVisualizacaoGrafica(mediaSalarialPanel, "media");
            AdicionarVisualizacaoGrafica(statusPanel, "status");
            
            tableLayout.Controls.Add(salariosDepartamentoPanel, 0, 0);
            tableLayout.Controls.Add(funcionariosCargoPanel, 1, 0);
            tableLayout.Controls.Add(mediaSalarialPanel, 0, 1);
            tableLayout.Controls.Add(statusPanel, 1, 1);
            
            graficosPanel.Controls.Add(tableLayout);
        }

        private Panel CriarPainelGrafico(string titulo, string[] colunas)
        {
            var panel = new Panel
            {
                BackColor = Color.White,
                Dock = DockStyle.Fill,
                Margin = new Padding(10),
                Padding = new Padding(0),
                BorderStyle = BorderStyle.None
            };
            
            // Header panel for the title
            var headerPanel = new Panel
            {
                BackColor = Color.FromArgb(51, 51, 76),
                Dock = DockStyle.Top,
                Height = 40
            };
            
            var titleLabel = new Label
            {
                Text = titulo,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12F, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill,
                Padding = new Padding(10, 0, 0, 0)
            };
            
            headerPanel.Controls.Add(titleLabel);
            
            // Content panel for the ListView
            var contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };
            
            // Create and configure the ListView
            var listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };
            
            // Add columns
            foreach (var coluna in colunas)
            {
                listView.Columns.Add(coluna, -2);
            }
            
            // Store the ListView in our dictionary
            if (listViews.ContainsKey(titulo))
            {
                // Remove the old ListView from the dictionary
                listViews.Remove(titulo);
            }
            
            // Add the new ListView to the dictionary
            listViews[titulo] = listView;
            
            contentPanel.Controls.Add(listView);
            
            panel.Controls.Add(contentPanel);
            panel.Controls.Add(headerPanel);
            
            return panel;
        }

        private void ConfigurarListViews()
        {
            // The ListViews are already configured in CriarPainelGrafico
            // This method is kept for compatibility with the constructor call sequence
        }

        private void CriarIndicadores()
        {
            try
            {
                var funcionarios = funcionarioDAO.ListarFuncionarios();
                var totalFuncionarios = funcionarios.Count;
                var totalSalarios = funcionarios.Sum(f => f.Salario);
                var mediaSalarial = totalFuncionarios > 0 ? totalSalarios / totalFuncionarios : 0;
                var departamentos = funcionarios.Select(f => f.Departamento?.Nome).Distinct().Count();

                var cards = new[]
                {
                    CriarCardIndicador("Total Funcionários", totalFuncionarios.ToString(), "user"),
                    CriarCardIndicador("Custo Total", totalSalarios.ToString("C2"), "money"),
                    CriarCardIndicador("Média Salarial", mediaSalarial.ToString("C2"), "chart"),
                    CriarCardIndicador("Departamentos", departamentos.ToString(), "building")
                };

                // Clear existing controls before adding new ones
                indicadoresPanel.Controls.Clear();
                
                for (int i = 0; i < cards.Length; i++)
                {
                    var card = cards[i];
                    indicadoresPanel.Controls.Add(card, i, 0);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao criar indicadores: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Panel CriarCardIndicador(string titulo, string valor, string icone)
        {
            // Determine card color based on icon type
            Color cardAccentColor = Color.FromArgb(51, 51, 76); // Default
            switch (icone.ToLower())
            {
                case "user":
                    cardAccentColor = Color.FromArgb(0, 166, 90); // Green
                    break;
                case "money":
                    cardAccentColor = Color.FromArgb(221, 75, 57); // Red
                    break;
                case "chart":
                    cardAccentColor = Color.FromArgb(243, 156, 18); // Orange
                    break;
                case "building":
                    cardAccentColor = Color.FromArgb(60, 141, 188); // Blue
                    break;
            }
            
            var panel = new Panel
            {
                BackColor = Color.White,
                Margin = new Padding(5),
                Padding = new Padding(0),
                Dock = DockStyle.Fill
            };
            
            // Add a left border with accent color
            var borderPanel = new Panel
            {
                BackColor = cardAccentColor,
                Dock = DockStyle.Left,
                Width = 5
            };
            
            var contentPanel = new Panel
            {
                BackColor = Color.White,
                Dock = DockStyle.Fill,
                Padding = new Padding(15, 10, 10, 10)
            };
            
            var lblTitulo = new Label
            {
                Text = titulo,
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 100, 100),
                Dock = DockStyle.Top,
                Height = 25
            };
            
            var lblValor = new Label
            {
                Text = valor,
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = cardAccentColor,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            
            var iconePictureBox = new PictureBox
            {
                Image = ObterIcone(icone),
                SizeMode = PictureBoxSizeMode.Zoom,
                Width = 50,
                Height = 50,
                Dock = DockStyle.Right,
                BackColor = Color.Transparent
            };
            
            // Add drop shadow effect
            panel.Paint += (s, e) =>
            {
                var rect = new Rectangle(0, 0, panel.Width, panel.Height);
                using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    path.AddRectangle(rect);
                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    using (var pen = new Pen(Color.FromArgb(30, 0, 0, 0), 1))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            };
            
            contentPanel.Controls.Add(iconePictureBox);
            contentPanel.Controls.Add(lblValor);
            contentPanel.Controls.Add(lblTitulo);
            
            panel.Controls.Add(contentPanel);
            panel.Controls.Add(borderPanel);
            
            return panel;
        }

        private Button CriarBotaoSubMenu(string texto)
        {
            var btn = new Button
            {
                Text = texto,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                Dock = DockStyle.Top,
                Height = 40,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(35, 0, 0, 0),
                BackColor = Color.FromArgb(35, 35, 60)
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }
        
        private void ToggleSubMenu(Panel subMenu)
        {
            if (subMenu.Visible)
            {
                subMenu.Visible = false;
                subMenu.Height = 0;
            }
            else
            {
                subMenu.Visible = true;
                subMenu.Height = 80; // Height for 2 submenu items
            }
        }
        
        private void MostrarDashboard()
        {
            // Show dashboard panel, hide other panels if needed
            dashboardPanel.Visible = true;
            // You could hide other panels here if you add more views
        }
        
        private Image ObterIcone(string nome)
        {
            // Placeholder - in a real application, you would load actual icons
            var bitmap = new Bitmap(24, 24);
            using (var g = Graphics.FromImage(bitmap))
            {
                var color = Color.White;
                
                switch (nome.ToLower())
                {
                    case "dashboard":
                        color = Color.FromArgb(0, 192, 239); // Light blue
                        break;
                    case "user":
                        color = Color.FromArgb(0, 166, 90); // Green
                        break;
                    case "building":
                        color = Color.FromArgb(243, 156, 18); // Orange
                        break;
                    case "briefcase":
                        color = Color.FromArgb(221, 75, 57); // Red
                        break;
                    case "chart":
                        color = Color.FromArgb(60, 141, 188); // Blue
                        break;
                    case "money":
                        color = Color.FromArgb(0, 192, 239); // Light blue
                        break;
                }
                
                g.Clear(color);
            }
            return bitmap;
        }

        private void CarregarDashboard()
        {
            try
            {
                var funcionarios = funcionarioDAO.ListarFuncionarios();
                CriarIndicadores();
                AtualizarListViews(funcionarios);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar dashboard: {ex.Message}", "Erro", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AdicionarVisualizacaoGrafica(Panel panel, string tipo)
        {
            // Find the content panel that contains the ListView
            var contentPanel = panel.Controls.OfType<Panel>().FirstOrDefault(p => p.Dock == DockStyle.Fill);
            if (contentPanel == null) return;
            
            // Get the ListView
            var listView = contentPanel.Controls.OfType<ListView>().FirstOrDefault();
            if (listView == null) return;
            
            // Create a panel for the chart
            var chartPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 150,
                BackColor = Color.White
            };
            
            // Add the chart panel above the ListView
            contentPanel.Controls.Add(chartPanel);
            chartPanel.BringToFront();
            
            // We'll draw the chart when data is loaded
            chartPanel.Paint += (s, e) => 
            {
                if (listView.Items.Count == 0) return;
                
                var g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                
                // Draw different chart types based on the data
                switch (tipo)
                {
                    case "salarios":
                        DesenharGraficoBarras(g, listView, chartPanel.ClientRectangle, Color.FromArgb(0, 192, 239));
                        break;
                    case "funcionarios":
                        DesenharGraficoPizza(g, listView, chartPanel.ClientRectangle, new[] { 
                            Color.FromArgb(0, 166, 90),  // Green
                            Color.FromArgb(243, 156, 18), // Orange
                            Color.FromArgb(221, 75, 57),  // Red
                            Color.FromArgb(60, 141, 188)  // Blue
                        });
                        break;
                    case "media":
                        DesenharGraficoBarras(g, listView, chartPanel.ClientRectangle, Color.FromArgb(243, 156, 18));
                        break;
                    case "status":
                        DesenharGraficoPizza(g, listView, chartPanel.ClientRectangle, new[] { 
                            Color.FromArgb(0, 166, 90),   // Green for Active
                            Color.FromArgb(221, 75, 57),   // Red for Inactive
                            Color.FromArgb(243, 156, 18),  // Orange for Leave
                            Color.FromArgb(60, 141, 188)   // Blue for Other
                        });
                        break;
                }
            };
        }
        
        private void DesenharGraficoBarras(Graphics g, ListView listView, Rectangle rect, Color barColor)
        {
            if (listView.Items.Count == 0) return;
            
            // Calculate the maximum value for scaling
            decimal maxValue = 0;
            var values = new List<decimal>();
            var labels = new List<string>();
            
            for (int i = 0; i < Math.Min(listView.Items.Count, 5); i++) // Limit to 5 items
            {
                var item = listView.Items[i];
                string valueText = item.SubItems[item.SubItems.Count - 1].Text.Replace("R$", "").Replace(".", "").Replace(",", ".");
                
                if (decimal.TryParse(valueText, out decimal value))
                {
                    values.Add(value);
                    labels.Add(item.Text);
                    maxValue = Math.Max(maxValue, value);
                }
            }
            
            if (values.Count == 0) return;
            
            // Draw the bars
            int barWidth = (rect.Width - 40) / values.Count;
            int maxHeight = rect.Height - 40;
            
            for (int i = 0; i < values.Count; i++)
            {
                int barHeight = maxValue > 0 ? (int)((values[i] / maxValue) * maxHeight) : 0;
                Rectangle barRect = new Rectangle(
                    rect.X + 20 + (i * barWidth),
                    rect.Y + rect.Height - 20 - barHeight,
                    barWidth - 10,
                    barHeight
                );
                
                // Draw the bar
                using (var brush = new SolidBrush(barColor))
                {
                    g.FillRectangle(brush, barRect);
                }
                
                // Draw the label
                using (var brush = new SolidBrush(Color.Black))
                using (var font = new Font("Segoe UI", 8))
                {
                    var labelSize = g.MeasureString(labels[i], font);
                    g.DrawString(labels[i], font, brush, 
                        rect.X + 20 + (i * barWidth) + ((barWidth - 10) / 2) - (labelSize.Width / 2),
                        rect.Y + rect.Height - 15);
                }
            }
        }
        
        private void DesenharGraficoPizza(Graphics g, ListView listView, Rectangle rect, Color[] colors)
        {
            if (listView.Items.Count == 0) return;
            
            // Calculate the total value
            decimal totalValue = 0;
            var values = new List<decimal>();
            var labels = new List<string>();
            
            for (int i = 0; i < Math.Min(listView.Items.Count, colors.Length); i++) // Limit to available colors
            {
                var item = listView.Items[i];
                string valueText = item.SubItems[1].Text; // Assume the count is in the second column
                
                if (decimal.TryParse(valueText, out decimal value))
                {
                    values.Add(value);
                    labels.Add(item.Text);
                    totalValue += value;
                }
            }
            
            if (totalValue == 0) return;
            
            // Draw the pie chart
            int size = Math.Min(rect.Width, rect.Height) - 40;
            Rectangle pieRect = new Rectangle(
                rect.X + (rect.Width - size) / 2,
                rect.Y + 10,
                size,
                size
            );
            
            float startAngle = 0;
            for (int i = 0; i < values.Count; i++)
            {
                float sweepAngle = (float)((values[i] / totalValue) * 360);
                
                // Draw the pie slice
                using (var brush = new SolidBrush(colors[i % colors.Length]))
                {
                    g.FillPie(brush, pieRect, startAngle, sweepAngle);
                }
                
                // Draw the legend item
                int legendY = rect.Y + 10 + (i * 20);
                Rectangle legendRect = new Rectangle(rect.X + rect.Width - 100, legendY, 15, 15);
                
                using (var brush = new SolidBrush(colors[i % colors.Length]))
                {
                    g.FillRectangle(brush, legendRect);
                }
                
                using (var brush = new SolidBrush(Color.Black))
                using (var font = new Font("Segoe UI", 8))
                {
                    g.DrawString(labels[i], font, brush, rect.X + rect.Width - 80, legendY);
                }
                
                startAngle += sweepAngle;
            }
        }

        private void AtualizarListViews(List<Funcionario> funcionarios)
        {
            AtualizarListViewSalarios(funcionarios);
            AtualizarListViewCargos(funcionarios);
            AtualizarListViewMediaSalarial(funcionarios);
            AtualizarListViewStatus(funcionarios);
            
            // Refresh the chart panels
            foreach (Control control in graficosPanel.Controls)
            {
                if (control is TableLayoutPanel tablePanel)
                {
                    foreach (Control panelControl in tablePanel.Controls)
                    {
                        if (panelControl is Panel panel)
                        {
                            var contentPanel = panel.Controls.OfType<Panel>().FirstOrDefault(p => p.Dock == DockStyle.Fill);
                            if (contentPanel != null)
                            {
                                var chartPanel = contentPanel.Controls.OfType<Panel>().FirstOrDefault(p => p.Dock == DockStyle.Top);
                                if (chartPanel != null)
                                {
                                    chartPanel.Invalidate(); // Trigger repaint to update the chart
                                }
                            }
                        }
                    }
                }
            }
        }

        private void AtualizarListViewSalarios(List<Funcionario> funcionarios)
        {
            if (!listViews.TryGetValue("Salários por Departamento", out var listView))
                return;
                
            listView.Items.Clear();
            var salariosPorDepartamento = funcionarios
                .GroupBy(f => f.Departamento?.Nome ?? "Sem Departamento")
                .Select(g => new
                {
                    Departamento = g.Key,
                    Quantidade = g.Count(),
                    TotalSalarios = g.Sum(f => f.Salario)
                })
                .OrderByDescending(g => g.TotalSalarios);

            foreach (var item in salariosPorDepartamento)
            {
                var listViewItem = new ListViewItem(item.Departamento);
                listViewItem.SubItems.Add(item.Quantidade.ToString());
                listViewItem.SubItems.Add(item.TotalSalarios.ToString("C"));
                listView.Items.Add(listViewItem);
            }
        }

        private void AtualizarListViewCargos(List<Funcionario> funcionarios)
        {
            if (!listViews.TryGetValue("Funcionários por Cargo", out var listView))
                return;
                
            listView.Items.Clear();
            var totalFuncionarios = funcionarios.Count;
            var funcionariosPorCargo = funcionarios
                .GroupBy(f => f.Cargo?.Nome ?? "Sem Cargo")
                .Select(g => new
                {
                    Cargo = g.Key,
                    Quantidade = g.Count(),
                    Percentual = totalFuncionarios > 0 ? (double)g.Count() / totalFuncionarios : 0
                })
                .OrderByDescending(g => g.Quantidade);

            foreach (var item in funcionariosPorCargo)
            {
                var listViewItem = new ListViewItem(item.Cargo);
                listViewItem.SubItems.Add(item.Quantidade.ToString());
                listViewItem.SubItems.Add(item.Percentual.ToString("P1"));
                listView.Items.Add(listViewItem);
            }
        }
        
        private void AtualizarListViewMediaSalarial(List<Funcionario> funcionarios)
        {
            if (!listViews.TryGetValue("Média Salarial por Cargo", out var listView))
                return;
                
            listView.Items.Clear();
            var mediaSalarialPorCargo = funcionarios
                .GroupBy(f => f.Cargo?.Nome ?? "Sem Cargo")
                .Select(g => new
                {
                    Cargo = g.Key,
                    MediaSalarial = g.Average(f => f.Salario)
                })
                .OrderByDescending(g => g.MediaSalarial);

            foreach (var item in mediaSalarialPorCargo)
            {
                var listViewItem = new ListViewItem(item.Cargo);
                listViewItem.SubItems.Add(item.MediaSalarial.ToString("C2"));
                listView.Items.Add(listViewItem);
            }
        }
        
        private void AtualizarListViewStatus(List<Funcionario> funcionarios)
        {
            if (!listViews.TryGetValue("Status dos Funcionários", out var listView))
                return;
                
            listView.Items.Clear();
            var totalFuncionarios = funcionarios.Count;
            var statusFuncionarios = funcionarios
                .GroupBy(f => f.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Total = g.Count(),
                    Percentual = totalFuncionarios > 0 ? (double)g.Count() / totalFuncionarios : 0
                })
                .OrderByDescending(g => g.Total);

            foreach (var item in statusFuncionarios)
            {
                var listViewItem = new ListViewItem(item.Status.ToString());
                listViewItem.SubItems.Add(item.Total.ToString());
                listViewItem.SubItems.Add(item.Percentual.ToString("P1"));
                listView.Items.Add(listViewItem);
            }
        }
    }
}
