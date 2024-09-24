//****************** 代码文件申明 ************************
//* 文件：SaveItem                      
//* 作者：wheat
//* 创建时间：2023年09月03日 星期日 20:33
//* 描述：存档的数据
//*****************************************************

using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.Serialization;

namespace KFrame.Systems
{
    [System.Serializable]
    public class SaveItem
    {
        /// <summary>
        /// 存档id
        /// </summary>
        public int SaveID;
        /// <summary>
        /// 上次存档时间
        /// </summary>
        private DateTime lastSaveTime;
        /// <summary>
        /// 上次存档时间
        /// </summary>
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
            private set
            {
                lastSaveTime = value;
                lastSaveTimeString = lastSaveTime.ToString(CultureInfo.CurrentCulture);
            }
        }
        /// <summary>
        /// 上次存档时间
        /// Json不支持DateTime，用来持久化的
        /// </summary>
        [SerializeField] 
        private string lastSaveTimeString; 

        public SaveItem(int saveID, DateTime lastSaveTime)
        {
            this.SaveID = saveID;
            LastSaveTime = lastSaveTime;
        }
        public SaveItem(int saveID, string lastSaveTimeString)
        {
            this.SaveID = saveID;
            this.lastSaveTimeString = lastSaveTimeString;
        }
        /// <summary>
        /// 更新保存时间
        /// </summary>
        /// <param name="newSaveTime">新的保存时间</param>
        public void UpdateTime(DateTime newSaveTime)
        {
            LastSaveTime = newSaveTime;
        }
    }
}