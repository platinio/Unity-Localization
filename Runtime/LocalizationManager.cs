using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArcaneOnyx.Localization
{
    public class LocalizationManager : MonoBehaviour
    {
        [SerializeField] private LocalizationLanguageDatabase localizationLanguageDatabase;
        [SerializeField] private LocalizationLanguage activeLanguage;

        private LocalizationDatabase[] localizationDatabases;

        public Action OnLanguageChanged;
        public LocalizationLanguage ActiveLanguage => activeLanguage;
        
        protected void Awake()
        {
            activeLanguage = localizationLanguageDatabase.Items.FirstOrDefault();
            LoadLocalizationDatabases();
        }

        public List<string> GetLocalizationOptions()
        {
            List<string> options = new();

            foreach (var localizationLanguage in localizationLanguageDatabase.Items)
            {
                options.Add(localizationLanguage.Name);
            }

            return options;
        }

        public string GetLocalization(LocalizationKey localizationKey) => GetLocalization(localizationKey.Name);

        public string GetLocalization(string key)
        {
            foreach (var localizationDatabase in localizationDatabases)
            {
                foreach (var localizationKey in localizationDatabase.Items)
                {
                    if (activeLanguage != localizationKey.Language) continue;
                    
                    if (string.Compare(key, localizationKey.Name, StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        return localizationKey.Text;
                    }
                }
            }

            return null;
        }

        public void ChangeActiveLanguage(string language)
        {
            foreach (var localizationLanguage in localizationLanguageDatabase.Items)
            {
                if (string.Compare(language, localizationLanguage.Name, StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    activeLanguage = localizationLanguage;
                    OnLanguageChanged?.Invoke();
                    break;
                }
            }
        }

        private void LoadLocalizationDatabases()
        {
            localizationDatabases = Resources.LoadAll<LocalizationDatabase>("Localization");
        }
    }

}

