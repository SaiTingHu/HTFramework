using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// UI逻辑基类
    /// </summary>
    public abstract class UILogic
    {
        /// <summary>
        /// UI实体
        /// </summary>
        public GameObject UIEntity;

        /// <summary>
        /// UI是否打开
        /// </summary>
        public bool IsOpened
        {
            get
            {
                return UIEntity ? UIEntity.activeSelf : false;
            }
        }

        /// <summary>
        /// UI实体是否已创建
        /// </summary>
        public bool IsCreated
        {
            get
            {
                return UIEntity;
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void OnInit()
        {
        }

        /// <summary>
        /// 打开UI
        /// </summary>
        public virtual void OnOpen(params object[] args)
        {
        }

        /// <summary>
        /// 关闭UI
        /// </summary>
        public virtual void OnClose()
        {
        }

        /// <summary>
        /// 销毁UI
        /// </summary>
        public virtual void OnDestroy()
        {
        }

        /// <summary>
        /// UI逻辑刷新
        /// </summary>
        public virtual void OnUpdate()
        {
        }

        /// <summary>
        /// 打开自己
        /// </summary>
        protected virtual void Open()
        {
        }

        /// <summary>
        /// 关闭自己
        /// </summary>
        protected virtual void Close()
        {
        }
    }
}
