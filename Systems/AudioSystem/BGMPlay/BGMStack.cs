//****************** 代码文件申明 ************************
//* 文件：BGMStack                                       
//* 作者：wheat
//* 创建时间：2024/01/20 10:29:43 星期六
//* 描述：存储BGM相关的信息
//*****************************************************

using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace KFrame.Systems
{
    [System.Serializable]
    public class BGMStack
    {
        #region 参数(请不要在游戏运行的时候更改)

        [SerializeField, LabelText("BGMid")] public int BGMIndex;
        [SerializeField, LabelText("BGM名称")] public string BGMName;
        [SerializeField, LabelText("音量")] public float Volume;
        [SerializeField, LabelText("音乐Clip")] public bool Loop;
        [SerializeField, LabelText("音乐Clip")] public int ClipIndex;        

        #endregion

        public BGMStack()
        {
            
        }
        public BGMStack(int bgmIndex, string bgmName, float volume,bool loop, int clipIndex)
        {
            BGMIndex = bgmIndex;
            BGMName = bgmName;
            Volume = volume;
            Loop = loop;
            ClipIndex = clipIndex;
        }
        /// <summary>
        /// 获取播放的Clip
        /// </summary>
        public AudioClip GetClip()
        {
            return AudioDic.GetBGMClip(ClipIndex);
        }
        
    }
}