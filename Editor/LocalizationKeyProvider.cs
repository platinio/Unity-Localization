using ArcaneOnyx.AttributeStringSelector;
using UnityEditor;

namespace ArcaneOnyx.Localization
{
    // Plugs the Localization database into the generic string-selector registry
    // so any string field marked [StringSelector("Localization")] can offer the
    // available localization keys, without that field's module depending on
    // Localization at all.
    [InitializeOnLoad]
    public static class LocalizationKeyProvider
    {
        public const string SourceId = "LocalizationKeySelector";

        static LocalizationKeyProvider()
        {
            StringSelectorRegistry.Register(SourceId, new LocalizationKeySelectorDrawer());
        }
    }
}
