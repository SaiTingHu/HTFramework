using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HT.Framework
{
    /// <summary>
    /// 自动化任务
    /// </summary>
    internal static class AutomaticTask
    {
        private static readonly BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        /// <summary>
        /// 获取目标类型的所有自动化字段
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <returns>所有自动化字段</returns>
        public static FieldInfo[] GetAutomaticFields(Type type)
        {
            return type.GetFields(Flags);
        }

        /// <summary>
        /// 应用UI逻辑类的对象路径定义
        /// </summary>
        /// <param name="uILogicBase">UI逻辑实例</param>
        /// <param name="fieldInfos">所有自动化字段</param>
        public static void ApplyObjectPath(UILogicBase uILogicBase, FieldInfo[] fieldInfos)
        {
            ApplyObjectPath(uILogicBase, uILogicBase.UIEntity, fieldInfos);
        }
        /// <summary>
        /// 应用实体逻辑类的对象路径定义
        /// </summary>
        /// <param name="entityLogicBase">实体逻辑实例</param>
        /// <param name="fieldInfos">所有自动化字段</param>
        public static void ApplyObjectPath(EntityLogicBase entityLogicBase, FieldInfo[] fieldInfos)
        {
            ApplyObjectPath(entityLogicBase, entityLogicBase.Entity, fieldInfos);
        }
        /// <summary>
        /// 应用HT行为类的对象路径定义
        /// </summary>
        /// <param name="behaviour">HT行为类实例</param>
        /// <param name="fieldInfos">所有自动化字段</param>
        public static void ApplyObjectPath(HTBehaviour behaviour, FieldInfo[] fieldInfos)
        {
            ApplyObjectPath(behaviour, behaviour.gameObject, fieldInfos);
        }
        /// <summary>
        /// 应用FSM数据的对象路径定义
        /// </summary>
        /// <param name="fsmData">FSM数据实例</param>
        /// <param name="fieldInfos">所有自动化字段</param>
        public static void ApplyObjectPath(FSMDataBase fsmData, FieldInfo[] fieldInfos)
        {
            ApplyObjectPath(fsmData, fsmData.StateMachine.gameObject, fieldInfos);
        }
        /// <summary>
        /// 应用FSM参数的对象路径定义
        /// </summary>
        /// <param name="fsmArgs">FSM参数实例</param>
        /// <param name="fieldInfos">所有自动化字段</param>
        public static void ApplyObjectPath(FSMArgsBase fsmArgs, FieldInfo[] fieldInfos)
        {
            ApplyObjectPath(fsmArgs, fsmArgs.StateMachine.gameObject, fieldInfos);
        }
        /// <summary>
        /// 应用FSM状态的对象路径定义
        /// </summary>
        /// <param name="fsmState">FSM状态实例</param>
        /// <param name="fieldInfos">所有自动化字段</param>
        public static void ApplyObjectPath(FiniteStateBase fsmState, FieldInfo[] fieldInfos)
        {
            ApplyObjectPath(fsmState, fsmState.StateMachine.gameObject, fieldInfos);
        }
        /// <summary>
        /// 应用对象路径定义
        /// </summary>
        /// <param name="instance">目标实例</param>
        /// <param name="entity">目标在场景中的实体</param>
        /// <param name="fieldInfos">所有自动化字段</param>
        private static void ApplyObjectPath(object instance, GameObject entity, FieldInfo[] fieldInfos)
        {
            for (int i = 0; i < fieldInfos.Length; i++)
            {
                if (fieldInfos[i].IsDefined(typeof(InjectPathAttribute), true))
                {
                    string path = fieldInfos[i].GetCustomAttribute<InjectPathAttribute>().Path;
                    Type type = fieldInfos[i].FieldType;
                    if (type == typeof(GameObject))
                    {
                        fieldInfos[i].SetValue(instance, entity.FindChildren(path));
                    }
                    else if (type.IsSubclassOf(typeof(Component)))
                    {
                        GameObject obj = entity.FindChildren(path);
                        fieldInfos[i].SetValue(instance, obj != null ? obj.GetComponent(type) : null);
                    }
                }
            }
        }

        /// <summary>
        /// 应用数据绑定
        /// </summary>
        /// <param name="instance">目标实例</param>
        /// <param name="fieldInfos">所有自动化字段</param>
        public static void ApplyDataBinding(object instance, FieldInfo[] fieldInfos)
        {
            Type type = instance.GetType();
            Dictionary<string, FieldInfo> targetFieldsCache = new Dictionary<string, FieldInfo>();
            object[] args = new object[1];

            for (int i = 0; i < fieldInfos.Length; i++)
            {
                if (!fieldInfos[i].IsDefined(typeof(DataBindingAttribute), true))
                    continue;

                if (!fieldInfos[i].FieldType.IsSubclassOf(typeof(UIBehaviour)))
                {
                    Log.Error(string.Format("自动化任务：数据绑定失败，字段 {0}.{1} 的类型不支持数据绑定，只有 UnityEngine.EventSystems.UIBehaviour 的子类型支持数据绑定！", type.FullName, fieldInfos[i].Name));
                    continue;
                }

                //获取绑定的目标数据模型
                DataBindingAttribute attribute = fieldInfos[i].GetCustomAttribute<DataBindingAttribute>();
                if (attribute.TargetType == null)
                {
                    Log.Error(string.Format("自动化任务：数据绑定失败，字段 {0}.{1} 绑定的目标数据模型类不存在！", type.FullName, fieldInfos[i].Name));
                    continue;
                }
                DataModelBase dataModel = Main.Current.GetDataModel(attribute.TargetType);
                if (dataModel == null)
                {
                    Log.Error(string.Format("自动化任务：数据绑定失败，数据模型类 {0} 未添加至当前环境！", attribute.TargetType.FullName));
                    continue;
                }

                //获取绑定的目标数据模型的数据字段
                string fieldName = attribute.TargetType.FullName + "." + attribute.TargetField;
                FieldInfo dataField = null;
                if (targetFieldsCache.ContainsKey(fieldName))
                {
                    dataField = targetFieldsCache[fieldName];
                }
                else
                {
                    dataField = attribute.TargetType.GetField(attribute.TargetField, Flags);
                    if (dataField != null)
                    {
                        targetFieldsCache.Add(fieldName, dataField);
                    }
                }
                if (dataField == null)
                {
                    Log.Error(string.Format("自动化任务：数据绑定失败，未找到字段 {0}.{1} 绑定的目标数据字段 {2}！", type.FullName, fieldInfos[i].Name, fieldName));
                    continue;
                }
                if (!(dataField.FieldType.BaseType.IsGenericType && dataField.FieldType.BaseType.GetGenericTypeDefinition() == typeof(BindableType<>)))
                {
                    Log.Error(string.Format("自动化任务：数据绑定失败，目标数据字段 {0} 并不是可绑定的数据类型 BindableType！", fieldName));
                    continue;
                }

                //初始化目标数据字段
                object dataValue = dataField.GetValue(dataModel);
                if (dataValue == null)
                {
                    dataValue = Activator.CreateInstance(dataField.FieldType);
                    dataField.SetValue(dataModel, dataValue);
                }

                //获取UI控件
                object controlValue = fieldInfos[i].GetValue(instance);
                if (controlValue == null)
                {
                    Log.Error(string.Format("自动化任务：数据绑定失败，字段 {0}.{1} 是个空引用！", type.FullName, fieldInfos[i].Name));
                    continue;
                }

                //绑定UI控件
                MethodInfo binding = dataField.FieldType.GetMethod("Binding", Flags);
                args[0] = controlValue;
                binding.Invoke(dataValue, args);
            }
        }
        /// <summary>
        /// 清空数据绑定
        /// </summary>
        /// <param name="dataModel">数据模型实例</param>
        public static void ClearDataBinding(DataModelBase dataModel)
        {
            FieldInfo[] fieldInfos = dataModel.GetType().GetFields(Flags);
            for (int i = 0; i < fieldInfos.Length; i++)
            {
                if (!(fieldInfos[i].FieldType.BaseType.IsGenericType && fieldInfos[i].FieldType.BaseType.GetGenericTypeDefinition() == typeof(BindableType<>)))
                    continue;

                object fieldValue = fieldInfos[i].GetValue(dataModel);
                if (fieldValue == null)
                    continue;

                //解除控件的绑定
                MethodInfo unbind = fieldInfos[i].FieldType.GetMethod("Unbind", Flags);
                unbind.Invoke(fieldValue, null);
            }
        }
    }
}