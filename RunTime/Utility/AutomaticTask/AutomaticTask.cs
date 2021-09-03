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
                if (fieldInfos[i].IsDefined(typeof(ObjectPathAttribute), true))
                {
                    string path = fieldInfos[i].GetCustomAttribute<ObjectPathAttribute>().Path;
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
        /// 应用UI逻辑类的数据绑定
        /// </summary>
        /// <param name="uILogicBase">UI逻辑实例</param>
        /// <param name="fieldInfos">所有自动化字段</param>
        public static void ApplyDataBinding(UILogicBase uILogicBase, FieldInfo[] fieldInfos)
        {
            ApplyDataBinding(uILogicBase, uILogicBase, fieldInfos);
        }
        /// <summary>
        /// 应用实体逻辑类的数据绑定
        /// </summary>
        /// <param name="entityLogicBase">实体逻辑实例</param>
        /// <param name="fieldInfos">所有自动化字段</param>
        public static void ApplyDataBinding(EntityLogicBase entityLogicBase, FieldInfo[] fieldInfos)
        {
            ApplyDataBinding(entityLogicBase, entityLogicBase, fieldInfos);
        }
        /// <summary>
        /// 应用HT行为类的数据绑定
        /// </summary>
        /// <param name="behaviour">HT行为类实例</param>
        /// <param name="fieldInfos">所有自动化字段</param>
        public static void ApplyDataBinding(HTBehaviour behaviour, FieldInfo[] fieldInfos)
        {
            ApplyDataBinding(behaviour, behaviour, fieldInfos);
        }
        /// <summary>
        /// 应用FSM数据的数据绑定
        /// </summary>
        /// <param name="fsmData">FSM数据实例</param>
        /// <param name="fieldInfos">所有自动化字段</param>
        public static void ApplyDataBinding(FSMDataBase fsmData, FieldInfo[] fieldInfos)
        {
            ApplyDataBinding(fsmData, fsmData, fieldInfos);
        }
        /// <summary>
        /// 应用FSM参数的数据绑定
        /// </summary>
        /// <param name="fsmArgs">FSM参数实例</param>
        /// <param name="fieldInfos">所有自动化字段</param>
        public static void ApplyDataBinding(FSMArgsBase fsmArgs, FieldInfo[] fieldInfos)
        {
            ApplyDataBinding(fsmArgs, fsmArgs.StateMachine.CurrentData, fieldInfos);
        }
        /// <summary>
        /// 应用FSM状态的数据绑定
        /// </summary>
        /// <param name="fsmState">FSM状态实例</param>
        /// <param name="fieldInfos">所有自动化字段</param>
        public static void ApplyDataBinding(FiniteStateBase fsmState, FieldInfo[] fieldInfos)
        {
            ApplyDataBinding(fsmState, fsmState.StateMachine.CurrentData, fieldInfos);
        }
        /// <summary>
        /// 应用数据绑定
        /// </summary>
        /// <param name="instance">目标实例</param>
        /// <param name="dataInstance">数据Data实例</param>
        /// <param name="fieldInfos">所有自动化字段</param>
        private static void ApplyDataBinding(object instance, object dataInstance, FieldInfo[] fieldInfos)
        {
            //初始化数据 Data
            PropertyInfo dataInfo = dataInstance.GetType().GetProperty("Data", Flags);
            Type dataType = dataInfo.PropertyType;
            object dataValue = dataInfo.GetValue(dataInstance);
            if (dataValue == null)
            {
                dataValue = Activator.CreateInstance(dataType);
                dataInfo.SetValue(dataInstance, dataValue);
            }

            Type type = instance.GetType();
            Dictionary<string, FieldInfo> targetFieldsCache = new Dictionary<string, FieldInfo>();
            object[] args = new object[1];

            for (int i = 0; i < fieldInfos.Length; i++)
            {
                if (!fieldInfos[i].IsDefined(typeof(DataBindingAttribute), false))
                    continue;

                if (!fieldInfos[i].FieldType.IsSubclassOf(typeof(UIBehaviour)))
                {
                    Log.Error(string.Format("自动化任务：数据绑定失败，字段 {0}.{1} 的类型不支持数据绑定，只有 UnityEngine.EventSystems.UIBehaviour 的子类型支持数据绑定！", type.FullName, fieldInfos[i].Name));
                    continue;
                }

                //获取绑定的目标数据字段
                string target = fieldInfos[i].GetCustomAttribute<DataBindingAttribute>().Target;
                FieldInfo targetField = null;
                if (targetFieldsCache.ContainsKey(target))
                {
                    targetField = targetFieldsCache[target];
                }
                else
                {
                    targetField = dataType.GetField(target, Flags);
                    if (targetField != null)
                    {
                        targetFieldsCache.Add(target, targetField);
                    }
                }

                if (targetField == null)
                {
                    Log.Error(string.Format("自动化任务：数据绑定失败，未找到字段 {0}.{1} 绑定的目标数据字段 {2}.{3}！", type.FullName, fieldInfos[i].Name, dataType.FullName, target));
                    continue;
                }
                if (!(targetField.FieldType.BaseType.IsGenericType && targetField.FieldType.BaseType.GetGenericTypeDefinition() == typeof(BindableType<>)))
                {
                    Log.Error(string.Format("自动化任务：数据绑定失败，目标数据字段 {0}.{1} 并不是可绑定的数据类型 BindableType！", dataType.FullName, target));
                    continue;
                }

                //初始化目标数据字段
                object targetValue = targetField.GetValue(dataValue);
                if (targetValue == null)
                {
                    targetValue = Activator.CreateInstance(targetField.FieldType);
                    targetField.SetValue(dataValue, targetValue);
                }

                object controlValue = fieldInfos[i].GetValue(instance);
                if (controlValue == null)
                {
                    Log.Error(string.Format("自动化任务：数据绑定失败，字段 {0}.{1} 是个空引用！", type.FullName, fieldInfos[i].Name));
                    continue;
                }

                //绑定控件
                MethodInfo binding = targetField.FieldType.GetMethod("Binding", Flags);
                args[0] = controlValue;
                binding.Invoke(targetValue, args);
            }
        }
        /// <summary>
        /// 清空数据绑定
        /// </summary>
        /// <param name="dataInstance">数据Data实例</param>
        public static void ClearDataBinding(object dataInstance)
        {
            //清空数据 Data
            PropertyInfo dataInfo = dataInstance.GetType().GetProperty("Data", Flags);
            Type dataType = dataInfo.PropertyType;
            object dataValue = dataInfo.GetValue(dataInstance);
            if (dataValue == null)
                return;

            FieldInfo[] fieldInfos = dataType.GetFields(Flags);
            for (int i = 0; i < fieldInfos.Length; i++)
            {
                if (!(fieldInfos[i].FieldType.BaseType.IsGenericType && fieldInfos[i].FieldType.BaseType.GetGenericTypeDefinition() == typeof(BindableType<>)))
                    continue;

                object fieldValue = fieldInfos[i].GetValue(dataValue);
                if (fieldValue == null)
                    continue;

                //解除控件的绑定
                MethodInfo unbind = fieldInfos[i].FieldType.GetMethod("Unbind", Flags);
                unbind.Invoke(fieldValue, null);
            }

            dataInfo.SetValue(dataInstance, null);
        }
    }
}