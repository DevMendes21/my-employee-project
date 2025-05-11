using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace MyEmployeeProject.Utils
{
    /// <summary>
    /// Gerencia atalhos de teclado para a aplicação
    /// </summary>
    public static class KeyboardShortcuts
    {
        // Dicionario para armazenar os atalhos registrados
        private static Dictionary<Keys, Action> _shortcuts = new Dictionary<Keys, Action>();
        
        /// <summary>
        /// Registra um atalho de teclado
        /// </summary>
        /// <param name="key">A tecla do atalho</param>
        /// <param name="action">A ação a ser executada</param>
        public static void RegisterShortcut(Keys key, Action action)
        {
            if (!_shortcuts.ContainsKey(key))
            {
                _shortcuts.Add(key, action);
            }
            else
            {
                _shortcuts[key] = action;
            }
        }
        
        /// <summary>
        /// Processa uma tecla pressionada e executa a ação correspondente, se houver
        /// </summary>
        /// <param name="key">A tecla pressionada</param>
        /// <returns>True se a tecla corresponder a um atalho registrado</returns>
        public static bool ProcessKey(Keys key)
        {
            if (_shortcuts.TryGetValue(key, out Action? action) && action != null)
            {
                action.Invoke();
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Configura os atalhos padroes para um formulário
        /// </summary>
        /// <param name="form">O formulário que recebera os atalhos</param>
        public static void SetupFormShortcuts(Form form)
        {
            // Adiciona o manipulador de eventos de tecla
            form.KeyPreview = true;
            form.KeyDown += (sender, e) => {
                // Combinau00e7u00f5es com Ctrl
                if (e.Control)
                {
                    // Ctrl + tecla
                    ProcessKey(e.KeyCode | Keys.Control);
                }
                // Combinau00e7u00f5es com Alt
                else if (e.Alt)
                {
                    // Alt + tecla
                    ProcessKey(e.KeyCode | Keys.Alt);
                }
                // Teclas de funu00e7u00e3o (F1-F12)
                else if (e.KeyCode >= Keys.F1 && e.KeyCode <= Keys.F12)
                {
                    ProcessKey(e.KeyCode);
                }
            };
        }
        
        /// <summary>
        /// Obtem uma Descrição amigavel do atalho
        /// </summary>
        /// <param name="key">A tecla do atalho</param>
        /// <returns>Descrição do atalho em formato legivel</returns>
        public static string GetShortcutDescription(Keys key)
        {
            string description = "";
            
            if ((key & Keys.Control) == Keys.Control)
            {
                description += "Ctrl+";
                key = key & ~Keys.Control;
            }
            
            if ((key & Keys.Alt) == Keys.Alt)
            {
                description += "Alt+";
                key = key & ~Keys.Alt;
            }
            
            if ((key & Keys.Shift) == Keys.Shift)
            {
                description += "Shift+";
                key = key & ~Keys.Shift;
            }
            
            description += key.ToString();
            return description;
        }
    }
}
