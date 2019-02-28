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

        public virtual void OnInit()
        {
        }

        public virtual void OnOpen(params object[] args)
        {
        }

        public virtual void OnClose()
        {
        }

        public virtual void OnDestroy()
        {
        }

        public virtual void OnUpdate()
        {
        }
    }
}
