using Sirenix.OdinInspector;
using UnityEngine;
namespace KFrame.Systems
{
    /// <summary>
    /// UI元素数据
    /// </summary>
    public class UIWindowData
    {
        [LabelText("是否需要缓存")] public bool IsCache;
        [LabelText("预制体Path或AssetKey")] public string AssetPath;
        [LabelText("UI层级")] public int LayerNum;
        /// <summary>
        /// 这个窗口对象的预制体
        /// </summary>
        [LabelText("窗口预制体")] public GameObject Prefab;

        public UIWindowData(bool isCache, string assetPath, int layerNum)
        {
            this.IsCache = isCache;
            this.AssetPath = assetPath;
            this.LayerNum = layerNum;
        }
    }

}