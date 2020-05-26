using System;
using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 数据集管理器
    /// </summary>
    [DisallowMultipleComponent]
    [InternalModule(HTFrameworkModule.DataSet)]
    public sealed class DataSetManager : InternalModuleBase
    {
        private IDataSetHelper _helper;

        internal override void OnInitialization()
        {
            base.OnInitialization();

            _helper = Helper as IDataSetHelper;
            _helper.OnInitialization();
        }
        internal override void OnTermination()
        {
            base.OnTermination();

            _helper.OnTermination();
        }

        /// <summary>
        /// 添加数据集至数据集仓库
        /// </summary>
        /// <param name="dataSet">数据集</param>
        public void AddDataSet(DataSetBase dataSet)
        {
            _helper.AddDataSet(dataSet);
        }
        /// <summary>
        /// 从数据集仓库中移除数据集
        /// </summary>
        /// <param name="dataSet">数据集</param>
        public void RemoveDataSet(DataSetBase dataSet)
        {
            _helper.RemoveDataSet(dataSet);
        }
        /// <summary>
        /// 新建数据集并添加至数据集仓库
        /// </summary>
        /// <typeparam name="T">数据集类型</typeparam>
        /// <param name="data">填充数据</param>
        /// <returns>新建的数据集</returns>
        public T CreateDataSet<T>(JsonData data = null) where T : DataSetBase
        {
            return _helper.CreateDataSet(typeof(T), data) as T;
        }
        /// <summary>
        /// 新建数据集并添加至数据集仓库
        /// </summary>
        /// <param name="type">数据集类型</param>
        /// <param name="data">填充数据</param>
        /// <returns>新建的数据集</returns>
        public DataSetBase CreateDataSet(Type type, JsonData data = null)
        {
            return _helper.CreateDataSet(type, data);
        }
        
        /// <summary>
        /// 获取某一类型的所有数据集
        /// </summary>
        /// <typeparam name="T">数据集类型</typeparam>
        /// <returns>数据集列表</returns>
        public List<T> GetAllDataSets<T>() where T : DataSetBase
        {
            return _helper.GetAllDataSets(typeof(T)).ConvertAllAS<T, DataSetBase>();
        }
        /// <summary>
        /// 获取某一类型的所有数据集
        /// </summary>
        /// <param name="type">数据集类型</param>
        /// <returns>数据集列表</returns>
        public List<DataSetBase> GetAllDataSets(Type type)
        {
            return _helper.GetAllDataSets(type);
        }
        /// <summary>
        /// 获取某一类型的满足匹配条件的所有数据集
        /// </summary>
        /// <typeparam name="T">数据集类型</typeparam>
        /// <param name="match">匹配条件</param>
        /// <returns>数据集列表</returns>
        public List<T> GetAllDataSets<T>(Predicate<T> match) where T : DataSetBase
        {
            return _helper.GetAllDataSets(typeof(T), match as Predicate<DataSetBase>).ConvertAllAS<T, DataSetBase>();
        }
        /// <summary>
        /// 获取某一类型的满足匹配条件的所有数据集
        /// </summary>
        /// <param name="type">数据集类型</param>
        /// <param name="match">匹配条件</param>
        /// <returns>数据集列表</returns>
        public List<DataSetBase> GetAllDataSets(Type type, Predicate<DataSetBase> match)
        {
            return _helper.GetAllDataSets(type, match);
        }
        
        /// <summary>
        /// 获取某一类型的满足匹配条件的第一条数据集
        /// </summary>
        /// <typeparam name="T">数据集类型</typeparam>
        /// <param name="match">匹配条件</param>
        /// <returns>数据集</returns>
        public T GetDataSet<T>(Predicate<T> match) where T : DataSetBase
        {
            return _helper.GetDataSet(typeof(T), match as Predicate<DataSetBase>) as T;
        }
        /// <summary>
        /// 获取某一类型的满足匹配条件的第一条数据集
        /// </summary>
        /// <param name="type">数据集类型</param>
        /// <param name="match">匹配条件</param>
        /// <returns>数据集</returns>
        public DataSetBase GetDataSet(Type type, Predicate<DataSetBase> match)
        {
            return _helper.GetDataSet(type, match);
        }
        /// <summary>
        /// 根据先后顺序获取某一类型的第一条数据集
        /// </summary>
        /// <typeparam name="T">数据集类型</typeparam>
        /// <param name="isCut">是否同时在数据集仓库中移除该数据集</param>
        /// <returns>数据集</returns>
        public T GetDataSet<T>(bool isCut = false) where T : DataSetBase
        {
            return _helper.GetDataSet(typeof(T), isCut) as T;
        }
        /// <summary>
        /// 根据先后顺序获取某一类型的第一条数据集
        /// </summary>
        /// <param name="type">数据集类型</param>
        /// <param name="isCut">是否同时在数据集仓库中移除该数据集</param>
        /// <returns>数据集</returns>
        public DataSetBase GetDataSet(Type type, bool isCut = false)
        {
            return _helper.GetDataSet(type, isCut);
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
            return _helper.GetDataSet(typeof(T), index, isCut) as T;
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
            return _helper.GetDataSet(type, index, isCut);
        }

        /// <summary>
        /// 获取数据集仓库中某一类型的数据集数量
        /// </summary>
        /// <typeparam name="T">数据集类型</typeparam>
        /// <returns>数据集数量</returns>
        public int GetCount<T>() where T : DataSetBase
        {
            return _helper.GetCount(typeof(T));
        }
        /// <summary>
        /// 获取数据集仓库中某一类型的数据集数量
        /// </summary>
        /// <param name="type">数据集类型</param>
        /// <returns>数据集数量</returns>
        public int GetCount(Type type)
        {
            return _helper.GetCount(type);
        }

        /// <summary>
        /// 清空某一类型的数据集仓库
        /// </summary>
        /// <typeparam name="T">数据集类型</typeparam>
        public void ClearDataSet<T>() where T : DataSetBase
        {
            _helper.ClearDataSet(typeof(T));
        }
        /// <summary>
        /// 清空某一类型的数据集仓库
        /// </summary>
        /// <param name="type">数据集类型</param>
        public void ClearDataSet(Type type)
        {
            _helper.ClearDataSet(type);
        }
    }
}