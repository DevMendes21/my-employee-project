using System;
using System.Drawing;
using System.Windows.Forms;
using MyEmployeeProject.Utils;
using static MyEmployeeProject.Utils.ConfigManager;

namespace MyEmployeeProject.Views
{
    /// <summary>
    /// Formulu00e1rio para seleu00e7u00e3o de idioma
    /// </summary>
    public class SeletorIdioma : Form
    {
        private ComboBox? cmbIdioma;
        
        public SeletorIdioma()
        {
            InitializeComponent();
        }
        
        private void InitializeComponent()
        {
            this.Text = "Selecionar Idioma";
            this.Width = 400;
            this.Height = 200;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = UITheme.LightBackground;
            
            // Tu00edtulo
            Label lblTitle = new Label
            {
                Text = "Selecione o Idioma",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = UITheme.PrimaryColor,
                Location = new Point(20, 20),
                AutoSize = true
            };
            
            // ComboBox para seleu00e7u00e3o de idioma
            cmbIdioma = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(20, 60),
                Width = 350,
                Font = new Font("Segoe UI", 12)
            };
            
            // Adicionar idiomas disponu00edveis
            cmbIdioma.Items.Add("Portuguu00eas (Brasil)");
            cmbIdioma.Items.Add("English (US)");
            
            // Selecionar o idioma atual
            cmbIdioma.SelectedIndex = Config.Idioma == "pt-BR" ? 0 : 1;
            
            // Botu00f5es
            Button btnSalvar = new Button
            {
                Text = "Salvar",
                Location = new Point(120, 110),
                Width = 100,
                Height = 40,
                BackColor = UITheme.SuccessColor,
                ForeColor = UITheme.DarkTextColor,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnSalvar.FlatAppearance.BorderSize = 0;
            
            Button btnCancelar = new Button
            {
                Text = "Cancelar",
                Location = new Point(230, 110),
                Width = 100,
                Height = 40,
                BackColor = UITheme.DangerColor,
                ForeColor = UITheme.DarkTextColor,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnCancelar.FlatAppearance.BorderSize = 0;
            
            // Eventos
            btnSalvar.Click += BtnSalvar_Click;
            btnCancelar.Click += BtnCancelar_Click;
            
            // Adicionar controles ao formulu00e1rio
            this.Controls.AddRange(new Control[] { lblTitle, cmbIdioma, btnSalvar, btnCancelar });
        }
        
        private void BtnSalvar_Click(object? sender, EventArgs e)
        {
            try
            {
                // Salvar o idioma selecionado
                if (cmbIdioma == null) return;
                string selectedLanguage = cmbIdioma.SelectedIndex == 0 ? "pt-BR" : "en-US";
                
                // Atualizar a configurau00e7u00e3o
                var appConfig = Config;
                appConfig.Idioma = selectedLanguage;
                ConfigManager.SalvarConfiguracoes(appConfig);
                
                // Aplicar o idioma
                LocalizationManager.CurrentLanguage = selectedLanguage;
                
                // Mostrar mensagem de sucesso
                MessageBox.Show("Idioma alterado com sucesso! Algumas alterau00e7u00f5es seru00e3o aplicadas apu00f3s reiniciar o aplicativo.", 
                    "Configurau00e7u00f5es", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // Perguntar se deseja reiniciar
                if (MessageBox.Show("Deseja reiniciar o aplicativo agora para aplicar todas as alterau00e7u00f5es?", 
                    "Reiniciar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Application.Restart();
                }
                
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
        
        private void BtnCancelar_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
