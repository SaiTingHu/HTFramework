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
        /// 是否支持数据驱动
        /// </summary>
        public bool IsSupportedDataDriver
        {
            get
            {
                return this is IDataDriver;
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
            if (IsAutomate)
            {
                FieldInfo[] fieldInfos = AutomaticTask.GetAutomaticFields(GetType());
                AutomaticTask.ApplyObjectPath(this, fieldInfos);

                if (IsSupportedDataDriver)
                {
                    AutomaticTask.ApplyDataBinding(this, fieldInfos);
                }
            }
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
    }
}