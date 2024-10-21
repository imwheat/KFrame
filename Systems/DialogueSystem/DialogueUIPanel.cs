//****************** 代码文件申明 ***********************
//* 文件：DialogueUIPanel
//* 作者：wheat
//* 创建时间：2024/10/19 21:42:24 星期六
//* 描述：对话UI面板
//*******************************************************

using System;
using System.Collections;
using KFrame.Systems.NodeSystem;
using KFrame.UI;
using KFrame.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KFrame.Systems.DialogueSystem
{
    [UIData("DialogueUIPanel", "DialogueUIPanel", true, 2)]
    public class DialogueUIPanel : UIPanelBase, IPointerClickHandler
    {
        #region UI设置

        /// <summary>
        /// 对话文本
        /// </summary>
        [SerializeField]
        private TMP_Text dialogueText;

        #endregion
        
        #region 对话相关参数

        /// <summary>
        /// 当前对话树
        /// </summary>
        public DialogueTree Tree;
        /// <summary>
        /// 当前所在节点
        /// </summary>
        [SerializeField]
        private DialogueNode node;
        /// <summary>
        /// 交互操作CD
        /// 防止不小心多按跳过
        /// </summary>
        private const float InteractCD = 0.2f;
        /// <summary>
        /// 交互CD计时器
        /// </summary>
        private float interactCDTimer;
        /// <summary>
        /// 打印文本的协程
        /// </summary>
        private Coroutine printTextCoroutine;
        /// <summary>
        /// 要打印的文本
        /// </summary>
        [SerializeField]
        private string printText;
        /// <summary>
        /// 打印文本中
        /// </summary>
        [SerializeField]
        private bool isPrintingText;
        /// <summary>
        /// 打印文本中
        /// </summary>
        public bool IsPrintingText => isPrintingText;
        
        #endregion

        #region 生命周期

        private void Update()
        {
            if (interactCDTimer > 0)
            {
                interactCDTimer -= Time.unscaledDeltaTime;
            }
        }

        #endregion
        
        #region 对话进度控制

        /// <summary>
        /// 对话树初始化
        /// </summary>
        /// <param name="tree"></param>
        public void Init(DialogueTree tree)
        {
            //设置树和节点并初始化
            Tree = tree;
            Tree.InitTree();
            node = null;
            
            //更新CD，开始下一步
            interactCDTimer = 0f;
            Next();
        }
        /// <summary>
        /// 下一步
        /// </summary>
        public void Next()
        {
            //交互等待CD
            if (interactCDTimer > 0f)
            {
                return;
            }
            
            //如果还在打印文本，那就结束打印
            if (IsPrintingText)
            {
                
                //更新CD
                interactCDTimer = InteractCD;

                SkipPrintText();
                return;
            }
            //如果当前没有节点了，那就获取对话树的首个节点
            else if (node == null)
            {
                if (Tree == null)
                {
                    Debug.Log("还没有对话树，无法开始对话");
                    return;
                }
                
                node = Tree.StartNode as DialogueNode;
            }
            //如果有节点那就获取下一个节点
            else
            {
                node = node.NextNode();
            }

            //如果没有下一个节点了那就结束了
            if (node == null)
            {
                DialogueSystem.EndDialogue();
                return;
            }
            
            //切换到Running状态
            node.StartRunning();
            
            //获取到了节点，那就开始打印文本
            StartPrintText(node.GetText());
        }

        #endregion

        #region 对话文本效果控制

        /// <summary>
        /// 开始打字
        /// </summary>
        private void StartPrintText(string text)
        {
            
#if UNITY_EDITOR
            //如果游戏没有运行那就直接显示
            if (Application.isPlaying == false)
            {
                printText = text;
                dialogueText.text = text;
                return;
            }
#endif
            
            //如果正在打字那就结束之前那个打字的协程
            if (IsPrintingText)
            {
                StopCoroutine(printTextCoroutine);
            }
            
            //开启打字协程
            printTextCoroutine = StartCoroutine(PrintTextCoroutine(text));
        }
        /// <summary>
        /// 跳过文本打印
        /// </summary>
        private void SkipPrintText()
        {
            //如果不在打印文本就返回
            if(!IsPrintingText) return;
            
            //结束打印文本协程，然后更新状态，直接把要打印的文本完全显示
            StopCoroutine(printTextCoroutine);
            isPrintingText = false;
            dialogueText.text = printText;
        }
        /// <summary>
        /// 打字协程
        /// </summary>
        /// <returns></returns>
        private IEnumerator PrintTextCoroutine(string text)
        {
            //标记正在打印文本
            isPrintingText = true;
            
            //先清空文本
            dialogueText.text = "";
            //设置要打印的文本
            printText = text;
            //然后遍历开始打字
            for (int i = 0; i < printText.Length; i++)
            {
                float interval = 0.2f;
                dialogueText.text += printText[i];
                //等待一会打印下个字
                yield return CoroutineExtensions.WaitForSeconds(interval);
            }

            //标记打印文本结束
            isPrintingText = false;
            printTextCoroutine = null;
        }

        #endregion

        #region UI操作

        /// <summary>
        /// 点击事件
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData)
        {
            //点击UI面板就触发下一步
            Next();
        }

        #endregion
        

    }
}

