using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 摄像机注视目标
    /// </summary>
    public sealed class CameraTarget : MonoBehaviour
    {
        private static CameraTarget _instance;
        public static CameraTarget Instance
        {
            get
            {
                return _instance;
            }
        }

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            
        }
    }
}
