using System;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 物体位置（包含局部坐标、局部旋转、局部缩放）
    /// </summary>
    [Serializable]
    public sealed class Location
    {
        /// <summary>
        /// 局部坐标
        /// </summary>
        public Vector3 Position;
        /// <summary>
        /// 局部旋转
        /// </summary>
        public Vector3 Rotation;
        /// <summary>
        /// 局部缩放
        /// </summary>
        public Vector3 Scale;
    }
}