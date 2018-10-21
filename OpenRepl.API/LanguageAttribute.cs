using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace OpenRepl.API
{
    /// <summary>
    /// Represents a compilable language. Place this attribute on your evaluation class to mark it as a usable language.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class LanguageAttribute : Attribute
    {
        /// <summary>
        /// The primary name of the language should go here.
        /// This will be used as the display name of the language.
        /// </summary>
        public string LanguageName { get; private set; }
        /// <summary>
        /// Additional language names should go here. This should include
        /// any names provided by the user (typically formatting names).
        /// </summary>
        public IReadOnlyCollection<string> Aliases { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="languageName">The primary name of the language should go here.
        /// This will be used as the display name of the language.</param>
        /// <param name="aliases">Additional language names should go here. This should include
        /// any names provided by the user (typically formatting names).</param>
        public LanguageAttribute(string languageName, params string[] aliases)
        {
            LanguageName = languageName;
            Aliases = Array.AsReadOnly(aliases);
        }
    }
}
