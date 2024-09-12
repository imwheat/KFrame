//****************** 代码文件申明 ***********************
//* 文件：AudioGroupSelector
//* 作者：wheat
//* 创建时间：2024/05/28 14:40:00 星期二
//* 描述：选择AudioGroup
//*******************************************************

using UnityEngine;
using UnityEditor;
using KFrame;
using KFrame.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using KFrame.Systems;

namespace KFrame.Editor
{
    public class AudioGroupSelector : KDropDownPopupWindow
    {
        public AudioGroupSelector(AudioGroup curGroup,Action<string> callback)
        {
            //筛选选项
            List<AudioGroup> allGroup = new List<AudioGroup>();
            //如果当前传入group不为空
            if(curGroup != null)
            {
                //忽略不计算的Group
                HashSet<AudioGroup> ignoreGroup = new HashSet<AudioGroup>{};
                //忽略所有子集(包括子集的子集)
                Queue<AudioGroup> tmpQueue = new Queue<AudioGroup>();
                tmpQueue.Enqueue(curGroup);
                //通过深度优先所搜逐个添加忽略group
                while (tmpQueue.Count > 0)
                {
                    AudioGroup _group = tmpQueue.Dequeue();
                    ignoreGroup.Add(_group);
                    foreach (var _child in _group.Children)
                    {
                        tmpQueue.Enqueue(_child);
                    }
                }

                //遍历所有group
                foreach (var group in AudioEditor.AudioLibrary.AudioGroups)
                {
                    //如果没有忽略的话就添加
                    if (ignoreGroup.Contains(group)) continue;
                    allGroup.Add(group);
                }
            }
            //如果为空那就都可以选
            else
            {
                allGroup.AddRange(AudioEditor.AudioLibrary.AudioGroups);
            }

            //把所有group添加入选项列表
            List<string> options = new List<string>();
            foreach (var group in allGroup)
            {
                options.Add(group.GroupName);
            }

            //初始化GUI
            InitGUI(options, callback);
        }

        public override void OnGUI(Rect rect)
        {
            base.OnGUI(rect);
        }
    }
}

