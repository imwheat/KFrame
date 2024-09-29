//****************** 代码文件申明 ************************
//* 文件：InputSystemManager                                       
//* 作者：32867
//* 创建时间：2023/08/13 15:21:05 星期日
//* 功能：管理InputAction
//*****************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using GameBuild;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

namespace KFrame.Systems
{
    public enum GameInputControlScheme
    {
        KeyBoardAndMouse, //键鼠方案
        GamePad,          //手柄
    }

    public class InputSystemManager : MonoSingleton<InputSystemManager>
    {
        public PlayerInputManager Manager;

        /// <summary>
        /// 最大玩家数量
        /// </summary>
        [Header("最大玩家数量")] private int maxPlayers;

        public int MaxPlayers
        {
            get => maxPlayers;
            set => maxPlayers = value;
        }

        /// <summary>
        /// 输入模块
        /// </summary>
        [SerializeField] public List<InputModule> InputModules = new();


        /// <summary>
        /// 输入设备数据
        /// </summary>
        [SerializeField] private List<InputDeviceData> _deviceDatas = new();


        /// <summary>
        /// 当前的输入方案
        /// </summary>
        public GameInputControlScheme CurControlScheme;


        [Header("输入管理初始化")] [SerializeField, Tooltip("是否在开始时隐藏鼠标指针")]
        private bool hideCursorAtBeginning = false;

        [SerializeField, Tooltip("是否在开始时锁定鼠标指针于屏幕内")]
        private bool confinedCursorAtBeginning = false;

        [SerializeField, Tooltip("是否在开始时锁定鼠标指针于屏幕中央")]
        private bool lockCursorAtBeginning = false;

#if UNITY_EDITOR
        [Header("UI输入选项")] public static bool detectUIInputOnly = false; //是否只检测UI输入
        public static InputSystemUIInputModule UIInputModule;            // 对InputSystemUIInputModule的引用
#endif


        #region Unity生命周期

        protected override void Awake()
        {
            base.Awake();
            Manager = GetComponent<PlayerInputManager>();
            PlayerInputInit();
            HandleCursor();
            HandleUIInputModule();
            UpdateDevice();
        }

        #endregion

        [Button("更新玩家与输入设备")]
        private void UpdateDevice()
        {
            //获取当前的所有玩家
            var players = GameObject.FindGameObjectsWithTag("Player");
            InputModules.Clear();
            foreach (var player in players)
            {
                if (player.TryGetComponent(out InputModule module))
                {
                    InputModules.Add(module);
                }

                //按照 inputIndex 属性排序
                InputModules = InputModules.OrderBy(inputModule => inputModule.InputIndex).ToList();
            }

            // 获取当前已连接的所有输入设备
            InputDevice[] currentDevices = InputSystem.devices.ToArray();
            _deviceDatas.Clear();
            for (int i = 0, j = 0; i < currentDevices.Length; i++)
            {
                switch (currentDevices[i])
                {
                    case Keyboard:
                        _deviceDatas.Add(new InputDeviceData(j++, currentDevices[i]));
                        break;
                    case Mouse:
                        _deviceDatas.Add(new InputDeviceData(j++, currentDevices[i]));
                        break;
                    case Gamepad:
                        _deviceDatas.Add(new InputDeviceData(j++, currentDevices[i]));
                        break;
                }
            }
        }
        
        private void PlayerInputInit()
        {
            //_inputModules.Add(GameObject.FindWithTag("Player").GetComponent<PlayerInputModule>());
            //尝试获取初始的输入设备
            MaxPlayers = this.GetComponent<PlayerInputManager>().maxPlayerCount;
            try
            {
                _deviceDatas.Add(new InputDeviceData(0, InputSystem.devices[0]));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// 根据序号获取输入设备
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public InputDeviceData GetDataByIndex(int index)
        {
            if (index >= 0 && index <= _deviceDatas.Count - 1)
            {
                return _deviceDatas[index];
            }

            return null;
        }

        public InputModule GetModuleByIndex(int index)
        {
            if (index >= 0 && index <= InputModules.Count - 1)
            {
                return InputModules[index];
            }

            return null;
        }

        /// <summary>
        /// 鼠标指针处理
        /// </summary>
        private void HandleCursor()
        {
            //处理鼠标指针的显隐
            Cursor.visible = !hideCursorAtBeginning;

            if (lockCursorAtBeginning)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else if (confinedCursorAtBeginning)
            {
                Cursor.lockState = CursorLockMode.Confined;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }

        private void HandleUIInputModule()
        {
            // 在场景中查找InputSystemUIInputModule
#if UNITY_EDITOR
            UIInputModule = FindObjectOfType<InputSystemUIInputModule>(true);
            // 如果未找到UI Input Module并且启用了detectUIInputOnly选项，则显示错误
            if (UIInputModule == null && detectUIInputOnly)
            {
                Debug.LogError("无法找到UI Input Module！请检查场景中是否有Event System并且是否正在使用UI Input Module！");
            }
#endif
        }
    }
}