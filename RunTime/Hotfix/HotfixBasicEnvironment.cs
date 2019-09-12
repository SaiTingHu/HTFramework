using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 热更新基本环境
    /// </summary>
    public abstract class HotfixBasicEnvironment
    {
        private Dictionary<Type, HotfixProcedureBase> _procedureInstances = new Dictionary<Type, HotfixProcedureBase>();
        private HotfixProcedureBase _currentProcedure;
        private Type _entranceProcedure;
        private float _timer = 0;

        public HotfixBasicEnvironment(Assembly hotfixAssembly)
        {
            Type[] types = hotfixAssembly.GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                if (types[i].IsSubclassOf(typeof(HotfixProcedureBase)))
                {
                    HotfixProcedureStateAttribute hpsa = types[i].GetCustomAttribute<HotfixProcedureStateAttribute>();
                    if (hpsa != null)
                    {
                        if (hpsa.State == HotfixProcedureState.Entrance)
                        {
                            _entranceProcedure = types[i];
                            if (!_procedureInstances.ContainsKey(types[i]))
                            {
                                _procedureInstances.Add(types[i], Activator.CreateInstance(types[i]) as HotfixProcedureBase);
                            }
                        }
                        else if (hpsa.State == HotfixProcedureState.Normal)
                        {
                            if (!_procedureInstances.ContainsKey(types[i]))
                            {
                                _procedureInstances.Add(types[i], Activator.CreateInstance(types[i]) as HotfixProcedureBase);
                            }
                        }
                    }
                }
            }

            foreach (var procedureInstance in _procedureInstances)
            {
                procedureInstance.Value.OnInit();
            }

            if (_entranceProcedure != null)
            {
                _currentProcedure = _procedureInstances[_entranceProcedure];
                _currentProcedure.OnEnter();
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Hotfix, "进入热更新流程失败：未指定热更新入口流程！");
            }

            Main.m_Hotfix.UpdateHotfixLogicEvent += UpdateHotfixLogic;
        }

        public void UpdateHotfixLogic()
        {
            if (_currentProcedure != null)
            {
                _currentProcedure.OnUpdate();

                if (_timer < 1)
                {
                    _timer += Time.deltaTime;
                }
                else
                {
                    _timer = 0;
                    _currentProcedure.OnUpdateSecond();
                }
            }
        }

        /// <summary>
        /// 切换热更新流程
        /// </summary>
        /// <typeparam name="T">流程类型</typeparam>
        public void SwitchProcedure<T>() where T : HotfixProcedureBase
        {
            if (_procedureInstances.ContainsKey(typeof(T)))
            {
                if (_currentProcedure == _procedureInstances[typeof(T)])
                {
                    return;
                }

                if (_currentProcedure != null)
                {
                    _currentProcedure.OnLeave();
                }

                _currentProcedure = _procedureInstances[typeof(T)];
                _currentProcedure.OnEnter();
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Hotfix, "切换热更新流程失败：不存在流程 " + typeof(T).Name + " 或者流程未激活！");
            }
        }
    }
}