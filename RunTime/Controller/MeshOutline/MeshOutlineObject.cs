using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    [DisallowMultipleComponent]
    public sealed class MeshOutlineObject : MonoBehaviour
    {
        private static Material GetOpaqueMaterial()
        {
            Material mat = new Material(OpaqueShader);
            mat.hideFlags = HideFlags.HideAndDontSave;
            return mat;
        }
        private static Material GetTransparentMaterial()
        {
            Material mat = new Material(TransparentShader);
            mat.hideFlags = HideFlags.HideAndDontSave;
            return mat;
        }
        private static Shader _opaqueShader;
        private static Shader OpaqueShader
        {
            get
            {
                if (_opaqueShader == null)
                {
                    _opaqueShader = Shader.Find("Hidden/MeshOutline/Opaque");
                }
                return _opaqueShader;
            }
        }
        private static Shader _transparentShader;
        private static Shader TransparentShader
        {
            get
            {
                if (_transparentShader == null)
                {
                    _transparentShader = Shader.Find("Hidden/MeshOutline/Transparent");
                }
                return _transparentShader;
            }
        }
        
        private List<MeshOutlineRenderer> MeshOutlineRenderers = new List<MeshOutlineRenderer>();
        private bool _isInit = false;
        private bool _isOpened = false;
        private Color _outlineColor = Color.yellow;
        private float _outlineIntensity = 1f;

        /// <summary>
        /// 重置轮廓
        /// </summary>
        public void ResetOutline()
        {
            Close();

            MeshOutlineRenderers.Clear();

            MeshRenderer[] mrs = GetComponentsInChildren<MeshRenderer>(true);
            CacheRenderers(mrs);

            SkinnedMeshRenderer[] smrs = GetComponentsInChildren<SkinnedMeshRenderer>(true);
            CacheRenderers(smrs);
        }
        /// <summary>
        /// 开启轮廓高亮
        /// </summary>
        /// <param name="color">高亮颜色</param>
        /// <param name="isFlash">是否闪烁</param>
        public void Open(Color color, float intensity)
        {
            if (!_isInit)
            {
                _isInit = true;
                ResetOutline();
            }

            if (!_isOpened)
            {
                _isOpened = true;

                for (int i = 0; i < MeshOutlineRenderers.Count; i++)
                {
                    MeshOutlineRenderers[i].SetOutlineState(true);
                }
            }

            if (color != _outlineColor)
            {
                _outlineColor = color;
                for (int i = 0; i < MeshOutlineRenderers.Count; i++)
                {
                    MeshOutlineRenderers[i].SetOutlineColor(_outlineColor);
                }
            }

            if (!_outlineIntensity.Approximately(intensity))
            {
                _outlineIntensity = intensity;
                for (int i = 0; i < MeshOutlineRenderers.Count; i++)
                {
                    MeshOutlineRenderers[i].SetOutlineIntensity(_outlineIntensity);
                }
            }
        }
        /// <summary>
        /// 关闭轮廓高亮
        /// </summary>
        public void Close()
        {
            if (_isOpened)
            {
                _isOpened = false;

                for (int i = 0; i < MeshOutlineRenderers.Count; i++)
                {
                    MeshOutlineRenderers[i].SetOutlineState(false);
                }
            }
        }
        /// <summary>
        /// 死亡
        /// </summary>
        public void Die()
        {
            Destroy(this);
        }

        private void OnDisable()
        {
            Close();
        }
        private void CacheRenderers(Renderer[] renderers)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                Material[] materials = renderers[i].sharedMaterials;

                if (materials != null)
                {
                    MeshOutlineRenderers.Add(new MeshOutlineRenderer(renderers[i], materials));
                }
            }
        }
        
        private class MeshOutlineRenderer
        {
            public Renderer RendererCached;
            public GameObject GameObjectCached;
            private Material[] SourceMaterials;
            private Material[] ReplacementMaterials;

            public MeshOutlineRenderer(Renderer renderer, Material[] sourceMaterials)
            {
                RendererCached = renderer;
                GameObjectCached = renderer.gameObject;
                SourceMaterials = sourceMaterials;
                ReplacementMaterials = new Material[sourceMaterials.Length];

                for (int i = 0; i < sourceMaterials.Length; i++)
                {
                    Material material = sourceMaterials[i];
                    if (material == null)
                    {
                        continue;
                    }

                    string tag = material.GetTag("RenderType", true);
                    if (tag == "Transparent" || tag == "TransparentCutout")
                    {
                        ReplacementMaterials[i] = GetTransparentMaterial();
                    }
                    else
                    {
                        ReplacementMaterials[i] = GetOpaqueMaterial();
                    }

                    if (material.HasProperty("_MainTex"))
                    {
                        ReplacementMaterials[i].SetTexture("_MainTex", material.mainTexture);
                        ReplacementMaterials[i].SetTextureOffset("_MainTex", material.mainTextureOffset);
                        ReplacementMaterials[i].SetTextureScale("_MainTex", material.mainTextureScale);
                    }
                    ReplacementMaterials[i].SetColor("_Diffuse", material.color);
                }
            }

            public void SetOutlineState(bool isOutline)
            {
                if (RendererCached)
                    RendererCached.sharedMaterials = isOutline ? ReplacementMaterials : SourceMaterials;
            }

            public void SetOutlineColor(Color color)
            {
                for (int i = 0; i < ReplacementMaterials.Length; i++)
                {
                    ReplacementMaterials[i].SetColor("_HighlightColor", color);
                }
            }

            public void SetOutlineIntensity(float intensity)
            {
                for (int i = 0; i < ReplacementMaterials.Length; i++)
                {
                    ReplacementMaterials[i].SetFloat("_HighlightIntensity", intensity);
                }
            }
        }
    }
}