//****************** 代码文件申明 ***********************
//* 文件：TagLayerTool
//* 作者：wheat
//* 创建时间：2024/09/26 09:17:27 星期四
//* 描述：管理生成游戏内Tag、SortingLayer、Layer的工具
//*******************************************************

using UnityEngine;
using KFrame.Utilities;
using System.Collections.Generic;
using System.Text;

namespace KFrame.Editor
{
    public static class TagLayerTool
    {

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
            }

            //获取一下游戏是不是使用2D物理的
            bool use2DPhysics = FrameSettings.Instance.Use2DPhysics;
            StringBuilder scriptSB = new StringBuilder();
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
                
                //更新库中数据
                TagAndLayerDatas.Instance.UpdateData(dataBase);
                scriptSB.AppendLine(GetLayerParams(dataBase, "        "));
            }
            
            //更新脚本
            ScriptTool.UpdateCode(nameof(Layers),scriptSB.ToString(), "        ");
            
            //保存库
            TagAndLayerDatas.Instance.SaveAsset();
        }

        #endregion

        #region Tag

        /// <summary>
        /// 根据Unity目前的tag状态更新脚本
        /// </summary>
        private static void TagUpdate()
        {
            StringBuilder scriptSB = new StringBuilder();
            
            //获取所有的tag
            string[] tags = UnityEditorInternal.InternalEditorUtility.tags;
            //逐个进行遍历
            foreach (string tag in tags)
            {
                scriptSB.Append("        public static readonly string ").Append(tag).Append(" = \"").Append(tag)
                    .AppendLine("\";");
            }
            
            //更新脚本
            ScriptTool.UpdateCode(nameof(Tags),scriptSB.ToString(), "        ");

        }

        #endregion

        #region SortingLayer

        /// <summary>
        /// 根据Unity目前的SortingLayer状态更新脚本
        /// </summary>
        private static void SortingLayerUpdate()
        {
            StringBuilder scriptSB = new StringBuilder();
            
            //获取所有的tag
            SortingLayer[] sortingLayers = SortingLayer.layers;
            
            //更新脚本
            ScriptTool.UpdateCode(nameof(Tags),scriptSB.ToString(), "        ");

        }

        #endregion
        
    }
}

