using ArcaneOnyx.ScriptableObjectDatabase;
using UnityEditor;
using UnityEngine;

namespace ArcaneOnyx.Localization
{
    public class LocalizationLanguageEditorWindow : DatabaseEditorWindow<LocalizationLanguageDatabase, LocalizationLanguage>
    {
        [MenuItem("Window/Localization/Languages")]
        public static void OpenEditor()
        {
            LocalizationLanguageEditorWindow wnd = GetWindow<LocalizationLanguageEditorWindow>();
            wnd.titleContent = new GUIContent(wnd.GetWindowTitle());
        }
        
        public override string GetWindowTitle() => "Localization Language";
    }
}