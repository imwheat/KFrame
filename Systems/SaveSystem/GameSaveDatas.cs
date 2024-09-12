//****************** 代码文件申明 ************************
//* 文件：GameSaveDatas                                       
//* 作者：wheat
//* 创建时间：2024/02/21 13:00:41 星期三
//* 描述：游戏保存数据
//*****************************************************
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Collections;

namespace KFrame.Systems
{
    /// <summary>
    /// 游戏保存数据
    /// </summary>
    [System.Serializable]
    public class GameSaveDatas:ISerializationCallbackReceiver
    {
        [SerializeField] private List<string> keyList;
        [SerializeField] private List<string> valueList;

        /// <summary>
        /// 存放数据的字典
        /// </summary>
        [NonSerialized]
        public Dictionary<string, string> DataDic;
        public GameSaveDatas()
        {
            DataDic = new Dictionary<string, string>();
        }
        // 序列化的时候把字典里面的内容放进list
        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            OnBeforeSerialize();
        }

        // 反序列化时候自动完成字典的初始化
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            OnAfterDeserialize();
        }
        /// <summary>
        /// Unity序列化前调用
        /// </summary>
        public void OnBeforeSerialize()
        {
            keyList = new List<string>(DataDic.Keys);
            valueList = new List<string>(DataDic.Values);
        }

        /// <summary>
        /// Unity反序列化后调用
        /// </summary>
        public void OnAfterDeserialize()
        {
            DataDic = new Dictionary<string, string>();
            for (int i = 0; i < keyList.Count; i++)
            {
                DataDic.Add(keyList[i], valueList[i]);
            }

            keyList.Clear();
            valueList.Clear();
        }
    }
}

