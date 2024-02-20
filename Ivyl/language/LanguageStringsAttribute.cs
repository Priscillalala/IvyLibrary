using System;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;
using RoR2;
using System.Linq;

namespace IvyLibrary
{
    /// <summary>
    /// Register a mock language file within your project.
    /// </summary>
    /// <remarks>
    /// <para>Must be applied to a static method with return type <see cref="IEnumerable{T}"/> of type <see cref="KeyValuePair{TKey, TValue}"/> where keys and values are both of type <see cref="string"/>. <see cref="LanguageDictionary"/> is recommended for this purpose.</para>
    /// <para><see cref="LanguageStringsAttribute"/> is a <see cref="HG.Reflection.SearchableAttribute"/>. <see cref="HG.Reflection.SearchableAttribute.OptInAttribute"/> must be present.</para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class LanguageStringsAttribute : HG.Reflection.SearchableAttribute
    {
        const string defaultLanguage = "en";

        public LanguageStringsAttribute(string languageName = defaultLanguage)
        {
            this.languageName = languageName;
        }

        public string languageName;

        private static Lookup<string, KeyValuePair<string, string>> languageTokenPairsLookup;

        private struct LanguageStringPair
        {
            public string languageName;
            public KeyValuePair<string, string> tokenPair;
        }

        [SystemInitializer]
        private static void Init()
        {
            List<LanguageStringPair> languageStringsPairs = new List<LanguageStringPair>();
            List<LanguageStringsAttribute> attributes = new List<LanguageStringsAttribute>();
            GetInstances(attributes);
            foreach (LanguageStringsAttribute attribute in attributes)
            {
                if (attribute.target is not MethodInfo methodInfo)
                {
                    Debug.LogError($"{nameof(LanguageStringsAttribute)} cannot be applied to object of type '{attribute.target?.GetType().Name}'");
                    continue;
                }
                string cannotBeAppliedToMethod = $"{nameof(LanguageStringsAttribute)} cannot be applied to method {methodInfo.DeclaringType.FullName}.{methodInfo.Name}: ";
                if (!methodInfo.IsStatic)
                {
                    Debug.LogError(cannotBeAppliedToMethod + $"Method is not static.");
                    continue;
                }
                else if (!typeof(IEnumerable<KeyValuePair<string, string>>).IsAssignableFrom(methodInfo.ReturnType))
                {
                    Debug.LogError(cannotBeAppliedToMethod + $"{methodInfo.DeclaringType.FullName}.{methodInfo.Name} returns type '{methodInfo.ReturnType?.FullName ?? "void"}' instead of {typeof(IEnumerable<KeyValuePair<string, string>>).FullName}.");
                }
                else if (methodInfo.GetGenericArguments().Length != 0)
                {
                    Debug.LogError(cannotBeAppliedToMethod + $"{methodInfo.DeclaringType.FullName}.{methodInfo.Name} must take no arguments.");
                }
                else
                {
                    IEnumerable<KeyValuePair<string, string>> tokenPairs = (IEnumerable<KeyValuePair<string, string>>)methodInfo.Invoke(null, Array.Empty<object>());
                    if (tokenPairs == null)
                    {
                        Debug.LogError($"{methodInfo.DeclaringType.FullName}.{methodInfo.Name} returned null.");
                    }
                    else
                    {
                        string languageName = defaultLanguage;
                        if (!string.IsNullOrEmpty(attribute.languageName))
                        {
                            languageName = attribute.languageName;
                        }
                        foreach (KeyValuePair<string, string> tokenPair in tokenPairs)
                        {
                            if (tokenPair.Key == null || tokenPair.Value == null)
                            {
                                continue;
                            }
                            languageStringsPairs.Add(new LanguageStringPair
                            {
                                languageName = languageName,
                                tokenPair = tokenPair
                            });
                        }
                    }
                }
            }

            if (languageStringsPairs.Count <= 0)
            {
                return;
            }

            languageTokenPairsLookup = (Lookup<string, KeyValuePair<string, string>>)languageStringsPairs.ToLookup(x => x.languageName, x => x.tokenPair);

            foreach (var tokenPairs in languageTokenPairsLookup)
            {
                Language language = Language.FindLanguageByName(tokenPairs.Key);
                if (language != null && language.stringsLoaded)
                {
                    language.SetStringsByTokens(tokenPairs);
                }
            }

            On.RoR2.Language.LoadStrings += Language_LoadStrings;
        }

        private static void Language_LoadStrings(On.RoR2.Language.orig_LoadStrings orig, Language self)
        {
            bool stringsLoaded = self.stringsLoaded;
            orig(self);
            if (!stringsLoaded && self.stringsLoaded && languageTokenPairsLookup.Contains(self.name))
            {
                self.SetStringsByTokens(languageTokenPairsLookup[self.name]);
            }
        }
    }
}