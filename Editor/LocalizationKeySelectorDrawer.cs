using System;
using System.Collections.Generic;
using System.Linq;
using ArcaneOnyx.AttributeStringSelector;
using ArcaneOnyx.ScriptableObjectDatabase;
using UnityEditor;
using UnityEngine;

namespace ArcaneOnyx.Localization
{
    [CustomPropertyDrawer(typeof(LocalizationKeySelector))]
    public class LocalizationKeySelectorDrawer : PropertyDrawer, IStringSelectorRenderer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            string[] dropdownOptions = GetDropdownOptions();

            int index = EditorGUI.Popup(position, property.displayName, GetSelectedIndex(property, dropdownOptions), dropdownOptions);
            property.stringValue = index == 0 ? null : dropdownOptions[index];

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) * 1.5f;
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
            HashSet<string> dropdownOptions = new() { "Null" };
            var localizationItems = ScriptableDatabaseUtil.GetAllItems<LocalizationItem, LocalizationDatabase>();

            foreach (var localizationItem in localizationItems)
            {
                dropdownOptions.Add(localizationItem.Key);
            }

            return dropdownOptions.ToArray();
        }
    }
}
