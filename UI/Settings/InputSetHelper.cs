//****************** 代码文件申明 ***********************
//* 文件：InputSetHelper
//* 作者：wheat
//* 创建时间：2024/10/05 10:58:18 星期六
//* 描述：辅助一些按键设置
//*******************************************************


using System.Text;
using KFrame.Utilities;
using UnityEngine.InputSystem;

namespace KFrame.UI
{
    public static class InputSetHelper
    {
        #region 参数设置

        /// <summary>
        /// 按键保存的key的前缀
        /// </summary>
        public const string KeySetSaveKeyPrefix = "keySet_";
        /// <summary>
        /// 按键设置数据
        /// </summary>
        private static InputSetSaveData InputData => UISystem.Settings.InputData;

        #endregion

        #region 按键设置
        
        /// <summary>
        /// 获取玩家的按键设置数据
        /// </summary>
        /// <param name="playerIndex">玩家id</param>
        /// <param name="saveKey">保存的key</param>
        /// <returns>按键配置数据</returns>
        public static string GetInputSetData(int playerIndex, string saveKey)
        {
            return InputData.GetPlayerKeySet(playerIndex, saveKey);
        }
        /// <summary>
        /// 设置玩家按键配置data
        /// </summary>
        /// <param name="playerIndex">玩家id</param>
        /// <param name="saveKey">按键保存Key</param>
        /// <param name="jsonData">要设置的数据</param>
        public static void SetPlayerKeySet(int playerIndex, string saveKey, string jsonData)
        {
            InputData.SetPlayerKeySet(playerIndex, saveKey, jsonData);
        }
        /// <summary>
        /// 读取玩家按键设置数据
        /// </summary>
        /// <param name="action">按键事件</param>
        /// <param name="playerIndex">玩家id</param>
        public static void LoadKeySet(this InputAction action, int playerIndex)
        {
            action.LoadBindingOverridesFromJson(GetInputSetData(playerIndex, GetSaveKey(action)));
        }

        #endregion
        
        
        #region 工具方法
        
        /// <summary>
        /// 从绑定路径获取按键文本
        /// </summary>
        /// <param name="bindPath">按键绑定路径</param>
        /// <returns>按键文本</returns>
        public static string GetKeyFromBindPath(this string bindPath)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();
            bool skip = false;
            //遍历路径
            foreach (var c in bindPath)
            {
                //如果遇到'<'那就跳过这段文本直到'>'
                if (c == '<')
                {
                    skip = true;
                    continue;
                }
                if (skip)
                {
                    if (c == '>')
                    {
                        skip = false;
                    }

                    continue;
                }
                //遇到'/'表示隔了一个单词那就调整一下格式，清空一下继续读取下一个单词
                if (c == '/')
                {
                    sb.Append(sb2.ToString().GetNiceFormat());
                    sb2.Clear();
                    continue;
                }

                sb2.Append(c);
            }
            
            sb.Append(sb2.ToString().GetNiceFormat());
            sb2.Clear();
            
            //最后输出文本
            return sb.ToString();
        }
        /// <summary>
        /// 获取保存key
        /// </summary>
        /// <param name="action">按键事件</param>
        /// <returns>保存的key</returns>
        public static string GetSaveKey(this InputAction action)
        {
            return KeySetSaveKeyPrefix + action.name;
        }

        #endregion
    }
}

