//****************** 代码文件申明 ************************
//* 文件：InputSaveData                                       
//* 作者：wheat
//* 创建时间：2024/03/09 14:55:44 星期六
//* 描述：按键设置配置
//*****************************************************
using System;
using KFrame.Utilities;

namespace KFrame.Systems
{
    [System.Serializable]
    public class InputSaveData
    {
        public readonly Serialized_Dic<int, Serialized_Dic<string, string>> JsonDic = new();

        /// <summary>
        /// 获取玩家按键配置data
        /// </summary>
        /// <param name="playerIndex">玩家id</param>
        /// <param name="key">按键保存Key</param>
        /// <returns>如果找到的话那就返回对应按键的保存data，找不到的话那就返回空</returns>
        public string GetPlayerKeySet(int playerIndex, string key)
        {
            if(JsonDic.Dictionary.ContainsKey(playerIndex) && JsonDic.Dictionary[playerIndex].Dictionary.ContainsKey(key))
            {
                return JsonDic.Dictionary[playerIndex].Dictionary[key];
            }

            return "";
        }
        /// <summary>
        /// 获取玩家键盘按键配置的文本提示
        /// </summary>
        /// <param name="playerIndex">玩家id</param>
        /// <param name="key">按键保存Key</param>
        /// <returns>如果找到的话那就返回对应按键的保存data，找不到的话那就返回空</returns>
        public string GetInputKeyboardSet(int playerIndex, string key)
        {
            string data = GetPlayerKeySet(playerIndex, key);

            //找到键盘按键配置
            int i = data.IndexOf("<Keyboard>/", StringComparison.Ordinal);
            //如果没有就返回空
            if (i == -1)
            {
                return "";
            }
            else
            {
                i += "<Keyboard>/".Length;

                //获取文本
                string res = "";
                for (int j = i; j < data.Length; j++)
                {
                    if (data[j] == '"')
                    {
                        break;
                    }
                    res += data[j].ToString();
                }

                return res;
            }
        }

        /// <summary>
        /// 设置玩家按键配置data
        /// </summary>
        /// <param name="playerIndex">玩家id</param>
        /// <param name="key">按键保存Key</param>
        /// <param name="value">参数</param>
        public void SetPlayerKeySet(int playerIndex, string key, string value)
        {
            if (JsonDic.Dictionary.ContainsKey(playerIndex) == false)
            {
                JsonDic.Dictionary.Add(playerIndex, new Serialized_Dic<string, string>());
            }

            JsonDic.Dictionary[playerIndex].Dictionary[key] = value;
        }
    }
}

