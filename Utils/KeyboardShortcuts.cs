using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace MyEmployeeProject.Utils
{
    /// <summary>
    /// Gerencia atalhos de teclado para a aplicau00e7u00e3o
    /// </summary>
    public static class KeyboardShortcuts
    {
        // Dicionu00e1rio para armazenar os atalhos registrados
        private static Dictionary<Keys, Action> _shortcuts = new Dictionary<Keys, Action>();
        
        /// <summary>
        /// Registra um atalho de teclado
        /// </summary>
        /// <param name="key">A tecla do atalho</param>
        /// <param name="action">A au00e7u00e3o a ser executada</param>
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
        /// Processa uma tecla pressionada e executa a au00e7u00e3o correspondente, se houver
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
        /// Configura os atalhos padru00e3o para um formulu00e1rio
        /// </summary>
        /// <param name="form">O formulu00e1rio que receberu00e1 os atalhos</param>
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
        /// Obtu00e9m uma descriu00e7u00e3o amiu00e1vel do atalho
        /// </summary>
        /// <param name="key">A tecla do atalho</param>
        /// <returns>Descriu00e7u00e3o do atalho em formato legiu00edvel</returns>
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
