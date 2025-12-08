using System.Collections.Generic;
using ArcaneOnyx.AdvancedDropdown;
using ArcaneOnyx.ScriptableObjectDatabase;
using UnityEditor;
using UnityEngine;

namespace ArcaneOnyx.Localization
{
    [CustomPropertyDrawer(typeof(LocalizationSelector))]
    public class LocalizationSelectorDrawer : PropertyDrawer
    {
         public override void OnGUI (Rect position,SerializedProperty property,GUIContent label) 
        {
            EditorGUI.BeginProperty(position,label,property);
            
            Rect labelRect = position;
            labelRect.width = position.width / 2.0f;
            
            Rect dropdownRect = position;
            dropdownRect.x += position.width / 2.0f;
            dropdownRect.width = position.width / 2.0f;
            
            Rect buttonDropdownRect = position;
            buttonDropdownRect.width = 25;
            buttonDropdownRect.x += (position.width / 2.0f) - (buttonDropdownRect.width) - 5.0f;

            Rect iconRect = buttonDropdownRect;
            iconRect.width = 30;
            iconRect.x -= buttonDropdownRect.width + 10;

            buttonDropdownRect.height /= 1.5f;
            
            EditorGUI.LabelField(labelRect, label);
            DrawDatabaseDropDown(dropdownRect, property);
            
            dynamic item = property.objectReferenceValue;

            if (item != null && item.Icon != null)
            {
                Sprite sprite = item.Icon;
                GUI.DrawTexture(iconRect, sprite.texture);
            }

            if (property.objectReferenceValue != null && GUI.Button(buttonDropdownRect, "►"))
            {
                
                Editor editorInstance = Editor.CreateEditor(property.objectReferenceValue);

                try
                {
                    ((dynamic)editorInstance).OpenInEditorWindow();
                }
                catch
                {
                    EditorGUIUtility.PingObject(property.objectReferenceValue);
                }

                Object.DestroyImmediate(editorInstance);
            }

            EditorGUI.EndProperty();
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