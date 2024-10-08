//****************** 代码文件申明 ***********************
//* 文件：TagLayerTool
//* 作者：wheat
//* 创建时间：2024/09/26 09:17:27 星期四
//* 描述：管理生成游戏内Tag、SortingLayer、Layer的工具
//*******************************************************

using System.Collections.Generic;
using System.Text;
using KFrame.Utilities;
using UnityEditor;
using UnityEngine;

namespace KFrame.Editor.Tools
{
    internal static class TagLayerTool
    {
        #region 设置参数

        /// <summary>
        /// 代码空格
        /// </summary>
        private const string Space = "        ";
        /// <summary>
        /// Layer脚本文件名称
        /// </summary>
        private const string LayerScriptFileName = "KLayers";
        /// <summary>
        /// Tag脚本文件名称
        /// </summary>
        private const string TagScriptFileName = "KTags";
        /// <summary>
        /// SortingLayer脚本文件名称
        /// </summary>
        private const string SortingLayerScriptFileName = "KSortingLayers";

        #endregion
        
        /// <summary>
        /// 更新参数
        /// </summary>
        [MenuItem("项目工具/更新Tags&Layers")]
        private static void UpdateParams()
        {
            //更新各个数据
            LayersUpdate();
            TagUpdate();
            SortingLayerUpdate();
            
            //保存数据
            TagAndLayerDatas.Instance.SaveAsset();
        }
        
        #region Layer
        
        /// <summary>
        /// 获取Data的Summary
        /// </summary>
        /// <param name="data">层级数据</param>
        /// <param name="space">每行的空格</param>
        private static string GetLayerSummary(LayerDataBase data, string space)
        {
            //如果为空直接返回空
            if (data == null) return "";
            string tab = space + "/// ";
            //开头
            StringBuilder sb = new StringBuilder();
            sb.Append(tab).Append("<summary>").AppendLine();
            
            //中间内容
            sb.Append(tab).Append("名称: ").Append(data.layerName).AppendLine();
            sb.Append(tab).Append("碰撞图层: ").Append(data.collisionLayer).AppendLine();
            if (!string.IsNullOrEmpty(data.description))
            {
                sb.Append(tab).Append("描述: ").Append(data.description).AppendLine();
            }
            
            //中间内容
            sb.Append(tab).Append("</summary>").AppendLine();

            return sb.ToString();
        }
        /// <summary>
        /// 获取Data的参数
        /// </summary>
        /// <param name="data">层级数据</param>
        /// <param name="space">每行的空格</param>
        private static string GetLayerParams(LayerDataBase data, string space)
        {
            //如果为空直接返回空
            if (data == null) return "";
            StringBuilder sb = new StringBuilder();
            string tab = GetLayerSummary(data, space) + space + "public static readonly ";
            string paramName = data.layerName.ConnectWords();
            sb.Append(tab).Append("string ").Append(paramName).Append("Layer = \"").Append(data.layerName)
                .AppendLine("\";");
            sb.Append(tab).Append("int ").Append(paramName).Append("LayerIndex = LayerMask.NameToLayer(").Append(paramName)
                .AppendLine("Layer);");
            sb.Append(tab).Append("LayerMask ").Append(paramName).Append("LayerMask = LayerMask.GetMask(").Append(paramName)
                .AppendLine("Layer);");

            return sb.ToString();
        }
        /// <summary>
        /// 层级更新
        /// </summary>
        private static void LayersUpdate()
        {
            //记录当前的层级数据
            Dictionary<int, LayerDataBase> newDatas = new Dictionary<int, LayerDataBase>();
            
            int layerCount = 32; //Unity最多支持32个层
            //遍历每个层级
            for (int i = 0; i < layerCount; i++)
            {
                //尝试转化，如果转化失败就跳过
                string layerName = LayerMask.LayerToName(i);
                if (string.IsNullOrEmpty(layerName)) continue;
                
                //转化成功那就新建数据然后添加
                LayerDataBase data = new LayerDataBase(i, layerName);
                newDatas.Add(i, data);
                
                //获取旧的数据
                var prevData = TagAndLayerDatas.Instance.GetLayerData(i);
                //如果为空就跳过，不为空就复制一下旧的数据
                if (prevData == null) continue;
                data.UpdateData(prevData);
            }
            
            
            //清除旧的数据
            TagAndLayerDatas.Instance.layerDatas.Clear();

            //获取一下游戏是不是使用2D物理的
            bool use2DPhysics = FrameSettings.Instance.Use2DPhysics;
            StringBuilder scriptSb = new StringBuilder();
            foreach (LayerDataBase dataBase in newDatas.Values)
            {
                //如果是2d物理的
                if (use2DPhysics)
                {
                    StringBuilder sb = new StringBuilder();
                    //获取记录一下各个图层直接的碰撞关系
                    foreach (LayerDataBase dataBase2 in newDatas.Values)
                    {
                        //如果会碰撞那就记录
                        if (!Physics2D.GetIgnoreLayerCollision(dataBase.layerIndex, dataBase2.layerIndex))
                        {
                            sb.Append(dataBase2.layerName).Append(", ");
                        }
                    }
                    //记录碰撞图层
                    dataBase.collisionLayer = sb.ToString();
                }
                
                //更新代码和库
                scriptSb.AppendLine(GetLayerParams(dataBase, Space));
                TagAndLayerDatas.Instance.layerDatas.Add(dataBase);
            }
            
            //更新脚本如果代码变了
            string newCode = scriptSb.ToString();
            if (string.CompareOrdinal(newCode, TagAndLayerDatas.Instance.prevLayerUpdateCode) != 0)
            {
                TagAndLayerDatas.Instance.prevLayerUpdateCode = newCode;
                ScriptTool.UpdateCode(LayerScriptFileName,scriptSb.ToString(), Space, refresh:false);
            }
            
            //保存库
            TagAndLayerDatas.Instance.InitLayerDic();
        }

        #endregion

        #region Tag

        /// <summary>
        /// 获取Tag的Summary
        /// </summary>
        /// <param name="data">Tag数据</param>
        /// <param name="space">空格</param>
        /// <returns>Tag的Summary</returns>
        private static string GetTagSummary(TagDataBase data, string space)
        {
            //如果为空或者没有描述那就返回空
            if (data == null || string.IsNullOrWhiteSpace(data.description)) return "";

            StringBuilder sb = new StringBuilder();
            string tab = space + "/// ";
            sb.Append(tab).AppendLine("<summary>");
            sb.Append(tab).AppendLine(data.description);
            sb.Append(tab).AppendLine("</summary>");

            return sb.ToString();
        }
        
        /// <summary>
        /// 获取Tag的参数
        /// </summary>
        /// <param name="data">Tag数据</param>
        /// <param name="space">空格</param>
        /// <returns>Tag的参数</returns>
        private static string GetTagParams(TagDataBase data, string space)
        {
            //如果为空那就返回空
            if (data == null) return "";

            StringBuilder sb = new StringBuilder();
            string tab = GetTagSummary(data, space) + space + "public static readonly string ";
            sb.Append(tab).Append(data.tagName).Append(" = \"").Append(data.tagName)
                .AppendLine("\";");
            
            return sb.ToString();
        }
        /// <summary>
        /// 根据Unity目前的tag状态更新脚本
        /// </summary>
        private static void TagUpdate()
        {
            List<TagDataBase> newTags = new List<TagDataBase>();
            StringBuilder scriptSb = new StringBuilder();
            
            //获取所有的tag
            string[] tags = UnityEditorInternal.InternalEditorUtility.tags;
            //逐个进行遍历
            foreach (string tag in tags)
            {
                //创建新的数据
                TagDataBase data = new TagDataBase(tag);
                var prevData = TagAndLayerDatas.Instance.GetTagData(tag);
                if (prevData != null)
                {
                    data.UpdateData(prevData);
                }
                
                //获取脚本文本
                scriptSb.Append(GetTagParams(data, Space));
                newTags.Add(data);
            }
            
            //更新脚本如果代码变了
            string newCode = scriptSb.ToString();
            if (string.CompareOrdinal(newCode, TagAndLayerDatas.Instance.prevTagUpdateCode) != 0)
            {
                TagAndLayerDatas.Instance.prevTagUpdateCode = newCode;
                ScriptTool.UpdateCode(TagScriptFileName,scriptSb.ToString(), Space, refresh:false);
            }
            
            //保存库
            TagAndLayerDatas.Instance.tagDatas = newTags;
            TagAndLayerDatas.Instance.InitTagDic();
        }

        #endregion

        #region SortingLayer
        
        /// <summary>
        /// 获取Data的Summary
        /// </summary>
        /// <param name="data">层级数据</param>
        /// <param name="space">每行的空格</param>
        private static string GetSortingLayerSummary(SortingLayerDataBase data, string space)
        {
            //如果为空直接返回空
            if (data == null) return "";
            string tab = space + "/// ";
            //开头
            StringBuilder sb = new StringBuilder();
            sb.Append(tab).Append("<summary>").AppendLine();
            
            //中间内容
            sb.Append(tab).Append("名称: ").Append(data.layerName).AppendLine();
            if (!string.IsNullOrEmpty(data.description))
            {
                sb.Append(tab).Append("描述: ").Append(data.description).AppendLine();
            }
            
            //中间内容
            sb.Append(tab).Append("</summary>").AppendLine();

            return sb.ToString();
        }

        /// <summary>
        /// 获取Data的参数
        /// </summary>
        /// <param name="data">层级数据</param>
        /// <param name="space">每行的空格</param>
        private static string GetSortingLayerParams(SortingLayerDataBase data, string space)
        {
            //如果为空直接返回空
            if (data == null) return "";
            StringBuilder sb = new StringBuilder();
            string tab = GetSortingLayerSummary(data, space) + space + "public static readonly ";
            string paramName = data.layerName.ConnectWords();
            sb.Append(tab).Append("int ").Append(paramName).Append("LayerIndex = ")
                .Append(data.layerIndex)
                .AppendLine(";");
            sb.Append(tab).Append("string ").Append(paramName).Append("LayerName = \"")
                .Append(data.layerName)
                .AppendLine("\";");

            return sb.ToString();
        }

        /// <summary>
        /// 根据Unity目前的SortingLayer状态更新脚本
        /// </summary>
        private static void SortingLayerUpdate()
        {
            List<SortingLayerDataBase> newSortingLayers = new();
            //清除旧的数据
            TagAndLayerDatas.Instance.sortingLayerDatas.Clear();
            
            StringBuilder scriptSb = new StringBuilder();
            
            //获取所有的tag
            SortingLayer[] sortingLayers = SortingLayer.layers;

            foreach (SortingLayer layer in sortingLayers)
            {
                SortingLayerDataBase data = new SortingLayerDataBase(layer.id, layer.name);
                newSortingLayers.Add(data);
                var prevData = TagAndLayerDatas.Instance.GetSortingLayerData(layer.id);
                if (prevData != null)
                {
                    data.UpdateData(prevData);
                }
                //更叫代码
                scriptSb.Append(GetSortingLayerParams(data, Space));
            }
            
            //更新脚本如果代码变了
            string newCode = scriptSb.ToString();
            if (string.CompareOrdinal(newCode, TagAndLayerDatas.Instance.prevSortingLayerUpdateCode) != 0)
            {
                TagAndLayerDatas.Instance.prevSortingLayerUpdateCode = newCode;
                ScriptTool.UpdateCode(SortingLayerScriptFileName,scriptSb.ToString(), Space, refresh:false);
            }
            
            //保存库
            TagAndLayerDatas.Instance.sortingLayerDatas = newSortingLayers;
            TagAndLayerDatas.Instance.InitSortingLayerDic();
        }

        #endregion
        
    }
}

