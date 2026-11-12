using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace ArcaneOnyx.Localization.Tests
{
    public class LocalizationKeysTests
    {
        private readonly List<Object> made = new List<Object>();
        private LocalizationDatabase uiDatabase;
        private LocalizationLanguage english;
        private LocalizationLanguage spanish;

        [SetUp]
        public void MakeADatabaseAndTwoLanguages()
        {
            uiDatabase = Make<LocalizationDatabase>("UI");
            english = Language("English");
            spanish = Language("Spanish");
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
            made.Add(instance);
            return instance;
        }

        private LocalizationLanguage Language(string name)
        {
            LocalizationLanguage language = Make<LocalizationLanguage>(name);
            language.Name = name;
            return language;
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
            JArray keys = Describe(Item("Title", spanish, "Título", uiDatabase), Item("Title", english, "Title", uiDatabase));

            Assert.That(keys.Count, Is.EqualTo(1));
            Assert.That((string)keys[0]["key"], Is.EqualTo("UI/Title"));
            Assert.That((string)keys[0]["text"], Is.EqualTo("Title"));
            Assert.That((string)keys[0]["language"], Is.EqualTo("English"));
        }

        [Test]
        public void KeysAreSortedByKeyWithLetterCaseIgnored()
        {
            JArray keys = Describe(Item("zeta", english, "", uiDatabase), Item("Beta", english, "", uiDatabase), Item("alpha", english, "", uiDatabase));

            Assert.That(keys.Select(key => (string)key["key"]), Is.EqualTo(new[] { "UI/alpha", "UI/Beta", "UI/zeta" }));
        }

        [Test]
        public void AnItemInALanguageNotListedComesAfterTheListedOnes()
        {
            LocalizationLanguage klingon = Language("Klingon");

            JArray keys = Describe(Item("Title", klingon, "nuqneH", uiDatabase), Item("Title", spanish, "Título", uiDatabase));

            Assert.That((string)keys[0]["text"], Is.EqualTo("Título"));
        }

        [Test]
        public void AnItemNoDatabaseOwnsHasNoKeyAndIsLeftOut()
        {
            JArray keys = Describe(Item("Loose", english, "Loose"), Item("Title", english, "Title", uiDatabase));

            Assert.That(keys.Select(key => (string)key["key"]), Is.EqualTo(new[] { "UI/Title" }));
        }
    }
}
