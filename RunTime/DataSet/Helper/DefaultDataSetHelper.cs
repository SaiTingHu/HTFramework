using System;
using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 默认的数据集管理器助手
    /// </summary>
    public sealed class DefaultDataSetHelper : IDataSetHelper
    {
        /// <summary>
        /// 数据集管理器
        /// </summary>
        public IModuleManager Module { get; set; }
        /// <summary>
        /// 所有数据集
        /// </summary>
        public Dictionary<Type, List<DataSetBase>> DataSets { get; private set; } = new Dictionary<Type, List<DataSetBase>>();
        
        /// <summary>
        /// 初始化助手
        /// </summary>
        public void OnInit()
        {
            List<Type> types = ReflectionToolkit.GetTypesInRunTimeAssemblies(type =>
            {
                return type.IsSubclassOf(typeof(DataSetBase)) && !type.IsAbstract;
            });
            for (int i = 0; i < types.Count; i++)
            {
                DataSets.Add(types[i], new List<DataSetBase>());
            }
        }
        /// <summary>
        /// 助手准备工作
        /// </summary>
        public void OnReady()
        {
            
        }
        /// <summary>
        /// 刷新助手
        /// </summary>
        public void OnUpdate()
        {
            
        }
        /// <summary>
        /// 终结助手
        /// </summary>
        public void OnTerminate()
        {
            foreach (var dataset in DataSets)
            {
                dataset.Value.Clear();
            }
            DataSets.Clear();
        }
        /// <summary>
        /// 暂停助手
        /// </summary>
        public void OnPause()
        {
            
        }
        /// <summary>
        /// 恢复助手
        /// </summary>
        public void OnResume()
        {
            
        }

        /// <summary>
        /// 添加数据集至数据集仓库
        /// </summary>
        /// <param name="dataSet">数据集</param>
        public void AddDataSet(DataSetBase dataSet)
        {
            if (dataSet == null)
                return;

            Type type = dataSet.GetType();
            if (!DataSets.ContainsKey(type))
            {
                DataSets.Add(type, new List<DataSetBase>());
            }
            DataSets[type].Add(dataSet);
        }
        /// <summary>
        /// 从数据集仓库中移除数据集
        /// </summary>
        /// <param name="dataSet">数据集</param>
        public void RemoveDataSet(DataSetBase dataSet)
        {
            if (dataSet == null)
                return;

            Type type = dataSet.GetType();
            if (!DataSets.ContainsKey(type))
            {
                DataSets.Add(type, new List<DataSetBase>());
            }
            if (DataSets[type].Contains(dataSet))
            {
                DataSets[type].Remove(dataSet);
            }
        }
        /// <summary>
        /// 新建数据集并添加至数据集仓库
        /// </summary>
        /// <param name="type">数据集类型</param>
        /// <param name="data">填充数据</param>
        /// <returns>新建的数据集</returns>
        public DataSetBase CreateDataSet(Type type, JsonData data = null)
        {
            if (DataSets.ContainsKey(type))
            {
                DataSetBase dataSet = ScriptableObject.CreateInstance(type) as DataSetBase;
                if (data != null)
                {
                    dataSet.Fill(data);
                }
                DataSets[type].Add(dataSet);
                return dataSet;
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.DataSet, $"新建数据集失败：{type.Name} 并不是有效的数据集类型！");
            }
        }
        
        /// <summary>
        /// 获取某一类型的所有数据集
        /// </summary>
        /// <param name="type">数据集类型</param>
        /// <returns>数据集列表</returns>
        public List<DataSetBase> GetAllDataSets(Type type)
        {
            if (DataSets.ContainsKey(type))
            {
                return DataSets[type];
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.DataSet, $"获取所有数据集失败：{type.Name} 并不是有效的数据集类型！");
            }
        }
        /// <summary>
        /// 获取某一类型的满足匹配条件的所有数据集
        /// </summary>
        /// <param name="type">数据集类型</param>
        /// <param name="match">匹配条件</param>
        /// <returns>数据集列表</returns>
        public List<DataSetBase> GetAllDataSets(Type type, Predicate<DataSetBase> match)
        {
            if (DataSets.ContainsKey(type))
            {
                return DataSets[type].FindAll(match);
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.DataSet, $"获取所有数据集失败：{type.Name} 并不是有效的数据集类型！");
            }
        }

        /// <summary>
        /// 获取某一类型的满足匹配条件的第一条数据集
        /// </summary>
        /// <param name="type">数据集类型</param>
        /// <param name="match">匹配条件</param>
        /// <param name="isCut">是否同时在数据集仓库中移除该数据集</param>
        /// <returns>数据集</returns>
        public DataSetBase GetDataSet(Type type, Predicate<DataSetBase> match, bool isCut = false)
        {
            if (DataSets.ContainsKey(type))
            {
                DataSetBase dataset = DataSets[type].Find(match);
                if (isCut && dataset)
                {
                    DataSets[type].Remove(dataset);
                }
                return dataset;
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.DataSet, $"获取数据集失败：{type.Name} 并不是有效的数据集类型！");
            }
        }
        /// <summary>
        /// 根据先后顺序获取某一类型的第一条数据集
        /// </summary>
        /// <param name="type">数据集类型</param>
        /// <param name="isCut">是否同时在数据集仓库中移除该数据集</param>
        /// <returns>数据集</returns>
        public DataSetBase GetDataSet(Type type, bool isCut = false)
        {
            if (DataSets.ContainsKey(type))
            {
                if (DataSets[type].Count > 0)
                {
                    DataSetBase dataset = DataSets[type][0];
                    if (isCut)
                    {
                        DataSets[type].RemoveAt(0);
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
                throw new HTFrameworkException(HTFrameworkModule.DataSet, $"获取数据集失败：{type.Name} 并不是有效的数据集类型！");
            }
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
            if (DataSets.ContainsKey(type))
            {
                if (index >= 0 && index < DataSets[type].Count)
                {
                    DataSetBase dataset = DataSets[type][index];
                    if (isCut)
                    {
                        DataSets[type].RemoveAt(index);
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
                throw new HTFrameworkException(HTFrameworkModule.DataSet, $"获取数据集失败：{type.Name} 并不是有效的数据集类型！");
            }
        }
        
        /// <summary>
        /// 获取数据集仓库中某一类型的数据集数量
        /// </summary>
        /// <param name="type">数据集类型</param>
        /// <returns>数据集数量</returns>
        public int GetCount(Type type)
        {
            if (DataSets.ContainsKey(type))
            {
                return DataSets[type].Count;
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.DataSet, $"获取数据集数量失败：{type.Name} 并不是有效的数据集类型！");
            }
        }
        /// <summary>
        /// 清空某一类型的数据集仓库
        /// </summary>
        /// <param name="type">数据集类型</param>
        public void ClearDataSet(Type type)
        {
            if (DataSets.ContainsKey(type))
            {
                for (int i = 0; i < DataSets[type].Count; i++)
                {
                    Main.Kill(DataSets[type][i]);
                }
                DataSets[type].Clear();
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.DataSet, $"清空数据集失败：{type.Name} 并不是有效的数据集类型！");
            }
        }
    }
}