using System.Collections.Generic;
using System.Linq;
using ArcaneOnyx.ScriptableObjectDatabase;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

namespace ArcaneOnyx.Localization
{
    public class LocalizationEditorWindow : DatabaseEditorWindowWithCategory<LocalizationDatabase, LocalizationKey>
    {
        private static LocalizationLanguageDatabase localizationLanguageDatabase;
        
        [MenuItem("Window/Localization/Keys Editor")]
        public static void OpenEditor()
        {
            LocalizationEditorWindow wnd = GetWindow<LocalizationEditorWindow>();
            wnd.titleContent = new GUIContent(wnd.GetWindowTitle());
        }

        public override string GetWindowTitle() => "Localization";

        protected override void SetupToolBar(ToolbarMenu toolbarMenu)
        {
            toolbarMenu.menu.AppendSeparator("Create");

            var types = GetEnumerableOfType(typeof(LocalizationKey));
            
            foreach (var type in types)
            {
                toolbarMenu.menu.AppendAction($"Create/{type.Name}", _ =>
                {
                    var newKey = CreateNewEntry(type);
                    
                    var localizationLanguage = ScriptableDatabaseUtil.GetActiveDatabase<LocalizationLanguage, LocalizationLanguageDatabase>();
                    if (localizationLanguage == null) return;

                    LocalizationLanguage selectedLanguage = null;
                    
                    foreach (var language in localizationLanguage.Items)
                    {
                        if (language.Name == selectedCategory.ID)
                        {
                            selectedLanguage = language;
                            break;
                        }
                    }
                    
                    newKey.Language = selectedLanguage;
                    
                    UpdateSelectedCategory();
                });
            }
            
            
            foreach (var database in activeDatabases)
            {
                toolbarMenu.menu.AppendAction($"Copy To/{database.name}", (_) => CopyTo(database, selectedItem));
            }
            toolbarMenu.menu.AppendAction("Duplicate Selected Item", DuplicateEntry);
            toolbarMenu.menu.AppendAction("Remove Selected Item", RemoveEntry);
            toolbarMenu.menu.AppendAction("Move Up", (_) => MoveSelectedItem(-1));
            toolbarMenu.menu.AppendAction("Move Down", (_) => MoveSelectedItem(1));
            toolbarMenu.menu.AppendAction("Migrate Ids", (_) => MigrateIds());
            toolbarMenu.menu.AppendAction("Save", Save);
        }
        
        protected override IReadOnlyList<LocalizationKey> FilterEntries(IReadOnlyList<LocalizationKey> entries)
        {
            var selectedLanguage = GetSelectedCategory();
            if (selectedLanguage == null) return entries;
            
            return entries.Where(x => x.Language == selectedLanguage).ToList();
        }
        
        private LocalizationLanguage GetSelectedCategory()
        {
            if (selectedCategory == null) return null;
            
            var languageDatabase = GetLanguageDatabase();

            foreach (var language in languageDatabase.Items)
            {
                if (selectedCategory.Name == language.Name) return language;
            }

            return null;
        }
        
        protected override Category GetItemCategory(LocalizationKey item)
        {
            var categories = GetDatabaseCategories();

            foreach (var category in categories)
            {
                if (category.ID == item.Language.name) return category;
            }
            
            return null;
        }

        protected override List<Category> GetDatabaseCategories()
        {
            List<Category> result = new List<Category>();

            var skillDatabase = GetLanguageDatabase();

            foreach (var skillClassType in skillDatabase.Items)
            {
                result.Add(new Category(skillClassType.Name, skillClassType.Name, skillClassType.Icon));
            }

            return result;
        }
        
        private static LocalizationLanguageDatabase GetLanguageDatabase()
        {
            if (localizationLanguageDatabase == null)
            {
                localizationLanguageDatabase = ScriptableDatabaseUtil.GetActiveDatabase<LocalizationLanguage, LocalizationLanguageDatabase>();
            }

            return localizationLanguageDatabase;
        }
    }
}