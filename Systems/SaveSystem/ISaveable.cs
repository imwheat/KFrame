//*********************** 代码文件申明 ************************
//* 文件：ISaveable                                       
//* 作者：wheat
//* 创建时间：2024/02/21 12:35:03 星期三
//* 描述：用于管理游戏中一些对象的保存，接入接口说明可以保存
//************************************************************

using System.Diagnostics;

namespace KFrame.Systems
{
    public interface ISaveable
    {
        /// <summary>
        /// 保存的Key
        /// 不能相等
        /// </summary>
        public string SaveKey { get; }
        /// <summary>
        /// Awake的时候调用的接口方法
        /// </summary>
        public void IAwake();
        /// <summary>
        /// Awake的时候调用
        /// </summary>
        public void OnAwake()
        {
            //注册
            GameSaveSystem.RegisterISaveable(this);

            //调用实现的接口方法
            IAwake();
        }
        /// <summary>
        /// Destroy的时候调用的接口方法
        /// </summary>
        public void IDestroy();
        /// <summary>
        /// Destroy的时候调用
        /// </summary>
        public void OnIDestroy()
        {
            //调用实现的接口方法
            IDestroy();

            //注销
            GameSaveSystem.UnRegisterISaveable(this);
        }
        /// <summary>
        /// 保存数据的接口方法
        /// </summary>
        public void ISave();
        /// <summary>
        /// 保存数据的时候调用
        /// </summary>
        public void OnSave()
        {
            //先调用接口方法
            ISave();

            //然后保存数据
            GameSaveSystem.SaveData(this);
        }
        /// <summary>
        /// 加载数据的时候调用
        /// </summary>
        public void OnLoad()
        {
            string jsonData = "";

#if UNITY_EDITOR

            //如果是编辑器中,没有打开存档，那就尝试从缓存池获取
            if (GameSaveSystem.CurSaveIndex == -1)
            {
                //如果为空那就新建一个
                if(GameSaveSystem.EditorSaveDatas==null)
                {
                    GameSaveSystem.EditorSaveDatas = new GameSaveDatas();
                }
                else if(GameSaveSystem.EditorSaveDatas.DataDic.TryGetValue(SaveKey, out var cache))
                {
                    //如果获取到了，那就赋值
                    jsonData = cache;
                }
            }
#endif

            //先初步加载
            if (GameSaveSystem.SaveDatas != null && GameSaveSystem.SaveDatas.DataDic.TryGetValue(SaveKey, out var data))
            {
                //如果获取到了，那就赋值
                jsonData = data;
            }


            //再调用接口方法
            ILoad(jsonData);
        }
        /// <summary>
        /// 加载数据的接口方法
        /// </summary>
        public void ILoad(string jsonData);

        /// <summary>
        /// 获取JsonData
        /// </summary>
        public string GetJsonData();

    }
}

