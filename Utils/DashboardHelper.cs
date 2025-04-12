using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using MyEmployeeProject.Views;

namespace MyEmployeeProject.Utils
{
    /// <summary>
    /// Classe auxiliar para adicionar funcionalidades ao Dashboard
    /// </summary>
    public static class DashboardHelper
    {
        /// <summary>
        /// Adiciona um botu00e3o de seleu00e7u00e3o de idioma ao Dashboard
        /// </summary>
        public static void AdicionarBotaoIdioma(Form form)
        {
            if (form is Dashboard dashboard)
            {
                // Encontrar o painel de cabeu00e7alho
                Panel? headerPanel = dashboard.Controls.OfType<Panel>().FirstOrDefault(p => 
                    p.Location.Y == 0 && p.Height == 80);
                
                if (headerPanel != null)
                {
                    // Criar botu00e3o de idioma
                    Button btnIdioma = new Button
                    {
                        Text = "🌐 Idioma",
                        Location = new Point(headerPanel.Width - 380, 30),
                        Width = 120,
                        Height = 30,
                        BackColor = Color.FromArgb(41, 128, 185),
                        ForeColor = Color.White,
                        Font = new Font("Segoe UI", 9, FontStyle.Bold),
                        FlatStyle = FlatStyle.Flat,
                        Anchor = AnchorStyles.Top | AnchorStyles.Right
                    };
                    btnIdioma.FlatAppearance.BorderSize = 0;
                    
                    // Adicionar evento de clique
                    btnIdioma.Click += (sender, e) =>
                    {
                        SeletorIdioma seletor = new SeletorIdioma();
                        seletor.ShowDialog(dashboard);
                    };
                    
                    // Adicionar tooltip
                    ToolTip tooltip = new ToolTip();
                    tooltip.SetToolTip(btnIdioma, "Alterar o idioma do sistema (Alt+L)");
                    
                    // Adicionar botu00e3o ao painel
                    headerPanel.Controls.Add(btnIdioma);
                    btnIdioma.BringToFront();
                    
                    // Registrar atalho de teclado Alt+L para abrir o seletor de idioma
                    dashboard.KeyDown += (sender, e) =>
                    {
                        if (e.Alt && e.KeyCode == Keys.L)
                        {
                            SeletorIdioma seletor = new SeletorIdioma();
                            seletor.ShowDialog(dashboard);
                        }
                    };
                }
            }
        }
    }
}
