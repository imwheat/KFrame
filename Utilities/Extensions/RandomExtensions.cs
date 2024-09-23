using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace KFrame.Utilities
{
    /// <summary>
    /// 基于List随机工具 返回int类型 随机索引 可设置权重
    /// 使用方法 new 一个RandomTool后 使用SetWeightByIndex方法来设置索引和权重值
    /// 通过GetRandomIndex 来获得随机索引
    /// </summary>
    public class RandomToolByList
    {
        /// <summary>
        /// 权重
        /// </summary>
        private List<int> weightList = new();

        /// <summary>
        /// 权重区间
        /// </summary>
        private List<int> weightSectionList = new List<int>();

        /// <summary>
        /// 总权重
        /// </summary>
        private int totalWeight;

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            totalWeight = 0;
            weightList.Clear();
            weightSectionList.Clear();
        }


        /// <summary>
        /// 更新总权重与权重区间
        /// </summary>
        /// <returns>返回总权重</returns>
        private int UpdateTotalWeightInList()
        {
            totalWeight = 0;
            foreach (var weight in weightList)
            {
                totalWeight += weight;
            }

            //更新权重区间
            int count = weightList.Count; //权重字典数量
            for (int i = 1; i <= count; i++)
            {
                int id = i - 1;
                int tempWeight = 0;
                for (int j = 0; j < i; j++)
                {
                    //比如 id1的 权重是10 id2是20, id3是30  id1的权重区间为0-10  id2 为 10-30 id3 为 30-60
                    //所以这里i从1开始而不是从0开始
                    tempWeight += weightList[j];
                }

                weightSectionList[id] = tempWeight;
            }

            return totalWeight;
        }


        /// <summary>
        /// 根据int类型索引设置权重 性能高于字典方式
        /// </summary>
        /// <param name="index">权重索引</param>
        /// <param name="weight">权重</param>
        public void SetWeightByIndex(int index, int weight)
        {
            weightList[index] = weight;
            //更新权重
            UpdateTotalWeightInList();
        }

        /// <summary>
        /// 根据权重得到随机索引
        /// </summary>
        /// <returns></returns>
        public int GetRandomIndex()
        {
            //这里加1的原因是 Range是前闭后开
            int randomNum = Random.Range(0, totalWeight + 1);
            for (int i = 0; i < weightSectionList.Count; i++)
            {
                int temp = weightSectionList[i];
                //随机获取一个数值的时候 将随机值和权重字典中的任意一个值进行比较
                if (randomNum <= temp)
                {
                    //Debug.Log(string.Format("随机数字：{0}，随机结果：{1}", ranNum, weightDict.ElementAt(i).Key));
                    return i;
                }
            }

            return 0;
        }
    }

    /// <summary>
    /// 基于字典随机工具 返回string类型 随机索引 可设置权重
    /// 使用方法 new 一个RandomTool后 使用SetWeightByIndex方法来设置索引和权重值
    /// 通过GetRandomIndex 来获得随机索引
    /// </summary>
    public class RandomToolByDic
    {
        /// <summary>
        /// 权重字典
        /// </summary>
        private Dictionary<string, int> weightDic = new();

        /// <summary>
        /// 权重区间字典
        /// </summary>
        private Dictionary<string, int> weightSectionDic = new();

        /// <summary>
        /// 总权重
        /// </summary>
        private int totalWeight;

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            totalWeight = 0;
            weightDic.Clear();
            weightSectionDic.Clear();
        }

        /// <summary>
        /// 更新总权重
        /// </summary>
        private int UpdateTotalWeightInDic()
        {
            totalWeight = 0;
            foreach (var weight in weightDic.Values)
            {
                totalWeight += weight;
            }

            //更新权重区间
            int count = weightDic.Count;
            for (int i = 1; i <= count; i++)
            {
                string id = weightDic.ElementAt(i - 1).Key;
                int tempWeight = 0;
                for (int j = 0; j < i; j++)
                {
                    //比如 id1的 权重是10 id2是20, id3是30  id1的权重区间为0-10  id2 为 10-30 id3 为 30-60
                    //所以这里i从1开始而不是从0开始
                    tempWeight += weightDic.ElementAt(j).Value;
                }

                weightSectionDic.Add(id, tempWeight);
            }

            return totalWeight;
        }

        /// <summary>
        /// 设置索引名称与权重
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="weight">权重</param>
        public void SetWeightByIndex(string index, int weight)
        {
            weightDic[index] = weight;
            //更新权重
            UpdateTotalWeightInDic();
        }

        /// <summary>
        /// 得到随机的string id
        /// </summary>
        private string GetRanId()
        {
            int ranNum = Random.Range(0, totalWeight + 1);
            for (int i = 0; i < weightSectionDic.Count; i++)
            {
                int temp = weightSectionDic.ElementAt(i).Value;
                if (ranNum <= temp)
                {
                    //Debug.Log(string.Format("随机数字：{0}，随机结果：{1}", ranNum, weightDict.ElementAt(i).Key));
                    return weightDic.ElementAt(i).Key;
                }
            }

            return "";
        }
    }


    /// <summary>
    /// 随机工具
    /// </summary>
    public static class RandomExtensions
    {
        #region Random静态拓展

        /// <summary>
        /// 从给定的一组值中随机选择一个值并返回。
        /// </summary>
        /// <typeparam name="T">值的类型。</typeparam>
        /// <param name="values">可选的值集合。</param>
        /// <returns>随机选择的值。</returns>
        public static T GetRandomValueFrom<T>(this T[] values)
        {
            // 如果没有传递任何值，返回默认值
            if (values.Length == 0)
            {
                return default(T);
            }

            // 生成一个随机索引
            int randomIndex = Random.Range(0, values.Length);

            // 返回随机索引对应的值
            return values[randomIndex];
        }

        /// <summary>
        /// 从给定的集合中随机选择一个值并返回。
        /// </summary>
        /// <typeparam name="T">值的类型。</typeparam>
        /// <param name="collection">可选的集合。</param>
        /// <returns>随机选择的值。</returns>
        public static T GetRandomValueFrom<T>(this IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            // 将集合转换为数组，以便获取元素个数和随机索引
            T[] values = collection as T[] ?? collection.ToArray();

            if (values.Length == 0)
            {
                return default(T);
            }

            int randomIndex = UnityEngine.Random.Range(0, values.Length);

            return values[randomIndex];
        }


        /// <summary>
        /// 获得一个随机方向的二维单位向量
        /// </summary>
        /// <returns></returns>
        public static Vector2 GetRandomUnitVector2()
        {
            return Random.insideUnitCircle;
        }
        /// <summary>
        /// 在范围内随机获取一个值[x,y)
        /// </summary>
        /// <param name="range">范围</param>
        /// <returns>一个在range范围内的值[x,y)</returns>
        public static float GetRandomValue(this Vector2 range)
        {
            float min, max;
            if (range.x < range.y)
            {
                min = range.x;
                max = range.y;
            }
            else
            {
                min = range.y;
                max = range.x;
            }

            return Random.Range(min, max);
        }
        /// <summary>
        /// 在范围内随机获取一个值[x,y)
        /// </summary>
        /// <param name="range">范围</param>
        /// <returns>一个在range范围内的值[x,y)</returns>
        public static float GetRandomValue(this Vector2Int range)
        {
            int min, max;
            if (range.x < range.y)
            {
                min = range.x;
                max = range.y;
            }
            else
            {
                min = range.y;
                max = range.x;
            }

            return Random.Range(min, max);
        }
        /// <summary>
        /// 随机获取一个list里的下标
        /// </summary>
        /// <param name="list">获取对象</param>
        /// <returns>随机一个下标，如果传入list为空返回-1</returns>
        public static int GetRandomIndex(this IList list)
        {
            if (list == null) return -1;
            return Random.Range(0, list.Count);
        }

        #endregion
    }
}