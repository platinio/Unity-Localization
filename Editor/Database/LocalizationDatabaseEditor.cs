using ArcaneOnyx.ScriptableObjectDatabase;
using UnityEditor;

namespace ArcaneOnyx.Localization
{
    [CustomEditor(typeof(LocalizationDatabase))]
    public class LocalizationDatabaseEditor : ScriptableDatabaseEditor<LocalizationEditorWindow, LocalizationDatabase, LocalizationItem> 
    {
        
    }
}