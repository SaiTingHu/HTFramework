using System;
using System.Reflection;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// UI逻辑基类
    /// </summary>
    public abstract class UILogicBase
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
            ApplyObjectPath();
        }

        /// <summary>
        /// 打开UI
        /// </summary>
        /// <param name="args">可选参数</param>
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

        /// <summary>
        /// 应用对象路径定义
        /// </summary>
        private void ApplyObjectPath()
        {
            FieldInfo[] infos = GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            for (int i = 0; i < infos.Length; i++)
            {
                if (infos[i].IsDefined(typeof(ObjectPathAttribute), true))
                {
                    string path = infos[i].GetCustomAttribute<ObjectPathAttribute>().Path;
                    Type type = infos[i].FieldType;
                    if (type == typeof(GameObject))
                    {
                        infos[i].SetValue(this, UIEntity.FindChildren(path));
                    }
                    else if (type.IsSubclassOf(typeof(Component)))
                    {
                        GameObject obj = UIEntity.FindChildren(path);
                        infos[i].SetValue(this, obj != null ? obj.GetComponent(type) : null);
                    }
                }
            }
        }
    }
}