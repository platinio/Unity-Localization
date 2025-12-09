using ArcaneOnyx.ScriptableObjectDatabase;
using UnityEngine;

namespace ArcaneOnyx.Localization
{
    [CreateAssetMenu(menuName = "Database/Localization/Localization Database")]
    public class LocalizationDatabase : ScriptableDatabase<LocalizationItem> { }
}