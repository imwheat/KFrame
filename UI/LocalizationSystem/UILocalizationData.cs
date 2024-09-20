//****************** 代码文件申明 ***********************
//* 文件：UILocalizationData
//* 作者：wheat
//* 创建时间：2024/09/19 16:27:46 星期四
//* 描述：UI的本地化配置绑定数据
//*******************************************************

using UnityEngine.UI;

namespace KFrame.UI
{
    [System.Serializable]
    public class UILocalizationData
    {
        public string Key;
        public Graphic Component;

        public UILocalizationData()
        {
            
        }

        public UILocalizationData(string key, Graphic component)
        {
            this.Key = key;
            this.Component = component;
        }
    }
}

