using ArcaneOnyx.ScriptableObjectDatabase;
using UnityEngine;

namespace ArcaneOnyx.Localization
{
    public class LocalizationItem : ScriptableItem
    {
        [SerializeField, HideInInspector] private LocalizationDatabase ownerDatabase;
        [SerializeField, HideInInspector] private LocalizationLanguage language;
        [SerializeField, TextArea] private string text;
        
        public LocalizationLanguage Language
        {
            get => language;
            set => language = value;
        }

        public string Text => text;
        public string Key => $"{ownerDatabase.name}/{Name}";

        public void SetOwnerDatabase(LocalizationDatabase localizationDatabase)
        {
            ownerDatabase = localizationDatabase;
        }
    }
}