using System;
using System.Reflection;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// HT框架公共行为脚本基类
    /// </summary>
    public abstract class HTBehaviour : MonoBehaviour
    {
        /// <summary>
        /// 是否支持数据驱动
        /// </summary>
        public bool IsSupportedDataDriver
        {
            get
            {
                return Array.Exists(GetType().GetInterfaces(), t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IDataDriver<>));
            }
        }
        /// <summary>
        /// 是否启用自动化，这将造成反射的性能消耗
        /// </summary>
        protected virtual bool IsAutomate => false;

        protected virtual void Awake()
        {
            useGUILayout = false;

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
        protected virtual void OnDestroy()
        {
            if (IsAutomate && IsSupportedDataDriver)
            {
                AutomaticTask.ClearDataBinding(this);
            }
        }
    }
}