//****************** 代码文件申明 ***********************
//* 文件：AnimatorParameterData
//* 作者：wheat
//* 创建时间：2024/09/28 09:14:36 星期六
//* 描述：
//*******************************************************

using UnityEngine;
using UnityEditor;
using KFrame;
using KFrame.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;

namespace KFrame.Editor
{
    public class AnimatorParameterData
    {
        /// <summary>
        /// 名称
        /// </summary>
        public readonly string Name;
        /// <summary>
        /// 引用的动画机字典
        /// </summary>
        public readonly Dictionary<AnimatorControllerParameterType, List<string>> ReferenceDic = new();

        public AnimatorParameterData(string name)
        {
            Name = name;
        }
    }
}

