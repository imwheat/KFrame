//****************** 代码文件申明 ***********************
//* 文件：KFrameAssetsPath
//* 作者：wheat
//* 创建时间：2024/06/03 10:19:51 星期一
//* 描述：用来查询、管理框架配置的一些路径
//*******************************************************

using UnityEngine;
using UnityEditor;
using KFrame;
using KFrame.Utility;
using System;
using System.Collections;
using System.Collections.Generic;

namespace KFrame.Utility
{
    /// <summary>
    /// 用来查询、管理框架配置的一些路径
    /// </summary>
    [InitializeOnLoad]
    public static class KFrameAssetsPath
    {
        public static readonly string DefaultFrameAssetsPath = "Assets/KFrame/";
        static KFrameAssetsPath()
        {

        }
    }
}

