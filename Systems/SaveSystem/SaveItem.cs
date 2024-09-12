//****************** 代码文件申明 ************************
//* 文件：SaveItem                      
//* 作者：wheat
//* 创建时间：2023年09月03日 星期日 20:33
//* 描述：存档的数据
//*****************************************************

using System;
using UnityEngine;

namespace KFrame.Systems
{
    [System.Serializable]
    public class SaveItem
    {
        public int saveID;
        private DateTime lastSaveTime;

        public DateTime LastSaveTime
        {
            get
            {
                if (lastSaveTime == default(DateTime))
                {
                    DateTime.TryParse(lastSaveTimeString, out lastSaveTime);
                }

                return lastSaveTime;
            }
        }

        [SerializeField] private string lastSaveTimeString; // Json不支持DateTime，用来持久化的

        public SaveItem(int saveID, DateTime lastSaveTime)
        {
            this.saveID = saveID;
            this.lastSaveTime = lastSaveTime;
            lastSaveTimeString = lastSaveTime.ToString();
        }
        public SaveItem(int saveID, string lastSaveTimeString)
        {
            this.saveID = saveID;
            this.lastSaveTimeString = lastSaveTimeString;
        }

        public void UpdateTime(DateTime lastSaveTime)
        {
            this.lastSaveTime = lastSaveTime;
            lastSaveTimeString = lastSaveTime.ToString();
        }
    }
}