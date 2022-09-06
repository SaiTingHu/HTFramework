using System;
using System.Collections.Generic;
using System.Reflection;

namespace HT.Framework
{
    /// <summary>
    /// 默认的自定义模块管理器助手
    /// </summary>
    public sealed class DefaultCustomModuleHelper : ICustomModuleHelper
    {
        /// <summary>
        /// 自定义模块管理器
        /// </summary>
        public IModuleManager Module { get; set; }
        
        private Dictionary<string, CustomModuleBase> _customModules = new Dictionary<string, CustomModuleBase>();
        private List<CustomModuleBase> _customModulesList = new List<CustomModuleBase>();

        /// <summary>
        /// 初始化助手
        /// </summary>
        public void OnInit()
        {
            List<Type> types = ReflectionToolkit.GetTypesInRunTimeAssemblies(type =>
            {
                return type.IsSubclassOf(typeof(CustomModuleBase)) && !type.IsAbstract;
            });
            for (int i = 0; i < types.Count; i++)
            {
                CustomModuleAttribute att = types[i].GetCustomAttribute<CustomModuleAttribute>();
                if (att != null && att.IsEnable && !_customModules.ContainsKey(att.ModuleName))
                {
                    CustomModuleBase customModule = Activator.CreateInstance(types[i]) as CustomModuleBase;
                    customModule.Name = att.ModuleName;
                    _customModules.Add(att.ModuleName, customModule);
                    _customModulesList.Add(customModule);
                }
            }

            _customModulesList.Sort((a, b) => { return a.Priority.CompareTo(b.Priority); });

            for (int i = 0; i < _customModulesList.Count; i++)
            {
                _customModulesList[i].OnInit();
            }
        }
        /// <summary>
        /// 助手准备工作
        /// </summary>
        public void OnReady()
        {
            for (int i = 0; i < _customModulesList.Count; i++)
            {
                _customModulesList[i].OnReady();
            }
        }
        /// <summary>
        /// 刷新助手
        /// </summary>
        public void OnUpdate()
        {
            for (int i = 0; i < _customModulesList.Count; i++)
            {
                if (_customModulesList[i].IsRunning)
                {
                    _customModulesList[i].OnUpdate();
                }
            }
        }
        /// <summary>
        /// 终结助手
        /// </summary>
        public void OnTerminate()
        {
            for (int i = 0; i < _customModulesList.Count; i++)
            {
                _customModulesList[i].OnTerminate();
            }

            _customModules.Clear();
            _customModulesList.Clear();
        }
        /// <summary>
        /// 暂停助手
        /// </summary>
        public void OnPause()
        {
            for (int i = 0; i < _customModulesList.Count; i++)
            {
                _customModulesList[i].OnPause();
            }
        }
        /// <summary>
        /// 恢复助手
        /// </summary>
        public void OnResume()
        {
            for (int i = 0; i < _customModulesList.Count; i++)
            {
                _customModulesList[i].OnResume();
            }
        }

        /// <summary>
        /// 获取指定的自定义模块
        /// </summary>
        /// <param name="name">模块名称</param>
        /// <returns>模块实例</returns>
        public CustomModuleBase GetCustomModule(string name)
        {
            if (_customModules.ContainsKey(name))
            {
                return _customModules[name];
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 获取所有的自定义模块
        /// </summary>
        public List<CustomModuleBase> GetAllCustomModule()
        {
            return _customModulesList;
        }
        /// <summary>
        /// 终止指定的自定义模块
        /// </summary>
        /// <param name="name">模块名称</param>
        public void TerminationModule(string name)
        {
            if (_customModules.ContainsKey(name))
            {
                _customModules[name].OnTerminate();
                _customModulesList.Remove(_customModules[name]);
                _customModules.Remove(name);
            }
        }
    }
}