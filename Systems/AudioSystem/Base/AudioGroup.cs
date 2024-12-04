//****************** 代码文件申明 ***********************
//* 文件：AudioGroup
//* 作者：wheat
//* 创建时间：2024/04/29 07:40:51 星期一
//* 描述：音效的分组
//*******************************************************

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using KFrame.Attributes;
using UnityEngine.Audio;

namespace KFrame.Systems
{
    [System.Serializable]
    public class AudioGroup
    {
        #region 配置参数

        /// <summary>
        /// 分组名称
        /// </summary>
        [KLabelText("分组名称")]
        public string GroupName;
        /// <summary>
        /// 分组id
        /// </summary>
        [KLabelText("分组id")]
        public int GroupIndex;
        /// <summary>
        /// 子集id列表
        /// 用于序列化保存子集
        /// </summary>
        [KLabelText("子集下标")]
        public List<int> ChildrenIndexes;
        
        #endregion

        #region 参数

        /// <summary>
        /// 这个分组的父级，会受到父级音量影响
        /// </summary>
        [KLabelText("父级"), NonSerialized]
        public AudioGroup Parent;
        /// <summary>
        /// 这个分组的子集
        /// </summary>
        [KLabelText("子集"), NonSerialized]
        public List<AudioGroup> Children;
        /// <summary>
        /// 自身音量
        /// </summary>
        [KLabelText("音量"), Range(0,1f), NonSerialized]
        public float Volume;
        /// <summary>
        /// 当前音量值
        /// 会受到父级影响
        /// </summary>
        [KLabelText("当前音量"), Range(0, 1f)]
        private float curVolume =1f;
        /// <summary>
        /// 当前音量值
        /// 会受到父级影响
        /// </summary>
        public float CurVolume => curVolume;
        /// <summary>
        /// 更新音量事件
        /// </summary>
        public Action<float> UpdateVolumeAction;
        
        #endregion

        #region 构造函数

        public AudioGroup() 
        {
            Volume = 1f;
            Children = new List<AudioGroup>();
            ChildrenIndexes = new List<int>();
        }
        public AudioGroup(string groupName) : this()
        {
            GroupName = groupName;
        }
        public AudioGroup(string groupName, int groupIndex) : this()
        {
            GroupName = groupName;
            GroupIndex = groupIndex;
        }

        #endregion

        #region 音量调节

        /// <summary>
        /// 初始化音量设置
        /// </summary>
        /// <param name="v"></param>
        public void InitVolume(float v)
        {
            Volume = v;
            curVolume = v;
        }
        /// <summary>
        /// 更新音量
        /// </summary>
        public void UpdateVolume()
        {
            //更新音量
            curVolume = GetVolume();
            //调用更新音量事件
            UpdateVolumeAction?.Invoke(CurVolume);
        }
        /// <summary>
        /// 更新音量
        /// </summary>
        /// <param name="v">更新的音量值</param>
        public void UpdateVolume(float v)
        {
            Volume = v;
            UpdateVolume();
        }
        /// <summary>
        /// 获取音量
        /// </summary>
        /// <returns></returns>
        private float GetVolume()
        {
            //首先获取当前的音量
            float volume = Volume;

            //如果有父级再乘上父级的音量
            AudioGroup p = Parent;
            while (p != null)
            {
                volume *= p.Volume;
                p = p.Parent;
            }

            //返回音量
            return volume;
        }

        #endregion

        #region 参数配置获取

        /// <summary>
        /// 获取MixerGroup
        /// </summary>
        public AudioMixerGroup GetMixerGroup()
        {
            return AudioDic.GetAudioMixerGroup(GroupIndex);
        }
        /// <summary>
        /// 初始化的时候配置Child使用
        /// </summary>
        public void AddChild(int index)
        {
            //获取子集
            var group = AudioDic.GetAudioGroup(index);
            if(group == null) return;
            //设置父级，然后添加到列表里
            group.Parent = this;
            Children.Add(group);
        }

        #endregion

        #region 编辑器使用

                
#if UNITY_EDITOR
        /// <summary>
        /// 绑定父级group
        /// </summary>
        /// <param name="parent"></param>
        public void SetParentGroup(AudioGroup parent)
        {
            //如果现在已经有父级了，先把现在的父级解绑先
            Parent?.ChildrenIndexes.Remove(GroupIndex);

            //更新父级
            Parent = parent;
            Parent.ChildrenIndexes.Add(GroupIndex);
        }
#endif

        #endregion


    }
}

