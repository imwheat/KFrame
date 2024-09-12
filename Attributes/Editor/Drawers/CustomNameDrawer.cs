using UnityEngine;
using KFrame;
using KFrame.Attributes;
using UnityEditor;

namespace KFrame.AttributeDrawers
{
    [CustomPropertyDrawer(typeof(DisplayNameAttribute))]
    public class DisplayNameAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // // 将属性标签设置为自定义的显示名称
            // DisplayNameAttribute customNameAttribute = (DisplayNameAttribute)attribute;
            //
            // label.text = customNameAttribute.displayName;
            //
            // //绘制属性
            // EditorGUI.PropertyField(position, property, label, true);

            DisplayNameAttribute displayNameAttribute = attribute as DisplayNameAttribute;
            GUIContent customLabel = new GUIContent(displayNameAttribute.displayName);

            // 使用字段名作为默认Tooltip值
            string tooltip = ObjectNames.NicifyVariableName(property.name);
            if (!string.IsNullOrEmpty(displayNameAttribute.displayName))
            {
                tooltip = displayNameAttribute.displayName;
            }

            customLabel.tooltip = tooltip;

            EditorGUI.PropertyField(position, property, customLabel);
        }
    }
}



