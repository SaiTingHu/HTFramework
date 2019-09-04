using UnityEngine;

namespace HT.Framework
{
    [RequireComponent(typeof(Camera))]
    [DisallowMultipleComponent]
    public sealed class HighlightingEffect : MonoBehaviour
    {
        public static event HTFAction<bool, bool> highlightingEvent;

        #region Inspector Fields
        // Stencil (highlighting) buffer depth
        public int stencilZBufferDepth = 0;

        // Stencil (highlighting) buffer size downsample factor
        public int _downsampleFactor = 4;

        // Blur iterations
        public int iterations = 2;

        // Blur minimal spread
        public float blurMinSpread = 0.65f;

        // Blur spread per iteration
        public float blurSpread = 0.25f;

        // Blurring intensity for the blur material
        public float _blurIntensity = 0.3f;

        // These properties available only in Editor - we don't need them in standalone build
#if UNITY_EDITOR
        // Z-buffer writing state getter/setter
        public bool stencilZBufferEnabled
        {
            get
            {
                return (stencilZBufferDepth > 0);
            }
            set
            {
                if (stencilZBufferEnabled != value)
                {
                    stencilZBufferDepth = value ? 16 : 0;
                }
            }
        }

        // Downsampling factor getter/setter
        public int downsampleFactor
        {
            get
            {
                if (_downsampleFactor == 1)
                {
                    return 0;
                }
                if (_downsampleFactor == 2)
                {
                    return 1;
                }
                return 2;
            }
            set
            {
                if (value == 0)
                {
                    _downsampleFactor = 1;
                }
                if (value == 1)
                {
                    _downsampleFactor = 2;
                }
                if (value == 2)
                {
                    _downsampleFactor = 4;
                }
            }
        }

        // Blur alpha intensity getter/setter
        public float blurIntensity
        {
            get
            {
                return _blurIntensity;
            }
            set
            {
                if (_blurIntensity != value)
                {
                    _blurIntensity = value;
                    if (Application.isPlaying)
                    {
                        blurMaterial.SetFloat("_Intensity", _blurIntensity);
                    }
                }
            }
        }
#endif
        #endregion

        #region Private Fields
        // Highlighting camera layers culling mask
        private int layerMask = (1 << HighlightableObject.highlightingLayer);

        // This GameObject reference
        private GameObject go = null;

        // Camera for rendering stencil buffer GameObject
        private GameObject shaderCameraGO = null;

        // Camera for rendering stencil buffer
        private Camera shaderCamera = null;

        // RenderTexture with stencil buffer
        private RenderTexture stencilBuffer = null;

        // Camera reference
        private Camera refCam = null;

        // Blur Shader
        private static Shader _blurShader;
        private static Shader blurShader
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

        // Compositing Shader
        private static Shader _compShader;
        private static Shader compShader
        {
            get
            {
                if (_compShader == null)
                {
                    _compShader = Shader.Find("Hidden/Highlighted/Composite");
                }
                return _compShader;
            }
        }

        // Blur Material
        private static Material _blurMaterial = null;
        private static Material blurMaterial
        {
            get
            {
                if (_blurMaterial == null)
                {
                    _blurMaterial = new Material(blurShader);
                    _blurMaterial.hideFlags = HideFlags.HideAndDontSave;
                }
                return _blurMaterial;
            }
        }

        // Compositing Material
        private static Material _compMaterial = null;
        private static Material compMaterial
        {
            get
            {
                if (_compMaterial == null)
                {
                    _compMaterial = new Material(compShader);
                    _compMaterial.hideFlags = HideFlags.HideAndDontSave;
                }
                return _compMaterial;
            }
        }
        #endregion
        
        private void Awake()
        {
            go = gameObject;
            refCam = GetComponent<Camera>();
        }

        private void OnDisable()
        {
            if (shaderCameraGO != null)
            {
                DestroyImmediate(shaderCameraGO);
            }

            if (_blurShader)
            {
                _blurShader = null;
            }

            if (_compShader)
            {
                _compShader = null;
            }

            if (_blurMaterial)
            {
                DestroyImmediate(_blurMaterial);
            }

            if (_compMaterial)
            {
                DestroyImmediate(_compMaterial);
            }

            if (stencilBuffer != null)
            {
                RenderTexture.ReleaseTemporary(stencilBuffer);
                stencilBuffer = null;
            }
        }

        private void Start()
        {
            // Disable if Image Effects is not supported
            if (!SystemInfo.supportsImageEffects)
            {
                GlobalTools.LogWarning("HighlightingSystem : Image effects is not supported on this platform! Disabling.");
                enabled = false;
                return;
            }

            // Disable if required Render Texture Format is not supported
            if (!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGB32))
            {
                GlobalTools.LogWarning("HighlightingSystem : RenderTextureFormat.ARGB32 is not supported on this platform! Disabling.");
                enabled = false;
                return;
            }

            // Disable if HighlightingStencilOpaque shader is not supported
            if (!Shader.Find("Hidden/Highlighted/StencilOpaque").isSupported)
            {
                GlobalTools.LogWarning("HighlightingSystem : HighlightingStencilOpaque shader is not supported on this platform! Disabling.");
                enabled = false;
                return;
            }

            // Disable if HighlightingStencilTransparent shader is not supported
            if (!Shader.Find("Hidden/Highlighted/StencilTransparent").isSupported)
            {
                GlobalTools.LogWarning("HighlightingSystem : HighlightingStencilTransparent shader is not supported on this platform! Disabling.");
                enabled = false;
                return;
            }

            // Disable if HighlightingStencilOpaqueZ shader is not supported
            if (!Shader.Find("Hidden/Highlighted/StencilOpaqueZ").isSupported)
            {
                GlobalTools.LogWarning("HighlightingSystem : HighlightingStencilOpaqueZ shader is not supported on this platform! Disabling.");
                enabled = false;
                return;
            }

            // Disable if HighlightingStencilTransparentZ shader is not supported
            if (!Shader.Find("Hidden/Highlighted/StencilTransparentZ").isSupported)
            {
                GlobalTools.LogWarning("HighlightingSystem : HighlightingStencilTransparentZ shader is not supported on this platform! Disabling.");
                enabled = false;
                return;
            }

            // Disable if HighlightingBlur shader is not supported
            if (!blurShader.isSupported)
            {
                GlobalTools.LogWarning("HighlightingSystem : HighlightingBlur shader is not supported on this platform! Disabling.");
                enabled = false;
                return;
            }

            // Disable if HighlightingComposite shader is not supported
            if (!compShader.isSupported)
            {
                GlobalTools.LogWarning("HighlightingSystem : HighlightingComposite shader is not supported on this platform! Disabling.");
                enabled = false;
                return;
            }

            // Set the initial intensity in blur shader
            blurMaterial.SetFloat("_Intensity", _blurIntensity);
        }

        public void FourTapCone(RenderTexture source, RenderTexture dest, int iteration)
        {
            float off = blurMinSpread + iteration * blurSpread;
            blurMaterial.SetFloat("_OffsetScale", off);
            Graphics.Blit(source, dest, blurMaterial);
        }

        private void DownSample4x(RenderTexture source, RenderTexture dest)
        {
            float off = 1.0f;
            blurMaterial.SetFloat("_OffsetScale", off);
            Graphics.Blit(source, dest, blurMaterial);
        }

        private void OnPreRender()
        {
#if UNITY_4_0
            if (enabled == false || go.activeInHierarchy == false)
#else
            if (enabled == false || go.activeSelf == false)
#endif
                return;

            if (stencilBuffer != null)
            {
                RenderTexture.ReleaseTemporary(stencilBuffer);
                stencilBuffer = null;
            }

            // Turn on highlighted shaders
            if (highlightingEvent != null)
            {
                highlightingEvent(true, stencilZBufferDepth > 0);
            }
            // We don't need to render the scene if there's no HighlightableObjects
            else
            {
                return;
            }

            stencilBuffer = RenderTexture.GetTemporary(GetComponent<Camera>().pixelWidth, GetComponent<Camera>().pixelHeight, stencilZBufferDepth, RenderTextureFormat.ARGB32);

            if (!shaderCameraGO)
            {
                shaderCameraGO = new GameObject("HighlightingCamera", typeof(Camera));
                shaderCameraGO.GetComponent<Camera>().enabled = false;
                shaderCameraGO.hideFlags = HideFlags.HideAndDontSave;
            }

            if (!shaderCamera)
            {
                shaderCamera = shaderCameraGO.GetComponent<Camera>();
            }

            shaderCamera.CopyFrom(refCam);
            //shaderCamera.projectionMatrix = refCam.projectionMatrix;		// Uncomment this line if you have problems using Highlighting System with custom projection matrix on your camera
            shaderCamera.cullingMask = layerMask;
            shaderCamera.rect = new Rect(0f, 0f, 1f, 1f);
            shaderCamera.renderingPath = RenderingPath.VertexLit;
            shaderCamera.allowHDR = false;
            shaderCamera.useOcclusionCulling = false;
            shaderCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);
            shaderCamera.clearFlags = CameraClearFlags.SolidColor;
            shaderCamera.targetTexture = stencilBuffer;
            shaderCamera.Render();

            // Turn off highlighted shaders
            highlightingEvent?.Invoke(false, false);
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            // If stencilBuffer is not created by some reason
            if (stencilBuffer == null)
            {
                // Simply transfer framebuffer to destination
                Graphics.Blit(source, destination);
                return;
            }

            // Create two buffers for blurring the image
            int width = source.width / _downsampleFactor;
            int height = source.height / _downsampleFactor;
            RenderTexture buffer = RenderTexture.GetTemporary(width, height, stencilZBufferDepth, RenderTextureFormat.ARGB32);
            RenderTexture buffer2 = RenderTexture.GetTemporary(width, height, stencilZBufferDepth, RenderTextureFormat.ARGB32);

            // Copy stencil buffer to the 4x4 smaller texture
            DownSample4x(stencilBuffer, buffer);

            // Blur the small texture
            bool oddEven = true;
            for (int i = 0; i < iterations; i++)
            {
                if (oddEven)
                {
                    FourTapCone(buffer, buffer2, i);
                }
                else
                {
                    FourTapCone(buffer2, buffer, i);
                }

                oddEven = !oddEven;
            }

            // Compose
            compMaterial.SetTexture("_StencilTex", stencilBuffer);
            compMaterial.SetTexture("_BlurTex", oddEven ? buffer : buffer2);
            Graphics.Blit(source, destination, compMaterial);

            // Cleanup
            RenderTexture.ReleaseTemporary(buffer);
            RenderTexture.ReleaseTemporary(buffer2);
            if (stencilBuffer != null)
            {
                RenderTexture.ReleaseTemporary(stencilBuffer);
                stencilBuffer = null;
            }
        }
    }
}