using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MyEmployeeProject.DAO;
using MyEmployeeProject.Models;
using MyEmployeeProject.Utils;
using static MyEmployeeProject.Utils.ConfigManager;
using static MyEmployeeProject.Utils.UITheme;
using static MyEmployeeProject.Utils.KeyboardShortcuts;
namespace MyEmployeeProject.Views
{
    public partial class Dashboard : Form
    {
        private readonly FuncionarioDAO funcionarioDAO = new FuncionarioDAO();
        private readonly CargoDAO cargoDAO = new CargoDAO();
        private readonly DepartamentoDAO departamentoDAO = new DepartamentoDAO();
        
        // Configurações do Dashboard
        private bool temaEscuro = false;
        private bool atualizacaoAutomatica = true;
        private string ordenacaoGrafico = "Alfabética";
        private bool mostrarValores = true;
        
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
            
            // Inicializar o timer antes de carregar as configuraçoes
            timer = new System.Windows.Forms.Timer { Interval = 1000 };
            timer.Tick += (s, e) => UpdateDateTime();
            
            InitializeCustomComponents();
            CarregarConfiguracoes();
            ConfigurarTeclasAtalho();
            LoadDashboardData();
            
            // Iniciar o timer
            timer.Start();
            
            // Configurar suporte a teclas de atalho
            SetupFormShortcuts(this);
        }
        
        /// <summary>
        /// Carrega as configuraçoes salvas
        /// </summary>
        public void CarregarConfiguracoes()
        {
            try
            {
                // Carregar configuraçoes do arquivo
                temaEscuro = Config.TemaEscuro;
                atualizacaoAutomatica = Config.AtualizacaoAutomatica;
                ordenacaoGrafico = Config.OrdenacaoGrafico;
                mostrarValores = Config.MostrarValores;
                
                // Carregar configuraçoes de acessibilidade
                UITheme.LoadAccessibilitySettings();
                
                // Aplicar tema
                AplicarTema();
                
                // Configurar timer de atualizacao automatica (verificar se o timer foi inicializado)
                if (timer != null)
                {
                    timer.Enabled = atualizacaoAutomatica;
                }
            }
            catch (Exception ex)
            {
                // Em caso de erro, usar configuraçoes padroes
                Console.WriteLine($"Erro ao carregar configuraçoes: {ex.Message}");
                MessageBox.Show($"Ocorreu um erro ao carregar as configuraçoes. Serao usadas as configuraçoes padroes.\n\nDetalhes: {ex.Message}", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                
                // Definir valores padroes
                temaEscuro = false;
                atualizacaoAutomatica = true;
                ordenacaoGrafico = "Alfabu00e9tica";
                mostrarValores = true;
            }
        }
        
        private void InitializeComponent()
        {
            this.Text = "Sistema de Gestão de Funcionários - Dashboard";
            this.Width = 1200;
            this.Height = 800;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = UITheme.LightBackground;
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
                BackColor = UITheme.SecondaryColor,
                Location = new Point(0, 0),
                Width = 250,
                Height = this.ClientSize.Height,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left
            };
            
            // Painel de cabeçalho
            headerPanel = new Panel
            {
                BackColor = UITheme.PrimaryColor,
                Location = new Point(250, 0),
                Width = this.ClientSize.Width - 250,
                Height = 80,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            
            // Painel de conteúdo
            contentPanel = new Panel
            {
                BackColor = UITheme.LightBackground,
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
                Text = "DevMendes21 Enterprise",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 30),
                Width = sidePanel.Width,
                Height = 40
            };
            
            // Botão de configurações será adicionado depois do botão de relatórios
            
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
            btnDashboard.SetToolTip("Visualizar painel principal com estatísticas e gráficos");
            
            Button btnFuncionarios = CreateMenuButton("Funcionários", "\uD83D\uDC64", 180);
            btnFuncionarios.Click += (s, e) => new ListagemFuncionarios().Show();
            btnFuncionarios.SetToolTip("Visualizar e gerenciar lista de funcionários (Ctrl+F)");
            
            Button btnCadastrar = CreateMenuButton("Novo Funcionário", "\u2795", 240);
            btnCadastrar.Click += (s, e) => new CadastroFuncionario().Show();
            btnCadastrar.SetToolTip("Cadastrar novo funcionário no sistema (Ctrl+N)");
            
            Button btnRelatorios = CreateMenuButton("Relatórios", "\uD83D\uDCC8", 300);
            btnRelatorios.Click += (s, e) => MessageBox.Show("Funcionalidade de relatórios em desenvolvimento.", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
            btnRelatorios.SetToolTip("Gerar relatórios e exportar dados");
            
            // Botão de configurações
            Button btnConfiguracoes = CreateMenuButton("Configurações", "⚙️", 360);
            btnConfiguracoes.Click += (sender, e) => {
                AbrirConfiguracoes();
            };
            btnConfiguracoes.SetToolTip("Abrir tela de configurações do sistema (Ctrl+C)");
            
            // Adicionar controles ao painel lateral
            sidePanel.Controls.AddRange(new Control[] { 
                lblCompany, separator, btnDashboard, 
                btnFuncionarios, btnCadastrar, btnRelatorios, btnConfiguracoes
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
        
        public void LoadDashboardData()
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
            
            chartPanel.Controls.Clear();
            
            // Calcular o total de funcionários para porcentagens
            int totalFuncionarios = funcionarios.Count;
            
            // Aplicar ordenação conforme configurações
            if (ordenacaoGrafico == "Alfabética")
            {
                departamentos = departamentos.OrderBy(d => d.Nome).ToList();
            }
            else // Por quantidade
            {
                var departamentoCountDict = funcionarios.GroupBy(f => f.DepartamentoId)
                    .ToDictionary(g => g.Key, g => g.Count());
                    
                departamentos = departamentos.OrderByDescending(d => 
                    departamentoCountDict.ContainsKey(d.Id) ? departamentoCountDict[d.Id] : 0).ToList();
            }
            
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
            
            // Painel para as legendas com borda e estilo
            Panel legendPanel = new Panel
            {
                Location = new Point(chartX, chartY + chartHeight + 25),
                Width = chartWidth,
                Height = 120,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10)
            };
            
            // Adicionar título para a legenda com estilo melhorado
            Label lblLegendTitle = new Label
            {
                Text = "Departamentos",
                Location = new Point(10, 5),
                AutoSize = true,
                ForeColor = Color.FromArgb(41, 128, 185),
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            legendPanel.Controls.Add(lblLegendTitle);
            
            // Adicionar linha separadora abaixo do título
            Panel separator = new Panel
            {
                Location = new Point(10, 30),
                Width = chartWidth - 20,
                Height = 1,
                BackColor = Color.FromArgb(230, 230, 230)
            };
            legendPanel.Controls.Add(separator);
            
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
                
                // Criar barra com estilo melhorado e interatividade
                Panel bar = new Panel
                {
                    Location = new Point(i * barWidth + 20, chartHeight - barHeight),
                    Width = barWidth - 30, // Mais espaço entre as barras
                    Height = barHeight,
                    BackColor = colors[colorIndex % colors.Length],
                    BorderStyle = BorderStyle.FixedSingle, // Adicionar borda para melhor definição
                    Tag = departamento // Armazenar o departamento para uso nos eventos
                };
                
                // Calcular porcentagem
                double porcentagem = totalFuncionarios > 0 ? (double)count / totalFuncionarios * 100 : 0;
                
                // Adicionar tooltip com informações detalhadas
                ToolTip tooltip = new ToolTip();
                tooltip.SetToolTip(bar, $"Departamento: {departamento.Nome} | Funcionários: {count} | Porcentagem: {porcentagem:0.0}%");
                
                // Capturar o índice de cor atual para uso no evento
                int currentColorIndex = colorIndex;
                
                // Adicionar eventos de mouse para interatividade
                bar.MouseEnter += (sender, e) => {
                    // Destacar a barra quando o mouse passar por cima
                    if (sender is Panel currentBar)
                    {
                        currentBar.BackColor = ControlPaint.Light(colors[currentColorIndex % colors.Length]);
                        currentBar.Cursor = Cursors.Hand;
                    }
                };
                
                bar.MouseLeave += (sender, e) => {
                    // Restaurar a cor original quando o mouse sair
                    if (sender is Panel currentBar)
                    {
                        currentBar.BackColor = colors[currentColorIndex % colors.Length];
                    }
                };
                
                bar.Click += (sender, e) => {
                    // Mostrar detalhes do departamento quando clicar na barra
                    if (sender is Panel clickedBar && clickedBar.Tag is Departamento dept)
                    {
                        var deptFuncionarios = funcionarios.Where(f => f.DepartamentoId == dept.Id).ToList();
                        double percentagemDept = totalFuncionarios > 0 ? (double)deptFuncionarios.Count / totalFuncionarios * 100 : 0;
                        decimal salarioMedio = deptFuncionarios.Any() ? deptFuncionarios.Average(f => f.Salario) : 0;
                    
                    // Criar mensagem com quebras de linha usando Environment.NewLine
                    string mensagem = "Departamento: " + dept.Nome;
                    mensagem += Environment.NewLine + "Total de funcionários: " + deptFuncionarios.Count;
                    mensagem += Environment.NewLine + "Porcentagem do total: " + percentagemDept.ToString("0.0") + "%";
                    mensagem += Environment.NewLine + "Salário médio: R$ " + salarioMedio.ToString("N2");
                    
                        MessageBox.Show(
                            mensagem,
                            $"Detalhes do Departamento {dept.Nome}",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                };
                
                // Adicionar valor no topo da barra com estilo melhorado e porcentagem
                if (mostrarValores)
                {
                    Label lblBarValue = new Label
                    {
                        Text = $"{count} ({porcentagem:0.0}%)",
                        Location = new Point(0, -25),
                        Width = barWidth - 30,
                        TextAlign = ContentAlignment.MiddleCenter,
                        ForeColor = Color.FromArgb(41, 128, 185),
                        Font = new Font("Segoe UI", 10, FontStyle.Bold),
                        BackColor = Color.Transparent
                    };
                    bar.Controls.Add(lblBarValue);
                }
                
                // Adicionar barra ao painel
                barPanel.Controls.Add(bar);
                
                // Criar item de legenda com espaçamento melhorado
                Panel legendItem = new Panel
                {
                    Location = new Point(10 + (i % 3) * (chartWidth / 3 - 10), 40 + (i / 3) * 30),
                    Width = (chartWidth / 3) - 20,
                    Height = 25,
                    BackColor = Color.Transparent
                };
                
                // Garantir que a legenda do TI seja visível
                if (departamento.Nome == "TI")
                {
                    Console.WriteLine("Criando legenda para o departamento de TI");
                }
                
                // Caixa de cor com estilo melhorado
                Panel colorBox = new Panel
                {
                    Location = new Point(0, 5),
                    Width = 15,
                    Height = 15,
                    BackColor = colors[colorIndex % colors.Length],
                    BorderStyle = BorderStyle.FixedSingle
                };
                
                // Texto da legenda com estilo melhorado
                Label lblDepartamento = new Label
                {
                    Text = departamento.Nome,
                    Location = new Point(20, 3),
                    AutoSize = true,
                    ForeColor = Color.FromArgb(50, 50, 50),
                    Font = new Font("Segoe UI", 9, FontStyle.Regular)
                };
                
                // Nenhum destaque especial para o departamento de TI
                // Todos os departamentos usam o mesmo estilo
                
                legendItem.Controls.Add(colorBox);
                legendItem.Controls.Add(lblDepartamento);
                legendPanel.Controls.Add(legendItem);
                
                colorIndex++;
            }
            
            // Adicionar título do gráfico
            Label lblChartTitle = new Label
            {
                Text = "Distribuição de Funcionários por Departamento",
                Location = new Point(0, 10),
                Width = chartPanel.Width,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(50, 50, 50),
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            
            // Adicionar subtítulo com instruções de interatividade
            Label lblSubtitle = new Label
            {
                Text = "Passe o mouse sobre as barras para mais detalhes ou clique para ver estatísticas completas",
                Location = new Point(0, 35),
                Width = chartPanel.Width,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(100, 100, 100),
                Font = new Font("Segoe UI", 9, FontStyle.Italic)
            };
            
            // Adicionar controles ao painel do gráfico
            chartPanel.Controls.Add(lblChartTitle);
            chartPanel.Controls.Add(lblSubtitle);
            chartPanel.Controls.Add(barPanel);
            chartPanel.Controls.Add(legendPanel);
            
            // Adicionar botão para atualizar o gráfico
            Button btnRefresh = new Button
            {
                Text = "Atualizar Gráfico",
                Location = new Point(chartPanel.Width - 150, chartPanel.Height - 40),
                Width = 130,
                Height = 30,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(41, 128, 185),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                Cursor = Cursors.Hand
            };
            
            btnRefresh.Click += (sender, e) => {
                // Recarregar dados e redesenhar o gráfico
                LoadDashboardData();
            };
            
            chartPanel.Controls.Add(btnRefresh);
        }
        
        private void UpdateDateTime()
        {
            lblDateTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }
        
        // Métodos para configurações
        private void AbrirConfiguracoes()
        {
            Configuracoes configForm = new Configuracoes(this);
            if (configForm.ShowDialog() == DialogResult.OK)
            {
                // As configurações já foram aplicadas pelo formulário de configurações
                LoadDashboardData(); // Recarregar dados para aplicar as mudanças
            }
        }
        
        public void AplicarConfiguracoes(Configuracoes config)
        {
            // Aplicar as configurações ao Dashboard
            temaEscuro = config.TemaEscuro;
            atualizacaoAutomatica = config.AtualizacaoAutomatica;
            ordenacaoGrafico = config.OrdenacaoGrafico;
            mostrarValores = config.MostrarValores;
            
            // Aplicar tema
            AplicarTema();
            
            // Configurar timer de atualização automática
            timer.Enabled = atualizacaoAutomatica;
            
            // Redesenhar o gráfico com as novas configurações
            LoadDashboardData();
        }
        
        private void AplicarTema()
        {
            if (temaEscuro)
            {
                // Aplicar tema escuro
                this.BackColor = UITheme.DarkBackground;
                contentPanel.BackColor = UITheme.DarkPanelBackground;
                headerPanel.BackColor = UITheme.DarkHeaderBackground;
                
                // Atualizar cores dos painéis de estatísticas
                foreach (var panel in statPanels)
                {
                    panel.BackColor = UITheme.HighContrastMode ? Color.Black : Color.FromArgb(60, 60, 60);
                    foreach (Control control in panel.Controls)
                    {
                        if (control is Label label)
                        {
                            label.ForeColor = UITheme.DarkTextColor;
                        }
                    }
                }
            }
            else
            {
                // Restaurar tema claro (padrão)
                this.BackColor = UITheme.LightBackground;
                contentPanel.BackColor = UITheme.LightBackground;
                headerPanel.BackColor = UITheme.PrimaryColor;
                
                // Restaurar cores dos painéis de estatísticas
                foreach (var panel in statPanels)
                {
                    panel.BackColor = UITheme.HighContrastMode ? Color.White : Color.White;
                    foreach (Control control in panel.Controls)
                    {
                        if (control is Label label && label.Tag?.ToString() != "value")
                        {
                            label.ForeColor = UITheme.SubtitleTextColor;
                        }
                        else if (control is Label valueLabel && valueLabel.Tag?.ToString() == "value")
                        {
                            valueLabel.ForeColor = UITheme.LightTextColor;
                        }
                    }
                }
            }
            
            // Atualizar borda dos botões para melhor contraste se necessário
            foreach (Control control in sidePanel.Controls)
            {
                if (control is Button button)
                {
                    button.FlatAppearance.BorderSize = UITheme.HighContrastMode ? 1 : 0;
                }
            }
        }
        
        public List<Funcionario> ObterDadosFuncionarios()
        {
            // Método para obter a lista de funcionários para exportação
            return funcionarioDAO.ListarFuncionarios();
        }
        
        /// <summary>
        /// Configura as teclas de atalho para o Dashboard
        /// </summary>
        private void ConfigurarTeclasAtalho()
        {
            // Tecla F1 - Ajuda
            RegisterShortcut(Keys.F1, () => {
                string ajudaTexto = @"Teclas de atalho disponíveis:

F1 - Exibir esta ajuda
F5 - Atualizar dados
Ctrl+C - Abrir configurações
Ctrl+F - Abrir lista de funcionários
Ctrl+N - Novo funcionário
Alt+H - Alternar modo de alto contraste";
                MessageBox.Show(ajudaTexto, "Ajuda - Teclas de Atalho", MessageBoxButtons.OK, MessageBoxIcon.Information);
            });
            
            // Tecla F5 - Atualizar dados
            RegisterShortcut(Keys.F5, () => {
                LoadDashboardData();
                MostrarMensagemStatus("Dados atualizados com sucesso!");
            });
            
            // Ctrl+C - Abrir configurações
            RegisterShortcut(Keys.Control | Keys.C, () => {
                AbrirConfiguracoes();
            });
            
            // Ctrl+F - Abrir lista de funcionários
            RegisterShortcut(Keys.Control | Keys.F, () => {
                new ListagemFuncionarios().Show();
            });
            
            // Ctrl+N - Novo funcionário
            RegisterShortcut(Keys.Control | Keys.N, () => {
                new CadastroFuncionario().Show();
            });
            
            // Alt+H - Alternar modo de alto contraste
            RegisterShortcut(Keys.Alt | Keys.H, () => {
                UITheme.HighContrastMode = !UITheme.HighContrastMode;
                AplicarTema();
                MostrarMensagemStatus(UITheme.HighContrastMode ? "Modo de alto contraste ativado" : "Modo de alto contraste desativado");
            });
        }
        
        /// <summary>
        /// Exibe uma mensagem de status temporária para o usuário
        /// </summary>
        private void MostrarMensagemStatus(string mensagem)
        {
            // Criar um label temporário para exibir a mensagem
            Label lblStatus = new Label
            {
                Text = mensagem,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.None,
                Width = 300,
                Height = 40,
                Location = new Point((this.Width - 300) / 2, this.Height - 100),
                BackColor = UITheme.SuccessColor,
                ForeColor = UITheme.DarkTextColor,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10)
            };
            
            this.Controls.Add(lblStatus);
            lblStatus.BringToFront();
            
            // Timer para remover a mensagem após alguns segundos
            System.Windows.Forms.Timer statusTimer = new System.Windows.Forms.Timer { Interval = 3000 };
            statusTimer.Tick += (s, e) => {
                this.Controls.Remove(lblStatus);
                lblStatus.Dispose();
                statusTimer.Stop();
                statusTimer.Dispose();
            };
            statusTimer.Start();
        }
    }
}
