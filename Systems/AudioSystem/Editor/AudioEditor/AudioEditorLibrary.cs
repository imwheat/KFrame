//****************** 代码文件申明 ************************
//* 文件：AudioEditorLibrary                                       
//* 作者：wheat
//* 创建时间：2024/09/11 18:13:20 星期三
//* 描述：负责存储和管理编辑器中音效编辑器需要的数据
//*****************************************************
using System.Collections;
using System.Collections.Generic;
using KFrame.Attributes;
using KFrame.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KFrame.Systems
{
    [KGlobalConfigPath(GlobalPathType.Editor, typeof(AudioEditorLibrary), true)]
    public class AudioEditorLibrary : GlobalConfigBase<AudioEditorLibrary>
    {
        [LabelText("音效模版"), FoldoutGroup("信息浏览")] public List<AudioEditData> AudioTemplates;
        
        /// <summary>
        /// 保存音效模版
        /// </summary>
        /// <param name="data">要保存的数据</param>
        /// <param name="templateName">模版名称</param>
        public void SaveAudioTemplate(AudioEditData data, string templateName)
        {
            AudioEditData template = new AudioEditData(data);
            template.AudioName = templateName;
            AudioTemplates.Add(template);
        }
    }
}

