using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 数据集管理者
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class DataSetManager : ModuleManager
    {
        private Dictionary<Type, List<DataSet>> _dataSets = new Dictionary<Type, List<DataSet>>();

        public override void Initialization()
        {
            //注册所有数据集
            Assembly assembly = Assembly.GetAssembly(typeof(DataSet));
            Type[] types = assembly.GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                if (types[i].BaseType == typeof(DataSet))
                {
                    _dataSets.Add(types[i], new List<DataSet>());
                }
            }
        }

        /// <summary>
        /// 添加数据集
        /// </summary>
        public void AddDataSet(DataSet dataSet)
        {
            Type type = dataSet.GetType();
            if (!_dataSets.ContainsKey(type))
            {
                _dataSets.Add(type, new List<DataSet>());
            }
            _dataSets[type].Add(dataSet);
        }

        /// <summary>
        /// 移除数据集
        /// </summary>
        public void RemoveDataSet(DataSet dataSet)
        {
            Type type = dataSet.GetType();
            if (!_dataSets.ContainsKey(type))
            {
                _dataSets.Add(type, new List<DataSet>());
            }
            if (_dataSets[type].Contains(dataSet))
            {
                _dataSets[type].Remove(dataSet);
            }
        }

        /// <summary>
        /// 清空指定的数据集
        /// </summary>
        public void ClearDataSet<T>() where T : DataSet
        {
            Type type = typeof(T);
            if (_dataSets.ContainsKey(type))
            {
                _dataSets[type].Clear();
            }
        }

        /// <summary>
        /// 获取所有数据集
        /// </summary>
        public List<DataSet> GetAllDataSets<T>() where T : DataSet
        {
            Type type = typeof(T);
            if (_dataSets.ContainsKey(type))
            {
                return _dataSets[type];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取满足匹配条件的所有数据集
        /// </summary>
        public List<DataSet> GetAllDataSets<T>(Predicate<DataSet> match) where T : DataSet
        {
            Type type = typeof(T);
            if (_dataSets.ContainsKey(type))
            {
                return _dataSets[type].FindAll(match);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取满足匹配条件的第一条数据集
        /// </summary>
        public DataSet GetDataSet<T>(Predicate<DataSet> match) where T : DataSet
        {
            Type type = typeof(T);
            if (_dataSets.ContainsKey(type))
            {
                return _dataSets[type].Find(match);
            }
            else
            {
                return null;
            }
        }
    }
}
