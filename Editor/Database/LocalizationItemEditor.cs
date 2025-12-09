using ArcaneOnyx.ScriptableObjectDatabase;
using UnityEditor;

namespace ArcaneOnyx.Localization
{
    [CustomEditor(typeof(LocalizationItem))]
    public class LocalizationItemEditor : ScriptableItemDefaultEditor<LocalizationEditorWindow, LocalizationDatabase, LocalizationItem>
    {
        
    }
}