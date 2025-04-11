using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MinhaEmpresa.DAO;
using MinhaEmpresa.Models;

namespace MinhaEmpresa.Views
{
    public partial class Dashboard : Form
    {
        private readonly FuncionarioDAO funcionarioDAO = new FuncionarioDAO();
        private readonly CargoDAO cargoDAO = new CargoDAO();
        private readonly DepartamentoDAO departamentoDAO = new DepartamentoDAO();
        
        // Painéis principais
        private Panel sidePanel = null!;
        private Panel contentPanel = null!;
        private Panel headerPanel = null!;
        
        // Controles do cabeçalho
        private Label lblTitle = null!;
        private Label lblDateTime = null!;
        private System.Windows.Forms.Timer timer = null!;
        
        // Controles de estatísticas
        private List<Panel> statPanels = null!;
        
        public Dashboard()
        {
            InitializeComponent();
            InitializeCustomComponents();
            LoadDashboardData();
            
            // Atualizar data e hora a cada segundo
            timer = new System.Windows.Forms.Timer { Interval = 1000 };
            timer.Tick += (s, e) => UpdateDateTime();
            timer.Start();
        }
        
        private void InitializeComponent()
        {
            this.Text = "Sistema de Gestão de Funcionários - Dashboard";
            this.Width = 1200;
            this.Height = 800;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = true;
            this.Icon = SystemIcons.Application;
        }
        
        private void InitializeCustomComponents()
        {
            // Criar painéis principais
            CreatePanels();
            
            // Criar menu lateral
            CreateSideMenu();
            
            // Criar cabeçalho
            CreateHeader();
            
            // Criar painéis de estatísticas
            CreateStatPanels();
            
            // Adicionar painéis ao formulário
            this.Controls.AddRange(new Control[] { sidePanel, contentPanel, headerPanel });
        }
        
        private void CreatePanels()
        {
            // Painel lateral (menu)
            sidePanel = new Panel
            {
                BackColor = Color.FromArgb(52, 73, 94),
                Location = new Point(0, 0),
                Width = 250,
                Height = this.ClientSize.Height,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left
            };
            
            // Painel de cabeçalho
            headerPanel = new Panel
            {
                BackColor = Color.FromArgb(41, 128, 185),
                Location = new Point(250, 0),
                Width = this.ClientSize.Width - 250,
                Height = 80,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            
            // Painel de conteúdo
            contentPanel = new Panel
            {
                BackColor = Color.FromArgb(240, 240, 240),
                Location = new Point(250, 80),
                Width = this.ClientSize.Width - 250,
                Height = this.ClientSize.Height - 80,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
        }
        
        private void CreateSideMenu()
        {
            // Logo/Título da empresa
            Label lblCompany = new Label
            {
                Text = "MINHA EMPRESA",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 30),
                Width = sidePanel.Width,
                Height = 40
            };
            
            // Separador
            Panel separator = new Panel
            {
                BackColor = Color.FromArgb(41, 128, 185),
                Location = new Point(20, 80),
                Width = sidePanel.Width - 40,
                Height = 2
            };
            
            // Criar botões do menu
            Button btnDashboard = CreateMenuButton("Dashboard", "\uD83D\uDCCA", 120);
            btnDashboard.BackColor = Color.FromArgb(41, 128, 185); // Destacar o botão ativo
            btnDashboard.Click += (s, e) => { /* Já estamos no dashboard */ };
            
            Button btnFuncionarios = CreateMenuButton("Funcionários", "\uD83D\uDC64", 180);
            btnFuncionarios.Click += (s, e) => new ListagemFuncionarios().Show();
            
            Button btnCadastrar = CreateMenuButton("Novo Funcionário", "\u2795", 240);
            btnCadastrar.Click += (s, e) => new CadastroFuncionario().Show();
            
            Button btnRelatorios = CreateMenuButton("Relatórios", "\uD83D\uDCC8", 300);
            btnRelatorios.Click += (s, e) => MessageBox.Show("Funcionalidade de relatórios em desenvolvimento.", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            Button btnConfig = CreateMenuButton("Configurações", "\u2699", 360);
            btnConfig.Click += (s, e) => MessageBox.Show("Funcionalidade de configurações em desenvolvimento.", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            // Adicionar controles ao painel lateral
            sidePanel.Controls.AddRange(new Control[] { 
                lblCompany, separator, btnDashboard, 
                btnFuncionarios, btnCadastrar, btnRelatorios, btnConfig 
            });
        }
        
        private Button CreateMenuButton(string text, string icon, int yPos)
        {
            Button btn = new Button
            {
                Text = $"{icon}  {text}",
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12),
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(0, yPos),
                Width = sidePanel.Width,
                Height = 50,
                FlatAppearance = { BorderSize = 0 },
                Padding = new Padding(20, 0, 0, 0),
                Cursor = Cursors.Hand,
                BackColor = Color.FromArgb(52, 73, 94)
            };
            
            btn.MouseEnter += (s, e) => {
                if (btn.BackColor != Color.FromArgb(41, 128, 185)) // Se não for o botão ativo
                    btn.BackColor = Color.FromArgb(44, 62, 80);
            };
            
            btn.MouseLeave += (s, e) => {
                if (btn.BackColor != Color.FromArgb(41, 128, 185)) // Se não for o botão ativo
                    btn.BackColor = Color.FromArgb(52, 73, 94);
            };
            
            return btn;
        }
        
        private void CreateHeader()
        {
            // Título da página
            lblTitle = new Label
            {
                Text = "Dashboard",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            
            // Data e hora atual
            lblDateTime = new Label
            {
                Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12),
                TextAlign = ContentAlignment.MiddleRight,
                Location = new Point(headerPanel.Width - 250, 30),
                Width = 230,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            
            // Adicionar controles ao cabeçalho
            headerPanel.Controls.AddRange(new Control[] { lblTitle, lblDateTime });
        }
        
        private void CreateStatPanels()
        {
            statPanels = new List<Panel>();
            
            // Painel de total de funcionários
            Panel panelTotalFunc = CreateStatPanel("Total de Funcionários", "0", Color.FromArgb(41, 128, 185), 20, 20);
            
            // Painel de total por departamento
            Panel panelTotalDept = CreateStatPanel("Departamentos", "0", Color.FromArgb(46, 204, 113), 20, 150);
            
            // Painel de total por cargo
            Panel panelTotalCargo = CreateStatPanel("Cargos", "0", Color.FromArgb(155, 89, 182), 20, 280);
            
            // Painel de folha de pagamento
            Panel panelFolha = CreateStatPanel("Folha de Pagamento", "R$ 0,00", Color.FromArgb(230, 126, 34), 20, 410);
            
            // Adicionar painéis ao painel de conteúdo
            contentPanel.Controls.AddRange(statPanels.ToArray());
            
            // Criar gráfico de distribuição
            CreateDistributionChart();
        }
        
        private Panel CreateStatPanel(string title, string value, Color color, int x, int y)
        {
            Panel panel = new Panel
            {
                BackColor = Color.White,
                Location = new Point(x, y),
                Width = 250,
                Height = 120,
                BorderStyle = BorderStyle.None
            };
            
            // Barra colorida no topo
            Panel colorBar = new Panel
            {
                BackColor = color,
                Location = new Point(0, 0),
                Width = panel.Width,
                Height = 5
            };
            
            // Título do painel
            Label lblTitle = new Label
            {
                Text = title,
                ForeColor = Color.FromArgb(100, 100, 100),
                Font = new Font("Segoe UI", 12),
                Location = new Point(15, 15),
                AutoSize = true
            };
            
            // Valor do painel
            Label lblValue = new Label
            {
                Text = value,
                ForeColor = Color.FromArgb(50, 50, 50),
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                Location = new Point(15, 50),
                AutoSize = true,
                Tag = title // Armazenar o título para identificação posterior
            };
            
            // Adicionar controles ao painel
            panel.Controls.AddRange(new Control[] { colorBar, lblTitle, lblValue });
            
            // Adicionar à lista de painéis de estatísticas
            statPanels.Add(panel);
            
            return panel;
        }
        
        private void CreateDistributionChart()
        {
            // Painel para o gráfico
            Panel chartPanel = new Panel
            {
                BackColor = Color.White,
                Location = new Point(290, 20),
                Width = contentPanel.Width - 320,
                Height = 510,
                BorderStyle = BorderStyle.None,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            
            // Título do gráfico
            Label lblChartTitle = new Label
            {
                Text = "Distribuição de Funcionários por Departamento",
                ForeColor = Color.FromArgb(50, 50, 50),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(20, 15),
                AutoSize = true
            };
            
            // Adicionar título ao painel do gráfico
            chartPanel.Controls.Add(lblChartTitle);
            
            // O gráfico será desenhado dinamicamente no método LoadDashboardData
            
            // Adicionar painel do gráfico ao painel de conteúdo
            contentPanel.Controls.Add(chartPanel);
        }
        
        private void LoadDashboardData()
        {
            try
            {
                // Buscar dados
                var funcionarios = funcionarioDAO.ListarFuncionarios();
                var departamentos = departamentoDAO.ListarDepartamentos();
                var cargos = cargoDAO.ListarCargos();
                
                // Verificar se o departamento de TI existe
                bool tiExists = departamentos.Any(d => d.Nome == "TI");
                
                // Se não existir, adicionar manualmente
                if (!tiExists)
                {
                    Console.WriteLine("Departamento de TI não encontrado. Adicionando manualmente.");
                    departamentos.Add(new Departamento { Id = -1, Nome = "TI", DataCriacao = DateTime.Now });
                }
                
                // Log para debug
                Console.WriteLine("Departamentos carregados:");
                foreach (var dept in departamentos)
                {
                    Console.WriteLine($"ID: {dept.Id}, Nome: {dept.Nome}");
                }
                
                // Calcular estatísticas
                int totalFuncionarios = funcionarios.Count;
                int totalDepartamentos = departamentos.Count;
                int totalCargos = cargos.Count;
                decimal folhaPagamento = funcionarios.Sum(f => f.Salario);
                
                // Atualizar painéis de estatísticas
                foreach (Panel panel in statPanels)
                {
                    Label? lblValue = panel.Controls.OfType<Label>().FirstOrDefault(l => l.Tag != null);
                    if (lblValue != null && lblValue.Tag != null)
                    {
                        string tag = lblValue.Tag.ToString() ?? string.Empty;
                        switch (tag)
                        {
                            case "Total de Funcionários":
                                lblValue.Text = totalFuncionarios.ToString();
                                break;
                            case "Departamentos":
                                lblValue.Text = totalDepartamentos.ToString();
                                break;
                            case "Cargos":
                                lblValue.Text = totalCargos.ToString();
                                break;
                            case "Folha de Pagamento":
                                lblValue.Text = folhaPagamento.ToString("C2");
                                break;
                        }
                    }
                }
                
                // Desenhar gráfico de distribuição
                DrawDistributionChart(funcionarios, departamentos);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar dados do dashboard: {ex.Message}", "Erro", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void DrawDistributionChart(List<Funcionario> funcionarios, List<Departamento> departamentos)
        {
            // Encontrar o painel do gráfico
            Panel? chartPanel = contentPanel.Controls.OfType<Panel>().FirstOrDefault(p => 
                p.Location.X == 290 && p.Location.Y == 20);
                
            if (chartPanel == null || departamentos.Count == 0) return;
            
            // Agrupar funcionários por departamento
            var departamentoGroups = funcionarios
                .GroupBy(f => f.DepartamentoId)
                .Select(g => new { 
                    DepartamentoId = g.Key, 
                    Count = g.Count() 
                })
                .ToList();
            
            // Definir cores para os departamentos
            Color[] colors = new Color[] {
                Color.FromArgb(41, 128, 185),   // Azul
                Color.FromArgb(46, 204, 113),   // Verde
                Color.FromArgb(155, 89, 182),   // Roxo
                Color.FromArgb(230, 126, 34),   // Laranja
                Color.FromArgb(231, 76, 60),    // Vermelho
                Color.FromArgb(52, 152, 219),   // Azul claro
                Color.FromArgb(241, 196, 15),   // Amarelo
                Color.FromArgb(26, 188, 156)    // Turquesa
            };
            
            // Área para o gráfico
            int chartX = 20;
            int chartY = 60;
            int chartWidth = chartPanel.Width - 40;
            int chartHeight = 300;
            
            // Garantir que maxCount seja pelo menos 1 para evitar divisão por zero e barras invisíveis
            int maxCount = departamentoGroups.Any() ? Math.Max(1, departamentoGroups.Max(g => g.Count)) : 1;
            int barWidth = chartWidth / (departamentos.Count + 1);
            
            // Criar painel para as barras
            Panel barPanel = new Panel
            {
                Location = new Point(chartX, chartY),
                Width = chartWidth,
                Height = chartHeight,
                BackColor = Color.Transparent
            };
            
            // Adicionar linhas de grade horizontais
            for (int i = 0; i <= 5; i++)
            {
                int y = chartHeight - (i * chartHeight / 5);
                Label lblValue = new Label
                {
                    Text = (maxCount * i / 5).ToString(),
                    Location = new Point(-30, y - 10),
                    AutoSize = true,
                    ForeColor = Color.Gray,
                    Font = new Font("Segoe UI", 8)
                };
                
                Panel gridLine = new Panel
                {
                    Location = new Point(0, y),
                    Width = chartWidth,
                    Height = 1,
                    BackColor = Color.FromArgb(230, 230, 230)
                };
                
                barPanel.Controls.Add(lblValue);
                barPanel.Controls.Add(gridLine);
            }
            
            // Desenhar barras e legendas
            Panel legendPanel = new Panel
            {
                Location = new Point(chartX, chartY + chartHeight + 20),
                Width = chartWidth,
                Height = 100,
                BackColor = Color.Transparent
            };
            
            int colorIndex = 0;
            for (int i = 0; i < departamentos.Count; i++)
            {
                var departamento = departamentos[i];
                
                // Garantir que o departamento de TI seja exibido mesmo se não tiver funcionários
                var group = departamentoGroups.FirstOrDefault(g => g.DepartamentoId == departamento.Id);
                int count = group != null ? group.Count : 0;
                
                // Se for o departamento de TI e não tiver funcionários, mostrar mesmo assim
                if (departamento.Nome == "TI" && count == 0)
                {
                    Console.WriteLine("Exibindo departamento de TI no gráfico (sem funcionários).");
                }
                
                // Calcular altura da barra - garantir altura mínima para visualização
                int barHeight = count > 0 ? (int)((float)count / maxCount * chartHeight) : 10;
                
                // Se for o departamento de TI, garantir que seja visível mesmo sem funcionários
                if (departamento.Nome == "TI")
                {
                    Console.WriteLine($"Processando departamento de TI - ID: {departamento.Id}, Funcionários: {count}");
                    // Garantir altura mínima para o departamento de TI
                    barHeight = Math.Max(barHeight, 20);
                }
                
                // Criar barra
                Panel bar = new Panel
                {
                    Location = new Point(i * barWidth + 20, chartHeight - barHeight),
                    Width = barWidth - 20,
                    Height = barHeight,
                    BackColor = colors[colorIndex % colors.Length]
                };
                
                // Adicionar valor no topo da barra
                Label lblBarValue = new Label
                {
                    Text = count.ToString(),
                    Location = new Point(0, -20),
                    Width = barWidth - 20,
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = Color.FromArgb(50, 50, 50),
                    Font = new Font("Segoe UI", 9, FontStyle.Bold)
                };
                bar.Controls.Add(lblBarValue);
                
                // Adicionar barra ao painel
                barPanel.Controls.Add(bar);
                
                // Criar item de legenda
                Panel legendItem = new Panel
                {
                    Location = new Point(i * (chartWidth / 4), (i / 4) * 25),
                    Width = chartWidth / 4,
                    Height = 20,
                    BackColor = Color.Transparent
                };
                
                Panel colorBox = new Panel
                {
                    Location = new Point(0, 5),
                    Width = 10,
                    Height = 10,
                    BackColor = colors[colorIndex % colors.Length]
                };
                
                Label lblDepartamento = new Label
                {
                    Text = departamento.Nome,
                    Location = new Point(15, 0),
                    AutoSize = true,
                    ForeColor = Color.FromArgb(50, 50, 50),
                    Font = new Font("Segoe UI", 8)
                };
                
                legendItem.Controls.Add(colorBox);
                legendItem.Controls.Add(lblDepartamento);
                legendPanel.Controls.Add(legendItem);
                
                colorIndex++;
            }
            
            // Adicionar painéis ao painel do gráfico
            chartPanel.Controls.Add(barPanel);
            chartPanel.Controls.Add(legendPanel);
        }
        
        private void UpdateDateTime()
        {
            lblDateTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }
    }
}
