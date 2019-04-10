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
        /// 添加数据集至数据集仓库
        /// </summary>
        public DataSet AddDataSet(DataSet dataSet)
        {
            Type type = dataSet.GetType();
            if (!_dataSets.ContainsKey(type))
            {
                _dataSets.Add(type, new List<DataSet>());
            }
            _dataSets[type].Add(dataSet);
            return dataSet;
        }
        /// <summary>
        /// 添加数据集至数据集仓库
        /// </summary>
        /// <param name="data">数据集初始化数据</param>
        public T AddDataSet<T>(JsonData data = null) where T : DataSet
        {
            Type type = typeof(T);
            if (!_dataSets.ContainsKey(type))
            {
                _dataSets.Add(type, new List<DataSet>());
            }
            DataSet dataSet = ScriptableObject.CreateInstance<T>();
            if (data != null)
            {
                dataSet.Fill(data);
            }
            _dataSets[type].Add(dataSet);
            return dataSet as T;
        }
        /// <summary>
        /// 添加数据集至数据集仓库
        /// </summary>
        /// <param name="data">数据集初始化数据</param>
        public DataSet AddDataSet(Type type, JsonData data = null)
        {
            if (type.BaseType == typeof(DataSet))
            {
                if (!_dataSets.ContainsKey(type))
                {
                    _dataSets.Add(type, new List<DataSet>());
                }
                DataSet dataSet = ScriptableObject.CreateInstance(type) as DataSet;
                if (data != null)
                {
                    dataSet.Fill(data);
                }
                _dataSets[type].Add(dataSet);
                return dataSet;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 从数据集仓库中移除数据集
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
        /// 获取所有数据集
        /// </summary>
        public List<T> GetAllDataSets<T>() where T : DataSet
        {
            Type type = typeof(T);
            if (_dataSets.ContainsKey(type))
            {
                List<T> dataSets = _dataSets[type].ConvertAllAS<T, DataSet>();
                return dataSets;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 获取所有数据集
        /// </summary>
        public List<DataSet> GetAllDataSets(Type type)
        {
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
        public List<T> GetAllDataSets<T>(Predicate<T> match) where T : DataSet
        {
            Type type = typeof(T);
            if (_dataSets.ContainsKey(type))
            {
                List<T> dataSets = _dataSets[type].ConvertAllAS<T, DataSet>();
                return dataSets.FindAll(match);
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 获取满足匹配条件的所有数据集
        /// </summary>
        public List<DataSet> GetAllDataSets(Type type, Predicate<DataSet> match)
        {
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
        public T GetDataSet<T>(Predicate<T> match) where T : DataSet
        {
            Type type = typeof(T);
            if (_dataSets.ContainsKey(type))
            {
                List<T> dataSets = _dataSets[type].ConvertAllAS<T, DataSet>();
                return dataSets.Find(match);
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 获取满足匹配条件的第一条数据集
        /// </summary>
        public DataSet GetDataSet(Type type, Predicate<DataSet> match)
        {
            if (_dataSets.ContainsKey(type))
            {
                return _dataSets[type].Find(match);
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 根据索引获取一条数据集
        /// </summary>
        public T GetDataSet<T>(int index) where T : DataSet
        {
            Type type = typeof(T);
            if (_dataSets.ContainsKey(type))
            {
                if (index >= 0 && index < _dataSets[type].Count)
                {
                    return _dataSets[type][index] as T;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 根据索引获取一条数据集
        /// </summary>
        public DataSet GetDataSet(Type type, int index)
        {
            if (_dataSets.ContainsKey(type))
            {
                if (index >= 0 && index < _dataSets[type].Count)
                {
                    return _dataSets[type][index];
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 根据先后顺序获取一条数据集
        /// </summary>
        /// <param name="isCut">是否同时在数据集仓库中移除该数据集</param>
        public T GetDataSet<T>(bool isCut = false) where T : DataSet
        {
            Type type = typeof(T);
            if (_dataSets.ContainsKey(type))
            {
                if (_dataSets[type].Count > 0)
                {
                    DataSet dataset = _dataSets[type][0];
                    if (isCut)
                    {
                        _dataSets[type].RemoveAt(0);
                    }
                    return dataset as T;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 根据先后顺序获取一条数据集
        /// </summary>
        /// <param name="isCut">是否同时在数据集仓库中移除该数据集</param>
        public DataSet GetDataSet(Type type, bool isCut = false)
        {
            if (_dataSets.ContainsKey(type))
            {
                if (_dataSets[type].Count > 0)
                {
                    DataSet dataset = _dataSets[type][0];
                    if (isCut)
                    {
                        _dataSets[type].RemoveAt(0);
                    }
                    return dataset;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 根据索引获取一条数据集
        /// </summary>
        /// <param name="isCut">是否同时在数据集仓库中移除该数据集</param>
        public T GetDataSet<T>(int index, bool isCut = false) where T : DataSet
        {
            Type type = typeof(T);
            if (_dataSets.ContainsKey(type))
            {
                if (index >= 0 && index < _dataSets[type].Count)
                {
                    DataSet dataset = _dataSets[type][index];
                    if (isCut)
                    {
                        _dataSets[type].RemoveAt(index);
                    }
                    return dataset as T;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 根据索引获取一条数据集
        /// </summary>
        /// <param name="isCut">是否同时在数据集仓库中移除该数据集</param>
        public DataSet GetDataSet(Type type, int index, bool isCut = false)
        {
            if (_dataSets.ContainsKey(type))
            {
                if (index >= 0 && index < _dataSets[type].Count)
                {
                    DataSet dataset = _dataSets[type][index];
                    if (isCut)
                    {
                        _dataSets[type].RemoveAt(index);
                    }
                    return dataset;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 获取数据集仓库中的数据集数量
        /// </summary>
        public int GetCount<T>() where T : DataSet
        {
            Type type = typeof(T);
            if (_dataSets.ContainsKey(type))
            {
                return _dataSets[type].Count;
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// 获取数据集仓库中的数据集数量
        /// </summary>
        public int GetCount(Type type)
        {
            if (_dataSets.ContainsKey(type))
            {
                return _dataSets[type].Count;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 清空指定的数据集仓库
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
        /// 清空指定的数据集仓库
        /// </summary>
        public void ClearDataSet(Type type)
        {
            if (_dataSets.ContainsKey(type))
            {
                _dataSets[type].Clear();
            }
        }
    }
}
