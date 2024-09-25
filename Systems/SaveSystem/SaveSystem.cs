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
        /// 游戏全局相关的保存文件夹名称
        /// </summary>
        private const string GlobalSaveDirName = "User";
        /// <summary>
        /// 玩家存档文件名称
        /// </summary>
        private const string SaveFileName = "PlayerSave";
        /// <summary>
        /// 保存文件后缀
        /// </summary>
        private const string SaveFileSuffix = ".sav";
        /// <summary>
        /// 存档文件夹路径
        /// </summary>
        public static string SaveDirPath;
        /// <summary>
        /// 游戏全局相关的保存文件夹名称
        /// </summary>
        private static string globalSaveDirPath;

        private static IBinarySerializer binarySerializer;
        
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
            //初始化路径
            SaveDirPath = Application.persistentDataPath + "/" + SaveDirName + "/";
            globalSaveDirPath = Application.persistentDataPath + "/" + GlobalSaveDirName + "/";
            
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
            var saveItems = new List<SaveItem>(saveSystemData.SaveItemList);
            
            saveItems.Sort((x, y) =>
            {
                if (x.LastSaveTime > y.LastSaveTime)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            });
            
            return saveItems;
        }

        /// <summary>
        /// 获取所有存档
        /// 最新id小的在最上面
        /// </summary>
        /// <returns></returns>
        public static List<SaveItem> GetAllSaveItemBySaveId()
        {
            var saveItems = new List<SaveItem>(saveSystemData.SaveItemList);

            saveItems.Sort((a, b) => a.SaveID - b.SaveID);
            
            return saveItems;
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
            DeleteAllSaveItem();
            DeleteAllSetting();
        }

        #endregion

        #region 保存、加载文件

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
        /// 保存游戏存档
        /// </summary>
        /// <param name="saveItem">游戏存档</param>
        public static void SaveGameSave(SaveItem saveItem)
        {
            if(saveItem == null) return;
            
            SaveFile(saveItem, saveItem.SaveFilePath);
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
                        var read = file.Read(bytes, 0, bytes.Length);
                        file.Close();
                        return binarySerializer.Deserialize<T>(bytes);
                    }
                case SaveSystemType.Json:
                    return FileExtensions.LoadJson<T>(path);
            }

            return null;
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
            //先搜索是否已经有这个id的存档了，如果有了那就返回
            SaveItem find = GetSaveItem(saveID);
            if (find != null) return find;
            
            //没有那就创建一个新的存档
            string savePath = GetSavePath(saveID);
            SaveItem saveItem = new SaveItem(saveID, DateTime.Now, savePath);
            SaveFile(saveItem, savePath);
            
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
        /// <param name="saveItem">存档</param>
        public static void DeleteSaveItem(SaveItem saveItem)
        {
            //乳沟存档为空那就返回
            if(saveItem == null) return;
            
            string savePath = GetSavePath(saveItem.SaveID);
            //文件存在那就删除
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
            }
            
            //从列表移除
            saveSystemData.SaveItemList.Remove(saveItem);
        }
        
        /// <summary>
        /// 删除存档
        /// </summary>
        /// <param name="saveID">存档的ID</param>
        public static void DeleteSaveItem(int saveID)
        {
            DeleteSaveItem(GetSaveItem(saveID));
        }


        #endregion

        #region 保存、获取游戏全局存档

        /// <summary>
        /// 加载设置文件
        /// </summary>
        public static T LoadGlobalSave<T>(string fileName) where T : class
        {
            return LoadFile<T>(globalSaveDirPath + "/" + fileName);
        }

        /// <summary>
        /// 加载设置文件
        /// </summary>
        public static T LoadGlobalSave<T>() where T : class
        {
            return LoadGlobalSave<T>(typeof(T).GetNiceName());
        }

        /// <summary>
        /// 保存设置文件
        /// </summary>
        public static void SaveGlobalSave(object saveObject, string fileName)
        {
            SaveFile(saveObject, globalSaveDirPath + "/" + fileName);
        }

        /// <summary>
        /// 保存设置文件
        /// </summary>
        public static void SaveGlobalSave(object saveObject)
        {
            SaveGlobalSave(saveObject, saveObject.GetType().GetNiceName());
        }

        /// <summary>
        /// 删除设置文件
        /// </summary>
        public static void DeleteAllSetting()
        {
            if (Directory.Exists(globalSaveDirPath))
            {
                //直接删除目录
                Directory.Delete(globalSaveDirPath, true);
            }
            
            //然后重新创建文件夹
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
            FileExtensions.CreateDirectoryIfNotExist(globalSaveDirPath);

        }

        /// <summary>
        /// 获取某个存档的路径
        /// 保存路径为：存档路径/文件名称(id).后缀
        /// </summary>
        /// <param name="saveID">存档ID</param>
        /// <returns>对应id的存档路径</returns>
        private static string GetSavePath(int saveID)
        {
            return SaveDirPath + SaveFileName + saveID + SaveFileSuffix;
        }

       

        #endregion
    }
}