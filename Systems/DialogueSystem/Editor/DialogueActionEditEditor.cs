//****************** 代码文件申明 ***********************
//* 文件：DialogueActionEditEditor
//* 作者：wheat
//* 创建时间：2024/10/21 14:44:47 星期一
//* 描述：修改对话树事件绘制的AttributeDrawer
//*******************************************************

using KFrame.Editor;
using KFrame.Utilities;
using UnityEditor;
using UnityEngine;

namespace KFrame.Systems.DialogueSystem.Editor
{
    [CustomPropertyDrawer(typeof(DialogueActionEditAttribute))]
    public class DialogueActionEditEditor : PropertyDrawer
    {
        #region 绘制参数

        private static string[] typeDisplayNames;
        private static int[] typeSelectValues;
        private static SerializedProperty typeProperty;
        private static SerializedProperty dataProperty;
        /// <summary>
        /// 上次检测的id
        /// </summary>
        private string _prevCheckId;
        /// <summary>
        /// Label高度
        /// </summary>
        private const float LabelHeight = 20f;
        /// <summary>
        /// 空格数量
        /// </summary>
        private static float Spacing => KGUIStyle.spacing;

        #endregion

        #region 初始化更新

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="property"></param>
        private void Init(SerializedProperty property)
        {
            //初始化对话事件选择类型
            var types = EnumExtensions.GetValues<DialogueActionType>();
            typeDisplayNames = new string[types.Length];
            typeSelectValues = new int[types.Length];
            for (int i = 0; i < types.Length; i++)
            {
                var type = types[i];
                typeDisplayNames[i] = type switch
                {
                    _ => type.ToString()
                };
                typeSelectValues[i] = (int)type;
            }
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return LabelHeight * 2f + Spacing;
        }

        #endregion

        #region GUI绘制

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //枚举类型为空，那就初始化
            if (typeSelectValues == null)
            {
                Init(property);
            }
            typeProperty = property.FindPropertyRelative("type");
            dataProperty = property.FindPropertyRelative("data");
            
            EditorGUI.BeginProperty(position, label, property);

            Rect typeRect = position;
            typeRect.height = LabelHeight;
            Rect dataRect = typeRect;
            dataRect.y += typeRect.height + Spacing;

            EditorGUI.BeginChangeCheck();
            
            //事件类型
            typeProperty.enumValueFlag = EditorGUI.IntPopup(typeRect, "事件类型", typeProperty.enumValueFlag,
                typeDisplayNames, typeSelectValues);
            
            if (EditorGUI.EndChangeCheck())
            {
                //记录Undo
                Undo.RecordObject(property.serializedObject.targetObject, "DialogueNode (Modified)");
                
                //应用更改
                property.serializedObject.ApplyModifiedProperties();
            }
            
            EditorGUI.BeginChangeCheck();
            
            switch ((DialogueActionType)typeProperty.enumValueFlag)
            {
                default:
                    EditorGUI.PropertyField(dataRect ,dataProperty, KGUIHelper.TempContent("数据"));
                    break;
            }

            if (EditorGUI.EndChangeCheck())
            {
                //记录Undo
                Undo.RecordObject(property.serializedObject.targetObject, "DialogueNode (Modified)");
                
                //应用更改
                property.serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.EndProperty();
        }

        #endregion

    }
}

