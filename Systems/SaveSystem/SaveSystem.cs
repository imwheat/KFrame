//****************** 代码文件申明 ************************
//* 文件：SaveSystem                      
//* 作者：wheat
//* 创建时间：2024/09/24 15:23:18 星期二
//* 描述：存档系统
//*****************************************************


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using KFrame.Utilities;

namespace KFrame.Systems
{
    public interface IBinarySerializer
    {
        public byte[] Serialize<T>(T obj) where T : class;
        public T Deserialize<T>(byte[] bytes) where T : class;
    }

    public static class SaveSystem
    {
        #region 参数与路径配置

        /// <summary>
        /// 存档系统数据类
        /// </summary>
        private class SaveSystemData
        {
            /// <summary>
            /// 所有存档的列表
            /// </summary>
            public readonly List<SaveItem> SaveItemList = new List<SaveItem>();
            /// <summary>
            /// 获取最小的可用的存档id
            /// </summary>
            /// <returns>最小的可用的存档id</returns>
            public int GetMinAvailableSaveId()
            {
                //先记录下已经有的id
                var existId = new HashSet<int>();
                foreach (var saveItem in SaveItemList)
                {
                    existId.Add(saveItem.SaveID);
                }
                //从1开始，如果已经存在那就+1
                int min = 1;
                while (existId.Contains(min))
                {
                    min++;
                }
                //返回结果
                return min;
            }
        }
        
        /// <summary>
        /// 存档数据
        /// </summary>
        private static SaveSystemData saveSystemData;

        /// <summary>
        /// 存档保存路径文件夹名称
        /// </summary>
        private const string SaveDirName = "SaveData";
        /// <summary>
        /// 一些游戏设置的保存文件夹名称
        /// </summary>
        private const string SettingDirName = "Settings";
        /// <summary>
        /// 玩家存档文件名称
        /// </summary>
        private const string SaveFileName = "PlayerSave";
        /// <summary>
        /// 游戏设置存档文件名称
        /// </summary>
        private const string SettingFileName = "Settings";
        /// <summary>
        /// 保存文件后缀
        /// </summary>
        private const string SaveFileSuffix = ".sav";
        
        
        /// <summary>
        /// 存档文件夹路径
        /// </summary>
        public static string SaveDirPath;
        /// <summary>
        /// 游戏设置保存路径
        /// </summary>
        private static string settingDirPath;

        /// <summary>
        /// 存档中对象的缓存字典
        /// (存档ID,(文件名称，实际的对象))
        /// </summary>
        private static readonly Dictionary<int, Dictionary<string, object>> cacheDic =
            new Dictionary<int, Dictionary<string, object>>();


        public static IBinarySerializer binarySerializer;
        

        #endregion
        
        #region 存档系统、存档系统数据类及所有用户存档、设置存档数据


#if UNITY_EDITOR
        private static bool inited;
        static SaveSystem()
        {
            Init();
        }
#endif

        /// <summary>
        /// 获取存档系统数据
        /// </summary>
        /// <returns></returns>
        private static void InitSaveSystemData()
        {
            //检查路径
            CheckAndCreateDir();
            
            //读取本地文件，更新存档状态            
            saveSystemData = new SaveSystemData();
            string[] files = Directory.GetFiles(SaveDirPath);
            foreach (string file in files)
            {
                //获取文件名称
                string fileName = Path.GetFileName(file);
                //只要文件的名称或者后缀不对那就跳过
                if(!fileName.Contains(SaveFileName) || !fileName.EndsWith(SaveFileSuffix)) continue;
                
                //读取存档文件，如果不为空
                SaveItem saveItem = LoadFile<SaveItem>(SaveDirPath + fileName);
                if (saveItem != null)
                {
                    //那就添加入存档列表
                    saveSystemData.SaveItemList.Add(saveItem);
                }
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            SaveDirPath = Application.persistentDataPath + "/" + SaveDirName + "/";
            settingDirPath = Application.persistentDataPath + "/" + SettingDirName + "/";
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode || inited)
            {
                return;
            }
#endif

            //初始化SaveSystemData
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
            return saveSystemData.SaveItemList;
        }

        /// <summary>
        /// 获取所有存档
        /// 创建最新的在最前面
        /// </summary>
        /// <returns></returns>
        public static List<SaveItem> GetAllSaveItemByCreatTime()
        {
            List<SaveItem> saveItems = new List<SaveItem>(saveSystemData.SaveItemList.Count);

            for (int i = 0; i < saveSystemData.SaveItemList.Count; i++)
            {
                saveItems.Add(saveSystemData.SaveItemList[^(i + 1)]);
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
            List<SaveItem> saveItems = new List<SaveItem>(saveSystemData.SaveItemList.Count);
            for (int i = 0; i < saveSystemData.SaveItemList.Count; i++)
            {
                saveItems.Add(saveSystemData.SaveItemList[i]);
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
            List<SaveItem> saveItems = new List<SaveItem>(saveSystemData.SaveItemList.Count);
            for (int i = 0; i < saveSystemData.SaveItemList.Count; i++)
            {
                saveItems.Add(saveSystemData.SaveItemList[i]);
            }

            saveItems.Sort((a, b) => a.SaveID - b.SaveID);
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
                return saveSystemData.SaveItemList.OrderByDescending(orderFunc).ToList();
            }
            else
            {
                return saveSystemData.SaveItemList.OrderBy(orderFunc).ToList();
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
            foreach (var saveItem in saveSystemData.SaveItemList)
            {
                if (saveItem.SaveID == id)
                {
                    return saveItem;
                }
            }

            return null;
        }

        /// <summary>
        /// 获取SaveItem
        /// </summary>
        public static SaveItem GetSaveItem(SaveItem saveItem)
        {
            return GetSaveItem(saveItem.SaveID);
        }
        /// <summary>
        /// 添加一个指定ID的存档
        /// </summary>
        /// <param name="saveID">指定的ID</param>
        /// <returns></returns>
        public static SaveItem CreateSaveItem(int saveID)
        {
            //创建一个新的存档
            SaveItem saveItem = new SaveItem(saveID, DateTime.Now);
            
            //加入存档列表
            saveSystemData.SaveItemList.Add(saveItem);
            //返回结果
            return saveItem;
        }
        /// <summary>
        /// 添加一个存档
        /// </summary>
        /// <returns></returns>
        public static SaveItem CreateSaveItem()
        {
            //自动搜索一个最小的可用存档id然后创建存档
            return CreateSaveItem(saveSystemData.GetMinAvailableSaveId());
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

            saveSystemData.SaveItemList.Remove(GetSaveItem(saveID));
            // 移除缓存
            RemoveCache(saveID);
        }

        /// <summary>
        /// 删除存档
        /// </summary>
        public static void DeleteSaveItem(SaveItem saveItem)
        {
            DeleteSaveItem(saveItem.SaveID);
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
            SaveObject(saveObject, saveFileName, saveItem.SaveID);
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
            return LoadObject<T>(saveFileName, saveItem.SaveID);
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
            return LoadObject<T>(typeof(T).Name, saveItem.SaveID);
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
            DeleteObject<T>(saveFileName, saveItem.SaveID);
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
            DeleteObject<T>(typeof(T).Name, saveItem.SaveID);
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
        /// 检查路径并创建目录
        /// </summary>
        private static void CheckAndCreateDir()
        {
            //如果文件夹不存在就创建
            FileExtensions.CreateDirectoryIfNotExist(SaveDirPath);
            FileExtensions.CreateDirectoryIfNotExist(settingDirPath);

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
                        FileExtensions.SaveFile(saveObject, path);
                    }
                    else
                    {
                        byte[] bytes = binarySerializer.Serialize(saveObject);
                        File.WriteAllBytes(path, bytes);
                    }

                    break;
                case SaveSystemType.Json:
                    string jsonData = JsonUtility.ToJson(saveObject);
                    FileExtensions.SaveJson(jsonData, path);
                    break;
            }
        }
        
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
                        return FileExtensions.LoadFile<T>(path);
                    else
                    {
                        FileStream file = new FileStream(path, FileMode.Open);
                        byte[] bytes = new byte[file.Length];
                        file.Read(bytes, 0, bytes.Length);
                        file.Close();
                        return binarySerializer.Deserialize<T>(bytes);
                    }
                case SaveSystemType.Json:
                    return FileExtensions.LoadJson<T>(path);
            }

            return null;
        }

        #endregion
    }
}