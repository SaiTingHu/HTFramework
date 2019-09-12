using System;
using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 数据集管理者
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class DataSetManager : ModuleManagerBase
    {
        private Dictionary<Type, List<DataSetBase>> _dataSets = new Dictionary<Type, List<DataSetBase>>();

        public override void OnInitialization()
        {
            base.OnInitialization();

            //注册所有数据集
            List<Type> types = GlobalTools.GetTypesInRunTimeAssemblies();
            for (int i = 0; i < types.Count; i++)
            {
                if (types[i].IsSubclassOf(typeof(DataSetBase)))
                {
                    _dataSets.Add(types[i], new List<DataSetBase>());
                }
            }
        }

        public override void OnTermination()
        {
            base.OnTermination();

            foreach (var dataset in _dataSets)
            {
                dataset.Value.Clear();
            }
            _dataSets.Clear();
        }

        /// <summary>
        /// 添加数据集至数据集仓库
        /// </summary>
        /// <param name="dataSet">数据集</param>
        public void AddDataSet(DataSetBase dataSet)
        {
            if (!dataSet)
            {
                throw new HTFrameworkException(HTFrameworkModule.DataSet, "不能添加空的数据集至仓库！");
            }

            Type type = dataSet.GetType();
            if (!_dataSets.ContainsKey(type))
            {
                _dataSets.Add(type, new List<DataSetBase>());
            }
            _dataSets[type].Add(dataSet);
        }
        /// <summary>
        /// 从数据集仓库中移除数据集
        /// </summary>
        /// <param name="dataSet">数据集</param>
        public void RemoveDataSet(DataSetBase dataSet)
        {
            if (!dataSet)
            {
                throw new HTFrameworkException(HTFrameworkModule.DataSet, "不能移除空的数据集！");
            }

            Type type = dataSet.GetType();
            if (!_dataSets.ContainsKey(type))
            {
                _dataSets.Add(type, new List<DataSetBase>());
            }
            if (_dataSets[type].Contains(dataSet))
            {
                _dataSets[type].Remove(dataSet);
            }
        }

        /// <summary>
        /// 新建数据集并添加至数据集仓库
        /// </summary>
        /// <typeparam name="T">数据集类型</typeparam>
        /// <param name="data">填充数据</param>
        /// <returns>新建的数据集</returns>
        public T CreateDataSet<T>(JsonData data = null) where T : DataSetBase
        {
            return CreateDataSet(typeof(T), data) as T;
        }
        /// <summary>
        /// 新建数据集并添加至数据集仓库
        /// </summary>
        /// <param name="type">数据集类型</param>
        /// <param name="data">填充数据</param>
        /// <returns>新建的数据集</returns>
        public DataSetBase CreateDataSet(Type type, JsonData data = null)
        {
            if (type.IsSubclassOf(typeof(DataSetBase)))
            {
                if (!_dataSets.ContainsKey(type))
                {
                    _dataSets.Add(type, new List<DataSetBase>());
                }
                DataSetBase dataSet = ScriptableObject.CreateInstance(type) as DataSetBase;
                if (data != null)
                {
                    dataSet.Fill(data);
                }
                _dataSets[type].Add(dataSet);
                return dataSet;
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.DataSet, "新建数据集失败：" + type.Name + " 并不是有效的数据集类型！");
            }
        }
        
        /// <summary>
        /// 获取某一类型的所有数据集
        /// </summary>
        /// <typeparam name="T">数据集类型</typeparam>
        /// <returns>数据集列表</returns>
        public List<T> GetAllDataSets<T>() where T : DataSetBase
        {
            return GetAllDataSets(typeof(T)).ConvertAllAS<T, DataSetBase>();
        }
        /// <summary>
        /// 获取某一类型的所有数据集
        /// </summary>
        /// <param name="type">数据集类型</param>
        /// <returns>数据集列表</returns>
        public List<DataSetBase> GetAllDataSets(Type type)
        {
            if (_dataSets.ContainsKey(type))
            {
                return _dataSets[type];
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.DataSet, "获取所有数据集失败：" + type.Name + " 并不是有效的数据集类型！");
            }
        }
        /// <summary>
        /// 获取某一类型的满足匹配条件的所有数据集
        /// </summary>
        /// <typeparam name="T">数据集类型</typeparam>
        /// <param name="match">匹配条件</param>
        /// <returns>数据集列表</returns>
        public List<T> GetAllDataSets<T>(Predicate<T> match) where T : DataSetBase
        {
            return GetAllDataSets(typeof(T), match as Predicate<DataSetBase>).ConvertAllAS<T, DataSetBase>();
        }
        /// <summary>
        /// 获取某一类型的满足匹配条件的所有数据集
        /// </summary>
        /// <param name="type">数据集类型</param>
        /// <param name="match">匹配条件</param>
        /// <returns>数据集列表</returns>
        public List<DataSetBase> GetAllDataSets(Type type, Predicate<DataSetBase> match)
        {
            if (_dataSets.ContainsKey(type))
            {
                return _dataSets[type].FindAll(match);
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.DataSet, "获取所有数据集失败：" + type.Name + " 并不是有效的数据集类型！");
            }
        }
        
        /// <summary>
        /// 获取某一类型的满足匹配条件的第一条数据集
        /// </summary>
        /// <typeparam name="T">数据集类型</typeparam>
        /// <param name="match">匹配条件</param>
        /// <returns>数据集</returns>
        public T GetDataSet<T>(Predicate<T> match) where T : DataSetBase
        {
            return GetDataSet(typeof(T), match as Predicate<DataSetBase>) as T;
        }
        /// <summary>
        /// 获取某一类型的满足匹配条件的第一条数据集
        /// </summary>
        /// <param name="type">数据集类型</param>
        /// <param name="match">匹配条件</param>
        /// <returns>数据集</returns>
        public DataSetBase GetDataSet(Type type, Predicate<DataSetBase> match)
        {
            if (_dataSets.ContainsKey(type))
            {
                return _dataSets[type].Find(match);
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.DataSet, "获取数据集失败：" + type.Name + " 并不是有效的数据集类型！");
            }
        }
        /// <summary>
        /// 根据先后顺序获取某一类型的第一条数据集
        /// </summary>
        /// <typeparam name="T">数据集类型</typeparam>
        /// <param name="isCut">是否同时在数据集仓库中移除该数据集</param>
        /// <returns>数据集</returns>
        public T GetDataSet<T>(bool isCut = false) where T : DataSetBase
        {
            return GetDataSet(typeof(T), isCut) as T;
        }
        /// <summary>
        /// 根据先后顺序获取某一类型的第一条数据集
        /// </summary>
        /// <param name="type">数据集类型</param>
        /// <param name="isCut">是否同时在数据集仓库中移除该数据集</param>
        /// <returns>数据集</returns>
        public DataSetBase GetDataSet(Type type, bool isCut = false)
        {
            if (_dataSets.ContainsKey(type))
            {
                if (_dataSets[type].Count > 0)
                {
                    DataSetBase dataset = _dataSets[type][0];
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
                throw new HTFrameworkException(HTFrameworkModule.DataSet, "获取数据集失败：" + type.Name + " 并不是有效的数据集类型！");
            }
        }
        /// <summary>
        /// 根据索引获取某一类型的一条数据集
        /// </summary>
        /// <typeparam name="T">数据集类型</typeparam>
        /// <param name="index">索引</param>
        /// <param name="isCut">是否同时在数据集仓库中移除该数据集</param>
        /// <returns>数据集</returns>
        public T GetDataSet<T>(int index, bool isCut = false) where T : DataSetBase
        {
            return GetDataSet(typeof(T), index, isCut) as T;
        }
        /// <summary>
        /// 根据索引获取某一类型的一条数据集
        /// </summary>
        /// <param name="type">数据集类型</param>
        /// <param name="index">索引</param>
        /// <param name="isCut">是否同时在数据集仓库中移除该数据集</param>
        /// <returns>数据集</returns>
        public DataSetBase GetDataSet(Type type, int index, bool isCut = false)
        {
            if (_dataSets.ContainsKey(type))
            {
                if (index >= 0 && index < _dataSets[type].Count)
                {
                    DataSetBase dataset = _dataSets[type][index];
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
                throw new HTFrameworkException(HTFrameworkModule.DataSet, "获取数据集失败：" + type.Name + " 并不是有效的数据集类型！");
            }
        }

        /// <summary>
        /// 获取数据集仓库中某一类型的数据集数量
        /// </summary>
        /// <typeparam name="T">数据集类型</typeparam>
        /// <returns>数据集数量</returns>
        public int GetCount<T>() where T : DataSetBase
        {
            return GetCount(typeof(T));
        }
        /// <summary>
        /// 获取数据集仓库中某一类型的数据集数量
        /// </summary>
        /// <param name="type">数据集类型</param>
        /// <returns>数据集数量</returns>
        public int GetCount(Type type)
        {
            if (_dataSets.ContainsKey(type))
            {
                return _dataSets[type].Count;
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.DataSet, "获取数据集数量失败：" + type.Name + " 并不是有效的数据集类型！");
            }
        }

        /// <summary>
        /// 清空某一类型的数据集仓库
        /// </summary>
        /// <typeparam name="T">数据集类型</typeparam>
        public void ClearDataSet<T>() where T : DataSetBase
        {
            ClearDataSet(typeof(T));
        }
        /// <summary>
        /// 清空某一类型的数据集仓库
        /// </summary>
        /// <param name="type">数据集类型</param>
        public void ClearDataSet(Type type)
        {
            if (_dataSets.ContainsKey(type))
            {
                _dataSets[type].Clear();
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.DataSet, "清空数据集失败：" + type.Name + " 并不是有效的数据集类型！");
            }
        }
    }
}