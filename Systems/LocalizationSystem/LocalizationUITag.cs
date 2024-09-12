//****************** 代码文件申明 ************************
//* 文件：LocalizationUITag                                       
//* 作者：wheat
//* 创建时间：2023/12/17 16:13:59 星期日
//* 描述：方便编辑本地化设置
//*****************************************************
using UnityEngine;
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace KFrame.Systems
{
    [ExecuteInEditMode]
    public class LocalizationUITag : MonoBehaviour
    {
        [LabelText("Key")] public string Key;
        [LabelText("简体中文"),TabGroup("文本"), ReadOnly] public string CNstring;
        [LabelText("繁体中文"), TabGroup("文本"), ReadOnly] public string CTstring;
        [LabelText("英文"), TabGroup("文本"), ReadOnly] public string ENstring;
        [LabelText("日语"), TabGroup("文本"), ReadOnly] public string JPstring;
        [LabelText("简体中文"), TabGroup("图片"), ReadOnly] public Sprite CNimg;
        [LabelText("繁体中文"), TabGroup("图片"), ReadOnly] public Sprite CTimg;
        [LabelText("英文"), TabGroup("图片"), ReadOnly] public Sprite ENimg;
        [LabelText("日语"), TabGroup("图片"), ReadOnly] public Sprite JPimg;
    }
}

