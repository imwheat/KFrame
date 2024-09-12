using System;


namespace MyUtils
{
	/// <summary>
	/// 鼠标输入的设置
	/// </summary>
	[Serializable]
	public struct MouseSettings
	{
		/// <summary>
		/// 鼠标按键的ID.
		/// </summary>
		public int mouseButtonID;

		/// <summary>
		/// 鼠标灵敏度
		/// </summary>
		public float pointerSensitivity;

		/// <summary>
		/// 滚轮灵敏度
		/// </summary>
		public float wheelSensitivity;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="mouseButtonID">ID of mouse button.</param>
		/// <param name="pointerSensitivity">Sensitivity of mouse pointer.</param>
		/// <param name="wheelSensitivity">Sensitivity of mouse ScrollWheel.</param>
		public MouseSettings(int mouseButtonID, float pointerSensitivity, float wheelSensitivity)
		{
			this.mouseButtonID = mouseButtonID;
			this.pointerSensitivity = pointerSensitivity;
			this.wheelSensitivity = wheelSensitivity;
		}
	}
}

