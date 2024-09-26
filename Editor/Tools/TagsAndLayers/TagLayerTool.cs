//****************** 代码文件申明 ***********************
//* 文件：TagLayerTool
//* 作者：wheat
//* 创建时间：2024/09/26 09:17:27 星期四
//* 描述：管理生成游戏内Tag、SortingLayer、Layer的工具
//*******************************************************

using UnityEngine;
using UnityEditor;
using KFrame;
using KFrame.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace KFrame.Editor
{
    public static class TagLayerTool
    {
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
                LayerDatas.Instance.UpdateData(dataBase);
            }
            
            //保存库
            LayerDatas.Instance.SaveAsset();
        }
    }
}

