//****************** 代码文件申明 ***********************
//* 文件：InputSettingsPanelEditor
//* 作者：wheat
//* 创建时间：2024/10/05 10:22:31 星期六
//* 描述：重新绘制按键设置面板的GUI
//*******************************************************

using UnityEngine;
using UnityEditor;


namespace KFrame.UI.Editor
{
    [CustomEditor(typeof(InputSettingsPanel))]
    public class InputSettingsPanelEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.BeginVertical();
            
            GUILayout.Space(10f);

            if (GUILayout.Button("自动创建按键设置UI", GUILayout.Height(30f)))
            {
                InputSettingsPanel panel = target as InputSettingsPanel;
                if(panel == null) return;
                panel.AutoCreateInputSetUI();
                EditorUtility.SetDirty(panel);
            }
            
            EditorGUILayout.EndVertical();
        }
    }
}

