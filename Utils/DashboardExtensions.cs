using System;
using System.Windows.Forms;
using MyEmployeeProject.Views;
using static MyEmployeeProject.Utils.ConfigManager;

namespace MyEmployeeProject.Utils
{
    /// <summary>
    /// Classe de extensões para o Dashboard
    /// </summary>
    public static class DashboardExtensions
    {
        /// <summary>
        /// Abre a tela de configurações com suporte a idiomas
        /// </summary>
        /// <param name="dashboard">Instância do Dashboard</param>
        public static void AbrirConfiguracoesMelhoradas(this Dashboard dashboard)
        {
            // Usar a nova tela de configurações com suporte a idiomas
            ConfiguracoesIdioma configForm = new ConfiguracoesIdioma();
            if (configForm.ShowDialog() == DialogResult.OK)
            {
                // Aplicar o idioma selecionado
                LocalizationManager.CurrentLanguage = Config.Idioma;
                
                // Sugerir reinicialização para aplicar todas as configurações
                string mensagem = "É necessário reiniciar o aplicativo para aplicar todas as alterações de idioma. Deseja reiniciar agora?";
                if (MessageBox.Show(mensagem, "Reiniciar Aplicativo", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Application.Restart();
                }
                
                // Mostrar mensagem de feedback
                MessageBox.Show("Configurações aplicadas com sucesso!", "Configurações", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
