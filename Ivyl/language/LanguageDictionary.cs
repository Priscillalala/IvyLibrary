using System.Collections.Generic;

namespace IvyLibrary
{
    /// <summary>
    /// An string dictionary that does not throw when rejecting null keys or values.
    /// </summary>
    /// <remarks>
    /// For use with <see cref="LanguageStringsAttribute"/>. Useful when content may or may not exist.
    /// </remarks>
    public class LanguageDictionary : Dictionary<string, string>
    {
        public LanguageDictionary() : base() { }

        public new string this[string key]
        {
            get
            {
                return base[key];
            }
            set
            {
                if (key == null || value == null)
                {
                    return;
                }
                base[key] = value;
            }
        }

        public new void Add(string key, string value)
        {
            if (key == null || value == null)
            {
                return;
            }
            base.Add(key, value);
        }
    }
}