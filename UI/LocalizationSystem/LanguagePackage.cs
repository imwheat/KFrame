//****************** 代码文件申明 ***********************
//* 文件：LanguagePackage
//* 作者：wheat
//* 创建时间：2024/10/09 13:17:33 星期三
//* 描述：存储了一种游戏语言的包
//*******************************************************

using System.Collections.Generic;
using UnityEngine;

namespace KFrame.UI
{
    [CreateAssetMenu(menuName = "ScriptableObject/LanguagePackage", fileName = "LanguagePackage")]
    public class LanguagePackage : ConfigBase
    {
        /// <summary>
        /// 语言类型
        /// </summary>
        public LanguageClass language;
        /// <summary>
        /// 数据
        /// </summary>
        public List<LanguagePackageTextData> datas = new();
    }
}

