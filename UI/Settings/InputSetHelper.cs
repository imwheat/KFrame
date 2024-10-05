//****************** 代码文件申明 ***********************
//* 文件：InputSetHelper
//* 作者：wheat
//* 创建时间：2024/10/05 10:58:18 星期六
//* 描述：辅助一些按键设置
//*******************************************************


using System.Text;
using KFrame.Utilities;

namespace KFrame.UI
{
    public static class InputSetHelper
    {
        #region 工具方法
        
        /// <summary>
        /// 从绑定路径获取按键文本
        /// </summary>
        /// <param name="bindPath">按键绑定路径</param>
        /// <returns>按键文本</returns>
        public static string GetKeyFromBindPath(this string bindPath)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();
            bool skip = false;
            //遍历路径
            foreach (var c in bindPath)
            {
                //如果遇到'<'那就跳过这段文本直到'>'
                if (c == '<')
                {
                    skip = true;
                    continue;
                }
                if (skip)
                {
                    if (c == '>')
                    {
                        skip = false;
                    }

                    continue;
                }
                //遇到'/'表示隔了一个单词那就调整一下格式，清空一下继续读取下一个单词
                if (c == '/')
                {
                    sb.Append(sb2.ToString().GetNiceFormat());
                    sb2.Clear();
                    continue;
                }

                sb2.Append(c);
            }
            
            sb.Append(sb2.ToString().GetNiceFormat());
            sb2.Clear();
            
            //最后输出文本
            return sb.ToString();
        }

        #endregion
    }
}

