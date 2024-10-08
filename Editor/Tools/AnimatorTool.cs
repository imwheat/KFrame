//****************** 代码文件申明 ***********************
//* 文件：AnimatorTool
//* 作者：wheat
//* 创建时间：2024/09/28 09:10:29 星期六
//* 描述：动画机相关的工具
//*******************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using KFrame.Utilities;
using UnityEditor;
using UnityEngine;

namespace KFrame.Editor.Tools
{
    public static class AnimatorTool
    {

	    /// <summary>
	    /// Hash字典脚本名称
	    /// </summary>
	    private const string HashDicScriptFileName = "AnimHashDic";
	    
	    /// <summary>
	    /// 更新动画机参数数据
	    /// </summary>
	    [MenuItem("项目工具/更新动画机参数数据")]
        private static void UpdateDatas()
		{
			//查找所有AnimatorController类型的资源
			string[] guids = AssetDatabase.FindAssets("t:AnimatorController"); 
			Dictionary<string, AnimatorParameterData> animatorHashDic = new();
			
			//统计信息
			
			//遍历所有AnimatorController
			List<Task> analyseAnimTask = new List<Task>();
			foreach (string guid in guids)
			{
				string path = AssetDatabase.GUIDToAssetPath(guid);

				//忽略插件内和包和编辑器里的
				if (path.Contains("Plugins") || path.Contains("Packages/") || path.Contains("Editor/")) continue;

				//获取全局路径
				string fullPath = FileExtensions.ConvertAssetPathToSystemPath(path);
				
				Task task = Task.Run(() =>
				{
	
					//如果文件不存在就终止Task
					if (!File.Exists(fullPath)) return;
					
					//读取内容
					string content = File.ReadAllText(fullPath);
					
					//查找对应参数的位置
					int startIndex = content.IndexOf("m_AnimatorParameters:", StringComparison.Ordinal);
					int endIndex = content.IndexOf("m_AnimatorLayers:", StringComparison.Ordinal);
					
					//获取主要文本
					string extractedText = content.Substring(startIndex, endIndex - startIndex);
					//如果参数为空那就结束
					if (extractedText.Contains("m_AnimatorParameters: []"))
					{
						return;
					}
					
					//使用正则表达式来检索内容
					MatchCollection matches = Regex.Matches(extractedText, @"- m_Name: (\w+).*\s+m_Type: (\d+)");
					//遍历符合的数据
					foreach (Match match in matches)
					{
						string name = match.Groups[1].Value;
						int type = int.Parse(match.Groups[2].Value);
						lock (animatorHashDic)
						{
							//添加到字典
							AnimatorParameterData data;
							if (!animatorHashDic.TryGetValue(name, out data))
							{
								data = new AnimatorParameterData(name);
								animatorHashDic[name] = data;
							}
							//更新数据
							AnimatorControllerParameterType pType = (AnimatorControllerParameterType)type;
							if (!data.ReferenceDic.TryGetValue(pType, out var list))
							{
								list = new List<string>();
								data.ReferenceDic[pType] = list;
							}
							list.Add(Path.GetFileNameWithoutExtension(path));
						}
					}
				});
				
				analyseAnimTask.Add(task);
			}
			
			//等待所有Task完成
			Task.WaitAll(analyseAnimTask.ToArray());

			//生成代码
			StringBuilder codeSb = new StringBuilder();
			string space = "\t\t";
			string tab = space + "/// ";
			List<Task> tasks = new List<Task>();
			//生成代码和XML注释 如果data的Name重复的话 在注释上申明引用的动画机名称 和 可能的参数类型
			foreach (var item in animatorHashDic.Values)
			{
				Task dataAnalyse = Task.Run(() =>
				{
					StringBuilder sb = new StringBuilder();

					string paramName = item.Name.ConnectWords();
					sb.Append(tab).AppendLine("<summary>");
					sb.Append(tab).Append("名称: ").AppendLine(item.Name);
					sb.Append(tab).AppendLine("引用动画机");
					foreach (var reference in item.ReferenceDic)
					{
						sb.Append(tab).Append(reference.Key).Append(": ")
							.AppendLine(string.Join(",", reference.Value));
					}
					sb.Append(tab).AppendLine("</summary>");
					sb.Append(space).Append("public static readonly int ").Append(paramName).Append("Hash = Animator.StringToHash(\"").Append(paramName).AppendLine("\");");
					lock (codeSb)
					{
						codeSb.Append(sb);
					}
				});
				
				tasks.Add(dataAnalyse);
			}

			//等待所有进程结束
			Task.WaitAll(tasks.ToArray());

			//更新代码
			ScriptTool.UpdateCode(HashDicScriptFileName, codeSb.ToString(), "\t\t");

			"动画参数Hash生成完毕".Log();
		}
    }
}

