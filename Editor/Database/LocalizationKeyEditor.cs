using ArcaneOnyx.ScriptableObjectDatabase;
using UnityEditor;

namespace ArcaneOnyx.Localization
{
    [CustomEditor(typeof(LocalizationKey))]
    public class LocalizationKeyEditor : ScriptableItemDefaultEditor<LocalizationEditorWindow, LocalizationDatabase, LocalizationKey>
    {
        
    }
}