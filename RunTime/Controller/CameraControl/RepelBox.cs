using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 自由视角排斥盒
    /// </summary>
    [AddComponentMenu("HTFramework/Camera Control/Repel Box")]
    [RequireComponent(typeof(BoxCollider))]
    [DisallowMultipleComponent]
    public sealed class RepelBox : HTBehaviour
    {
        private Bounds _scope;
        private float _up;
        private float _down;
        private float _left;
        private float _right;
        private float _front;
        private float _back;

        private void Start()
        {
            if (Main.m_Controller) Main.m_Controller.AddRepelBox(this);

            RefreshScope();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (Main.m_Controller) Main.m_Controller.RemoveRepelBox(this);
        }

        /// <summary>
        /// 刷新排斥盒范围
        /// </summary>
        public void RefreshScope()
        {
            BoxCollider boxCollider = GetComponent<BoxCollider>();
            if (boxCollider)
            {
                _scope = boxCollider.bounds;
                _up = _scope.center.y + _scope.extents.y;
                _down = _scope.center.y - _scope.extents.y;
                _left = _scope.center.x - _scope.extents.x;
                _right = _scope.center.x + _scope.extents.x;
                _front = _scope.center.z + _scope.extents.z;
                _back = _scope.center.z - _scope.extents.z;
            }
        }
        /// <summary>
        /// 将一个顶点排斥到盒子之外
        /// </summary>
        /// <param name="pos">顶点</param>
        /// <returns>排斥出去的顶点</returns>
        public Vector3 Repel(Vector3 pos)
        {
            if (!_scope.Contains(pos))
                return pos;

            Direction dir = CalculateDirection(pos);
            Vector3 newPos = CorrectPos(pos, dir);
            return newPos;
        }

        /// <summary>
        /// 计算距离顶点最近的面
        /// </summary>
        private Direction CalculateDirection(Vector3 pos)
        {
            float newDis;
            float dis = _up - pos.y;
            Direction dir = Direction.Up;

            newDis = pos.y - _down;
            if (newDis < dis)
            {
                dis = newDis;
                dir = Direction.Down;
            }

            newDis = pos.x - _left;
            if (newDis < dis)
            {
                dis = newDis;
                dir = Direction.Left;
            }

            newDis = _right - pos.x;
            if (newDis < dis)
            {
                dis = newDis;
                dir = Direction.Right;
            }

            newDis = _front - pos.z;
            if (newDis < dis)
            {
                dis = newDis;
                dir = Direction.Front;
            }

            newDis = pos.z - _back;
            if (newDis < dis)
            {
                dis = newDis;
                dir = Direction.Back;
            }

            return dir;
        }
        /// <summary>
        /// 修正顶点位置
        /// </summary>
        private Vector3 CorrectPos(Vector3 pos, Direction dir)
        {
            switch (dir)
            {
                case Direction.Up:
                    pos.y = _up;
                    break;
                case Direction.Down:
                    pos.y = _down;
                    break;
                case Direction.Left:
                    pos.x = _left;
                    break;
                case Direction.Right:
                    pos.x = _right;
                    break;
                case Direction.Front:
                    pos.z = _front;
                    break;
                case Direction.Back:
                    pos.z = _back;
                    break;
            }
            return pos;
        }

        private enum Direction
        {
            Up,
            Down,
            Left,
            Right,
            Front,
            Back
        }
    }
}