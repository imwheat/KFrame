//****************** 代码文件申明 ***********************
//* 文件：ConfirmPanelData
//* 作者：wheat
//* 创建时间：2024/10/06 08:45:03 星期日
//* 描述：确认面板初始化所需的一些数据存储
//*******************************************************


using System;

namespace KFrame.UI
{
    public class ConfirmPanelData : IDisposable
    {
        public string SwitchPanelKey;
        public string ContentKey; 
        public string ConfirmKey;
        public string CancelKey;
        public Action CancelAction;
        public Action ConfirmAction;
        /// <summary>
        /// 确认面板初始化所需的一些数据存储
        /// </summary>
        /// <param name="switchPanelKey">关闭后切换的面板</param>
        /// <param name="contentKey">显示内容的key</param>
        /// <param name="confirmKey">确认的key</param>
        /// <param name="cancelKey">取消的key</param>
        /// <param name="cancelAction">取消事件</param>
        /// <param name="confirmAction">确认事件</param>
        public ConfirmPanelData(string switchPanelKey, string contentKey, string confirmKey, string cancelKey,
            Action cancelAction, Action confirmAction)
        {
            SwitchPanelKey = switchPanelKey;
            ContentKey = contentKey;
            ConfirmKey = confirmKey;
            CancelKey = cancelKey;
            CancelAction = cancelAction;
            ConfirmAction = confirmAction;
        }

        public void Dispose()
        {
            ConfirmAction = null;
            CancelAction = null;
        }
    }
}

