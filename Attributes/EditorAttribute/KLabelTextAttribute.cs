//****************** 代码文件声明 ***********************
//* 文件：KLabelTextAttribute
//* 作者：wheat
//* 创建时间：2024/05/01 20:21:15 星期三
//* 描述：用来修改GUI显示的Label
//*******************************************************

using System.Diagnostics;

namespace KFrame.Attributes
{
    //只有在Unity编辑器里编译
    [Conditional("UNITY_EDITOR")]
    public class KLabelTextAttribute : KGUIAttribute
    {
        /// <summary>
        /// GUI显示的文本
        /// </summary>
        public string Text;
        /// <summary>
        /// 用来修改GUI显示的Label
        /// </summary>
        /// <param name="text">要显示的内容</param>
        public KLabelTextAttribute(string text)
        {
            Text = text;
        }
    }
}

