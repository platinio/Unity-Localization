using ArcaneOnyx.ScriptableObjectDatabase;
using UnityEngine;

namespace ArcaneOnyx.Localization
{
    public class LocalizationKey : ScriptableItem
    {
        [SerializeField, HideInInspector] private LocalizationLanguage language;
        [SerializeField, TextArea] private string text;
        
        public LocalizationLanguage Language
        {
            get => language;
            set => language = value;
        }

        public string Text => text;
    }
}