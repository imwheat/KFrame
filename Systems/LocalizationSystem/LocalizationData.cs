//****************** 代码文件申明 ************************
//* 文件：LocalizationData                      
//* 作者：wheat
//* 创建时间：2023年08月22日 星期二 17:05
//* 功能：
//*****************************************************

using UnityEngine;

namespace KFrame.Systems
{
    public abstract class LocalizationDataBase
    {
    }

    public class LocalizationStringData : LocalizationDataBase
    {
        public string content;
    }

    public class LocalizationImageData : LocalizationDataBase
    {
        public Sprite content;
    }
}