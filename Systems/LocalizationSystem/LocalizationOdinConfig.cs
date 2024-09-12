//****************** 代码文件申明 ************************
//* 文件：LocalizationConfig                      
//* 作者：wheat
//* 创建时间：2023年08月22日 星期二 17:07
//* 功能：
//*****************************************************


using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.IO;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace KFrame.Systems
{
    [CreateAssetMenu(menuName = "KFrame/LocalizationConfig")]
    public class LocalizationOdinConfig : LocalizationOdinConfigBase<LanguageType>
    {

    }
}