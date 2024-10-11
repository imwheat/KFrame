//****************** 代码文件申明 ************************
//* 文件：LocalizationData                      
//* 作者：wheat
//* 创建时间：2023年08月22日 星期二 17:05
//* 功能：
//*****************************************************

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace KFrame.UI
{
    // sprite等object不支持序列化，还不清楚unity自身是怎么保存的，先这么处理
    [System.Serializable]
    public abstract class LocalizationDataBase
    {
        public string Key;

        public LocalizationDataBase(string key)
        {
            Key = key;
        }
    }
    [System.Serializable]
    public class LocalizationStringDataBase
    {
        [FormerlySerializedAs("Language")]
        public int LanguageId;
        public string Text;

        public LocalizationStringDataBase()
        {
            
        }

        public LocalizationStringDataBase(int language, string text)
        {
            LanguageId = language;
            Text = text;
        }
    }
    [System.Serializable]
    public class LocalizationStringData : LocalizationDataBase
    {
        public List<LocalizationStringDataBase> Datas = new();

        public LocalizationStringData(string key) : base(key)
        {
            
        }
        
        #if UNITY_EDITOR

        /// <summary>
        /// 复制数据
        /// </summary>
        public void CopyData(LocalizationStringData data)
        {
            //如果为空那就不进行复制
            if(data == null) return;

            Datas = new List<LocalizationStringDataBase>();

            foreach (LocalizationStringDataBase stringDataBase in data.Datas)
            {
                Datas.Add(new LocalizationStringDataBase(stringDataBase.LanguageId, stringDataBase.Text));
            }
        }
        
        #endif
        
    }
    [System.Serializable]
    public class LocalizationImageDataBase
    {
        [FormerlySerializedAs("Language")]
        public int LanguageId;
        public Sprite Sprite;
        public LocalizationImageDataBase()
        {
            
        }

        public LocalizationImageDataBase(int language, Sprite sprite)
        {
            LanguageId = language;
            Sprite = sprite;
        }
    }
    [System.Serializable]
    public class LocalizationImageData : LocalizationDataBase
    {
        public List<LocalizationImageDataBase> Datas = new();
        
        public LocalizationImageData(string key) : base(key)
        {
        }
        
#if UNITY_EDITOR

        /// <summary>
        /// 复制数据
        /// </summary>
        public void CopyData(LocalizationImageData data)
        {
            //如果为空那就不进行复制
            if(data == null) return;

            Datas = new List<LocalizationImageDataBase>();

            foreach (LocalizationImageDataBase imageDataBase in data.Datas)
            {
                Datas.Add(new LocalizationImageDataBase(imageDataBase.LanguageId, imageDataBase.Sprite));
            }
        }
        
#endif

    }
}