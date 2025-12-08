using ArcaneOnyx.ScriptableObjectDatabase;
using UnityEditor;

namespace ArcaneOnyx.Localization
{
    [CustomEditor(typeof(LocalizationLanguage))]
    public class LocalizationLanguageEditor : ScriptableItemDefaultEditor<LocalizationLanguageEditorWindow, LocalizationLanguageDatabase, LocalizationLanguage>
    {
        
    }
}