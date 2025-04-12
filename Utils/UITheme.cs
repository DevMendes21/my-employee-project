using System.Drawing;
using System.Windows.Forms;
using static MinhaEmpresa.Utils.ConfigManager;

namespace MinhaEmpresa.Utils
{
    /// <summary>
    /// Classe de utilitários para manter consistência visual em toda a aplicação
    /// </summary>
    public static class UITheme
    {
        // Modo de alto contraste ativado
        private static bool _highContrastMode = false;
        
        /// <summary>
        /// Ativa ou desativa o modo de alto contraste
        /// </summary>
        public static bool HighContrastMode
        {
            get { return _highContrastMode; }
            set
            {
                _highContrastMode = value;
                // Salvar a configuração
                var config = Config;
                ConfigManager.DefinirConfiguracao("HighContrastMode", value);
                ConfigManager.SalvarConfiguracoes(config);
            }
        }
        
        // Cores principais
        public static Color PrimaryColor => HighContrastMode ? Color.FromArgb(0, 102, 204) : Color.FromArgb(41, 128, 185);
        public static Color SecondaryColor => HighContrastMode ? Color.Black : Color.FromArgb(52, 73, 94);
        public static Color SuccessColor => HighContrastMode ? Color.FromArgb(0, 153, 0) : Color.FromArgb(46, 204, 113);
        public static Color DangerColor => HighContrastMode ? Color.FromArgb(204, 0, 0) : Color.FromArgb(231, 76, 60);
        public static Color WarningColor => HighContrastMode ? Color.FromArgb(204, 204, 0) : Color.FromArgb(241, 196, 15);
        
        // Cores para temas
        public static Color LightBackground => HighContrastMode ? Color.White : Color.FromArgb(240, 240, 240);
        public static Color DarkBackground => HighContrastMode ? Color.Black : Color.FromArgb(30, 30, 30);
        public static Color DarkPanelBackground => HighContrastMode ? Color.Black : Color.FromArgb(45, 45, 45);
        public static Color DarkHeaderBackground => HighContrastMode ? Color.Black : Color.FromArgb(20, 60, 90);
        
        // Cores para texto
        public static Color LightTextColor => HighContrastMode ? Color.Black : Color.FromArgb(50, 50, 50);
        public static Color DarkTextColor => HighContrastMode ? Color.White : Color.White;
        public static Color SubtitleTextColor => HighContrastMode ? Color.Black : Color.FromArgb(100, 100, 100);
        
        // Fontes comuns
        public static readonly Font TitleFont = new Font("Segoe UI", 14, FontStyle.Bold);
        public static readonly Font SubtitleFont = new Font("Segoe UI", 12, FontStyle.Regular);
        public static readonly Font ButtonFont = new Font("Segoe UI", 10, FontStyle.Regular);
        public static readonly Font LabelFont = new Font("Segoe UI", 10, FontStyle.Regular);
        
        /// <summary>
        /// Aplica estilo padrão a um botão
        /// </summary>
        public static void ApplyButtonStyle(Button button, Color? backgroundColor = null)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = HighContrastMode ? 1 : 0;
            button.BackColor = backgroundColor ?? PrimaryColor;
            button.ForeColor = DarkTextColor;
            button.Font = ButtonFont;
            button.Cursor = Cursors.Hand;
        }
        
        /// <summary>
        /// Carrega as configurações de acessibilidade
        /// </summary>
        public static void LoadAccessibilitySettings()
        {
            _highContrastMode = ConfigManager.ObterConfiguracao<bool>("HighContrastMode", false);
        }
    }
}
