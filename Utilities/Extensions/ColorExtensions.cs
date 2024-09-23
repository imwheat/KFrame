//****************** 代码文件申明 ***********************
//* 文件：ColorExtensions
//* 作者：wheat
//* 创建时间：2024/05/28 08:35:19 星期二
//* 描述：拓展Color的一些方法
//*******************************************************

using UnityEngine;
using UnityEditor;
using KFrame;
using KFrame.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;

namespace KFrame.Utilities
{
    public static class ColorExtensions
    {
        /// <summary>
        /// 修改颜色的Alpha
        /// </summary>
        /// <param name="color">原颜色</param>
        /// <param name="alpha">目标alpha</param>
        /// <returns>修改alpha后的原颜色</returns>
        public static Color ChangeAlpha(this ref Color color, float alpha)
        {
            color = new Color(color.r, color.g, color.b, alpha);
            return color;
        }
    }
}

