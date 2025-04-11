using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using MinhaEmpresa.Models;
using MinhaEmpresa.Views;

namespace MinhaEmpresa.Views
{
    public partial class Configuracoes : Form
    {
        // Propriedades para armazenar as configurações
        public bool TemaEscuro { get; private set; } = false;
        public bool AtualizacaoAutomatica { get; private set; } = true;
        public string OrdenacaoGrafico { get; private set; } = "Alfabética";
        public bool MostrarValores { get; private set; } = true;
        
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
            this.Text = "Configurações";
            this.Width = 450;
            this.Height = 380;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(240, 240, 240);
            
            // Painel principal
            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };
            
            // Título
            Label lblTitle = new Label
            {
                Text = "Configurações do Sistema",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 128, 185),
                Location = new Point(0, 15),
                Width = mainPanel.Width,
                Height = 35,
                TextAlign = ContentAlignment.MiddleCenter
            };
            
            // Container para as opções
            TableLayoutPanel optionsPanel = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 5,
                Height = 220,
                Width = mainPanel.Width,
                Location = new Point(0, 80),
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
                BackColor = Color.FromArgb(41, 128, 185),
                ForeColor = Color.White,
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
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
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
            
            // Adicionar controles ao painel de opções
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
            
            // Botões de Salvar e Cancelar
            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50
            };
            
            Button btnSalvar = new Button
            {
                Text = "Salvar",
                Width = 120,
                Height = 40,
                Location = new Point(this.Width / 2 - 130, 10),
                BackColor = Color.FromArgb(41, 128, 185),
                ForeColor = Color.White,
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
            
            Button btnCancelar = new Button
            {
                Text = "Cancelar",
                Width = 120,
                Height = 40,
                Location = new Point(this.Width / 2 + 10, 10),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += (sender, e) => {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };
            
            buttonPanel.Controls.Add(btnSalvar);
            buttonPanel.Controls.Add(btnCancelar);
            
            // Adicionar painéis ao formulário
            mainPanel.Controls.Add(optionsPanel);
            this.Controls.Add(buttonPanel);
            this.Controls.Add(lblTitle);
            this.Controls.Add(mainPanel);
        }
        
        private void CarregarConfiguracoes()
        {
            // Aqui você pode carregar as configurações de um arquivo ou banco de dados
            // Por enquanto, vamos usar os valores padrão definidos nas propriedades
        }
        
        private void SalvarConfiguracoes()
        {
            // Aqui você pode salvar as configurações em um arquivo ou banco de dados
            // Por enquanto, vamos apenas aplicar as configurações ao Dashboard
            dashboard.AplicarConfiguracoes(this);
        }
        
        private void AtualizarTema()
        {
            if (TemaEscuro)
            {
                this.BackColor = Color.FromArgb(52, 73, 94);
                foreach (Control control in this.Controls)
                {
                    if (control is Label label)
                    {
                        label.ForeColor = Color.White;
                    }
                }
            }
            else
            {
                this.BackColor = Color.FromArgb(240, 240, 240);
                foreach (Control control in this.Controls)
                {
                    if (control is Label label && label.Text != "Configurações do Sistema")
                    {
                        label.ForeColor = Color.Black;
                    }
                }
            }
        }
        
        private void ExportarDados()
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Arquivos CSV (*.csv)|*.csv",
                    Title = "Exportar dados como CSV",
                    FileName = "DadosFuncionarios_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv"
                };
                
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Obter dados do Dashboard
                    List<Funcionario> funcionarios = dashboard.ObterDadosFuncionarios();
                    
                    // Criar conteúdo CSV
                    StringBuilder csv = new StringBuilder();
                    csv.AppendLine("ID,Nome,Email,Telefone,Cargo,Departamento,Salário,Data Contratação");
                    
                    foreach (var funcionario in funcionarios)
                    {
                        string linha = funcionario.Id + "," + 
                                    "\"" + funcionario.Nome + "\"" + "," + 
                                    "\"" + funcionario.Email + "\"" + "," + 
                                    "\"" + funcionario.Telefone + "\"" + "," + 
                                    "\"" + (funcionario.Cargo?.Nome ?? "") + "\"" + "," + 
                                    "\"" + (funcionario.Departamento?.Nome ?? "") + "\"" + "," + 
                                    funcionario.Salario + "," + 
                                    "\"" + funcionario.DataContratacao.ToString("dd/MM/yyyy") + "\"";
                        csv.AppendLine(linha);
                    }
                    
                    // Salvar arquivo
                    File.WriteAllText(saveFileDialog.FileName, csv.ToString(), Encoding.UTF8);
                    
                    MessageBox.Show("Dados exportados com sucesso!", "Exportação Concluída", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao exportar dados: {ex.Message}", "Erro", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
