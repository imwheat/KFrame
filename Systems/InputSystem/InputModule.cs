//****************** 代码文件申明 ************************
//* 文件：InputModule                      
//* 作者：32867
//* 创建时间：2023年08月20日 星期日 17:14
//* 功能：输入模块的基类
//*****************************************************

using GameBuild;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace KFrame.Systems
{
    public sealed class InputModule : MonoBehaviour
    {
        public GameInputAction CurInput;

        public PlayerInput Input { get; private set; }

        public int PlayerIndex { get; set; }

        public bool IsReady;

        /// <summary>
        /// 用来区别玩家的颜色
        /// </summary>
        public Color playerColor; 

        public InputModule(PlayerInput playerInput)
        {
            PlayerIndex = playerInput.playerIndex;
            Input = playerInput;
        }


        /// <summary>
        /// 当前的输入设备
        /// </summary>
        private InputDevice currentDevice;

        /// <summary>
        /// 当前的输入设备
        /// </summary>
        public InputDevice InputDevice
        {
            get
            {
                return currentDevice;
            }
            set
            {
                currentDevice = value;
                UpdateInputDevice();
            }
        }
        /// <summary>
        /// 鼠标设备
        /// </summary>
        private Mouse Mouse;
        /// <summary>
        /// 键盘设备
        /// </summary>
        private Keyboard Keyboard;
        /// <summary>
        /// 手柄
        /// </summary>
        private Gamepad Gamepad;


        [field: SerializeField] public int InputIndex { private set; get; } //玩家所分配到的输入设备序号
        [SerializeField] private string currentDeviceName; //当前输入设备的名字(只作为辅助显示用)
        [SerializeField] public InputDeviceData currentDeviceData; //当前输入设备信息


        [Space(10f)] [SerializeField, FoldoutGroup("设备切换所触发的事件")]
        UnityEvent onSwitchToMouse = default;

        [SerializeField, FoldoutGroup("设备切换所触发的事件")]
        UnityEvent onSwitchToKeyboard = default;

        [SerializeField, FoldoutGroup("设备切换所触发的事件")]
        UnityEvent onSwitchToGamepad = default;


        private void Awake()
        {
            // 初始化当前的输入设备
            Mouse = Mouse.current;
            Keyboard = Keyboard.current;
            Gamepad = Gamepad.current;
        }
        /// <summary>
        /// 选择输入设备数据
        /// </summary>
        [Button("更新输入设备数据")]
        private void SelectDeviceDate()
        {
            currentDeviceData = InputSystemManager.Instance.GetDataByIndex(InputIndex);
            currentDeviceData.GameInputAction.Enable();
        }

        void OnEnable()
        {
            InputSystem.onActionChange += DetectCurrentInputDevice;
        }

        void OnDisable()
        {
            InputSystem.onActionChange -= DetectCurrentInputDevice;
        }

        /// <summary>
        /// 检测当前的输入设备
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="change"></param>
        private void DetectCurrentInputDevice(object obj, InputActionChange change)
        {
#if UNITY_EDITOR
            if (InputSystemManager.detectUIInputOnly &&
                !InputSystemManager.UIInputModule.isActiveAndEnabled) return;
#endif
            if (change == InputActionChange.ActionPerformed)
            {
                InputDevice = ((InputAction)obj).activeControl.device;
                currentDeviceName = InputDevice.name;
            }
        }
        /// <summary>
        /// 更新输入设备
        /// </summary>
        private void UpdateInputDevice()
        {
            switch (currentDevice)
            {
                case UnityEngine.InputSystem.Mouse:
                    onSwitchToMouse.Invoke();
                    currentDeviceData.CurInputControlScheme = GameInputControlScheme.KeyBoardAndMouse;
                    break;
                case UnityEngine.InputSystem.Keyboard:
                    onSwitchToKeyboard.Invoke();
                    currentDeviceData.CurInputControlScheme = GameInputControlScheme.KeyBoardAndMouse;
                    break;
                case UnityEngine.InputSystem.Gamepad:
                    onSwitchToGamepad.Invoke();
                    currentDeviceData.CurInputControlScheme = GameInputControlScheme.GamePad;
                    break;
                default:
                    break;
            }
        }
    }
}