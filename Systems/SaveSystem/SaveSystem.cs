//****************** 代码文件申明 ************************
//* 文件：SaveSystem                      
//* 作者：wheat
//* 创建时间：2023年09月03日 星期日 20:35
//* 描述：存档系统
//*****************************************************


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using KFrame.Extensions;

namespace KFrame.Systems
{
    public interface IBinarySerializer
    {
        public byte[] Serialize<T>(T obj) where T : class;
        public T Deserialize<T>(byte[] bytes) where T : class;
    }

    public static class SaveSystem
    {
        #region 存档系统、存档系统数据类及所有用户存档、设置存档数据

        /// <summary>
        /// 存档系统数据类
        /// </summary>
        [System.Serializable]
        private class SaveSystemData
        {
            // 当前的存档ID
            public int currID = 0;

            // 所有存档的列表
            public List<SaveItem> saveItemList = new List<SaveItem>();
        }

        private static SaveSystemData saveSystemData;

        // 存档的保存
        private const string saveDirName = "saveData";

        // 设置的保存：1.全局数据的保存（分辨率、按键设置） 2.存档的设置保存。
        // 常规情况下，存档系统自行维护
        private const string settingDirName = "setting";

        // 存档文件夹路径
        public static string SaveDirPath;
        private static string settingDirPath;

        // 存档中对象的缓存字典 
        // <存档ID,<文件名称，实际的对象>>
        private static Dictionary<int, Dictionary<string, object>> cacheDic =
            new Dictionary<int, Dictionary<string, object>>();


        public static IBinarySerializer binarySerializer;


#if UNITY_EDITOR
        private static bool inited;
        static SaveSystem()
        {
            Init();
        }
#endif

        // 初始化的事情
        public static void Init()
        {
            SaveDirPath = Application.persistentDataPath + "/" + saveDirName;
            settingDirPath = Application.persistentDataPath + "/" + settingDirName;
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode || inited)
            {
                return;
            }
#endif
            CheckAndCreateDir();
            // 初始化SaveSystemData
            InitSaveSystemData();

#if UNITY_EDITOR

            inited = true;
            // 避免Editor环境下使用了上一次运行的缓存
            CleanCache();
#endif
        }

        #endregion

        #region 获取、删除所有用户存档

        /// <summary>
        /// 获取所有存档
        /// 最新的在最后面
        /// </summary>
        /// <returns></returns>
        public static List<SaveItem> GetAllSaveItem()
        {
            return saveSystemData.saveItemList;
        }

        /// <summary>
        /// 获取所有存档
        /// 创建最新的在最前面
        /// </summary>
        /// <returns></returns>
        public static List<SaveItem> GetAllSaveItemByCreatTime()
        {
            List<SaveItem> saveItems = new List<SaveItem>(saveSystemData.saveItemList.Count);

            for (int i = 0; i < saveSystemData.saveItemList.Count; i++)
            {
                saveItems.Add(saveSystemData.saveItemList[saveSystemData.saveItemList.Count - (i + 1)]);
            }

            return saveItems;
        }

        /// <summary>
        /// 获取所有存档
        /// 最新更新的在最上面
        /// </summary>
        /// <returns></returns>
        public static List<SaveItem> GetAllSaveItemByUpdateTime()
        {
            List<SaveItem> saveItems = new List<SaveItem>(saveSystemData.saveItemList.Count);
            for (int i = 0; i < saveSystemData.saveItemList.Count; i++)
            {
                saveItems.Add(saveSystemData.saveItemList[i]);
            }

            OrderByUpdateTimeComparer orderBy = new OrderByUpdateTimeComparer();
            saveItems.Sort(orderBy);
            return saveItems;
        }

        /// <summary>
        /// 获取所有存档
        /// 最新id小的在最上面
        /// </summary>
        /// <returns></returns>
        public static List<SaveItem> GetAllSaveItemBySaveId()
        {
            List<SaveItem> saveItems = new List<SaveItem>(saveSystemData.saveItemList.Count);
            for (int i = 0; i < saveSystemData.saveItemList.Count; i++)
            {
                saveItems.Add(saveSystemData.saveItemList[i]);
            }

            saveItems.Sort((a, b) => a.saveID - b.saveID);
            return saveItems;
        }

        private class OrderByUpdateTimeComparer : IComparer<SaveItem>
        {
            public int Compare(SaveItem x, SaveItem y)
            {
                if (x.LastSaveTime > y.LastSaveTime)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
        }

        /// <summary>
        /// 获取所有存档
        /// 万能解决方案
        /// </summary>
        public static List<SaveItem> GetAllSaveItem<T>(Func<SaveItem, T> orderFunc, bool isDescending = false)
        {
            if (isDescending)
            {
                return saveSystemData.saveItemList.OrderByDescending(orderFunc).ToList();
            }
            else
            {
                return saveSystemData.saveItemList.OrderBy(orderFunc).ToList();
            }
        }

        public static void DeleteAllSaveItem()
        {
            if (Directory.Exists(SaveDirPath))
            {
                // 直接删除目录
                Directory.Delete(SaveDirPath, true);
            }

            CheckAndCreateDir();
            InitSaveSystemData();
        }


        public static void DeleteAll()
        {
            CleanCache();
            DeleteAllSaveItem();
            DeleteAllSetting();
        }

        #endregion

        #region 创建、获取、删除某一项用户存档

        /// <summary>
        /// 获取SaveItem
        /// </summary>
        public static SaveItem GetSaveItem(int id)
        {
            for (int i = 0; i < saveSystemData.saveItemList.Count; i++)
            {
                if (saveSystemData.saveItemList[i].saveID == id)
                {
                    return saveSystemData.saveItemList[i];
                }
            }

            return null;
        }

        /// <summary>
        /// 获取SaveItem
        /// </summary>
        public static SaveItem GetSaveItem(SaveItem saveItem)
        {
            GetSaveItem(saveItem.saveID);
            return null;
        }

        /// <summary>
        /// 添加一个存档
        /// </summary>
        /// <returns></returns>
        public static SaveItem CreateSaveItem()
        {
            if (saveSystemData.saveItemList.Count == 0)
            {
                saveSystemData.currID = 0;
            }
            else
            {
                for (int i = 0; i < saveSystemData.saveItemList.Count; i++)
                {
                    if (GetSaveItem(i) == null)
                    {
                        saveSystemData.currID = i;
                        break;
                    }
                    else
                    {
                        saveSystemData.currID = i + 1;
                    }
                }
            }

            SaveItem saveItem = new SaveItem(saveSystemData.currID, DateTime.Now);
            saveSystemData.saveItemList.Add(saveItem);
            saveSystemData.currID += 1;
            // 更新SaveSystemData 写入磁盘
            UpdateSaveSystemData();
            return saveItem;
        }

        /// <summary>
        /// 添加一个指定ID的存档
        /// </summary>
        /// <param name="saveID">指定的ID</param>
        /// <returns></returns>
        public static SaveItem CreateSaveItem(int saveID)
        {
            SaveItem saveItem = new SaveItem(saveSystemData.currID, DateTime.Now);
            saveSystemData.saveItemList.Add(saveItem);
            //更新存档的ID
            saveSystemData.currID = saveID;
            // 更新SaveSystemData 写入磁盘
            UpdateSaveSystemData();
            return saveItem;
        }

        /// <summary>
        /// 删除存档
        /// </summary>
        /// <param name="saveID">存档的ID</param>
        public static void DeleteSaveItem(int saveID)
        {
            string itemDir = GetSavePath(saveID, false);
            // 如果路径存在 且 有效
            if (itemDir != null)
            {
                // 把这个存档下的文件递归删除
                Directory.Delete(itemDir, true);
            }

            saveSystemData.saveItemList.Remove(GetSaveItem(saveID));
            // 移除缓存
            RemoveCache(saveID);
            // 更新SaveSystemData 写入磁盘
            UpdateSaveSystemData();
        }

        /// <summary>
        /// 删除存档
        /// </summary>
        public static void DeleteSaveItem(SaveItem saveItem)
        {
            DeleteSaveItem(saveItem.saveID);
        }

        #endregion

        #region 更新、获取、删除用户存档缓存

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="saveID">存档ID</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="saveObject">要缓存的对象</param>
        private static void SetCache(int saveID, string fileName, object saveObject)
        {
            // 缓存字典中是否有这个SaveID
            if (cacheDic.ContainsKey(saveID))
            {
                // 这个存档中有没有这个文件
                if (cacheDic[saveID].ContainsKey(fileName))
                {
                    cacheDic[saveID][fileName] = saveObject;
                }
                else
                {
                    cacheDic[saveID].Add(fileName, saveObject);
                }
            }
            else
            {
                cacheDic.Add(saveID, new Dictionary<string, object>() { { fileName, saveObject } });
            }
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="saveID">存档ID</param>
        /// <param name="saveObject">要缓存的对象</param>
        private static T GetCache<T>(int saveID, string fileName) where T : class
        {
            // 缓存字典中是否有这个SaveID
            if (cacheDic.ContainsKey(saveID))
            {
                // 这个存档中有没有这个文件
                if (cacheDic[saveID].ContainsKey(fileName))
                {
                    return cacheDic[saveID][fileName] as T;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        private static void RemoveCache(int saveID)
        {
            cacheDic.Remove(saveID);
        }

        /// <summary>
        /// 移除缓存中的某一个对象
        /// </summary>
        private static void RemoveCache(int saveID, string fileName)
        {
            cacheDic[saveID].Remove(fileName);
        }

        public static void CleanCache()
        {
            cacheDic.Clear();
        }

        #endregion

        #region 保存、获取、删除用户存档中某一对象

        /// <summary>
        /// 保存对象至某个存档中
        /// </summary>
        /// <param name="saveObject">要保存的对象</param>
        /// <param name="saveFileName">保存的文件名称</param>
        /// <param name="saveID">存档的ID</param>
        public static void SaveObject(object saveObject, string saveFileName, int saveID = 0)
        {
            // 存档所在的文件夹路径
            string dirPath = GetSavePath(saveID, true);
            // 具体的对象要保存的路径
            string savePath = dirPath + "/" + saveFileName;
            // 具体的保存
            SaveFile(saveObject, savePath);
            // 更新存档时间
            GetSaveItem(saveID).UpdateTime(DateTime.Now);
            // 更新SaveSystemData 写入磁盘
            UpdateSaveSystemData();

            // 更新缓存
            SetCache(saveID, saveFileName, saveObject);
        }


        /// <summary>
        /// 保存对象至某个存档中
        /// </summary>
        /// <param name="saveObject">要保存的对象</param>
        /// <param name="saveFileName">保存的文件名称</param>
        public static void SaveObject(object saveObject, string saveFileName, SaveItem saveItem)
        {
            SaveObject(saveObject, saveFileName, saveItem.saveID);
        }

        /// <summary>
        /// 保存对象至某个存档中
        /// </summary>
        /// <param name="saveObject">要保存的对象</param>
        /// <param name="saveID">存档的ID</param>
        public static void SaveObject(object saveObject, int saveID = 0)
        {
            SaveObject(saveObject, saveObject.GetType().Name, saveID);
        }

        /// <summary>
        /// 保存对象至某个存档中
        /// </summary>
        /// <param name="saveObject">要保存的对象</param>
        /// <param name="saveID">存档的ID</param>
        public static void SaveObject(object saveObject, SaveItem saveItem)
        {
            SaveObject(saveObject, saveObject.GetType().Name, saveItem);
        }

        /// <summary>
        /// 从某个具体的存档中加载某个对象
        /// </summary>
        /// <typeparam name="T">要返回的实际类型</typeparam>
        /// <param name="saveFileName">文件名称</param>
        /// <param name="id">存档ID</param>
        public static T LoadObject<T>(string saveFileName, int saveID = 0) where T : class
        {
            T obj = GetCache<T>(saveID, saveFileName);
            if (obj == null)
            {
                // 存档所在的文件夹路径
                string dirPath = GetSavePath(saveID);
                if (dirPath == null) return null;
                // 具体的对象要保存的路径
                string savePath = dirPath + "/" + saveFileName;
                obj = LoadFile<T>(savePath);
                SetCache(saveID, saveFileName, obj);
            }

            return obj;
        }

        /// <summary>
        /// 从某个具体的存档中加载某个对象
        /// </summary>
        /// <typeparam name="T">要返回的实际类型</typeparam>
        /// <param name="saveFileName">文件名称</param>
        public static T LoadObject<T>(string saveFileName, SaveItem saveItem) where T : class
        {
            return LoadObject<T>(saveFileName, saveItem.saveID);
        }


        /// <summary>
        /// 从某个具体的存档中加载某个对象
        /// </summary>
        /// <typeparam name="T">要返回的实际类型</typeparam>
        /// <param name="id">存档ID</param>
        public static T LoadObject<T>(int saveID = 0) where T : class
        {
            return LoadObject<T>(typeof(T).Name, saveID);
        }

        /// <summary>
        /// 从某个具体的存档中加载某个对象
        /// </summary>
        /// <typeparam name="T">要返回的实际类型</typeparam>
        /// <param name="saveItem">存档项</param>
        public static T LoadObject<T>(SaveItem saveItem) where T : class
        {
            return LoadObject<T>(typeof(T).Name, saveItem.saveID);
        }

        /// <summary>
        /// 删除某个存档中的某个对象
        /// </summary>
        /// <param name="saveID">存档的ID</param>
        public static void DeleteObject<T>(string saveFileName, int saveID) where T : class
        {
            //清空缓存中对象
            if (GetCache<T>(saveID, saveFileName) != null)
            {
                RemoveCache(saveID, saveFileName);
            }

            // 存档对象所在的文件路径
            string dirPath = GetSavePath(saveID);
            string savePath = dirPath + "/" + saveFileName;
            //删除对应的文件
            File.Delete(savePath);
        }

        /// <summary>
        /// 删除某个存档中的某个对象
        /// </summary>
        /// <param name="saveID">存档的ID</param>
        public static void DeleteObject<T>(string saveFileName, SaveItem saveItem) where T : class
        {
            DeleteObject<T>(saveFileName, saveItem.saveID);
        }

        /// <summary>
        /// 删除某个存档中的某个对象
        /// </summary>
        /// <param name="saveID">存档的ID</param>
        public static void DeleteObject<T>(int saveID) where T : class
        {
            DeleteObject<T>(typeof(T).Name, saveID);
        }

        /// <summary>
        /// 删除某个存档中的某个对象
        /// </summary>
        /// <param name="saveID">存档的ID</param>
        public static void DeleteObject<T>(SaveItem saveItem) where T : class
        {
            DeleteObject<T>(typeof(T).Name, saveItem.saveID);
        }

        #endregion

        #region 保存、获取全局设置存档

        /// <summary>
        /// 加载设置，全局生效，不关乎任何一个存档
        /// </summary>
        public static T LoadSetting<T>(string fileName) where T : class
        {
            return LoadFile<T>(settingDirPath + "/" + fileName);
        }

        /// <summary>
        /// 加载设置，全局生效，不关乎任何一个存档
        /// </summary>
        public static T LoadSetting<T>() where T : class
        {
            return LoadSetting<T>(typeof(T).Name);
        }

        /// <summary>
        /// 保存设置，全局生效，不关乎任何一个存档
        /// </summary>
        public static void SaveSetting(object saveObject, string fileName)
        {
            SaveFile(saveObject, settingDirPath + "/" + fileName);
        }

        /// <summary>
        /// 保存设置，全局生效，不关乎任何一个存档
        /// </summary>
        public static void SaveSetting(object saveObject)
        {
            SaveSetting(saveObject, saveObject.GetType().Name);
        }


        public static void DeleteAllSetting()
        {
            if (Directory.Exists(settingDirPath))
            {
                // 直接删除目录
                Directory.Delete(settingDirPath, true);
            }

            CheckAndCreateDir();
        }

        #endregion

        #region 内部工具函数

        /// <summary>
        /// 获取存档系统数据
        /// </summary>
        /// <returns></returns>
        private static void InitSaveSystemData()
        {
            saveSystemData = LoadFile<SaveSystemData>(SaveDirPath + "/SaveSystemData");
            if (saveSystemData == null)
            {
                saveSystemData = new SaveSystemData();
                UpdateSaveSystemData();
            }

            //检查一下存档有没有被删了
            CheckSaveExist();
        }

        /// <summary>
        /// 检查存档是否存在
        /// </summary>
        public static void CheckSaveExist()
        {
            //记录一下是否有更新
            bool update = false;
            string saveDataName = "GameSaveDatas.json";

            //遍历存档
            for (int i = 0; i < saveSystemData.saveItemList.Count; i++)
            {
                //如果文件不存在了那就删除这个存档
                if (!Directory.Exists($"{SaveDirPath}/{saveSystemData.saveItemList[i].saveID}") ||
                    !File.Exists($"{SaveDirPath}/{saveSystemData.saveItemList[i].saveID}/{saveDataName}"))
                {
                    saveSystemData.saveItemList.RemoveAt(i);
                    i--;
                    update = true;
                }
            }

            //检查有没有本地存档文件更新
            for (int i = 0; i < 3; i++)
            {
                //如果已经有这个存档了那就跳过
                if (GetSaveItem(i) != null) continue;

                //如果找到了存档文件
                if (Directory.Exists($"{SaveDirPath}/{i}") && File.Exists($"{SaveDirPath}/{i}/{saveDataName}"))
                {
                    //那就添加进列表中
                    SaveItem save = new SaveItem(i, DateTime.Now);
                    saveSystemData.saveItemList.Add(save);
                    update = true;
                }
            }

            //如果更新了那就保存
            if (update)
            {
                UpdateSaveSystemData();
            }
        }

        /// <summary>
        /// 更新存档系统数据
        /// </summary>
        private static void UpdateSaveSystemData()
        {
            SaveFile(saveSystemData, SaveDirPath + "/SaveSystemData");
        }

        /// <summary>
        /// 检查路径并创建目录
        /// </summary>
        private static void CheckAndCreateDir()
        {
            // 确保路径的存在
            if (Directory.Exists(SaveDirPath) == false)
            {
                Directory.CreateDirectory(SaveDirPath);
            }

            if (Directory.Exists(settingDirPath) == false)
            {
                Directory.CreateDirectory(settingDirPath);
            }
        }

        /// <summary>
        /// 获取某个存档的路径
        /// </summary>
        /// <param name="saveID">存档ID</param>
        /// <param name="createDir">如果不存在这个路径，是否需要创建</param>
        /// <returns></returns>
        private static string GetSavePath(int saveID, bool createDir = true)
        {
            // 验证是否有某个存档
            if (GetSaveItem(saveID) == null) throw new Exception("Frame:saveID对应的存档不存在！");

            string saveDir = SaveDirPath + "/" + saveID;
            // 确定文件夹是否存在
            if (Directory.Exists(saveDir) == false)
            {
                if (createDir)
                {
                    Directory.CreateDirectory(saveDir);
                }
                else
                {
                    return null;
                }
            }

            return saveDir;
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="saveObject">保存的对象</param>
        /// <param name="path">保存的路径</param>
        private static void SaveFile(object saveObject, string path)
        {
            switch (FrameRoot.Setting.SaveSystemType)
            {
                case SaveSystemType.Binary:
                    // 避免框架内部的数据类型也使用外部序列化工具序列化，这一般都会出现问题
                    if (binarySerializer == null || saveObject.GetType() == typeof(SaveSystemData))
                    {
                        FileTools.SaveFile(saveObject, path);
                    }
                    else
                    {
                        byte[] bytes = binarySerializer.Serialize(saveObject);
                        File.WriteAllBytes(path, bytes);
                    }

                    break;
                case SaveSystemType.Json:
                    string jsonData = JsonUtility.ToJson(saveObject);
                    FileTools.SaveJson(jsonData, path);
                    break;
            }
        }

        // /// <summary>
        // /// 加载文件
        // /// </summary>
        // /// <typeparam name="T">加载后要转为的类型</typeparam>
        // /// <param name="path">加载路径</param>
        // private static T LoadFile<T>(string path) where T : class
        // {
        //     switch (FrameRoot.Setting.SaveSystemType)
        //     {
        //         case SaveSystemType.Binary:
        //             return KNTool.LoadFile<T>(path);
        //         case SaveSystemType.Json:
        //             return KNTool.LoadJson<T>(path);
        //     }
        //
        //     return null;
        // }

        /// <summary>
        /// 加载文件
        /// </summary>
        /// <typeparam name="T">加载后要转为的类型</typeparam>
        /// <param name="path">加载路径</param>
        private static T LoadFile<T>(string path) where T : class
        {
            switch (FrameRoot.Setting.SaveSystemType)
            {
                case SaveSystemType.Binary:
                    // 避免框架内部的数据类型也使用外部序列化工具序列化，这一般都会出现问题
                    if (binarySerializer == null || typeof(T) == typeof(SaveSystemData))
                        return FileTools.LoadFile<T>(path);
                    else
                    {
                        FileStream file = new FileStream(path, FileMode.Open);
                        byte[] bytes = new byte[file.Length];
                        file.Read(bytes, 0, bytes.Length);
                        file.Close();
                        return binarySerializer.Deserialize<T>(bytes);
                    }
                case SaveSystemType.Json:
                    return FileTools.LoadJson<T>(path);
            }

            return null;
        }

        #endregion
    }
}