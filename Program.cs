using System;
using System.Windows.Forms;
using MinhaEmpresa.Views;

namespace MinhaEmpresa
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                
                // Configurar manipulador de exceu00e7u00f5es nu00e3o tratadas
                Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                
                Application.Run(new Dashboard());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao iniciar o aplicativo: {ex.Message}\n\nDetalhes: {ex.StackTrace}", "Erro Cru00edtico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MessageBox.Show($"Ocorreu um erro inesperado: {e.Exception.Message}\n\nDetalhes: {e.Exception.StackTrace}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            MessageBox.Show($"Ocorreu um erro cru00edtico: {ex.Message}\n\nDetalhes: {ex.StackTrace}", "Erro Cru00edtico", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
