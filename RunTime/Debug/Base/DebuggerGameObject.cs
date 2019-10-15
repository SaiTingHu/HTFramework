using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 调试器游戏对象
    /// </summary>
    public sealed class DebuggerGameObject : IReference
    {
        /// <summary>
        /// 游戏对象
        /// </summary>
        public GameObject Target;
        /// <summary>
        /// 游戏对象名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 游戏对象所在的层
        /// </summary>
        public string Layer;
        /// <summary>
        /// 是否在层级面板展开
        /// </summary>
        public bool IsExpand = false;
        /// <summary>
        /// 父对象
        /// </summary>
        public DebuggerGameObject Parent;
        /// <summary>
        /// 子对象
        /// </summary>
        public List<DebuggerGameObject> Childrens = new List<DebuggerGameObject>();

        public void Reset()
        {
            Target = null;
            IsExpand = false;
            Parent = null;
            Childrens.Clear();
        }
    }
}