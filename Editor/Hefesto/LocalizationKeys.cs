using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArcaneOnyx.Hefesto;
using ArcaneOnyx.ScriptableObjectDatabase;
using Newtonsoft.Json.Linq;
using UnityEditor;

namespace ArcaneOnyx.Localization
{
    /// <summary>
    /// Answers the web inspector's localization.keys request, which this module's key picker asks: every key, with the
    /// item holding its text in the first language, so the page can show what a key says and open that item.
    /// </summary>
    [InitializeOnLoad]
    internal static class LocalizationKeys
    {
        public const string Request = "localization.keys";

        static LocalizationKeys()
        {
            InspectorServer.RegisterRequest(Request, _ => Task.FromResult<JToken>(DescribeProjectKeys()));
        }

        private static JArray DescribeProjectKeys()
            => Describe(
                ScriptableDatabaseUtil.GetAllItems<LocalizationItem, LocalizationDatabase>(),
                ScriptableDatabaseUtil.GetAllItems<LocalizationLanguage, LocalizationLanguageDatabase>());

        /// <summary>
        /// One entry per key, sorted by key with letter case ignored, as the game matches keys: <c>{ key, id, text,
        /// language }</c>, from the item in the earliest of <paramref name="languages"/>. An item in a language not listed
        /// comes after those that are.
        /// </summary>
        internal static JArray Describe(IEnumerable<LocalizationItem> items, IReadOnlyList<LocalizationLanguage> languages)
        {
            Dictionary<LocalizationLanguage, int> places = languages
                .Select((language, place) => (language, place))
                .Where(entry => entry.language != null)
                .GroupBy(entry => entry.language)
                .ToDictionary(group => group.Key, group => group.First().place);
            var firstLanguageItems = items
                .Where(item => item != null && item.HasKey)
                .GroupBy(item => item.Key)
                .OrderBy(itemsWithTheKey => itemsWithTheKey.Key, StringComparer.OrdinalIgnoreCase)
                .Select(itemsWithTheKey => itemsWithTheKey.OrderBy(item => PlaceOf(item.Language, places)).First());
            return new JArray(firstLanguageItems.Select(item => new JObject
            {
                ["key"] = item.Key,
                ["id"] = GlobalObjectId.GetGlobalObjectIdSlow(item).ToString(),
                ["text"] = item.Text ?? "",
                ["language"] = item.Language != null ? item.Language.Name : null,
            }));
        }

        private static int PlaceOf(LocalizationLanguage language, Dictionary<LocalizationLanguage, int> places)
            => language != null && places.TryGetValue(language, out int place) ? place : int.MaxValue;
    }
}
