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
        private Panel sideMenu;
        private Panel dashboardPanel;
        private TableLayoutPanel indicadoresPanel;
        private Panel graficosPanel;
        private ListView listSalariosPorDepartamento;
        private ListView listFuncionariosPorCargo;
        private FuncionarioDAO funcionarioDAO;

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
            listSalariosPorDepartamento = new ListView();
            listFuncionariosPorCargo = new ListView();
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

            var btnFuncionarios = CriarBotaoMenu("Funcionários");
            btnFuncionarios.Click += (s, e) => new ListagemFuncionarios().Show();

            var btnCargos = CriarBotaoMenu("Cargos");
            var btnDepartamentos = CriarBotaoMenu("Departamentos");

            sideMenu.Controls.AddRange(new Control[] { btnDepartamentos, btnCargos, btnFuncionarios, logoPanel });
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
            
            // Store the ListView in the appropriate variable based on the title
            if (titulo == "Salários por Departamento")
            {
                // Replace the existing ListView with this one
                listSalariosPorDepartamento = listView;
            }
            else if (titulo == "Funcionários por Cargo")
            {
                // Replace the existing ListView with this one
                listFuncionariosPorCargo = listView;
            }
            
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
            var panel = new Panel
            {
                BackColor = Color.White,
                Margin = new Padding(5),
                Padding = new Padding(10),
                Dock = DockStyle.Fill
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
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 51, 76),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            
            var iconePictureBox = new PictureBox
            {
                Image = ObterIcone(icone),
                SizeMode = PictureBoxSizeMode.Zoom,
                Width = 40,
                Height = 40,
                Dock = DockStyle.Right
            };
            
            panel.Controls.Add(iconePictureBox);
            panel.Controls.Add(lblValor);
            panel.Controls.Add(lblTitulo);
            
            return panel;
        }

        private Image ObterIcone(string nome)
        {
            // Placeholder - in a real application, you would load actual icons
            var bitmap = new Bitmap(32, 32);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.FromArgb(51, 51, 76));
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

        private void AtualizarListViews(List<Funcionario> funcionarios)
        {
            AtualizarListViewSalarios(funcionarios);
            AtualizarListViewCargos(funcionarios);
            AtualizarListViewMediaSalarial(funcionarios);
            AtualizarListViewStatus(funcionarios);
        }

        private void AtualizarListViewSalarios(List<Funcionario> funcionarios)
        {
            listSalariosPorDepartamento.Items.Clear();
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
                listSalariosPorDepartamento.Items.Add(listViewItem);
            }
        }

        private void AtualizarListViewCargos(List<Funcionario> funcionarios)
        {
            listFuncionariosPorCargo.Items.Clear();
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
                listFuncionariosPorCargo.Items.Add(listViewItem);
            }
        }
        
        private void AtualizarListViewMediaSalarial(List<Funcionario> funcionarios)
        {
            // Find the ListView for media salarial
            ListView listViewMediaSalarial = null;
            foreach (Control control in graficosPanel.Controls)
            {
                if (control is TableLayoutPanel tablePanel)
                {
                    foreach (Control panelControl in tablePanel.Controls)
                    {
                        if (panelControl is Panel panel)
                        {
                            var headerPanel = panel.Controls.OfType<Panel>().FirstOrDefault(p => p.Dock == DockStyle.Top);
                            if (headerPanel != null)
                            {
                                var titleLabel = headerPanel.Controls.OfType<Label>().FirstOrDefault();
                                if (titleLabel != null && titleLabel.Text == "Média Salarial por Cargo")
                                {
                                    var contentPanel = panel.Controls.OfType<Panel>().FirstOrDefault(p => p.Dock == DockStyle.Fill);
                                    if (contentPanel != null)
                                    {
                                        listViewMediaSalarial = contentPanel.Controls.OfType<ListView>().FirstOrDefault();
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (listViewMediaSalarial != null) break;
                }
            }
            
            if (listViewMediaSalarial != null)
            {
                listViewMediaSalarial.Items.Clear();
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
                    listViewMediaSalarial.Items.Add(listViewItem);
                }
            }
        }
        
        private void AtualizarListViewStatus(List<Funcionario> funcionarios)
        {
            // Find the ListView for status
            ListView listViewStatus = null;
            foreach (Control control in graficosPanel.Controls)
            {
                if (control is TableLayoutPanel tablePanel)
                {
                    foreach (Control panelControl in tablePanel.Controls)
                    {
                        if (panelControl is Panel panel)
                        {
                            var headerPanel = panel.Controls.OfType<Panel>().FirstOrDefault(p => p.Dock == DockStyle.Top);
                            if (headerPanel != null)
                            {
                                var titleLabel = headerPanel.Controls.OfType<Label>().FirstOrDefault();
                                if (titleLabel != null && titleLabel.Text == "Status dos Funcionários")
                                {
                                    var contentPanel = panel.Controls.OfType<Panel>().FirstOrDefault(p => p.Dock == DockStyle.Fill);
                                    if (contentPanel != null)
                                    {
                                        listViewStatus = contentPanel.Controls.OfType<ListView>().FirstOrDefault();
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (listViewStatus != null) break;
                }
            }
            
            if (listViewStatus != null)
            {
                listViewStatus.Items.Clear();
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
                    listViewStatus.Items.Add(listViewItem);
                }
            }
        }
    }
}
