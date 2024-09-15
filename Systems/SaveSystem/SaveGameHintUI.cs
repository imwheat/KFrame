//****************** 代码文件申明 ************************
//* 文件：SaveGameHintUI                                       
//* 作者：wheat
//* 创建时间：2024/02/28 14:16:35 星期三
//* 描述：保存游戏提醒UI
//*****************************************************

using KFrame.UI;
using UnityEngine;
using Sirenix.OdinInspector;

namespace KFrame.Systems
{
    [UIData(typeof(SaveGameHintUI),"SaveGameHintUI",false, 6)]
    public class SaveGameHintUI : UIBase
    {
        [LabelText("最短存在时间")]
        public float MinLastTime = 2f;
        private float lastTime;
        /// <summary>
        /// 存档完成了
        /// </summary>
        public static bool SaveFinished;

        protected override void OnEnable()
        {
            base.OnEnable();
            
            lastTime = 0;
            SaveFinished = false;
        }
        private void Update()
        {
            //计时
            lastTime += Time.unscaledDeltaTime;

            //如果保存完成了，并且显现时间超过最短时间了，就消失
            if(SaveFinished&&lastTime >= MinLastTime)
            {
                UISystem.Close<SaveGameHintUI>();
            }
        }
    }
}

