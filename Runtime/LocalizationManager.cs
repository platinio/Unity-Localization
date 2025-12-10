using System;
using System.Collections.Generic;
using System.Linq;
using Platinio;
using UnityEngine;

namespace ArcaneOnyx.Localization
{
    public class LocalizationManager : Singleton<LocalizationManager>
    {
        [SerializeField] private LocalizationLanguageDatabase localizationLanguageDatabase;
        [SerializeField] private LocalizationLanguage activeLanguage;

        private LocalizationDatabase[] localizationDatabases;

        public Action OnLanguageChanged;
        public LocalizationLanguage ActiveLanguage => activeLanguage;
        
        protected override void Awake()
        {
            base.Awake();
            
            if (!ReferenceEquals( Instance, gameObject.GetComponent<LocalizationManager>()))
            {
                return;
            }
            
            activeLanguage = localizationLanguageDatabase.Items.FirstOrDefault();
            LoadLocalizationDatabases();
        }

        public List<string> GetAvailableLanguages()
        {
            List<string> options = new();

            foreach (var localizationLanguage in localizationLanguageDatabase.Items)
            {
                options.Add(localizationLanguage.Name);
            }

            return options;
        }

        public string GetLocalization(LocalizationItem localizationItem) => GetLocalization(localizationItem.Key);

        public string GetLocalization(string localizationKey)
        {
            string databaseName = localizationKey.Split('/')[0];
            string keyName = localizationKey.Split('/')[1];
            
            foreach (var localizationDatabase in localizationDatabases)
            {
                if (string.Compare(localizationDatabase.name, databaseName,
                        StringComparison.InvariantCultureIgnoreCase) != 0)
                {
                    continue;
                }

                foreach (var localization in localizationDatabase.Items)
                {
                    if (activeLanguage != localization.Language) continue;
                    
                    if (string.Compare(keyName, localization.Name, StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        return localization.Text;
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

