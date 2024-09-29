//****************** 代码文件申明 ************************
//* 文件：GameSaveSystem                                       
//* 作者：wheat
//* 创建时间：2024/02/21 12:51:39 星期三
//* 描述：用于管理当前的游戏存档
//*****************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using KFrame.Utilities;
using KFrame.UI;
using UnityEngine;

namespace KFrame.Systems
{
    public static class GameSaveSystem
    {
        #region 字段
        
        /// <summary>
        /// 当前存档
        /// </summary>
        public static SaveItem CurSave;
        /// <summary>
        /// 保存存档游玩数据的key
        /// </summary>
        private const string SavePlayDataKey = "SavePlayData";

        /// <summary>
        /// 当前存档游玩数据
        /// </summary>
        public static SavePlayData CurSavePlayData => CurSave.SavePlayData;

        /// <summary>
        /// 当前的存档ID
        /// </summary>
        public static int CurSaveIndex
        {
            get
            {
                if (CurSave == null)
                {
                    return -1;
                }

                return CurSave.SaveID;
            }
        }

        /// <summary>
        /// 可以存储的对象
        /// </summary>
        public static HashSet<ISaveable> Saveables;


        /// <summary>
        /// 保存中
        /// </summary>
        private static bool saving;

        #endregion

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            Saveables = new HashSet<ISaveable>();
        }
        
        #if UNITY_EDITOR

        /// <summary>
        /// 创建编辑器测试使用的临时存档
        /// </summary>
        private static void CreateEditorTempSave()
        {
            CurSave = new SaveItem(-1, DateTime.Now, "");
        }
        
        #endif

        #endregion

        #region 方法

        /// <summary>
        /// 开始保存游戏数据
        /// </summary>
        /// <param name="showUI">显示保存提示UI</param>
        public static void StartGameSave(bool showUI)
        {
            if (saving == false)
            {
                MonoSystem.Start_Coroutine(SaveGameDataCoroutine(showUI));
            }
        }

        /// <summary>
        /// 保存游戏数据
        /// </summary>
        /// <param name="showUI">显示保存提示UI</param>
        public static IEnumerator SaveGameDataCoroutine(bool showUI)
        {
            //保存中
            saving = true;

            //如果显示保存提示UI
            if (showUI)
            {
                //那就显示
                UISystem.Show<SaveGameHintUI>();
            }

            //保存游玩数据
            SaveGamePlayData();

            //等待一帧
            yield return CoroutineExtensions.WaitForEndOfFrame();

            //保存新的数据
            foreach (ISaveable s in Saveables)
            {
                //防空
                if (s != null)
                {
                    s.OnSave();
                    yield return CoroutineExtensions.WaitForEndOfFrame();
                }
            }

            //等待一帧
            yield return CoroutineExtensions.WaitForEndOfFrame();

            //等待一帧
            yield return CoroutineExtensions.WaitForEndOfFrame();

#if UNITY_EDITOR
            if (CurSaveIndex != -1)
            {
#endif
                //将数据保存到本地
                SaveSystem.SaveGameSave(CurSave);
#if UNITY_EDITOR
            }
#endif


            yield return CoroutineExtensions.WaitForEndOfFrame();

            if (showUI)
            {
                //关闭ui
                SaveGameHintUI.SaveFinished = true;
            }

            //结束保存
            saving = false;
        }

        /// <summary>
        /// 卸载当前存档
        /// </summary>
        public static void UnloadGameSaveData()
        {
            //如果现在没存档就返回
            if (CurSave == null) return;

            CurSave = null;
            Saveables = new HashSet<ISaveable>();
        }

        /// <summary>
        /// 注册
        /// </summary>
        public static void RegisterISaveable(ISaveable saveable)
        {
            Saveables.Add(saveable);
        }

        /// <summary>
        /// 注销
        /// </summary>
        public static void UnRegisterISaveable(ISaveable saveable)
        {
            Saveables.Remove(saveable);
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="saveable">可以保存数据的对象</param>
        public static void SaveData(this ISaveable saveable)
        {

            if (CurSave == null)
            {
                
#if UNITY_EDITOR
                //编辑器中如果存档为空，可能是直接打开游戏进行测试，这边创建一个临时存档
                CreateEditorTempSave();
#else
                Debug.LogError("发生错误：存档为空的时候进行了保存");
                return;
#endif
            }

            //更新数据
            CurSave.SaveData(saveable);
        }
        /// <summary>
        /// 加载数据
        /// </summary>
        /// <param name="saveable">可以保存数据的对象</param>
        public static void LoadData(this ISaveable saveable)
        {
            if (CurSave == null)
            {
                
#if UNITY_EDITOR
                //编辑器中如果存档为空，可能是直接打开游戏进行测试，这边创建一个临时存档
                CreateEditorTempSave();
#else
                Debug.LogError("发生错误：存档为空的时候进行了读取");
                return;
#endif
            }
            
            //从当前存档里面加载数据
            CurSave.LoadData(saveable);
        }

        /// <summary>
        /// 保存当前游戏游玩数据
        /// </summary>
        public static void SaveGamePlayData()
        {
            //如果现在没有对应存档就返回
            if (CurSaveIndex == -1)
            {
#if UNITY_EDITOR
                //如果是在编辑器中，可以存到本次缓存池中
                if (CurSave == null)
                {
                    CreateEditorTempSave();
                }
#else
                return;
#endif
            }
            
        }

        #endregion
    }
}