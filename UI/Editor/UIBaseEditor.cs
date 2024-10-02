//****************** 代码文件申明 ***********************
//* 文件：UIBaseEditor
//* 作者：wheat
//* 创建时间：2024/10/02 17:34:47 星期三
//* 描述：对UIPanel的GUI进行重绘
//*******************************************************

using UnityEngine;
using UnityEditor;

namespace KFrame.UI.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UIBase), true)]
    public class UIBaseEditor : UnityEditor.Editor
    {

        #region GUI重绘

        public override void OnInspectorGUI()
        {
            //绘制默认GUI
            base.DrawDefaultInspector();
            
            EditorGUILayout.BeginVertical();
            
            GUILayout.Space(10f);
            
            if (GUILayout.Button("搜寻可本地化组件", GUILayout.Height(30)))
            {
                foreach (var obj in targets)
                {
                    FindLocalizationSets(obj as UIBase);
                }
            }
            
            EditorGUILayout.EndVertical();
        }

        #endregion

        #region 按钮操作
        
        /// <summary>
        /// 搜索可本地化配置的资源
        /// </summary>
        private void FindLocalizationSets(UIBase ui)
        {
            //为空就返回
            if(ui == null) return;
            
            ui.CollectLocalizationUI();
        }

        #endregion

    }
}

