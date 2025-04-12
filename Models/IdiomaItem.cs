namespace MyEmployeeProject.Models
{
    /// <summary>
    /// Represents a language option in the application.
    /// </summary>
    public class IdiomaItem
    {
        /// <summary>
        /// Gets the language code (e.g., "pt-BR", "en-US").
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// Gets the display name of the language (e.g., "Português (Brasil)", "English (US)").
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Initializes a new instance of the IdiomaItem class.
        /// </summary>
        /// <param name="code">The language code.</param>
        /// <param name="displayName">The display name of the language.</param>
        public IdiomaItem(string code, string displayName)
        {
            Code = code;
            DisplayName = displayName;
        }

        /// <summary>
        /// Returns the display name of the language.
        /// </summary>
        public override string ToString()
        {
            return DisplayName;
        }
    }
}
