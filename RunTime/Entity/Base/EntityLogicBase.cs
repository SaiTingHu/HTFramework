using System;
using System.Reflection;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 实体逻辑基类
    /// </summary>
    public abstract class EntityLogicBase : IReference
    {
        /// <summary>
        /// 实体名称
        /// </summary>
        public string Name { get; internal set; }
        /// <summary>
        /// 实体
        /// </summary>
        public GameObject Entity { get; internal set; }

        /// <summary>
        /// 实体是否显示
        /// </summary>
        public bool IsShowed
        {
            get
            {
                return Entity ? Entity.activeSelf : false;
            }
        }

        /// <summary>
        /// 是否启用自动化，这将造成反射的性能消耗
        /// </summary>
        protected virtual bool IsAutomate => true;

        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void OnInit()
        {
            AutomaticTask();
        }
        /// <summary>
        /// 显示实体
        /// </summary>
        public virtual void OnShow()
        {
        }
        /// <summary>
        /// 隐藏实体
        /// </summary>
        public virtual void OnHide()
        {
        }
        /// <summary>
        /// 销毁实体
        /// </summary>
        public virtual void OnDestroy()
        {
        }
        /// <summary>
        /// 实体逻辑刷新
        /// </summary>
        public virtual void OnUpdate()
        {
        }
        /// <summary>
        /// 重置实体
        /// </summary>
        public virtual void Reset()
        {
        }
        /// <summary>
        /// 自动化任务
        /// </summary>
        private void AutomaticTask()
        {
            if (!IsAutomate)
                return;

            //应用对象路径定义
            FieldInfo[] infos = GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            for (int i = 0; i < infos.Length; i++)
            {
                if (infos[i].IsDefined(typeof(ObjectPathAttribute), true))
                {
                    string path = infos[i].GetCustomAttribute<ObjectPathAttribute>().Path;
                    Type type = infos[i].FieldType;
                    if (type == typeof(GameObject))
                    {
                        infos[i].SetValue(this, Entity.FindChildren(path));
                    }
                    else if (type.IsSubclassOf(typeof(Component)))
                    {
                        GameObject obj = Entity.FindChildren(path);
                        infos[i].SetValue(this, obj != null ? obj.GetComponent(type) : null);
                    }
                }
            }
        }
    }
}