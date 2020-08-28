using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-700)]
    internal sealed class HighlightableObject : MonoBehaviour
    {
        #region Static Fields
        //高亮物体所在的层
        public static int HighlightingLayer = 7;
        //高亮开启速度
        private static float ConstantOnSpeed = 4.5f;
        //高亮关闭速度
        private static float ConstantOffSpeed = 4f;
        //默认剪切值用于没有剪切属性的着色器
        private static float TransparentCutoff = 0.5f;
        #endregion

        #region Private Fields
        //2倍的PI值
        private const float DoublePI = 2f * Mathf.PI;

        //所有的缓存材质
        private List<HighlightingRendererCache> _highlightableRenderers;
        
        //材质是否已修改
        private bool _materialsIsDirty = true;

        //当前是否是高亮状态
        private bool _currentHighlightingState = false;

        //当前高亮颜色
        private Color _currentHighlightingColor;

        //是否启用转换
        private bool _transitionActive = false;

        //转换值
        private float _transitionValue = 0f;
        
        //是否只高亮一帧
        private bool _isOnce = false;

        //高亮一帧的颜色
        private Color _onceColor = Color.red;

        //是否启用闪光
        private bool _isFlashing = false;

        //闪光频率
        private float _flashingFrequency = 2f;

        //闪光开始颜色
        private Color _flashingColorMin = new Color(0.0f, 1.0f, 1.0f, 0.0f);

        //闪光结束颜色
        private Color _flashingColorMax = new Color(0.0f, 1.0f, 1.0f, 1.0f);

        //是否是持续闪光
        private bool _isConstantly = false;

        //持续闪光颜色
        private Color _constantColor = Color.yellow;

        //是否启用遮光板
        private bool _isOccluder = false;

        //是否启用深度缓冲
        private bool _isZWrite = false;

        //遮光板颜色
        private readonly Color _occluderColor = new Color(0.0f, 0.0f, 0.0f, 0.005f);

        //高亮材质
        private Material _highlightingMaterial
        {
            get
            {
                return _isZWrite ? opaqueZMaterial : opaqueMaterial;
            }
        }

        private Material _opaqueMaterial;
        private Material opaqueMaterial
        {
            get
            {
                if (_opaqueMaterial == null)
                {
                    _opaqueMaterial = new Material(opaqueShader);
                    _opaqueMaterial.hideFlags = HideFlags.HideAndDontSave;
                }
                return _opaqueMaterial;
            }
        }

        private Material _opaqueZMaterial;
        private Material opaqueZMaterial
        {
            get
            {
                if (_opaqueZMaterial == null)
                {
                    _opaqueZMaterial = new Material(opaqueZShader);
                    _opaqueZMaterial.hideFlags = HideFlags.HideAndDontSave;
                }
                return _opaqueZMaterial;
            }
        }

        private static Shader _opaqueShader;
        private static Shader opaqueShader
        {
            get
            {
                if (_opaqueShader == null)
                {
                    _opaqueShader = Shader.Find("Hidden/Highlighted/StencilOpaque");
                }
                return _opaqueShader;
            }
        }

        private static Shader _transparentShader;
        private static Shader transparentShader
        {
            get
            {
                if (_transparentShader == null)
                {
                    _transparentShader = Shader.Find("Hidden/Highlighted/StencilTransparent");
                }
                return _transparentShader;
            }
        }

        private static Shader _opaqueZShader;
        private static Shader opaqueZShader
        {
            get
            {
                if (_opaqueZShader == null)
                {
                    _opaqueZShader = Shader.Find("Hidden/Highlighted/StencilOpaqueZ");
                }
                return _opaqueZShader;
            }
        }

        private static Shader _transparentZShader;
        private static Shader transparentZShader
        {
            get
            {
                if (_transparentZShader == null)
                {
                    _transparentZShader = Shader.Find("Hidden/Highlighted/StencilTransparentZ");
                }
                return _transparentZShader;
            }
        }
        #endregion

        #region Common
        private class HighlightingRendererCache
        {
            private GameObject _gameObjectCached;
            private Renderer _rendererCached;
            private int _layerCached;
            private Material[] _sourceMaterials;
            private Material[] _replacementMaterials;
            private List<int> _transparentMaterialIndexes;
            
            public bool IsValid
            {
                get
                {
                    return _gameObjectCached && _rendererCached;
                }
            }

            public HighlightingRendererCache(Renderer renderer, Material[] sourceMaterials, Material sharedOpaqueMaterial, bool writeDepth)
            {
                _gameObjectCached = renderer.gameObject;
                _rendererCached = renderer;
                _layerCached = _gameObjectCached.layer;
                _sourceMaterials = sourceMaterials;
                _replacementMaterials = new Material[sourceMaterials.Length];
                _transparentMaterialIndexes = new List<int>();

                for (int i = 0; i < sourceMaterials.Length; i++)
                {
                    Material sourceMaterial = sourceMaterials[i];
                    if (sourceMaterial == null)
                    {
                        continue;
                    }

                    string tag = sourceMaterial.GetTag("RenderType", true);
                    if (tag == "Transparent" || tag == "TransparentCutout")
                    {
                        Material replacementMaterial = new Material(writeDepth ? transparentZShader : transparentShader);
                        if (sourceMaterial.HasProperty("_MainTex"))
                        {
                            replacementMaterial.SetTexture("_MainTex", sourceMaterial.mainTexture);
                            replacementMaterial.SetTextureOffset("_MainTex", sourceMaterial.mainTextureOffset);
                            replacementMaterial.SetTextureScale("_MainTex", sourceMaterial.mainTextureScale);
                        }

                        replacementMaterial.SetFloat("_Cutoff", sourceMaterial.HasProperty("_Cutoff") ? sourceMaterial.GetFloat("_Cutoff") : TransparentCutoff);

                        _replacementMaterials[i] = replacementMaterial;
                        _transparentMaterialIndexes.Add(i);
                    }
                    else
                    {
                        _replacementMaterials[i] = sharedOpaqueMaterial;
                    }
                }
            }

            public void CacheLayer()
            {
                if (_gameObjectCached)
                {
                    _layerCached = _gameObjectCached.layer;
                }
            }

            public void SetLayer(int layer)
            {
                if (_gameObjectCached)
                {
                    _gameObjectCached.layer = layer;
                }
            }

            public void ResetLayer()
            {
                if (_gameObjectCached)
                {
                    _gameObjectCached.layer = _layerCached;
                }
            }

            public void SetState(bool highlightingState)
            {
                if (_rendererCached)
                {
                    _rendererCached.sharedMaterials = highlightingState ? _replacementMaterials : _sourceMaterials;
                }
            }

            public void SetColorForTransparent(Color color)
            {
                for (int i = 0; i < _transparentMaterialIndexes.Count; i++)
                {
                    _replacementMaterials[_transparentMaterialIndexes[i]].SetColor("_Outline", color);
                }
            }
        }

        private void OnEnable()
        {
            StartCoroutine(EndOfFrame());
            HighlightingEffect.HighlightingEvent += UpdateHighlighting;
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            HighlightingEffect.HighlightingEvent -= UpdateHighlighting;

            if (_highlightableRenderers != null)
            {
                _highlightableRenderers.Clear();
            }

            //重置高亮参数
            _materialsIsDirty = true;
            _currentHighlightingState = false;
            _currentHighlightingColor = Color.clear;
            _transitionActive = false;
            _transitionValue = 0f;
            _isOnce = false;
            _isFlashing = false;
            _isConstantly = false;
            _isOccluder = false;
            _isZWrite = false;
            
            if (_opaqueMaterial)
            {
                DestroyImmediate(_opaqueMaterial);
            }

            if (_opaqueZMaterial)
            {
                DestroyImmediate(_opaqueZMaterial);
            }
        }
        #endregion

        #region public Methods
        /// <summary>
        /// 重新初始化材质
        /// </summary>
        public void ReinitMaterials()
        {
            _materialsIsDirty = true;
        }

        /// <summary>
        /// 设置只高亮一帧的参数
        /// </summary>
        /// <param name="color">颜色</param>
        public void SetOnceParams(Color color)
        {
            _onceColor = color;
        }

        /// <summary>
        /// 开启高亮一帧
        /// </summary>
        public void OpenOnce()
        {
            _isOnce = true;
        }

        /// <summary>
        /// 开启高亮一帧
        /// </summary>
        /// <param name="color">颜色</param>
        public void OpenOnce(Color color)
        {
            _onceColor = color;
            _isOnce = true;
        }

        /// <summary>
        /// 设置闪光参数
        /// </summary>
        /// <param name="color1">闪光开始颜色</param>
        /// <param name="color2">闪光结束颜色</param>
        /// <param name="freq">闪光频率</param>
        public void SetFlashingParams(Color color1, Color color2, float freq)
        {
            _flashingColorMin = color1;
            _flashingColorMax = color2;
            _flashingFrequency = freq;
        }

        /// <summary>
        /// 开启闪光
        /// </summary>
        public void OpenFlashing()
        {
            _isFlashing = true;
        }

        /// <summary>
        /// 开启闪光
        /// </summary>
        /// <param name="color1">闪光开始颜色</param>
        /// <param name="color2">闪光结束颜色</param>
        public void OpenFlashing(Color color1, Color color2)
        {
            _flashingColorMin = color1;
            _flashingColorMax = color2;
            _isFlashing = true;
        }

        /// <summary>
        /// 开启闪光
        /// </summary>
        /// <param name="color1">闪光开始颜色</param>
        /// <param name="color2">闪光结束颜色</param>
        /// <param name="freq">闪光频率</param>
        public void OpenFlashing(Color color1, Color color2, float freq)
        {
            _flashingColorMin = color1;
            _flashingColorMax = color2;
            _flashingFrequency = freq;
            _isFlashing = true;
        }

        /// <summary>
        /// 开启闪光
        /// </summary>
        /// <param name="freq">闪光频率</param>
        public void OpenFlashing(float freq)
        {
            _flashingFrequency = freq;
            _isFlashing = true;
        }

        /// <summary>
        /// 关闭闪光
        /// </summary>
        public void CloseFlashing()
        {
            _isFlashing = false;
        }

        /// <summary>
        /// 闪光模式切换
        /// </summary>
        public void FlashingSwitch()
        {
            _isFlashing = !_isFlashing;
        }

        /// <summary>
        /// 设置持续高亮参数
        /// </summary>
        /// <param name="color">颜色</param>
        public void SetConstantParams(Color color)
        {
            _constantColor = color;
        }

        /// <summary>
        /// 开启持续高亮
        /// </summary>
        public void OpenConstant()
        {
            _isConstantly = true;
            _transitionActive = true;
        }

        /// <summary>
        /// 开启持续高亮
        /// </summary>
        /// <param name="color">颜色</param>
        public void OpenConstant(Color color)
        {
            _constantColor = color;
            _isConstantly = true;
            _transitionActive = true;
        }

        /// <summary>
        /// 关闭持续高亮
        /// </summary>
        public void CloseConstant()
        {
            _isConstantly = false;
            _transitionActive = true;
        }

        /// <summary>
        /// 持续高亮模式切换
        /// </summary>
        public void ConstantSwitch()
        {
            _isConstantly = !_isConstantly;
            _transitionActive = true;
        }

        /// <summary>
        /// 立即开启持续高亮
        /// </summary>
        public void OpenConstantImmediate()
        {
            _isConstantly = true;
            _transitionValue = 1f;
            _transitionActive = false;
        }

        /// <summary>
        /// 立即开启持续高亮
        /// </summary>
        /// <param name="color">颜色</param>
        public void OpenConstantImmediate(Color color)
        {
            _constantColor = color;
            _isConstantly = true;
            _transitionValue = 1f;
            _transitionActive = false;
        }

        /// <summary>
        /// 立即关闭持续高亮
        /// </summary>
        public void CloseConstantImmediate()
        {
            _isConstantly = false;
            _transitionValue = 0f;
            _transitionActive = false;
        }

        /// <summary>
        /// 持续高亮模式立即切换
        /// </summary>
        public void ConstantSwitchImmediate()
        {
            _isConstantly = !_isConstantly;
            _transitionValue = _isConstantly ? 1f : 0f;
            _transitionActive = false;
        }

        /// <summary>
        /// 开启遮光板
        /// </summary>
        public void OpenOccluder()
        {
            _isOccluder = true;
        }

        /// <summary>
        /// 关闭遮光板
        /// </summary>
        public void CloseOccluder()
        {
            _isOccluder = false;
        }

        /// <summary>
        /// 遮光板模式切换
        /// </summary>
        public void OccluderSwitch()
        {
            _isOccluder = !_isOccluder;
        }

        /// <summary>
        /// 关闭所有高亮模式
        /// </summary>
        public void CloseAll()
        {
            _isOnce = false;
            _isFlashing = false;
            _isConstantly = false;
            _isOccluder = false;
            _transitionValue = 0f;
            _transitionActive = false;
        }

        /// <summary>
        /// 死亡
        /// </summary>
        public void Die()
        {
            Destroy(this);
        }
        #endregion

        #region Private Methods
        private void InitMaterials(bool writeDepth)
        {
            _currentHighlightingState = false;

            _isZWrite = writeDepth;

            _highlightableRenderers = new List<HighlightingRendererCache>();

            MeshRenderer[] mr = GetComponentsInChildren<MeshRenderer>();
            CacheRenderers(mr);

            SkinnedMeshRenderer[] smr = GetComponentsInChildren<SkinnedMeshRenderer>();
            CacheRenderers(smr);

            _currentHighlightingState = false;
            _materialsIsDirty = false;
            _currentHighlightingColor = Color.clear;
        }

        private void CacheRenderers(Renderer[] renderers)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                Material[] materials = renderers[i].sharedMaterials;

                if (materials != null)
                {
                    _highlightableRenderers.Add(new HighlightingRendererCache(renderers[i], materials, _highlightingMaterial, _isZWrite));
                }
            }
        }

        private void SetColor(Color color)
        {
            if (_currentHighlightingColor == color)
            {
                return;
            }

            if (_isZWrite)
            {
                opaqueZMaterial.SetColor("_Outline", color);
            }
            else
            {
                opaqueMaterial.SetColor("_Outline", color);
            }

            for (int i = 0; i < _highlightableRenderers.Count; i++)
            {
                _highlightableRenderers[i].SetColorForTransparent(color);
            }

            _currentHighlightingColor = color;
        }

        private void UpdateColors()
        {
            if (_currentHighlightingState == false)
            {
                return;
            }

            if (_isOccluder)
            {
                SetColor(_occluderColor);
                return;
            }

            if (_isOnce)
            {
                SetColor(_onceColor);
                return;
            }

            if (_isFlashing)
            {
                Color color = Color.Lerp(_flashingColorMin, _flashingColorMax, 0.5f * Mathf.Sin(Time.realtimeSinceStartup * _flashingFrequency * DoublePI) + 0.5f);
                SetColor(color);
                return;
            }

            if (_transitionActive)
            {
                Color color = new Color(_constantColor.r, _constantColor.g, _constantColor.b, _constantColor.a * _transitionValue);
                SetColor(color);
                return;
            }
            else if (_isConstantly)
            {
                SetColor(_constantColor);
                return;
            }
        }

        private void PerformTransition()
        {
            if (_transitionActive == false)
            {
                return;
            }

            float targetValue = _isConstantly ? 1f : 0f;

            if (_transitionValue == targetValue)
            {
                _transitionActive = false;
                return;
            }

            if (!Time.timeScale.Approximately(0f))
            {
                float unscaledDeltaTime = Time.deltaTime / Time.timeScale;
                _transitionValue += (_isConstantly ? ConstantOnSpeed : -ConstantOffSpeed) * unscaledDeltaTime;
                _transitionValue = Mathf.Clamp01(_transitionValue);
            }
            else
            {
                return;
            }
        }

        private void UpdateHighlighting(bool enable, bool writeDepth)
        {
            if (enable)
            {
                if (_isZWrite != writeDepth)
                {
                    _materialsIsDirty = true;
                }

                if (_materialsIsDirty)
                {
                    InitMaterials(writeDepth);
                }

                _currentHighlightingState = _isOnce || _isFlashing || _isConstantly || _transitionActive || _isOccluder;

                if (_currentHighlightingState)
                {
                    UpdateColors();

                    PerformTransition();

                    if (_highlightableRenderers != null)
                    {
                        for (int i = 0; i < _highlightableRenderers.Count; i++)
                        {
                            if (_highlightableRenderers[i].IsValid)
                            {
                                _highlightableRenderers[i].CacheLayer();
                                _highlightableRenderers[i].SetLayer(HighlightingLayer);
                                _highlightableRenderers[i].SetState(true);
                            }
                            else
                            {
                                _highlightableRenderers[i].ResetLayer();
                                _highlightableRenderers[i].SetState(false);
                            }
                        }
                    }
                }
            }
            else
            {
                if (_currentHighlightingState && _highlightableRenderers != null)
                {
                    for (int i = 0; i < _highlightableRenderers.Count; i++)
                    {
                        _highlightableRenderers[i].ResetLayer();
                        _highlightableRenderers[i].SetState(false);
                    }
                }
            }
        }

        private IEnumerator EndOfFrame()
        {
            while (enabled)
            {
                yield return YieldInstructioner.GetWaitForEndOfFrame();
                _isOnce = false;
            }
        }
        #endregion
    }
}