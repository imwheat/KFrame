//****************** 代码文件申明 ************************
//* 文件：SaveItem                      
//* 作者：wheat
//* 创建时间：2023年09月03日 星期日 20:33
//* 描述：存档的数据
//*****************************************************

using System;
using System.Globalization;
using KFrame.Utilities;
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
        /// <summary>
        /// 存档文件路径
        /// </summary>
        [NonSerialized]
        public readonly string SaveFilePath;
        /// <summary>
        /// 存档的游玩数据
        /// </summary>
        public readonly SavePlayData SavePlayData;
        /// <summary>
        /// 保存数据
        /// </summary>
        [SerializeField]
        private readonly Serialized_Dic<string, string> saveDatas;

        public SaveItem(int saveID, DateTime lastSaveTime, string saveFilePath)
        {
            this.SaveID = saveID;
            LastSaveTime = lastSaveTime;
            SaveFilePath = saveFilePath;
            SavePlayData = new SavePlayData();
            saveDatas = new Serialized_Dic<string, string>();
        }
        /// <summary>
        /// 更新保存时间
        /// </summary>
        /// <param name="newSaveTime">新的保存时间</param>
        public void UpdateTime(DateTime newSaveTime)
        {
            LastSaveTime = newSaveTime;
        }
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="key">保存的key</param>
        /// <param name="jsonData">保存的数据</param>
        public void SaveData(string key, string jsonData)
        {
            saveDatas[key] = jsonData;
        }
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="saveable">可以保存的对象</param>
        public void SaveData(ISaveable saveable)
        {
            saveDatas[saveable.SaveKey] = saveable.GetJsonData();
        }
        /// <summary>
        /// 加载数据
        /// </summary>
        /// <param name="key">数据key</param>
        /// <returns>json数据，如果没有的话返回""</returns>
        public string LoadData(string key)
        {
            return saveDatas.GetValueOrDefault(key);
        }
        /// <summary>
        /// 加载数据
        /// </summary>
        /// <param name="saveable">可以保存的对象</param>
        public void LoadData(ISaveable saveable)
        {
            //从数据字典里面获取数据
            if(!saveDatas.TryGetValue(saveable.SaveKey, out string jsonData)) return;
            //得到后加载
            saveable.ILoad(jsonData);
        }
    }
}