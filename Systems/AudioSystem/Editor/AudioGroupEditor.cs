//****************** 代码文件申明 ***********************
//* 文件：AudioGroupEditor
//* 作者：wheat
//* 创建时间：2024/05/28 14:27:15 星期二
//* 描述：
//*******************************************************

using UnityEngine;
using UnityEditor;
using KFrame;
using KFrame.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using KFrame.Systems;
using KFrame.Attributes;
using System.Text;

namespace KFrame.Editor
{
    /// <summary>
    /// 音效分组的编辑信息
    /// </summary>
    public struct AudioGroupEditData
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

        public AudioGroupEditData(AudioGroup group)
        {
            this.GroupName = group.GroupName;
            this.GroupIndex = group.GroupIndex;
            this.Parent = group.Parent;
            this.ChildrenIndexes = group.ChildrenIndexes;
        }
    }
    public class AudioGroupEditor : KEditorWindow
    {
        #region GUI逻辑

        /// <summary>
        /// 当前正在编辑的Group
        /// </summary>
        private AudioGroup curEditGroup;
        /// <summary>
        /// 原先的数据，取消编辑的时候使用
        /// </summary>
        private AudioGroupEditData oldEditData;
        /// <summary>
        /// 编辑了
        /// </summary>
        private bool editted;

        #endregion

        /// <summary>
        /// 打开编辑器
        /// </summary>
        /// <param name="group"></param>
        internal void OpenEditor(AudioGroup group)
        {
            //如果为空那就返回，不能打开编辑器
            if (group == null) return;

            //获取编辑器，然后初始化打开
            AudioGroupEditor editor = GetWindow<AudioGroupEditor>();
            editor.Init(group);
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private void Init(AudioGroup group)
        {
            //更新赋值
            curEditGroup = group;
        }

        protected override void OnGUI()
        {
            base.OnGUI();
        }
        private void OnDisable()
        {
            //如果有更改过，会弹出提示窗口
            if (editted)
            {
                bool save = EditorUtility.DisplayDialog("是否保存当前更改", "是否保存当前更改", "保存", "取消");
                if (save)
                {
                    SaveEdit();
                }
                else
                {
                    CancelEdit();
                }
            }
        }

        #region 保存

        /// <summary>
        /// 查看保存是否合理
        /// </summary>
        /// <returns></returns>
        private bool CheckSaveVaild()
        {
            bool valid = true;
            StringBuilder sb = new StringBuilder();
            if(string.IsNullOrEmpty(curEditGroup.GroupName))
            {
                sb.Append("音效分组名称不能为空");
            }

            return valid;
        }

        private void SaveEdit()
        {
            //清除更改标记
            ClearEditTag();
        }
        private void CancelEdit()
        {
            //清除更改标记
            ClearEditTag();
        }
        /// <summary>
        /// 设为被更改的状态
        /// </summary>
        private void SetEditted()
        {
            //如果已经是更改过的就返回
            if (editted) return;

            editted = true;
            titleContent.text += "*";
        }
        /// <summary>
        /// 清除编辑标记
        /// </summary>
        private void ClearEditTag()
        {
            //如果没有更改过就返回
            if (!editted) return;

            editted = false;
            //去掉星号标记
            if(titleContent.text.EndsWith('*'))
            {
                titleContent.text = titleContent.text.Substring(0, titleContent.text.Length - 1);
            }
        }

        #endregion

        #region 绘制GUI



        #endregion

    }
}

