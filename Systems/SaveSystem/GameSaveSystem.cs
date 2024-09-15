//****************** 代码文件申明 ************************
//* 文件：GameSaveSystem                                       
//* 作者：wheat
//* 创建时间：2024/02/21 12:51:39 星期三
//* 描述：用于管理当前的游戏存档
//*****************************************************

using System.Collections;
using System.Collections.Generic;
using KFrame.Tools;
using KFrame.UI;

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
        /// 当前游玩存档
        /// </summary>
        public static SavePlayData CurSavePlayData = new SavePlayData();

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

                return CurSave.saveID;
            }
        }

        /// <summary>
        /// 可以存储的对象
        /// </summary>
        public static HashSet<ISaveable> Saveables;

        /// <summary>
        /// 存档文件
        /// </summary>
        public static GameSaveDatas SaveDatas;

#if UNITY_EDITOR
        /// <summary>
        /// 编辑器中的存档文件
        /// 在直接进入场景，不通过存档进入时有效
        /// 不会被存到本地，只有当次游玩有用
        /// </summary>
        public static GameSaveDatas EditorSaveDatas;
#endif

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

            //如果当前游戏存档为空
            if (SaveDatas == null)
            {
                //那就从本地获取原来的存档数据
                LoadGameData();
            }

            //保存游玩数据
            SaveGamePlayData();

            //等待一帧
            yield return CoroutineTool.WaitForEndOfFrame();

            //保存新的数据
            foreach (ISaveable s in Saveables)
            {
                //防空
                if (s != null)
                {
                    s.OnSave();
                    yield return CoroutineTool.WaitForEndOfFrame();
                }
            }

            //等待一帧
            yield return CoroutineTool.WaitForEndOfFrame();

            //等待一帧
            yield return CoroutineTool.WaitForEndOfFrame();

#if UNITY_EDITOR
            if (CurSaveIndex != -1)
            {
#endif
                //将数据保存到本地
                SaveSystem.SaveObject(SaveDatas, CurSave);
#if UNITY_EDITOR
            }
#endif


            yield return CoroutineTool.WaitForEndOfFrame();

            if (showUI)
            {
                //关闭ui
                SaveGameHintUI.SaveFinished = true;
            }

            //结束保存
            saving = false;
        }

        /// <summary>
        /// 加载游戏存档
        /// </summary>
        public static void LoadGameData()
        {
            //当前存档ID小于0可能是编辑器直接打开游戏，不需要加载数据
            if (CurSaveIndex < 0) return;

            //那就从本地获取原来的存档数据
            GameSaveDatas data = SaveSystem.LoadObject<GameSaveDatas>(CurSave);

            //如果无法获取到
            if (data == null)
            {
                //那就新建存档
                data = new GameSaveDatas();

                //保存
                SaveSystem.SaveObject(data, CurSave);
            }

            //将当前的存档文件更新
            SaveDatas = data;

            //加载游玩数据
            SavePlayData playData = SaveSystem.LoadObject<SavePlayData>(CurSave);

            //如果无法获取到
            if (playData == null)
            {
                //那就新建
                playData = new SavePlayData();

                //保存
                SaveSystem.SaveObject(playData, CurSave);
            }

            CurSavePlayData = playData;
        }

        /// <summary>
        /// 卸载当前存档
        /// </summary>
        public static void UnloadGameSaveData()
        {
            //如果现在没存档就返回
            if (CurSaveIndex == -1) return;

            CurSave = null;
            CurSavePlayData = null;
            Saveables = new HashSet<ISaveable>();
            SaveDatas = null;
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
        public static void SaveData(ISaveable saveable)
        {
            //当前存档ID小于0可能是编辑器直接打开游戏
            if (CurSaveIndex < 0)
            {
#if UNITY_EDITOR
                //如果是在编辑器中，可以存到本次缓存池中
                if (EditorSaveDatas == null)
                {
                    EditorSaveDatas = new GameSaveDatas();
                }

                //更新数据
                EditorSaveDatas.DataDic[saveable.SaveKey] = saveable.GetJsonData();
#endif
                return;
            }

            //防空
            if (SaveDatas == null)
            {
                SaveDatas = new GameSaveDatas();
            }

            //更新数据
            SaveDatas.DataDic[saveable.SaveKey] = saveable.GetJsonData();
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
                if (EditorSaveDatas == null)
                {
                    EditorSaveDatas = new GameSaveDatas();
                }
#else
                return;
#endif
            }

            //防空
            if (CurSavePlayData == null)
            {
                CurSavePlayData = new SavePlayData();
            }

#if UNITY_EDITOR

            if (CurSaveIndex == -1)
            {
                return;
            }
#endif
            //保存到本地
            SaveSystem.SaveObject(CurSavePlayData, CurSaveIndex);
        }

        #endregion
    }
}