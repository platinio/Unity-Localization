using ArcaneOnyx.ScriptableObjectDatabase;
using UnityEditor;

namespace ArcaneOnyx.Localization
{
    [CustomEditor(typeof(LocalizationLanguageDatabase))]
    public class LocalizationLanguageDatabaseEditor : ScriptableDatabaseEditor<LocalizationLanguageEditorWindow, LocalizationLanguageDatabase, LocalizationLanguage>
    {
        
    }
}