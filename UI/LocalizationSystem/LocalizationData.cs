//****************** 代码文件申明 ************************
//* 文件：LocalizationData                      
//* 作者：wheat
//* 创建时间：2023年08月22日 星期二 17:05
//* 功能：
//*****************************************************

using System.Collections.Generic;
using UnityEngine;

namespace KFrame.UI
{
    // sprite等object不支持序列化，还不清楚unity自身是怎么保存的，先这么处理
    [System.Serializable]
    public abstract class LocalizationDataBase
    {
        public string Key;
    }
    [System.Serializable]
    public class LocalizationStringDataBase
    {
        public LanguageType Language;
        public string Text;

        public LocalizationStringDataBase()
        {
            
        }

        public LocalizationStringDataBase(LanguageType language, string text)
        {
            Language = language;
            Text = text;
        }
    }
    [System.Serializable]
    public class LocalizationStringData : LocalizationDataBase
    {
        public List<LocalizationStringDataBase> Datas = new();
    }
    [System.Serializable]
    public class LocalizationImageDataBase
    {
        public LanguageType Language;
        public Sprite Sprite;
        public LocalizationImageDataBase()
        {
            
        }

        public LocalizationImageDataBase(LanguageType language, Sprite sprite)
        {
            Language = language;
            Sprite = sprite;
        }
    }
    [System.Serializable]
    public class LocalizationImageData : LocalizationDataBase
    {
        public List<LocalizationImageDataBase> Datas = new();
    }
}