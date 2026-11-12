using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace ArcaneOnyx.Localization.Tests
{
    /// <summary>
    /// The keys the web inspector's key picker is given, built from items and languages made in memory.
    /// </summary>
    public class LocalizationKeysTests
    {
        private readonly List<Object> made = new List<Object>();
        private LocalizationDatabase ui;
        private LocalizationLanguage english;
        private LocalizationLanguage spanish;

        [SetUp]
        public void MakeADatabaseAndTwoLanguages()
        {
            ui = Make<LocalizationDatabase>("UI");
            english = Make<LocalizationLanguage>("English");
            spanish = Make<LocalizationLanguage>("Spanish");
        }

        [TearDown]
        public void DestroyThem()
        {
            foreach (Object instance in made)
                Object.DestroyImmediate(instance);
            made.Clear();
        }

        private T Make<T>(string name) where T : ScriptableObject
        {
            var instance = ScriptableObject.CreateInstance<T>();
            instance.name = name;
            if (instance is LocalizationLanguage language)
                language.Name = name;
            made.Add(instance);
            return instance;
        }

        private LocalizationItem Item(string name, LocalizationLanguage language, string text, LocalizationDatabase owner = null)
        {
            LocalizationItem item = Make<LocalizationItem>(name);
            item.Name = name;
            item.Language = language;
            if (owner != null)
                item.SetOwnerDatabase(owner);
            var serialized = new SerializedObject(item);
            serialized.FindProperty("text").stringValue = text;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            return item;
        }

        private JArray Describe(params LocalizationItem[] items) => LocalizationKeys.Describe(items, new[] { english, spanish });

        [Test]
        public void EachKeyIsListedOnceWithItsTextInTheFirstLanguage()
        {
            JArray keys = Describe(Item("Title", spanish, "Título", ui), Item("Title", english, "Title", ui));

            Assert.That(keys.Count, Is.EqualTo(1));
            Assert.That((string)keys[0]["key"], Is.EqualTo("UI/Title"));
            Assert.That((string)keys[0]["text"], Is.EqualTo("Title"));
            Assert.That((string)keys[0]["language"], Is.EqualTo("English"));
            StringAssert.StartsWith("GlobalObjectId_V1", (string)keys[0]["id"]);
        }

        [Test]
        public void KeysAreSortedByKey()
        {
            JArray keys = Describe(Item("Zeta", english, "", ui), Item("Alpha", english, "", ui));

            Assert.That(keys.Select(key => (string)key["key"]), Is.EqualTo(new[] { "UI/Alpha", "UI/Zeta" }));
        }

        [Test]
        public void AnItemInALanguageNotListedComesAfterTheListedOnes()
        {
            LocalizationLanguage klingon = Make<LocalizationLanguage>("Klingon");

            JArray keys = Describe(Item("Title", klingon, "nuqneH", ui), Item("Title", spanish, "Título", ui));

            Assert.That((string)keys[0]["text"], Is.EqualTo("Título"));
        }

        [Test]
        public void AnItemNoDatabaseOwnsHasNoKeyAndIsLeftOut()
        {
            JArray keys = Describe(Item("Loose", english, "Loose"), Item("Title", english, "Title", ui));

            Assert.That(keys.Select(key => (string)key["key"]), Is.EqualTo(new[] { "UI/Title" }));
        }
    }
}
