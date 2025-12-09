using System;
using System.Collections.Generic;
using System.Linq;
using ArcaneOnyx.AdvancedDropdown;
using ArcaneOnyx.ScriptableObjectDatabase;
using UnityEditor;
using UnityEngine;

namespace ArcaneOnyx.Localization
{
    [CustomPropertyDrawer(typeof(LocalizationKeySelector))]
    public class LocalizationKeySelectorDrawer : PropertyDrawer
    {
        public override void OnGUI (Rect position,SerializedProperty property,GUIContent label) 
        {
            EditorGUI.BeginProperty(position,label,property);
            string[] dropdownOptions = GetDropdownOptions();
            
            int index = EditorGUI.Popup(position, property.displayName, GetSelectedIndex(property, dropdownOptions), dropdownOptions);
            property.stringValue = index == 0? null : dropdownOptions[index];
            
            EditorGUI.EndProperty();
        }

        private int GetSelectedIndex(SerializedProperty property, string[] options)
        {
            string selected = property.stringValue;
            if (string.IsNullOrEmpty(selected)) return 0;
            
            int index = Array.IndexOf(options, selected);
            return index == -1 ? 0 : index;
        }

        private string[] GetDropdownOptions()
        {
            HashSet<string> dropdownOptions = new();
            var localizationItems = ScriptableDatabaseUtil.GetAllItems<LocalizationItem, LocalizationDatabase>();

            dropdownOptions.Add("Null");
            
            foreach (var localizationItem in localizationItems)
            {
                dropdownOptions.Add(localizationItem.Key);
            }

            return dropdownOptions.ToArray();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) * 1.5f;
        }

        private void DrawDatabaseDropDown(Rect rect, SerializedProperty property)
        {
            if (!EditorGUI.DropdownButton(rect, new GUIContent(GetSelectedItemName(property)), FocusType.Passive)) return;
        
            var dropDownItems = GetDropdownItems(property);

            AdvancedDropdownEditorWindow.ShowDropdown(dropDownItems, delegate(ScriptableItem item)
            {
                UpdateDropdownValue(property, item);
            });
        }

        private List<DropdownItem<ScriptableItem>> GetDropdownItems(SerializedProperty property)
        {
            var items = GetScriptableItems();
            List<DropdownItem<ScriptableItem>> dropDownItems = new();

            dropDownItems.Add(new DropdownItem<ScriptableItem>("Null", property.objectReferenceValue == null, null));
            
            foreach (var item in items)
            {
                bool isSelected = item == property.objectReferenceValue;
                dropDownItems.Add(new DropdownItem<ScriptableItem>(item.Name, item.Icon,  isSelected, item));
            }
            
            return dropDownItems;
        }

        private List<ScriptableItem> GetScriptableItems()
        {
            var dropdownItems = ScriptableDatabaseUtil.GetDropdownOptions(typeof(LocalizationDatabase));

            for (int i = dropdownItems.Count - 1; i >=0; i--)
            {
                if (i >= dropdownItems.Count)
                {
                    i = dropdownItems.Count - 1;
                }

                var item = dropdownItems[i];
                
                for (int j = i - 1; j >=0; j--)
                {
                    if (dropdownItems[j].Name == item.Name)
                    {
                        dropdownItems.RemoveAt(j);
                    }
                }
            }

            return dropdownItems;
        }

        private string GetSelectedItemName(SerializedProperty property)
        {
            if (property.objectReferenceValue is ScriptableItem item) return item.name;
            return "null";
        }

        private void UpdateDropdownValue(SerializedProperty property, ScriptableItem item)
        {
            property.serializedObject.Update();
            property.objectReferenceValue = item;
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}