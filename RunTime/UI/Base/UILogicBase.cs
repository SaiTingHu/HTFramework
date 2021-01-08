using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

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
        public GameObject UIEntity { get; internal set; }
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
        protected virtual bool IsAutomate => true;

        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void OnInit()
        {
            AutomaticTask();
        }
        /// <summary>
        /// 打开UI
        /// </summary>
        /// <param name="args">可选参数</param>
        public virtual void OnOpen(params object[] args)
        {
            Main.m_Event.Throw(Main.m_ReferencePool.Spawn<EventUIOpened>().Fill(this));
        }
        /// <summary>
        /// 关闭UI
        /// </summary>
        public virtual void OnClose()
        {
            Main.m_Event.Throw(Main.m_ReferencePool.Spawn<EventUIClosed>().Fill(this));
        }
        /// <summary>
        /// 销毁UI
        /// </summary>
        public virtual void OnDestroy()
        {
            //销毁绑定的数据
            if (IsSupportedDataDriver)
            {
                PropertyInfo dataInfo = GetType().GetProperty("Data", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                dataInfo.SetValue(this, null);
            }
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
        /// 自动化任务
        /// </summary>
        private void AutomaticTask()
        {
            if (!IsAutomate)
                return;

            Type uiType = GetType();
            FieldInfo[] infos = uiType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            //应用对象路径定义
            for (int i = 0; i < infos.Length; i++)
            {
                if (!infos[i].IsDefined(typeof(ObjectPathAttribute), true))
                {
                    continue;
                }

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
            
            //进行数据绑定
            if (IsSupportedDataDriver)
            {
                PropertyInfo dataInfo = uiType.GetProperty("Data", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                Type dataType = dataInfo.PropertyType;
                object dataValue = dataInfo.GetValue(this);
                if (dataValue == null)
                {
                    dataValue = Activator.CreateInstance(dataType);
                    dataInfo.SetValue(this, dataValue);
                }

                object[] args = new object[1];

                for (int i = 0; i < infos.Length; i++)
                {
                    if (!infos[i].IsDefined(typeof(DataBindingAttribute), false))
                    {
                        continue;
                    }

                    if (!infos[i].FieldType.IsSubclassOf(typeof(UIBehaviour)))
                    {
                        Log.Error(string.Format("数据驱动器：数据绑定失败，字段 {0}.{1} 的类型不支持数据绑定，只有 UnityEngine.EventSystems.UIBehaviour 类型支持数据绑定！", uiType.FullName, infos[i].Name));
                        continue;
                    }

                    string target = infos[i].GetCustomAttribute<DataBindingAttribute>().Target;
                    FieldInfo targetField = dataType.GetField(target, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (targetField == null)
                    {
                        Log.Error(string.Format("数据驱动器：数据绑定失败，未找到字段 {0}.{1} 绑定的目标数据字段 {2}.{3}！", uiType.FullName, infos[i].Name, dataType.FullName, target));
                        continue;
                    }
                    if (!(targetField.FieldType.BaseType.IsGenericType && targetField.FieldType.BaseType.GetGenericTypeDefinition() == typeof(BindableType<>)))
                    {
                        Log.Error(string.Format("数据驱动器：数据绑定失败，目标数据字段 {0}.{1} 并不是可绑定的数据类型 BindableType！", dataType.FullName, target));
                        continue;
                    }

                    object targetValue = targetField.GetValue(dataValue);
                    if (targetValue == null)
                    {
                        targetValue = Activator.CreateInstance(targetField.FieldType);
                        targetField.SetValue(dataValue, targetValue);
                    }

                    object controlValue = infos[i].GetValue(this);
                    if (controlValue == null)
                    {
                        Log.Error(string.Format("数据驱动器：数据绑定失败，字段 {0}.{1} 是个空引用！", uiType.FullName, infos[i].Name));
                        continue;
                    }

                    MethodInfo binding = targetField.FieldType.GetMethod("Binding", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    args[0] = controlValue;
                    binding.Invoke(targetValue, args);
                }
            }
        }
    }
}