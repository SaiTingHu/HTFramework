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
        /// 应用UI逻辑类的依赖注入
        /// </summary>
        /// <param name="uILogicBase">UI逻辑实例</param>
        /// <param name="fieldInfos">所有自动化字段</param>
        /// <returns>完成注入的数量</returns>
        public static int ApplyInject(UILogicBase uILogicBase, FieldInfo[] fieldInfos)
        {
            return ApplyInject(uILogicBase, uILogicBase.UIEntity, fieldInfos);
        }
        /// <summary>
        /// 应用实体逻辑类的依赖注入
        /// </summary>
        /// <param name="entityLogicBase">实体逻辑实例</param>
        /// <param name="fieldInfos">所有自动化字段</param>
        /// <returns>完成注入的数量</returns>
        public static int ApplyInject(EntityLogicBase entityLogicBase, FieldInfo[] fieldInfos)
        {
            return ApplyInject(entityLogicBase, entityLogicBase.Entity, fieldInfos);
        }
        /// <summary>
        /// 应用HT行为类的依赖注入
        /// </summary>
        /// <param name="behaviour">HT行为类实例</param>
        /// <param name="fieldInfos">所有自动化字段</param>
        /// <returns>完成注入的数量</returns>
        public static int ApplyInject(HTBehaviour behaviour, FieldInfo[] fieldInfos)
        {
            return ApplyInject(behaviour, behaviour.gameObject, fieldInfos);
        }
        /// <summary>
        /// 应用FSM数据的依赖注入
        /// </summary>
        /// <param name="fsmData">FSM数据实例</param>
        /// <param name="fieldInfos">所有自动化字段</param>
        /// <returns>完成注入的数量</returns>
        public static int ApplyInject(FSMDataBase fsmData, FieldInfo[] fieldInfos)
        {
            return ApplyInject(fsmData, fsmData.StateMachine.gameObject, fieldInfos);
        }
        /// <summary>
        /// 应用FSM参数的依赖注入
        /// </summary>
        /// <param name="fsmArgs">FSM参数实例</param>
        /// <param name="fieldInfos">所有自动化字段</param>
        /// <returns>完成注入的数量</returns>
        public static int ApplyInject(FSMArgsBase fsmArgs, FieldInfo[] fieldInfos)
        {
            return ApplyInject(fsmArgs, fsmArgs.StateMachine.gameObject, fieldInfos);
        }
        /// <summary>
        /// 应用FSM状态的依赖注入
        /// </summary>
        /// <param name="fsmState">FSM状态实例</param>
        /// <param name="fieldInfos">所有自动化字段</param>
        /// <returns>完成注入的数量</returns>
        public static int ApplyInject(FiniteStateBase fsmState, FieldInfo[] fieldInfos)
        {
            return ApplyInject(fsmState, fsmState.StateMachine.gameObject, fieldInfos);
        }
        /// <summary>
        /// 应用依赖注入
        /// </summary>
        /// <param name="instance">目标实例</param>
        /// <param name="entity">目标在场景中的实体</param>
        /// <param name="fieldInfos">所有自动化字段</param>
        /// <returns>完成注入的数量</returns>
        private static int ApplyInject(object instance, GameObject entity, FieldInfo[] fieldInfos)
        {
            int count = 0;
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
                    else
                    {
                        Log.Error($"自动化任务：依赖注入（Path）失败，字段 {fieldInfos[i].Name} 的类型不被支持！");
                    }
                    count += 1;
                }
                else if (fieldInfos[i].IsDefined(typeof(InjectUIAttribute), true))
                {
                    Type type = fieldInfos[i].FieldType;
                    if (type.IsSubclassOf(typeof(UILogicBase)) && !type.IsAbstract)
                    {
                        fieldInfos[i].SetValue(instance, Main.m_UI.GetUI(type));
                    }
                    else
                    {
                        Log.Error($"自动化任务：依赖注入（UI）失败，字段 {fieldInfos[i].Name} 必须为UI逻辑类对象（UILogicBase），且不能为抽象类！");
                    }
                    count += 1;
                }
            }
            return count;
        }

        /// <summary>
        /// 应用数据绑定
        /// </summary>
        /// <param name="instance">目标实例</param>
        /// <param name="fieldInfos">所有自动化字段</param>
        /// <returns>完成绑定的数量</returns>
        public static int ApplyDataBinding(object instance, FieldInfo[] fieldInfos)
        {
            int count = 0;

            Type type = instance.GetType();
            Dictionary<string, FieldInfo> targetFieldsCache = new Dictionary<string, FieldInfo>();
            object[] args = new object[1];

            for (int i = 0; i < fieldInfos.Length; i++)
            {
                if (!fieldInfos[i].IsDefined(typeof(DataBindingAttribute), true))
                    continue;

                count += 1;

                if (!fieldInfos[i].FieldType.IsSubclassOf(typeof(UIBehaviour)))
                {
                    Log.Error($"自动化任务：数据绑定失败，字段 {type.FullName}.{fieldInfos[i].Name} 的类型不支持数据绑定，只有 UnityEngine.EventSystems.UIBehaviour 的子类型支持数据绑定！");
                    continue;
                }

                //获取绑定的目标数据模型
                DataBindingAttribute attribute = fieldInfos[i].GetCustomAttribute<DataBindingAttribute>();
                if (attribute.TargetType == null)
                {
                    Log.Error($"自动化任务：数据绑定失败，字段 {type.FullName}.{fieldInfos[i].Name} 绑定的目标数据模型类不存在！");
                    continue;
                }
                DataModelBase dataModel = Main.Current.GetDataModel(attribute.TargetType);
                if (dataModel == null)
                {
                    Log.Error($"自动化任务：数据绑定失败，数据模型类 {attribute.TargetType.FullName} 未添加至当前环境！");
                    continue;
                }

                //获取绑定的目标数据模型的数据字段
                string fieldName = $"{attribute.TargetType.FullName}.{attribute.TargetField}";
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
                    Log.Error($"自动化任务：数据绑定失败，未找到字段 {type.FullName}.{fieldInfos[i].Name} 绑定的目标数据字段 {fieldName}！");
                    continue;
                }
                if (!(dataField.FieldType.BaseType.IsGenericType && dataField.FieldType.BaseType.GetGenericTypeDefinition() == typeof(BindableType<>)))
                {
                    Log.Error($"自动化任务：数据绑定失败，目标数据字段 {fieldName} 并不是可绑定的数据类型 BindableType！");
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
                    Log.Error($"自动化任务：数据绑定失败，字段 {type.FullName}.{fieldInfos[i].Name} 是个空引用！");
                    continue;
                }

                //绑定UI控件
                MethodInfo binding = dataField.FieldType.GetMethod("Binding", Flags);
                args[0] = controlValue;
                binding.Invoke(dataValue, args);
            }

            return count;
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