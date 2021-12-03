using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 自由视角排斥盒
    /// </summary>
    [AddComponentMenu("HTFramework/Camera Control/Repel Box")]
    [DisallowMultipleComponent]
    public class RepelBox : HTBehaviour
    {
        /// <summary>
        /// 生效半径（自由视角位置与此排斥盒的距离小于生效半径，才启动排斥检测）
        /// </summary>
        [RadiusHandler] public float ActiveRadius = 10;
        /// <summary>
        /// 排斥盒范围
        /// </summary>
        [BoundsHandler] public Bounds Scope;

        private Transform _mainCamera;
        private List<Vector3> _pointTracks = new List<Vector3>();
        private List<Vector3> _angleTracks = new List<Vector3>();

        protected override void Awake()
        {
            base.Awake();

            _mainCamera = Main.m_Controller.MainCamera.transform;
        }
        private void Update()
        {
            if (_mainCamera == null)
                return;

            if (Vector3.Distance(Scope.center, Main.m_Controller.LookPoint) > ActiveRadius && Vector3.Distance(Scope.center, _mainCamera.position) > ActiveRadius)
                return;

            RepelTest();
        }

        /// <summary>
        /// 排斥检测
        /// </summary>
        private void RepelTest()
        {
            if (Scope.Contains(Main.m_Controller.LookPoint))
            {
                BackToLastPointTrack();
            }
            else
            {
                RecordPointTrack(Main.m_Controller.LookPoint);
            }

            if (Scope.Contains(_mainCamera.position))
            {
                BackToLastAngleTrack();
            }
            else
            {
                RecordAngleTrack(_mainCamera.position);
            }
        }
        /// <summary>
        /// 记录视点轨迹
        /// </summary>
        /// <param name="point">视点</param>
        private void RecordPointTrack(Vector3 point)
        {
            _pointTracks.Add(point);
            if (_pointTracks.Count > 10)
            {
                _pointTracks.RemoveAt(0);
            }
        }
        /// <summary>
        /// 返回上一个位于排斥盒之外的视点轨迹
        /// </summary>
        private void BackToLastPointTrack()
        {
            while (_pointTracks.Count > 0)
            {
                Vector3 point = _pointTracks[_pointTracks.Count - 1];
                if (!Scope.Contains(point))
                {
                    Main.m_Controller.SetLookPoint(point, false);
                    return;
                }
                else
                {
                    _pointTracks.RemoveAt(_pointTracks.Count - 1);
                }
            }
        }
        /// <summary>
        /// 记录视角轨迹
        /// </summary>
        /// <param name="cameraPos">摄像机位置</param>
        private void RecordAngleTrack(Vector3 cameraPos)
        {
            _angleTracks.Add(cameraPos);
            if (_angleTracks.Count > 10)
            {
                _angleTracks.RemoveAt(0);
            }
        }
        /// <summary>
        /// 返回上一个位于排斥盒之外的视角轨迹
        /// </summary>
        private void BackToLastAngleTrack()
        {
            while (_angleTracks.Count > 0)
            {
                Vector3 cameraPos = _angleTracks[_angleTracks.Count - 1];
                if (!Scope.Contains(cameraPos))
                {
                    _mainCamera.position = cameraPos;
                    return;
                }
                else
                {
                    _angleTracks.RemoveAt(_angleTracks.Count - 1);
                }
            }
        }
    }
}