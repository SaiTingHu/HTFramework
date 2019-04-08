using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 数据集基类
    /// </summary>
    public abstract class DataSet : ScriptableObject
    {
        /// <summary>
        /// 通过Json数据填充数据集
        /// </summary>
        public abstract void Fill(JsonData data);
        /// <summary>
        /// 将数据集打包为Json数据
        /// </summary>
        public abstract JsonData Pack();
    }
}
