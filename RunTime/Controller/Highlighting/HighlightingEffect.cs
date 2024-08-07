using UnityEngine;

namespace HT.Framework
{
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-800)]
    public sealed class HighlightingEffect : HTBehaviour
    {
        #region Static Fields
        /// <summary>
        /// 高亮渲染事件
        /// </summary>
        public static event HTFAction<bool, bool> HighlightingEvent;
        
        private static Shader _blurShader;
        /// <summary>
        /// 模糊 Shader
        /// </summary>
        private static Shader BlurShader
        {
            get
            {
                if (_blurShader == null)
                {
                    _blurShader = Shader.Find("Hidden/Highlighted/Blur");
                }
                return _blurShader;
            }
        }

        private static Shader _compositeShader;
        /// <summary>
        /// 合成 Shader
        /// </summary>
        private static Shader CompositeShader
        {
            get
            {
                if (_compositeShader == null)
                {
                    _compositeShader = Shader.Find("Hidden/Highlighted/Composite");
                }
                return _compositeShader;
            }
        }

        private static Material _blurMaterial = null;
        /// <summary>
        /// 模糊 Material
        /// </summary>
        private static Material BlurMaterial
        {
            get
            {
                if (_blurMaterial == null)
                {
                    _blurMaterial = new Material(BlurShader);
                    _blurMaterial.hideFlags = HideFlags.HideAndDontSave;
                }
                return _blurMaterial;
            }
        }

        private static Material _compositeMaterial = null;
        /// <summary>
        /// 合成 Material
        /// </summary>
        private static Material CompositeMaterial
        {
            get
            {
                if (_compositeMaterial == null)
                {
                    _compositeMaterial = new Material(CompositeShader);
                    _compositeMaterial.hideFlags = HideFlags.HideAndDontSave;
                }
                return _compositeMaterial;
            }
        }
        #endregion

        #region Public Fields
        //Z缓冲深度
        public int StencilZBufferDepth = 0;
        //降采样因子
        public int DownSampleFactor = 4;
        //模糊迭代次数
        public int BlurIterations = 2;
        //模糊最小扩散值
        public float BlurMinSpread = 0.65f;
        //模糊扩散值
        public float BlurSpread = 0.25f;
        //材质的模糊强度
        public float BlurIntensity = 0.3f;

#if UNITY_EDITOR
        /// <summary>
        /// 是否启用Z缓冲深度
        /// </summary>
        public bool StencilZBufferEnabled
        {
            get
            {
                return StencilZBufferDepth > 0;
            }
            set
            {
                if (StencilZBufferEnabled != value)
                {
                    StencilZBufferDepth = value ? 16 : 0;
                }
            }
        }

        /// <summary>
        /// 采样因子
        /// </summary>
        public int DownSampleFactorProperty
        {
            get
            {
                if (DownSampleFactor == 1)
                {
                    return 0;
                }
                else if (DownSampleFactor == 2)
                {
                    return 1;
                }
                else
                {
                    return 2;
                }
            }
            set
            {
                if (value == 0)
                {
                    DownSampleFactor = 1;
                }
                if (value == 1)
                {
                    DownSampleFactor = 2;
                }
                if (value == 2)
                {
                    DownSampleFactor = 4;
                }
            }
        }

        /// <summary>
        /// 材质的模糊强度
        /// </summary>
        public float BlurIntensityProperty
        {
            get
            {
                return BlurIntensity;
            }
            set
            {
                if (BlurIntensity != value)
                {
                    BlurIntensity = value;

                    if (Application.isPlaying)
                    {
                        BlurMaterial.SetFloat("_Intensity", BlurIntensity);
                    }
                }
            }
        }
#endif
        #endregion

        #region Private Fields
        //高亮摄像机层遮罩
        private int _layerMask = 1 << HighlightableObject.HighlightingLayer;
        //高亮渲染的缓冲摄像机对象
        private GameObject _shaderCameraObject = null;
        //高亮渲染的缓冲摄像机
        private Camera _shaderCamera = null;
        //模板缓冲的渲染纹理
        private RenderTexture _stencilBuffer = null;
        //高亮渲染摄像机
        private Camera _camera = null;
        #endregion
        
        protected override void Awake()
        {
            base.Awake();

            _camera = GetComponent<Camera>();
        }
        private void Start()
        {
            //不支持渲染纹理格式
            if (!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGB32))
            {
                Log.Warning("HighlightingSystem : RenderTextureFormat.ARGB32 is not supported on this platform! Disabling.");
                enabled = false;
                return;
            }

            //不支持Highlighting Stencil着色器
            if (!Shader.Find("Hidden/Highlighted/StencilOpaque").isSupported)
            {
                Log.Warning("HighlightingSystem : HighlightingStencilOpaque shader is not supported on this platform! Disabling.");
                enabled = false;
                return;
            }

            //不支持Highlighting StencilTransparent着色器
            if (!Shader.Find("Hidden/Highlighted/StencilTransparent").isSupported)
            {
                Log.Warning("HighlightingSystem : HighlightingStencilTransparent shader is not supported on this platform! Disabling.");
                enabled = false;
                return;
            }

            //不支持Highlighting StencilZ着色器
            if (!Shader.Find("Hidden/Highlighted/StencilOpaqueZ").isSupported)
            {
                Log.Warning("HighlightingSystem : HighlightingStencilOpaqueZ shader is not supported on this platform! Disabling.");
                enabled = false;
                return;
            }

            //不支持Highlighting StencilTransparentZ着色器
            if (!Shader.Find("Hidden/Highlighted/StencilTransparentZ").isSupported)
            {
                Log.Warning("HighlightingSystem : HighlightingStencilTransparentZ shader is not supported on this platform! Disabling.");
                enabled = false;
                return;
            }

            //不支持HighlightingBlur着色器
            if (!BlurShader.isSupported)
            {
                Log.Warning("HighlightingSystem : HighlightingBlur shader is not supported on this platform! Disabling.");
                enabled = false;
                return;
            }

            //不支持HighlightingComposite着色器
            if (!CompositeShader.isSupported)
            {
                Log.Warning("HighlightingSystem : HighlightingComposite shader is not supported on this platform! Disabling.");
                enabled = false;
                return;
            }

            BlurMaterial.SetFloat("_Intensity", BlurIntensity);
        }
        private void OnDisable()
        {
            if (_shaderCameraObject != null)
            {
                DestroyImmediate(_shaderCameraObject);
            }

            if (_blurShader)
            {
                _blurShader = null;
            }

            if (_compositeShader)
            {
                _compositeShader = null;
            }

            if (_blurMaterial)
            {
                DestroyImmediate(_blurMaterial);
            }

            if (_compositeMaterial)
            {
                DestroyImmediate(_compositeMaterial);
            }

            if (_stencilBuffer != null)
            {
                RenderTexture.ReleaseTemporary(_stencilBuffer);
                _stencilBuffer = null;
            }
        }
        private void FourTapCone(RenderTexture source, RenderTexture dest, int iteration)
        {
            float off = BlurMinSpread + iteration * BlurSpread;
            BlurMaterial.SetFloat("_OffsetScale", off);
            Graphics.Blit(source, dest, BlurMaterial);
        }
        private void DownSample4x(RenderTexture source, RenderTexture dest)
        {
            float off = 1.0f;
            BlurMaterial.SetFloat("_OffsetScale", off);
            Graphics.Blit(source, dest, BlurMaterial);
        }
        private void OnPreRender()
        {
            if (enabled == false || gameObject.activeSelf == false)
                return;

            if (_stencilBuffer != null)
            {
                RenderTexture.ReleaseTemporary(_stencilBuffer);
                _stencilBuffer = null;
            }

            //启用渲染
            if (HighlightingEvent != null)
            {
                HighlightingEvent(true, StencilZBufferDepth > 0);
            }
            else
            {
                return;
            }

            _stencilBuffer = RenderTexture.GetTemporary(_camera.pixelWidth, _camera.pixelHeight, StencilZBufferDepth, RenderTextureFormat.ARGB32);

            if (!_shaderCameraObject)
            {
                _shaderCameraObject = new GameObject("HighlightingCamera", typeof(Camera));
                _shaderCameraObject.GetComponent<Camera>().enabled = false;
                _shaderCameraObject.hideFlags = HideFlags.HideAndDontSave;
            }

            if (!_shaderCamera)
            {
                _shaderCamera = _shaderCameraObject.GetComponent<Camera>();
            }

            _shaderCamera.CopyFrom(_camera);
            //_shaderCamera.projectionMatrix = _camera.projectionMatrix;
            _shaderCamera.cullingMask = _layerMask;
            _shaderCamera.rect = new Rect(0f, 0f, 1f, 1f);
            _shaderCamera.renderingPath = RenderingPath.VertexLit;
            _shaderCamera.allowHDR = false;
            _shaderCamera.useOcclusionCulling = false;
            _shaderCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);
            _shaderCamera.clearFlags = CameraClearFlags.SolidColor;
            _shaderCamera.targetTexture = _stencilBuffer;
            _shaderCamera.Render();

            //关闭渲染
            HighlightingEvent?.Invoke(false, false);
        }
        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (_stencilBuffer == null)
            {
                Graphics.Blit(source, destination);
                return;
            }

            //创建两个降采样纹理来模糊图像
            int width = source.width / DownSampleFactor;
            int height = source.height / DownSampleFactor;
            RenderTexture buffer1 = RenderTexture.GetTemporary(width, height, StencilZBufferDepth, RenderTextureFormat.ARGB32);
            RenderTexture buffer2 = RenderTexture.GetTemporary(width, height, StencilZBufferDepth, RenderTextureFormat.ARGB32);

            //将纹理降采样、模糊处理后存入buffer1
            DownSample4x(_stencilBuffer, buffer1);

            //循环迭代模糊处理图像
            bool oddEven = true;
            for (int i = 0; i < BlurIterations; i++)
            {
                if (oddEven)
                {
                    FourTapCone(buffer1, buffer2, i);
                }
                else
                {
                    FourTapCone(buffer2, buffer1, i);
                }

                oddEven = !oddEven;
            }

            //使用合成Shader，合成最终输出图像
            CompositeMaterial.SetTexture("_StencilTex", _stencilBuffer);
            CompositeMaterial.SetTexture("_BlurTex", oddEven ? buffer1 : buffer2);
            Graphics.Blit(source, destination, CompositeMaterial);

            //清理缓存
            RenderTexture.ReleaseTemporary(buffer1);
            RenderTexture.ReleaseTemporary(buffer2);
            if (_stencilBuffer != null)
            {
                RenderTexture.ReleaseTemporary(_stencilBuffer);
                _stencilBuffer = null;
            }
        }
    }
}