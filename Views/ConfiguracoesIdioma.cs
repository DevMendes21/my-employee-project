using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using MinhaEmpresa.Utils;
using static MinhaEmpresa.Utils.ConfigManager;

namespace MinhaEmpresa.Views
{
    /// <summary>
    /// Tela de configurau00e7u00f5es do sistema com suporte a idiomas
    /// </summary>
    public partial class ConfiguracoesIdioma : Form
    {
        // Propriedades para armazenar as configurau00e7u00f5es
        private bool _temaEscuro;
        private bool _atualizacaoAutomatica;
        private string _ordenacaoGrafico = "Alfabu00e9tica";  // Valor padru00e3o
        private bool _mostrarValores;
        private bool _altoContraste;
        private string _idioma = "pt-BR";  // Valor padru00e3o
        
        // Controles da interface
        private ComboBox? cmbIdioma;
        private Button? btnSalvar;
        private Button? btnCancelar;
        
        public ConfiguracoesIdioma()
        {
            InitializeComponent();
            CarregarConfiguracoes();
        }
        
        private void InitializeComponent()
        {
            this.Text = "Configurau00e7u00f5es do Sistema";
            this.Size = new Size(550, 500); // Aumentado para acomodar mais opu00e7u00f5es
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = UITheme.LightBackground;
            
            // Criar painel principal
            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };
            
            // Tu00edtulo
            Label lblTitle = new Label
            {
                Text = "Configurau00e7u00f5es do Sistema",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = UITheme.PrimaryColor,
                Dock = DockStyle.Top,
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter
            };
            
            // Painel de opu00e7u00f5es
            TableLayoutPanel optionsPanel = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 7, // Incluindo a opu00e7u00e3o de idioma
                Dock = DockStyle.Fill,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                Padding = new Padding(10)
            };
            
            optionsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
            optionsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));
            
            for (int i = 0; i < 7; i++)
            {
                optionsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            }
            
            // 1. Tema
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
                Margin = new Padding(10, 5, 10, 5)
            };
            btnAlternarTema.FlatAppearance.BorderSize = 0;
            btnAlternarTema.Click += (sender, e) =>
            {
                _temaEscuro = !_temaEscuro;
                AtualizarTema();
            };
            btnAlternarTema.SetToolTip("Alterna entre o tema claro e escuro da interface");
            
            // 2. Atualizau00e7u00e3o Automu00e1tica
            Label lblAtualizacao = new Label
            {
                Text = "Atualizau00e7u00e3o:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(5, 0, 0, 0)
            };
            
            CheckBox chkAtualizacaoAutomatica = new CheckBox
            {
                Text = "Atualizau00e7u00e3o automu00e1tica dos dados",
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Fill,
                Checked = _atualizacaoAutomatica,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };
            chkAtualizacaoAutomatica.CheckedChanged += (sender, e) =>
            {
                _atualizacaoAutomatica = chkAtualizacaoAutomatica.Checked;
            };
            chkAtualizacaoAutomatica.SetToolTip("Atualiza automaticamente os dados do dashboard");
            
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
                Text = "Exportar Dados",
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Fill,
                BackColor = UITheme.SuccessColor,
                ForeColor = UITheme.DarkTextColor,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Margin = new Padding(10, 5, 10, 5)
            };
            btnExportar.FlatAppearance.BorderSize = 0;
            btnExportar.Click += (sender, e) =>
            {
                ExportManager.ExportarComDialogo(new List<Models.Funcionario>());
            };
            btnExportar.SetToolTip("Exporta os dados para CSV, JSON ou Excel");
            
            // 4. Ordenau00e7u00e3o
            Label lblOrdenacao = new Label
            {
                Text = "Ordenau00e7u00e3o:",
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
            cmbOrdenacao.Items.AddRange(new object[] { "Alfabu00e9tica", "Por quantidade" });
            cmbOrdenacao.SelectedIndex = _ordenacaoGrafico == "Alfabu00e9tica" ? 0 : 1;
            cmbOrdenacao.SelectedIndexChanged += (sender, e) =>
            {
                _ordenacaoGrafico = cmbOrdenacao.SelectedItem?.ToString() ?? "Alfabu00e9tica";
            };
            cmbOrdenacao.SetToolTip("Define como os dados seru00e3o ordenados no gru00e1fico");
            
            // 5. Valores
            Label lblValores = new Label
            {
                Text = "Valores:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(5, 0, 0, 0)
            };
            
            CheckBox chkMostrarValores = new CheckBox
            {
                Text = "Mostrar valores no gru00e1fico",
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Fill,
                Checked = _mostrarValores,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };
            chkMostrarValores.CheckedChanged += (sender, e) =>
            {
                _mostrarValores = chkMostrarValores.Checked;
            };
            chkMostrarValores.SetToolTip("Exibe os valores numu00e9ricos no gru00e1fico de distribuiu00e7u00e3o");
            
            // 6. Alto Contraste
            Label lblAltoContraste = new Label
            {
                Text = "Acessibilidade:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(5, 0, 0, 0)
            };
            
            CheckBox chkAltoContraste = new CheckBox
            {
                Text = "Ativar modo de alto contraste",
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Fill,
                Checked = _altoContraste,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };
            chkAltoContraste.CheckedChanged += (sender, e) =>
            {
                _altoContraste = chkAltoContraste.Checked;
                UITheme.HighContrastMode = _altoContraste;
                AtualizarTema();
            };
            chkAltoContraste.SetToolTip("Ativa o modo de alto contraste para melhor visualizau00e7u00e3o por pessoas com deficiu00eancia visual");
            
            // 7. Idioma
            Label lblIdioma = new Label
            {
                Text = "Idioma:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(5, 0, 0, 0)
            };
            
            cmbIdioma = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10),
                Margin = new Padding(10, 5, 10, 5)
            };
            
            // Adicionar idiomas disponu00edveis
            cmbIdioma.Items.Add("Portuguu00eas (Brasil)");
            cmbIdioma.Items.Add("English (US)");
            
            // Selecionar o idioma atual
            cmbIdioma.SelectedIndex = _idioma == "pt-BR" ? 0 : 1;
            
            cmbIdioma.SelectedIndexChanged += (sender, e) =>
            {
                _idioma = cmbIdioma.SelectedIndex == 0 ? "pt-BR" : "en-US";
            };
            
            cmbIdioma.SetToolTip("Selecione o idioma da interface do sistema");
            
            // Adicionar controles ao painel de opu00e7u00f5es
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
            optionsPanel.Controls.Add(lblIdioma, 0, 6);
            optionsPanel.Controls.Add(cmbIdioma, 1, 6);
            
            // Painel de botu00f5es
            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60
            };
            
            btnSalvar = new Button
            {
                Text = "Salvar",
                Width = 100,
                Height = 40,
                Location = new Point(buttonPanel.Width / 2 - 110, 10),
                BackColor = UITheme.SuccessColor,
                ForeColor = UITheme.DarkTextColor,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSalvar.FlatAppearance.BorderSize = 0;
            btnSalvar.Click += (sender, e) => SalvarConfiguracoes();
            
            btnCancelar = new Button
            {
                Text = "Cancelar",
                Width = 100,
                Height = 40,
                Location = new Point(buttonPanel.Width / 2 + 10, 10),
                BackColor = UITheme.DangerColor,
                ForeColor = UITheme.DarkTextColor,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += (sender, e) => this.Close();
            
            // Adicionar botu00f5es ao painel
            buttonPanel.Controls.Add(btnSalvar);
            buttonPanel.Controls.Add(btnCancelar);
            
            // Adicionar controles ao formulu00e1rio
            mainPanel.Controls.Add(optionsPanel);
            this.Controls.Add(buttonPanel);
            this.Controls.Add(lblTitle);
            this.Controls.Add(mainPanel);
            
            // Ajustar posiu00e7u00e3o dos botu00f5es quando o formulu00e1rio for redimensionado
            this.Resize += (sender, e) =>
            {
                btnSalvar.Location = new Point(buttonPanel.Width / 2 - 110, 10);
                btnCancelar.Location = new Point(buttonPanel.Width / 2 + 10, 10);
            };
        }
        
        private void CarregarConfiguracoes()
        {
            try
            {
                // Carregar configurau00e7u00f5es do ConfigManager
                _temaEscuro = Config.TemaEscuro;
                _atualizacaoAutomatica = Config.AtualizacaoAutomatica;
                _ordenacaoGrafico = Config.OrdenacaoGrafico;
                _mostrarValores = Config.MostrarValores;
                _altoContraste = UITheme.HighContrastMode;
                _idioma = Config.Idioma;
                
                // Aplicar o tema carregado
                AtualizarTema();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar configurau00e7u00f5es: {ex.Message}", "Erro", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void SalvarConfiguracoes()
        {
            try
            {
                // Atualizar objeto de configurau00e7u00f5es
                var appConfig = new AppConfig
                {
                    TemaEscuro = _temaEscuro,
                    AtualizacaoAutomatica = _atualizacaoAutomatica,
                    OrdenacaoGrafico = _ordenacaoGrafico,
                    MostrarValores = _mostrarValores,
                    Idioma = _idioma
                };
                
                // Salvar configurau00e7u00f5es no arquivo
                ConfigManager.SalvarConfiguracoes(appConfig);
                
                // Atualizar idioma do sistema
                LocalizationManager.CurrentLanguage = _idioma;
                
                // Mostrar feedback ao usuu00e1rio
                MessageBox.Show("Configurau00e7u00f5es salvas com sucesso!", "Configurau00e7u00f5es", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // Fechar o formulu00e1rio
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar configurau00e7u00f5es: {ex.Message}", "Erro", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void AtualizarTema()
        {
            // Atualizar cores do formulu00e1rio com base no tema selecionado
            this.BackColor = _temaEscuro ? UITheme.DarkBackground : UITheme.LightBackground;
            
            // Atualizar cores dos controles
            foreach (Control control in this.Controls)
            {
                if (control is Label label)
                {
                    label.ForeColor = _temaEscuro ? UITheme.DarkTextColor : UITheme.LightTextColor;
                }
                else if (control is Panel panel)
                {
                    panel.BackColor = _temaEscuro ? UITheme.DarkBackground : UITheme.LightBackground;
                    
                    foreach (Control panelControl in panel.Controls)
                    {
                        if (panelControl is TableLayoutPanel tablePanel)
                        {
                            tablePanel.BackColor = _temaEscuro ? UITheme.DarkPanelBackground : UITheme.LightBackground;
                            
                            foreach (Control tablePanelControl in tablePanel.Controls)
                            {
                                if (tablePanelControl is Label lbl)
                                {
                                    lbl.ForeColor = _temaEscuro ? UITheme.DarkTextColor : UITheme.LightTextColor;
                                    lbl.BackColor = _temaEscuro ? UITheme.DarkPanelBackground : UITheme.LightBackground;
                                }
                                else if (tablePanelControl is CheckBox chk)
                                {
                                    chk.ForeColor = _temaEscuro ? UITheme.DarkTextColor : UITheme.LightTextColor;
                                    chk.BackColor = _temaEscuro ? UITheme.DarkPanelBackground : UITheme.LightBackground;
                                }
                                else if (tablePanelControl is ComboBox cmb)
                                {
                                    cmb.BackColor = _temaEscuro ? UITheme.DarkBackground : Color.White;
                                    cmb.ForeColor = _temaEscuro ? UITheme.DarkTextColor : UITheme.LightTextColor;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
