using System.Windows.Forms;

namespace MyEmployeeProject.Utils
{
    /// <summary>
    /// Classe de extensu00f5es para controles da interface
    /// </summary>
    public static class ControlExtensions
    {
        private static readonly ToolTip _toolTip = new ToolTip { AutoPopDelay = 5000, InitialDelay = 1000, ReshowDelay = 500, ShowAlways = true };
        
        /// <summary>
        /// Define uma dica de ferramenta (tooltip) para um controle
        /// </summary>
        /// <param name="control">O controle que receberu00e1 o tooltip</param>
        /// <param name="text">O texto do tooltip</param>
        public static void SetToolTip(this Control control, string text)
        {
            _toolTip.SetToolTip(control, text);
        }
    }
}
