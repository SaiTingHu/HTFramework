using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 数据集基类
    /// </summary>
    public abstract class DataSetBase : ScriptableObject
    {
        /// <summary>
        /// 通过Json数据填充数据集
        /// </summary>
        public virtual void Fill(JsonData data)
        {

        }
        /// <summary>
        /// 将数据集打包为Json数据
        /// </summary>
        public virtual JsonData Pack()
        {
            return null;
        }
    }
}