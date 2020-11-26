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
        /// 是否启用自动化，这将造成反射的性能消耗
        /// </summary>
        protected virtual bool IsAutomate => false;

        protected virtual void Awake()
        {
            AutomaticTask();
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
                        infos[i].SetValue(this, transform.FindChildren(path));
                    }
                    else if (type.IsSubclassOf(typeof(Component)))
                    {
                        GameObject obj = transform.FindChildren(path);
                        infos[i].SetValue(this, obj != null ? obj.GetComponent(type) : null);
                    }
                }
            }
        }
    }
}