using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace IvyLibrary
{
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