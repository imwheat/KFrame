//****************** 代码文件申明 ***********************
//* 文件：UIGlobalConfig
//* 作者：wheat
//* 创建时间：2024/09/23 09:13:59 星期一
//* 描述：UI的全局配置
//*******************************************************

using System;
using System.Collections.Generic;
using System.Reflection;
using KFrame.Attributes;
using KFrame.Utilities;
using UnityEngine;

namespace KFrame.UI
{
    /// <summary>
    /// UI的全局配置
    /// </summary>
    [KGlobalConfigPath(GlobalPathType.Assets, typeof(UIGlobalConfig), true)]
    public class UIGlobalConfig : GlobalConfigBase<UIGlobalConfig>
    {
        #region 视觉设置

        /// <summary>
        /// 普通状态下的颜色
        /// </summary>
        public Color NormalColor = new Color(0.7f, 0.7f, 0.7f, 1f);
        /// <summary>
        /// 按下后的颜色
        /// </summary>
        public Color PressColor = new Color(0.4f, 0.4f, 0.4f, 1f);
        /// <summary>
        /// 选择后的颜色
        /// </summary>
        public Color SelectedColor = new Color(1f, 1f, 1f, 1f);
        /// <summary>
        /// 禁用的颜色
        /// </summary>
        public Color DisabledColor = new Color(0.2f, 0.2f, 0.2f, 1f);

        #endregion

        #region 语言相关

        /// <summary>
        /// 数字对话间隔
        /// </summary>
        public float NumberDialogueInterval = 0.1f;
        /// <summary>
        /// 字母对话间隔
        /// </summary>
        public float CharacterDialogueInterval = 0.1f;
        /// <summary>
        /// 其他字的对话间隔
        /// </summary>
        public float OtherCharacterDialogueInterval = 0.2f;

        #endregion

        /// <summary>
        /// UI数据
        /// </summary>
        public List<UIData> UIDatas = new List<UIData>();

        #region 编辑器操作

#if UNITY_EDITOR
        
        
        /// <summary>
        /// 编辑器初始化UI数据
        /// </summary>
        public void InitUIDataOnEditor()
        {
            //白名单
            string[] whiteListAssembly = new[] { "Assembly-CSharp" };
            string[] whiteListNameSpace = new[] { "GameBuild","KFrame" };
            
            UIDatas.Clear();
            //获取所有程序集
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Type baseType = typeof(UIBase);
            //遍历程序集
            foreach (Assembly assembly in assemblies)
            {
                //如果不符合白名单那就跳过
                if (!assembly.FullName.ContainsAny(whiteListAssembly)) continue;
                
                //遍历程序集下的每一个类型
                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    //如果类型不在白名单内就跳过
                    if(!type.FullName.ContainsAny(whiteListNameSpace)) continue;
                    
                    if (baseType.IsAssignableFrom(type)
                        && !type.IsAbstract)
                    {
                        var attributes = type.GetCustomAttributes<UIDataAttribute>();
                        foreach (var attribute in attributes)
                        {
                            UIDatas.Add(new UIData(attribute.UIKey, attribute.AssetPath, attribute.IsCache, attribute.LayerNum));
                        }
                    }
                }
            }
        }
        
#endif

        #endregion
        

    }
}

