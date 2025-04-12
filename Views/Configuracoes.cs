using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using MinhaEmpresa.Models;
using MinhaEmpresa.Utils;
using static MinhaEmpresa.Utils.ConfigManager;

namespace MinhaEmpresa.Views
{
    public partial class Configuracoes : Form
    {
        // Propriedades para armazenar as configurações
        public bool TemaEscuro { get; private set; }
        public bool AtualizacaoAutomatica { get; private set; }
        public string OrdenacaoGrafico { get; private set; } = "Alfabética";  // Valor padrão
        public bool MostrarValores { get; private set; }
        
        // Referência ao Dashboard para atualizar as configurações
        private readonly Dashboard dashboard;
        
        public Configuracoes(Dashboard dashboard)
        {
            this.dashboard = dashboard;
            InitializeComponent();
            CarregarConfiguracoes();
        }
        
        private void InitializeComponent()
        {
            ConfigurarFormulario();
            
            // Criar componentes principais
            Panel mainPanel = CriarPainelPrincipal();
            Label lblTitle = CriarTitulo();
            TableLayoutPanel optionsPanel = CriarPainelOpcoes(mainPanel);
            Panel buttonPanel = CriarPainelBotoes();
            
            // Adicionar controles ao formulário
            mainPanel.Controls.Add(optionsPanel);
            this.Controls.Add(buttonPanel);
            this.Controls.Add(lblTitle);
            this.Controls.Add(mainPanel);
        }
        
        /// <summary>
        /// Configura as propriedades básicas do formulário
        /// </summary>
        private void ConfigurarFormulario()
        {
            this.Text = "Configurações";
            this.Width = 500;
            this.Height = 450;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = UITheme.LightBackground;
        }
        
        /// <summary>
        /// Cria o painel principal que conterá os controles
        /// </summary>
        private Panel CriarPainelPrincipal()
        {
            return new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };
            
        }
        
        /// <summary>
        /// Cria o título do formulário
        /// </summary>
        private Label CriarTitulo()
        {
            return new Label
            {
                Text = "Configurações do Sistema",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = UITheme.PrimaryColor,
                Dock = DockStyle.Top,
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = UITheme.LightBackground
            };
            
        }
        
        /// <summary>
        /// Cria o painel de opções com todas as configurações
        /// </summary>
        private TableLayoutPanel CriarPainelOpcoes(Panel mainPanel)
        {
            var optionsPanel = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 5,
                Height = 280,
                Width = mainPanel.Width - 20,
                Location = new Point(10, 60),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Padding = new Padding(10)
            };
            
            optionsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
            optionsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));
            
            for (int i = 0; i < 5; i++)
            {
                optionsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
            }
            
            // 1. Alternar Cores
            Label lblTema = new Label
            {
                Text = "Tema:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(5, 0, 0, 0)
            };
            
            Button btnAlternarTema = new Button
            {
                Text = "Alternar Tema Claro/Escuro",
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Fill,
                BackColor = UITheme.PrimaryColor,
                ForeColor = UITheme.DarkTextColor,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Margin = new Padding(10, 5, 10, 5),
                AutoSize = false,
                AutoEllipsis = true
            };
            btnAlternarTema.FlatAppearance.BorderSize = 0;
            btnAlternarTema.Click += (sender, e) => {
                TemaEscuro = !TemaEscuro;
                AtualizarTema();
            };
            
            // 2. Atualização Automática
            Label lblAtualizacao = new Label
            {
                Text = "Atualização:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(5, 0, 0, 0)
            };
            
            CheckBox chkAtualizacaoAutomatica = new CheckBox
            {
                Text = "Atualização automática dos dados",
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Fill,
                Checked = AtualizacaoAutomatica,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };
            chkAtualizacaoAutomatica.CheckedChanged += (sender, e) => {
                AtualizacaoAutomatica = chkAtualizacaoAutomatica.Checked;
            };
            
            // 3. Exportar
            Label lblExportar = new Label
            {
                Text = "Exportar:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(5, 0, 0, 0)
            };
            
            Button btnExportar = new Button
            {
                Text = "Exportar Dados como CSV",
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Fill,
                BackColor = UITheme.SuccessColor,
                ForeColor = UITheme.DarkTextColor,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Margin = new Padding(10, 5, 10, 5),
                AutoSize = false,
                AutoEllipsis = true
            };
            btnExportar.FlatAppearance.BorderSize = 0;
            btnExportar.Click += (sender, e) => {
                ExportarDados();
            };
            
            // 4. Ordenação
            Label lblOrdenacao = new Label
            {
                Text = "Ordenação:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(5, 0, 0, 0)
            };
            
            ComboBox cmbOrdenacao = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10),
                Margin = new Padding(10, 5, 10, 5)
            };
            cmbOrdenacao.Items.AddRange(new object[] { "Alfabética", "Por quantidade" });
            cmbOrdenacao.SelectedIndex = OrdenacaoGrafico == "Alfabética" ? 0 : 1;
            cmbOrdenacao.SelectedIndexChanged += (sender, e) => {
                OrdenacaoGrafico = cmbOrdenacao.SelectedItem?.ToString() ?? "Alfabética";
            };
            
            // 5. Ocultar Valores
            Label lblValores = new Label
            {
                Text = "Valores:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(5, 0, 0, 0)
            };
            
            // 6. Alto Contraste
            Label lblAltoContraste = new Label
            {
                Text = "Acessibilidade:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(5, 0, 0, 0)
            };
            
            CheckBox chkMostrarValores = new CheckBox
            {
                Text = "Mostrar valores no gráfico",
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Fill,
                Checked = MostrarValores,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };
            chkMostrarValores.CheckedChanged += (sender, e) => {
                MostrarValores = chkMostrarValores.Checked;
            };
            chkMostrarValores.SetToolTip("Exibe os valores numéricos no gráfico de distribuição");
            
            // Checkbox para alto contraste
            CheckBox chkAltoContraste = new CheckBox
            {
                Text = "Modo de alto contraste",
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Fill,
                Checked = UITheme.HighContrastMode,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };
            chkAltoContraste.CheckedChanged += (sender, e) => {
                UITheme.HighContrastMode = chkAltoContraste.Checked;
                AtualizarTema();
            };
            chkAltoContraste.SetToolTip("Ativa o modo de alto contraste para melhor visualização por pessoas com deficiência visual");
            
            // Adicionar controles ao painel de opções
            optionsPanel.RowCount = 6; // Aumentar o número de linhas
            optionsPanel.Height = 330; // Aumentar a altura do painel
            
            for (int i = 5; i < 6; i++)
            {
                optionsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
            }
            
            optionsPanel.Controls.Add(lblTema, 0, 0);
            optionsPanel.Controls.Add(btnAlternarTema, 1, 0);
            optionsPanel.Controls.Add(lblAtualizacao, 0, 1);
            optionsPanel.Controls.Add(chkAtualizacaoAutomatica, 1, 1);
            optionsPanel.Controls.Add(lblExportar, 0, 2);
            optionsPanel.Controls.Add(btnExportar, 1, 2);
            optionsPanel.Controls.Add(lblOrdenacao, 0, 3);
            optionsPanel.Controls.Add(cmbOrdenacao, 1, 3);
            optionsPanel.Controls.Add(lblValores, 0, 4);
            optionsPanel.Controls.Add(chkMostrarValores, 1, 4);
            optionsPanel.Controls.Add(lblAltoContraste, 0, 5);
            optionsPanel.Controls.Add(chkAltoContraste, 1, 5);
            
            // Opções já adicionadas diretamente acima
            
            return optionsPanel;
        }
        
        /// <summary>
        /// Cria o painel de botões na parte inferior do formulário
        /// </summary>
        private Panel CriarPainelBotoes()
        {
            var buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60
            };
            
            var btnSalvar = new Button
            {
                Text = "Salvar",
                Width = 120,
                Height = 40,
                Location = new Point(this.Width / 2 - 130, 15),
                BackColor = UITheme.PrimaryColor,
                ForeColor = UITheme.DarkTextColor,
                Font = new Font("Segoe UI", 9),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSalvar.FlatAppearance.BorderSize = 0;
            btnSalvar.Click += (sender, e) => {
                SalvarConfiguracoes();
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
            btnSalvar.SetToolTip("Salva as configurau00e7u00f5es e fecha a janela");
            
            var btnCancelar = new Button
            {
                Text = "Cancelar",
                Width = 120,
                Height = 40,
                Location = new Point(this.Width / 2 + 10, 15),
                BackColor = UITheme.DangerColor,
                ForeColor = UITheme.DarkTextColor,
                Font = new Font("Segoe UI", 9),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += (sender, e) => {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };
            btnCancelar.SetToolTip("Fecha a janela sem salvar as alterau00e7u00f5es");
            
            buttonPanel.Controls.Add(btnSalvar);
            buttonPanel.Controls.Add(btnCancelar);
            
            return buttonPanel;
        }
        
        /// <summary>
        /// Adiciona a opção de tema ao painel
        /// </summary>
        private void AdicionarOpcaoTema(TableLayoutPanel panel)
        {
            var lblTema = new Label
            {
                Text = "Tema:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(5, 0, 0, 0)
            };
            
            var btnAlternarTema = new Button
            {
                Text = "Alternar Tema Claro/Escuro",
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Fill,
                BackColor = UITheme.PrimaryColor,
                ForeColor = UITheme.DarkTextColor,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Margin = new Padding(10, 5, 10, 5),
                AutoSize = false,
                AutoEllipsis = true
            };
            btnAlternarTema.FlatAppearance.BorderSize = 0;
            btnAlternarTema.Click += (sender, e) => {
                TemaEscuro = !TemaEscuro;
                AtualizarTema();
            };
            
            panel.Controls.Add(lblTema, 0, 0);
            panel.Controls.Add(btnAlternarTema, 1, 0);
        }
        
        /// <summary>
        /// Adiciona a opção de atualização automática ao painel
        /// </summary>
        private void AdicionarOpcaoAtualizacao(TableLayoutPanel panel)
        {
            var lblAtualizacao = new Label
            {
                Text = "Atualização:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(5, 0, 0, 0)
            };
            
            var chkAtualizacaoAutomatica = new CheckBox
            {
                Text = "Atualização automática dos dados",
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Fill,
                Checked = AtualizacaoAutomatica,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };
            chkAtualizacaoAutomatica.CheckedChanged += (sender, e) => {
                AtualizacaoAutomatica = chkAtualizacaoAutomatica.Checked;
            };
            
            panel.Controls.Add(lblAtualizacao, 0, 1);
            panel.Controls.Add(chkAtualizacaoAutomatica, 1, 1);
        }
        
        /// <summary>
        /// Adiciona a opção de exportação ao painel
        /// </summary>
        private void AdicionarOpcaoExportar(TableLayoutPanel panel)
        {
            var lblExportar = new Label
            {
                Text = "Exportar:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(5, 0, 0, 0)
            };
            
            var btnExportar = new Button
            {
                Text = "Exportar Dados como CSV",
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Fill,
                BackColor = UITheme.SuccessColor,
                ForeColor = UITheme.DarkTextColor,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Margin = new Padding(10, 5, 10, 5),
                AutoSize = false,
                AutoEllipsis = true
            };
            btnExportar.FlatAppearance.BorderSize = 0;
            btnExportar.Click += (sender, e) => {
                ExportarDados();
            };
            
            panel.Controls.Add(lblExportar, 0, 2);
            panel.Controls.Add(btnExportar, 1, 2);
        }
        
        /// <summary>
        /// Adiciona a opção de ordenação ao painel
        /// </summary>
        private void AdicionarOpcaoOrdenacao(TableLayoutPanel panel)
        {
            var lblOrdenacao = new Label
            {
                Text = "Ordenação:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(5, 0, 0, 0)
            };
            
            var cmbOrdenacao = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10),
                Margin = new Padding(10, 5, 10, 5)
            };
            cmbOrdenacao.Items.AddRange(new object[] { "Alfabética", "Por quantidade" });
            cmbOrdenacao.SelectedIndex = OrdenacaoGrafico == "Alfabética" ? 0 : 1;
            cmbOrdenacao.SelectedIndexChanged += (sender, e) => {
                OrdenacaoGrafico = cmbOrdenacao.SelectedItem?.ToString() ?? "Alfabética";
            };
            
            panel.Controls.Add(lblOrdenacao, 0, 3);
            panel.Controls.Add(cmbOrdenacao, 1, 3);
        }
        
        /// <summary>
        /// Adiciona a opção de mostrar valores ao painel
        /// </summary>
        private void AdicionarOpcaoValores(TableLayoutPanel panel)
        {
            var lblValores = new Label
            {
                Text = "Valores:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(5, 0, 0, 0)
            };
            
            var chkMostrarValores = new CheckBox
            {
                Text = "Mostrar valores no gráfico",
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Fill,
                Checked = MostrarValores,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };
            chkMostrarValores.CheckedChanged += (sender, e) => {
                MostrarValores = chkMostrarValores.Checked;
            };
            
            panel.Controls.Add(lblValores, 0, 4);
            panel.Controls.Add(chkMostrarValores, 1, 4);
        }
        
        private void CarregarConfiguracoes()
        {
            // Carregar configurações do ConfigManager
            TemaEscuro = Config.TemaEscuro;
            AtualizacaoAutomatica = Config.AtualizacaoAutomatica;
            OrdenacaoGrafico = Config.OrdenacaoGrafico;
            MostrarValores = Config.MostrarValores;
            
            // Aplicar o tema carregado
            AtualizarTema();
        }
        
        private void SalvarConfiguracoes()
        {
            // Atualizar objeto de configurações
            var appConfig = new AppConfig
            {
                TemaEscuro = TemaEscuro,
                AtualizacaoAutomatica = AtualizacaoAutomatica,
                OrdenacaoGrafico = OrdenacaoGrafico,
                MostrarValores = MostrarValores
            };
            
            // Salvar configurações no arquivo
            ConfigManager.SalvarConfiguracoes(appConfig);
            
            // Aplicar configurações ao Dashboard
            dashboard.AplicarConfiguracoes(this);
            
            // Mostrar feedback ao usuário
            MessageBox.Show("Configurações salvas com sucesso!", "Configurações", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        
        private void AtualizarTema()
        {
            if (TemaEscuro)
            {
                this.BackColor = UITheme.SecondaryColor;
                foreach (Control control in this.Controls)
                {
                    if (control is Label label)
                    {
                        label.ForeColor = UITheme.DarkTextColor;
                    }
                }
            }
            else
            {
                this.BackColor = UITheme.LightBackground;
                foreach (Control control in this.Controls)
                {
                    if (control is Label label && label.Text != "Configurações do Sistema")
                    {
                        label.ForeColor = UITheme.LightTextColor;
                    }
                }
            }
        }
        
        /// <summary>
        /// Exporta os dados dos funcionários para um arquivo CSV
        /// </summary>
        private void ExportarDados()
        {
            try
            {
                // Obter dados do Dashboard antes de abrir o diálogo
                List<Funcionario> funcionarios = dashboard.ObterDadosFuncionarios();
                
                if (funcionarios == null || funcionarios.Count == 0)
                {
                    MessageBox.Show("Não há dados para exportar.", "Aviso", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Arquivos CSV (*.csv)|*.csv",
                    Title = "Exportar dados como CSV",
                    FileName = $"DadosFuncionarios_{DateTime.Now:yyyyMMdd_HHmmss}.csv",
                    RestoreDirectory = true
                };
                
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Usar um cursor de espera durante a exportação
                    Cursor = Cursors.WaitCursor;
                    
                    try
                    {
                        // Criar conteúdo CSV usando CsvHelper ou método manual otimizado
                        using (var writer = new StreamWriter(saveFileDialog.FileName, false, Encoding.UTF8))
                        {
                            // Escrever cabeçalho
                            writer.WriteLine("ID,Nome,Email,Telefone,Cargo,Departamento,Salário,Data Contratação");
                            
                            // Escrever linhas de dados
                            foreach (var funcionario in funcionarios)
                            {
                                // Escapar campos com vírgulas e aspas
                                string nome = EscaparCampoCSV(funcionario.Nome);
                                string email = EscaparCampoCSV(funcionario.Email);
                                string telefone = EscaparCampoCSV(funcionario.Telefone);
                                string cargo = EscaparCampoCSV(funcionario.Cargo?.Nome ?? "");
                                string departamento = EscaparCampoCSV(funcionario.Departamento?.Nome ?? "");
                                string dataContratacao = funcionario.DataContratacao.ToString("dd/MM/yyyy");
                                
                                // Escrever linha
                                writer.WriteLine($"{funcionario.Id},{nome},{email},{telefone},{cargo},{departamento},{funcionario.Salario},{dataContratacao}");
                            }
                        }
                        
                        MessageBox.Show("Dados exportados com sucesso!", "Exportação Concluída", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    finally
                    {
                        // Restaurar cursor normal
                        Cursor = Cursors.Default;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao exportar dados: {ex.Message}", "Erro", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        /// <summary>
        /// Escapa um campo para formato CSV, adicionando aspas se necessário
        /// </summary>
        private string EscaparCampoCSV(string campo)
        {
            if (string.IsNullOrEmpty(campo))
                return "";
                
            // Se o campo contiver vírgula, aspas ou quebra de linha, coloque entre aspas
            bool precisaEscapar = campo.Contains(",") || campo.Contains("\"") || campo.Contains("\n") || campo.Contains("\r");
            
            if (precisaEscapar)
            {
                // Substituir aspas por aspas duplas e colocar entre aspas
                return $"\"{campo.Replace("\"", "\"\"")}\"";
            }
            
            return campo;
        }
    }
}
