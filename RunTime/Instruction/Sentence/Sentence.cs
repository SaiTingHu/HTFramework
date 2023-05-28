using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 可执行语句
    /// </summary>
    internal abstract class Sentence : IReference
    {
        /// <summary>
        /// 目标路径
        /// </summary>
        public string TargetPath;

        private GameObject _target;

        /// <summary>
        /// 目标
        /// </summary>
        public GameObject Target
        {
            get
            {
                if (_target == null && !string.IsNullOrEmpty(TargetPath))
                {
                    int index = TargetPath.IndexOf('/');
                    if (index > 0)
                    {
                        string parent = TargetPath.Substring(0, index);
                        string child = TargetPath.Substring(index + 1);
                        _target = GameObject.Find(parent).FindChildren(child);
                    }
                    else
                    {
                        _target = GameObject.Find(TargetPath);
                    }
                }
                return _target;
            }
        }

        /// <summary>
        /// 执行
        /// </summary>
        public abstract void Execute();
        /// <summary>
        /// 重置
        /// </summary>
        public virtual void Reset()
        {
            TargetPath = null;
            _target = null;
        }
    }

    /// <summary>
    /// 参数类型
    /// </summary>
    internal enum ArgsType
    {
        /// <summary>
        /// 字符串
        /// </summary>
        String,
        /// <summary>
        /// bool
        /// </summary>
        Bool,
        /// <summary>
        /// 整型
        /// </summary>
        Int,
        /// <summary>
        /// 浮点
        /// </summary>
        Float,
        /// <summary>
        /// 二维向量
        /// </summary>
        Vector2,
        /// <summary>
        /// 三维向量
        /// </summary>
        Vector3,
        /// <summary>
        /// 无
        /// </summary>
        None
    }
}