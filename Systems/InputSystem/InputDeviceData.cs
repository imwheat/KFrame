//****************** 代码文件申明 ************************
//* 文件：InputDeviceData                      
//* 作者：32867
//* 创建时间：2023年08月20日 星期日 17:10
//* 功能：输入设备数据
//*****************************************************

using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace KFrame.Systems
{
	[Serializable]
    public class InputDeviceData
    {
        public int Index; //设备序号
        public InputDevice Device; //输入设备
        public GameInputAction GameInputAction; //输入配置
        [field: SerializeField] public int DeviceId { private set; get; }

        public InputDeviceScheme curInputScheme;
        
        /// <summary>
        /// 设备名称
        /// </summary>
        [SerializeField]
        private string m_deviceName;

        public InputDeviceData(int index, InputDevice device)
        {
            Index = index;
            Device = device;
            DeviceId = device.deviceId;
            m_deviceName = device.name;
            GameInputAction = new GameInputAction();
        }
        
        
        
        
    }
}