using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 自定义模块管理者
    /// </summary>
    [DisallowMultipleComponent]
    [InternalModule(HTFrameworkModule.CustomModule)]
    public sealed class CustomModuleManager : InternalModuleBase
    {
        private Dictionary<string, CustomModuleBase> _customModules = new Dictionary<string, CustomModuleBase>();

        public override void OnInitialization()
        {
            base.OnInitialization();

            List<Type> types = ReflectionToolkit.GetTypesInRunTimeAssemblies();
            for (int i = 0; i < types.Count; i++)
            {
                if (types[i].IsSubclassOf(typeof(CustomModuleBase)))
                {
                    CustomModuleAttribute att = types[i].GetCustomAttribute<CustomModuleAttribute>();
                    if (att != null && att.IsEnable && !_customModules.ContainsKey(att.ModuleName))
                    {
                        _customModules.Add(att.ModuleName, Activator.CreateInstance(types[i]) as CustomModuleBase);
                    }
                }
            }

            foreach (var module in _customModules)
            {
                module.Value.OnInitialization();
            }
        }

        public override void OnPreparatory()
        {
            base.OnPreparatory();

            foreach (var module in _customModules)
            {
                module.Value.OnPreparatory();
            }
        }

        public override void OnRefresh()
        {
            base.OnRefresh();

            foreach (var module in _customModules)
            {
                if (module.Value.IsRunning)
                {
                    module.Value.OnRefresh();
                }
            }
        }

        public override void OnTermination()
        {
            base.OnTermination();

            foreach (var module in _customModules)
            {
                module.Value.OnTermination();
            }

            _customModules.Clear();
        }

        public override void OnPause()
        {
            base.OnPause();

            foreach (var module in _customModules)
            {
                module.Value.OnPause();
            }
        }

        public override void OnUnPause()
        {
            base.OnUnPause();

            foreach (var module in _customModules)
            {
                module.Value.OnUnPause();
            }
        }

        /// <summary>
        /// 自定义模块
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <returns>自定义模块</returns>
        public CustomModuleBase this[string moduleName]
        {
            get
            {
                if (_customModules.ContainsKey(moduleName))
                {
                    return _customModules[moduleName];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 终止指定的自定义模块
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        public void TerminationModule(string moduleName)
        {
            if (_customModules.ContainsKey(moduleName))
            {
                _customModules[moduleName].OnTermination();
                _customModules.Remove(moduleName);
            }
        }
    }
}