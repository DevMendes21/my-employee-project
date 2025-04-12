using System;
using System.Windows.Forms;
using MyEmployeeProject.Views;
using static MyEmployeeProject.Utils.ConfigManager;

namespace MyEmployeeProject.Utils
{
    /// <summary>
    /// Classe para gerenciar a seleção de idiomas no sistema
    /// </summary>
    public static class LanguageSelector
    {
        /// <summary>
        /// Abre o diálogo de seleção de idioma
        /// </summary>
        public static void ShowLanguageSelector(Form parentForm)
        {
            // Criar o formulário de seleção de idioma
            Form languageForm = new Form
            {
                Text = "Selecionar Idioma",
                Width = 400,
                Height = 200,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = UITheme.LightBackground
            };
            
            // Título
            Label lblTitle = new Label
            {
                Text = "Selecione o Idioma",
                Font = new System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Bold),
                ForeColor = UITheme.PrimaryColor,
                Location = new System.Drawing.Point(20, 20),
                AutoSize = true
            };
            
            // ComboBox para seleção de idioma
            ComboBox cmbIdioma = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new System.Drawing.Point(20, 60),
                Width = 350,
                Font = new System.Drawing.Font("Segoe UI", 12)
            };
            
            // Adicionar idiomas disponíveis
            cmbIdioma.Items.Add("Português (Brasil)");
            cmbIdioma.Items.Add("English (US)");
            
            // Selecionar o idioma atual
            cmbIdioma.SelectedIndex = Config.Idioma == "pt-BR" ? 0 : 1;
            
            // Botões
            Button btnSalvar = new Button
            {
                Text = "Salvar",
                Location = new System.Drawing.Point(120, 110),
                Width = 100,
                Height = 40,
                BackColor = UITheme.SuccessColor,
                ForeColor = UITheme.DarkTextColor,
                Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnSalvar.FlatAppearance.BorderSize = 0;
            
            Button btnCancelar = new Button
            {
                Text = "Cancelar",
                Location = new System.Drawing.Point(230, 110),
                Width = 100,
                Height = 40,
                BackColor = UITheme.DangerColor,
                ForeColor = UITheme.DarkTextColor,
                Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnCancelar.FlatAppearance.BorderSize = 0;
            
            // Eventos
            btnSalvar.Click += (sender, e) =>
            {
                // Salvar o idioma selecionado
                string selectedLanguage = cmbIdioma.SelectedIndex == 0 ? "pt-BR" : "en-US";
                
                // Atualizar a configuração
                var appConfig = Config;
                appConfig.Idioma = selectedLanguage;
                ConfigManager.SalvarConfiguracoes(appConfig);
                
                // Aplicar o idioma
                LocalizationManager.CurrentLanguage = selectedLanguage;
                
                // Mostrar mensagem de sucesso
                MessageBox.Show("Idioma alterado com sucesso! Algumas alterações serão aplicadas após reiniciar o aplicativo.", 
                    "Configurações", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // Fechar o formulário
                languageForm.DialogResult = DialogResult.OK;
                languageForm.Close();
            };
            
            btnCancelar.Click += (sender, e) =>
            {
                languageForm.DialogResult = DialogResult.Cancel;
                languageForm.Close();
            };
            
            // Adicionar controles ao formulário
            languageForm.Controls.AddRange(new Control[] { lblTitle, cmbIdioma, btnSalvar, btnCancelar });
            
            // Mostrar o formulário
            languageForm.ShowDialog(parentForm);
        }
        
        /// <summary>
        /// Adiciona um botão de seleção de idioma ao formulário
        /// </summary>
        public static Button AddLanguageButton(Form form, int x, int y)
        {
            Button btnIdioma = new Button
            {
                Text = "🌐 Idioma",
                Location = new System.Drawing.Point(x, y),
                Width = 120,
                Height = 30,
                BackColor = UITheme.PrimaryColor,
                ForeColor = UITheme.DarkTextColor,
                Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnIdioma.FlatAppearance.BorderSize = 0;
            
            btnIdioma.Click += (sender, e) =>
            {
                ShowLanguageSelector(form);
            };
            
            form.Controls.Add(btnIdioma);
            return btnIdioma;
        }
    }
}
