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

namespace KFrame.Systems
{
    [System.Serializable]
    public class AudioGroup
    {
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
        /// 这个分组的父级，会受到父级音量影响
        /// </summary>
        [KLabelText("父级")]
        public AudioGroup Parent;
        /// <summary>
        /// 子集id列表
        /// 用于序列化保存子集
        /// </summary>
        [KLabelText("子集下标")]
        public List<int> ChildrenIndexes;
        /// <summary>
        /// 这个分组的子集
        /// </summary>
        [KLabelText("子集"), NonSerialized]
        public List<AudioGroup> Children;
        /// <summary>
        /// 音量
        /// </summary>
        [KLabelText("音量"), Range(0,1f), NonSerialized]
        public float Volume;
        /// <summary>
        /// 当前音量
        /// </summary>
        [KLabelText("当前音量"), Range(0, 1f)]
        private float curVolume;
        /// <summary>
        /// 当前音量
        /// </summary>
        public float CurVolume => curVolume;
        /// <summary>
        /// 更新音量事件
        /// </summary>
        public Action<float> UpdateVolumeAction;
        public AudioGroup() 
        {
            Volume = 1f;
            Children = new List<AudioGroup>();
            ChildrenIndexes = new List<int>();
        }
        public AudioGroup(string groupName, int groupIndex) : this()
        {
            GroupName = groupName;
            GroupIndex = groupIndex;
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

#if UNITY_EDITOR
        /// <summary>
        /// 绑定父级group
        /// </summary>
        /// <param name="parent"></param>
        public void SetParentGroup(AudioGroup parent)
        {
            //如果现在已经有父级了，先把现在的父级解绑先
            if(Parent != null)
            {
                Parent.ChildrenIndexes.Remove(this.GroupIndex);
            }

            //更新父级
            Parent = parent;
            ChildrenIndexes.Add(this.GroupIndex);
        }
#endif

    }
}

