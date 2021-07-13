using System;
using System.Collections.Generic;

namespace HT.Framework
{
    /// <summary>
    /// 数据集管理器的助手接口
    /// </summary>
    public interface IDataSetHelper : IInternalModuleHelper
    {
        /// <summary>
        /// 所有数据集
        /// </summary>
        Dictionary<Type, List<DataSetBase>> DataSets { get; }

        /// <summary>
        /// 添加数据集至数据集仓库
        /// </summary>
        /// <param name="dataSet">数据集</param>
        void AddDataSet(DataSetBase dataSet);
        /// <summary>
        /// 从数据集仓库中移除数据集
        /// </summary>
        /// <param name="dataSet">数据集</param>
        void RemoveDataSet(DataSetBase dataSet);
        /// <summary>
        /// 新建数据集并添加至数据集仓库
        /// </summary>
        /// <param name="type">数据集类型</param>
        /// <param name="data">填充数据</param>
        /// <returns>新建的数据集</returns>
        DataSetBase CreateDataSet(Type type, JsonData data = null);

        /// <summary>
        /// 获取某一类型的所有数据集
        /// </summary>
        /// <param name="type">数据集类型</param>
        /// <returns>数据集列表</returns>
        List<DataSetBase> GetAllDataSets(Type type);
        /// <summary>
        /// 获取某一类型的满足匹配条件的所有数据集
        /// </summary>
        /// <param name="type">数据集类型</param>
        /// <param name="match">匹配条件</param>
        /// <returns>数据集列表</returns>
        List<DataSetBase> GetAllDataSets(Type type, Predicate<DataSetBase> match);

        /// <summary>
        /// 获取某一类型的满足匹配条件的第一条数据集
        /// </summary>
        /// <param name="type">数据集类型</param>
        /// <param name="match">匹配条件</param>
        /// <param name="isCut">是否同时在数据集仓库中移除该数据集</param>
        /// <returns>数据集</returns>
        DataSetBase GetDataSet(Type type, Predicate<DataSetBase> match, bool isCut = false);
        /// <summary>
        /// 根据先后顺序获取某一类型的第一条数据集
        /// </summary>
        /// <param name="type">数据集类型</param>
        /// <param name="isCut">是否同时在数据集仓库中移除该数据集</param>
        /// <returns>数据集</returns>
        DataSetBase GetDataSet(Type type, bool isCut = false);
        /// <summary>
        /// 根据索引获取某一类型的一条数据集
        /// </summary>
        /// <param name="type">数据集类型</param>
        /// <param name="index">索引</param>
        /// <param name="isCut">是否同时在数据集仓库中移除该数据集</param>
        /// <returns>数据集</returns>
        DataSetBase GetDataSet(Type type, int index, bool isCut = false);

        /// <summary>
        /// 获取数据集仓库中某一类型的数据集数量
        /// </summary>
        /// <param name="type">数据集类型</param>
        /// <returns>数据集数量</returns>
        int GetCount(Type type);
        /// <summary>
        /// 清空某一类型的数据集仓库
        /// </summary>
        /// <param name="type">数据集类型</param>
        void ClearDataSet(Type type);
    }
}