//****************** 代码文件申明 ***********************
//* 文件：InputSettingsPanel
//* 作者：wheat
//* 创建时间：2024/10/03 08:41:14 星期四
//* 描述：输入的设置面板
//*******************************************************

#if UNITY_EDITOR

using UnityEditor;

#endif

using System.Collections.Generic;
using KFrame.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KFrame.UI
{
    [UIData(typeof(InputSettingsPanel), "InputSettingsPanel", true, 3)]
    public class InputSettingsPanel : UIPanelBase
    {
        #region UI配置

        /// <summary>
        /// 按键保存的key的前缀
        /// </summary>
        [SerializeField]
        private string keySetSaveKeyPrefix = "keySet_";
        /// <summary>
        /// 返回按钮
        /// </summary>
        [SerializeField] 
        private KButton returnBtn;
        /// <summary>
        /// 按键设置UI数据
        /// </summary>
        [SerializeField]
        private List<InputSetUIData> inputSetUIDatas = new();
        #endregion

        #region 编辑器配置参数

        #if UNITY_EDITOR

        /// <summary>
        /// 按键配置
        /// </summary>
        [SerializeField]
        private List<InputActionReference> inputActions = new();
        /// <summary>
        /// 按键设置UI父级
        /// </summary>
        [SerializeField]
        private RectTransform keySetUIParent;
        /// <summary>
        /// 按键设置UI起始位置
        /// </summary>
        [SerializeField]
        private Vector2 keySetUIOrigin = new Vector2(-100, 80f);
        /// <summary>
        /// 按键设置UI间隔
        /// </summary>
        [SerializeField]
        private float keySetUIInterval = 24f;
        /// <summary>
        /// 按键设置UI预制体
        /// </summary>
        [SerializeField]
        private GameObject keySetUIPrefab;
        
        #endif

        #endregion
        
        protected override void Awake()
        {
            base.Awake();
            
            //按键事件注册
            returnBtn.OnClick.AddListener(OnPressESC);
            //更新数据
            foreach (var uiData in inputSetUIDatas)
            {
                uiData.bindButton.Data = uiData;
            }
        }

        #region 编辑器配置使用

        #if UNITY_EDITOR
        
        /// <summary>
        /// 获取UI的排版坐标
        /// </summary>
        /// <param name="index">列表里的下标</param>
        /// <returns>UI的排版坐标</returns>
        private Vector2 GetUILayoutPos(int index)
        {
            return new Vector2(keySetUIOrigin.x, keySetUIOrigin.y - keySetUIInterval * index);
        }
        /// <summary>
        /// 一键创建按键设置UI
        /// </summary>
        public void AutoCreateInputSetUI()
        {
            //先记录注册当前已经有了的数据字典
            HashSet<InputActionReference> uiInputActionDic = new();
            foreach (var setUIData in inputSetUIDatas)
            {
                uiInputActionDic.Add(setUIData.bindInput);
            }

            //遍历每个配置
            foreach (var input in inputActions)
            {
                //如果已经有了那就跳过
                if(uiInputActionDic.Contains(input)) continue;
                
                //k表示当前按键有几个键位
                int k = 1;
                //如果当前按键是多键位的
                if (input.action.bindings[0].isComposite)
                {
                    // 那就判断一下是多少个按键的
                    // 如果下一位按键小于总数
                    // 并且是多按键的一部分
                    // 那么k++
                    while (k+1 < input.action.bindings.Count && 
                           input.action.bindings[k+1].isPartOfComposite)
                    {
                        k++;
                    }
                }

                int posIndex = inputSetUIDatas.Count;
                int initIndex = 0;
                string name1 = input.action.name;
                //如果是多键位那就跳过第一个
                if (input.action.bindings[0].isComposite)
                {
                    initIndex++;
                    k++;
                }
                //根据按键需求补充，循环k次
                for (int j = initIndex; j < k; j++)
                {

                    //生成预制体，然后设置父级和位置
                    GameObject uiObj = Object.Instantiate(keySetUIPrefab, keySetUIParent.transform);
                    InputSetButton uiBtn = uiObj.GetComponent<InputSetButton>();
                    RectTransform uiRect = uiObj.GetComponent<RectTransform>();
                    uiRect.anchoredPosition = GetUILayoutPos(posIndex++);
                    //配置本地化
                    string name2 = name1 + input.action.bindings[j].name.GetNiceFormat();
                    uiBtn.KeyNameText.text = name2;
                    uiBtn.KeyText.text = input.action.bindings[j].ToDisplayString();
                    string saveKey = keySetSaveKeyPrefix + name2;
                    uiObj.name = saveKey;
                    LocalizationEditHelper uiLocalization = uiObj.GetComponent<LocalizationEditHelper>();
                    uiLocalization.Key = saveKey;
                    //新建data
                    InputSetUIData data = new InputSetUIData()
                    {
                        bindButton = uiBtn,
                        bindInput = input,
                        rebindId = j,
                        saveKey = saveKey
                    };
                    uiBtn.Data = data;
                    
                    //存储data
                    inputSetUIDatas.Add(data);
                }

            }
        }
        /// <summary>
        /// 一键配置导航
        /// </summary>
        public void AutoLinkNavigation()
        {
            //遍历设置导航
            for (int i = 0; i < inputSetUIDatas.Count; i++)
            {
                KButton button = inputSetUIDatas[i].bindButton;
                if (i > 0)
                {
                    button.navigation = button.navigation.ChangeSelectOnUp(inputSetUIDatas[i - 1].bindButton);
                }

                if (i < inputSetUIDatas.Count - 1)
                {
                    button.navigation = button.navigation.ChangeSelectOnDown(inputSetUIDatas[i + 1].bindButton);
                }
                
                EditorUtility.SetDirty(button);
            }
        }
        
        #endif

        #endregion

    }
}

