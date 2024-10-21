//****************** 代码文件申明 ***********************
//* 文件：NodeTransition
//* 作者：wheat
//* 创建时间：2024/10/21 08:44:59 星期一
//* 描述：节点的切换，包括条件和节点
//*******************************************************

using System.Collections.Generic;

namespace KFrame.Systems.NodeSystem
{
    [System.Serializable]
    public class NodeTransition
    {
        /// <summary>
        /// 切换节点
        /// </summary>
        public Node node;
        /// <summary>
        /// 节点切换条件
        /// </summary>
        public List<NodeTransitionCondition> conditions = new();
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="node">绑定的节点</param>
        public NodeTransition(Node node)
        {
            this.node = node;
        }
        /// <summary>
        /// 判断能否切换
        /// </summary>
        /// <returns></returns>
        public bool CanTranslate()
        {
            if (conditions.Count == 0) return true;


            return true;
        }
    }
}

