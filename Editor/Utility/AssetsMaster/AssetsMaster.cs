using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityObject = UnityEngine.Object;

namespace HT.Framework
{
    /// <summary>
    /// 资产的主人
    /// </summary>
    internal sealed class AssetsMaster : HTFEditorWindow
    {
        #region Field
        private AssetType _type = AssetType.Texture;

        private bool _isIncludeDisabled = true;
        private bool _isIncludeLightmap = false;
        private bool _isIncludeUGUI = true;
        private bool _isIncludeMonoScript = false;

        private Dictionary<Material, MaterialContent> _materials = new Dictionary<Material, MaterialContent>();
        private Dictionary<Texture, TextureContent> _textures = new Dictionary<Texture, TextureContent>();
        private Dictionary<Mesh, MeshContent> _meshes = new Dictionary<Mesh, MeshContent>();
        private Dictionary<Type, MonoScriptContent> _scripts = new Dictionary<Type, MonoScriptContent>();
        private Dictionary<GameObject, MissingContent> _missing = new Dictionary<GameObject, MissingContent>();

        private float _previewWidth = 40;
        private float _previewHeight = 40;

        private Vector2 _materialGUIScroll;
        private Vector2 _textureGUIScroll;
        private Vector2 _meshGUIScroll;
        private Vector2 _monoScriptGUIScroll;
        private Vector2 _missingGUIScroll;

        private string _filterName = "";
        private int _totalMeshVertices = 0;
        #endregion

        #region GUI Function
        protected override void OnTitleGUI()
        {
            base.OnTitleGUI();

            _isIncludeDisabled = GUILayout.Toggle(_isIncludeDisabled, "Include Disabled", EditorStyles.toolbarButton);
            _isIncludeLightmap = GUILayout.Toggle(_isIncludeLightmap, "Include Lightmap", EditorStyles.toolbarButton);
            _isIncludeUGUI = GUILayout.Toggle(_isIncludeUGUI, "Include UGUI", EditorStyles.toolbarButton);
            _isIncludeMonoScript = GUILayout.Toggle(_isIncludeMonoScript, "Include MonoScript", EditorStyles.toolbarButton);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Refresh", EditorStyles.toolbarPopup))
            {
                SearchAssetsInOpenedScene();
            }
        }
        protected override void OnBodyGUI()
        {
            base.OnBodyGUI();

            OnHeaderGUI();

            switch (_type)
            {
                case AssetType.Material:
                    OnMaterialGUI();
                    break;
                case AssetType.Texture:
                    OnTextureGUI();
                    break;
                case AssetType.Mesh:
                    OnMeshGUI();
                    break;
                case AssetType.MonoScript:
                    OnMonoScriptGUI();
                    break;
                case AssetType.Missing:
                    OnMissingGUI();
                    break;
            }
        }
        private void OnHeaderGUI()
        {
            if (_missing.Count > 0)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox("Some gameObjects are missing asset!", MessageType.Error);
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Toggle(_type == AssetType.Material, "Material", "LargeButtonLeft"))
            {
                _type = AssetType.Material;
            }
            if (GUILayout.Toggle(_type == AssetType.Texture, "Texture", "LargeButtonMid"))
            {
                _type = AssetType.Texture;
            }
            if (GUILayout.Toggle(_type == AssetType.Mesh, "Mesh", "LargeButtonMid"))
            {
                _type = AssetType.Mesh;
            }
            if (_missing.Count <= 0)
            {
                GUI.enabled = _isIncludeMonoScript;
                if (GUILayout.Toggle(_type == AssetType.MonoScript, "MonoScript", "LargeButtonRight"))
                {
                    _type = AssetType.MonoScript;
                }
                GUI.enabled = true;
            }
            else
            {
                GUI.enabled = _isIncludeMonoScript;
                if (GUILayout.Toggle(_type == AssetType.MonoScript, "MonoScript", "LargeButtonMid"))
                {
                    _type = AssetType.MonoScript;
                }
                GUI.enabled = true;
                GUI.backgroundColor = Color.red;
                if (GUILayout.Toggle(_type == AssetType.Missing, "Missing", "LargeButtonRight"))
                {
                    _type = AssetType.Missing;
                }
                GUI.backgroundColor = Color.white;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Search:", GUILayout.Width(60));
            _filterName = EditorGUILayout.TextField("", _filterName, EditorGlobalTools.Styles.SearchTextField, GUILayout.Width(200));
            if (GUILayout.Button("", _filterName != "" ? EditorGlobalTools.Styles.SearchCancelButton : EditorGlobalTools.Styles.SearchCancelButtonEmpty))
            {
                _filterName = "";
                GUI.FocusControl(null);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        private void OnMaterialGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.Label(_materials.Count.ToString(), GUILayout.Width(_previewWidth));
            GUILayout.Label("Name", GUILayout.Width(200));
            GUILayout.Label("Shader", GUILayout.Width(200));
            GUILayout.Label("RenderQueue", GUILayout.Width(140));
            GUILayout.Label("In GameObject", GUILayout.Width(140));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);
            _materialGUIScroll = GUILayout.BeginScrollView(_materialGUIScroll);

            string filter = _filterName.ToLower();
            foreach (var content in _materials)
            {
                if (content.Value.Mat && content.Value.Mat.name.ToLower().Contains(filter))
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(AssetPreview.GetAssetPreview(content.Value.Mat), EditorGlobalTools.Styles.IconButton, GUILayout.Width(_previewWidth), GUILayout.Height(_previewHeight)))
                    {
                        Selection.activeObject = content.Value.Mat;
                        EditorGUIUtility.PingObject(content.Value.Mat);
                    }
                    GUILayout.Label(content.Value.Mat.name, GUILayout.Width(200));
                    GUILayout.Label(content.Value.ShaderName, GUILayout.Width(200));
                    GUILayout.Label(content.Value.RenderQueue.ToString(), GUILayout.Width(140));
                    GUI.enabled = content.Value.InGameObjects.Count > 0;
                    if (GUILayout.Button(content.Value.InGameObjects.Count + " GameObject", GUILayout.Width(140)))
                    {
                        Selection.objects = content.Value.InGameObjects.ToArray();
                    }
                    GUI.enabled = true;
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }
        private void OnTextureGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.Label(_textures.Count.ToString(), GUILayout.Width(_previewWidth));
            GUILayout.Label("Name", GUILayout.Width(200));
            GUILayout.Label("Detail", GUILayout.Width(200));
            GUILayout.Label("Crunched", GUILayout.Width(80));
            GUILayout.Label("MipMaps", GUILayout.Width(120));
            GUILayout.Label("In Material", GUILayout.Width(100));
            GUILayout.Label("In GameObject", GUILayout.Width(140));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);
            _textureGUIScroll = GUILayout.BeginScrollView(_textureGUIScroll);

            string filter = _filterName.ToLower();
            foreach (var content in _textures)
            {
                if (content.Value.Tex && content.Value.Tex.name.ToLower().Contains(filter))
                {
                    GUILayout.BeginHorizontal();
                    GUI.enabled = content.Value.IsKnown;
                    if (GUILayout.Button(AssetPreview.GetAssetPreview(content.Value.Tex), EditorGlobalTools.Styles.IconButton, GUILayout.Width(_previewWidth), GUILayout.Height(_previewHeight)))
                    {
                        Selection.activeObject = content.Value.Tex;
                        EditorGUIUtility.PingObject(content.Value.Tex);
                    }
                    GUILayout.Label(content.Value.Tex.name, GUILayout.Width(200));
                    GUILayout.Label(content.Value.Detail, GUILayout.Width(200));
                    GUILayout.Toggle(content.Value.IsCrunched, "", GUILayout.Width(80));
                    GUILayout.Toggle(content.Value.MipMapCount > 1, "", GUILayout.Width(20));
                    GUILayout.Label(content.Value.MipMapCount + "MipMap", GUILayout.Width(100));
                    GUI.enabled = content.Value.IsKnown && content.Value.InMaterials.Count > 0;
                    if (GUILayout.Button(content.Value.InMaterials.Count + " Material", GUILayout.Width(100)))
                    {
                        Selection.objects = content.Value.InMaterials.ToArray();
                    }
                    GUI.enabled = content.Value.IsKnown;
                    if (GUILayout.Button(content.Value.InGameObjects.Count + " GameObject", GUILayout.Width(140)))
                    {
                        Selection.objects = content.Value.InGameObjects.ToArray();
                    }
                    GUILayout.FlexibleSpace();
                    GUI.enabled = true;
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }
        private void OnMeshGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.Label(_meshes.Count.ToString(), GUILayout.Width(_previewWidth));
            GUILayout.Label("Name", GUILayout.Width(200));
            GUILayout.Label("VertexCount [" + _totalMeshVertices + " verts]", GUILayout.Width(200));
            GUILayout.Label("In StaticBatching", GUILayout.Width(140));
            GUILayout.Label("In Skinned", GUILayout.Width(140));
            GUILayout.Label("In GameObject", GUILayout.Width(140));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);
            _meshGUIScroll = GUILayout.BeginScrollView(_meshGUIScroll);

            string filter = _filterName.ToLower();
            foreach (var content in _meshes)
            {
                if (content.Value.Me && content.Value.Me.name.ToLower().Contains(filter))
                {
                    GUILayout.BeginHorizontal();
                    GUIContent gc = EditorGUIUtility.IconContent("Mesh Icon");
                    if (GUILayout.Button(gc, EditorGlobalTools.Styles.IconButton, GUILayout.Width(_previewWidth), GUILayout.Height(_previewHeight)))
                    {
                        Selection.activeObject = content.Value.Me;
                        EditorGUIUtility.PingObject(content.Value.Me);
                    }
                    GUILayout.Label(content.Value.Me.name, GUILayout.Width(200));
                    GUILayout.Label(content.Value.VertexCount + "x" + content.Value.InGameObjects.Count + " verts", GUILayout.Width(200));
                    GUI.enabled = content.Value.InStaticBatching.Count > 0;
                    if (GUILayout.Button(content.Value.InStaticBatching.Count + " Static Batching", GUILayout.Width(140)))
                    {
                        Selection.objects = content.Value.InStaticBatching.ToArray();
                    }
                    GUI.enabled = content.Value.InSkinned.Count > 0;
                    if (GUILayout.Button(content.Value.InSkinned.Count + " Skinned Mesh", GUILayout.Width(140)))
                    {
                        Selection.objects = content.Value.InSkinned.ToArray();
                    }
                    GUI.enabled = true;
                    if (GUILayout.Button(content.Value.InGameObjects.Count + " GameObject", GUILayout.Width(140)))
                    {
                        Selection.objects = content.Value.InGameObjects.ToArray();
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }
        private void OnMonoScriptGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.Label(_scripts.Count.ToString(), GUILayout.Width(_previewWidth));
            GUILayout.Label("Name", GUILayout.Width(200));
            GUILayout.Label("In GameObject", GUILayout.Width(140));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);
            _monoScriptGUIScroll = GUILayout.BeginScrollView(_monoScriptGUIScroll);

            string filter = _filterName.ToLower();
            foreach (var content in _scripts)
            {
                if (content.Value.Script && content.Value.Script.name.ToLower().Contains(filter))
                {
                    GUILayout.BeginHorizontal();
                    GUIContent gc = EditorGUIUtility.IconContent("cs Script Icon");
                    if (GUILayout.Button(gc, EditorGlobalTools.Styles.IconButton, GUILayout.Width(_previewWidth), GUILayout.Height(_previewHeight)))
                    {
                        Selection.activeObject = content.Value.Script;
                        EditorGUIUtility.PingObject(content.Value.Script);
                    }
                    GUILayout.Label(content.Value.Script.name, GUILayout.Width(200));
                    if (GUILayout.Button(content.Value.InGameObjects.Count + " GameObject", GUILayout.Width(140)))
                    {
                        Selection.objects = content.Value.InGameObjects.ToArray();
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }
        private void OnMissingGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.Label(_missing.Count.ToString(), GUILayout.Width(_previewWidth));
            GUILayout.Label("Name", GUILayout.Width(200));
            GUILayout.Label("Missing", GUILayout.Width(100));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);
            _missingGUIScroll = GUILayout.BeginScrollView(_missingGUIScroll);

            string filter = _filterName.ToLower();
            foreach (var content in _missing)
            {
                if (content.Value.Target && content.Value.Target.name.ToLower().Contains(filter))
                {
                    GUILayout.BeginHorizontal();
                    GUIContent gc = EditorGUIUtility.IconContent("Prefab Icon");
                    if (GUILayout.Button(gc, EditorGlobalTools.Styles.IconButton, GUILayout.Width(_previewWidth), GUILayout.Height(_previewHeight)))
                    {
                        Selection.activeObject = content.Value.Target;
                        EditorGUIUtility.PingObject(content.Value.Target);
                    }
                    GUILayout.Label(content.Value.Target.name, GUILayout.Width(200));
                    GUI.color = Color.red;
                    foreach (var item in content.Value.MissingInfos)
                    {
                        GUILayout.Label("[" + item + "]");
                    }
                    GUI.color = Color.white;
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }
        #endregion

        #region Search Function
        /// <summary>
        /// 从当前打开的场景中重新检索资产
        /// </summary>
        public void SearchAssetsInOpenedScene()
        {
            _type = AssetType.Material;
            _filterName = "";

            _materials.Clear();
            _textures.Clear();
            _meshes.Clear();
            _scripts.Clear();
            _missing.Clear();

            SearchRenderer();
            SearchLightmap();
            SearchUGUI();
            SearchMeshFilter();
            SearchSkinnedMeshRenderer();
            SearchMonoScript();
            SearchTextureInMaterial();

            GetTotalMeshVertices();
        }

        private void SearchRenderer()
        {
            Material skybox = RenderSettings.skybox;
            if (skybox != null)
            {
                GetMaterialContent(skybox, null);
            }

            List<Renderer> renderers = FindAssets<Renderer>();
            for (int i = 0; i < renderers.Count; i++)
            {
                Renderer renderer = renderers[i];
                foreach (Material material in renderer.sharedMaterials)
                {
                    if (material != null)
                    {
                        GetMaterialContent(material, renderer.gameObject);
                    }
                    else
                    {
                        GetMissingContent(renderer.gameObject, "Renderer.Material");
                    }
                }
                if (renderer is SpriteRenderer)
                {
                    SpriteRenderer spriteRenderer = renderer as SpriteRenderer;
                    if (spriteRenderer.sprite != null)
                    {
                        GetTextureContent(spriteRenderer.sprite.texture, renderer.gameObject);
                    }
                    else
                    {
                        GetMissingContent(renderer.gameObject, "SpriteRenderer.Sprite");
                    }
                }
            }
        }
        private void SearchLightmap()
        {
            if (_isIncludeLightmap)
            {
                LightmapData[] lightmapData = LightmapSettings.lightmaps;
                foreach (LightmapData data in lightmapData)
                {
                    if (data.lightmapColor != null)
                    {
                        GetTextureContent(data.lightmapColor, null);
                    }
                    if (data.lightmapDir != null)
                    {
                        GetTextureContent(data.lightmapDir, null);
                    }
                    if (data.shadowMask != null)
                    {
                        GetTextureContent(data.shadowMask, null);
                    }
                }
            }
        }
        private void SearchUGUI()
        {
            if (_isIncludeUGUI)
            {
                List<Graphic> graphics = FindAssets<Graphic>();
                for (int i = 0; i < graphics.Count; i++)
                {
                    Graphic graphic = graphics[i];
                    if (graphic.mainTexture != null)
                    {
                        GetTextureContent(graphic.mainTexture, graphic.gameObject);
                    }
                    if (graphic.materialForRendering != null)
                    {
                        GetMaterialContent(graphic.materialForRendering, graphic.gameObject);
                    }
                }

                List<Selectable> selectables = FindAssets<Selectable>();
                for (int i = 0; i < selectables.Count; i++)
                {
                    Selectable selectable = selectables[i];
                    if (selectable.spriteState.highlightedSprite != null)
                    {
                        GetTextureContent(selectable.spriteState.highlightedSprite.texture, selectable.gameObject);
                    }
                    if (selectable.spriteState.pressedSprite != null)
                    {
                        GetTextureContent(selectable.spriteState.pressedSprite.texture, selectable.gameObject);
                    }
                    if (selectable.spriteState.disabledSprite != null)
                    {
                        GetTextureContent(selectable.spriteState.disabledSprite.texture, selectable.gameObject);
                    }
                }
            }
        }
        private void SearchMeshFilter()
        {
            List<MeshFilter> meshFilters = FindAssets<MeshFilter>();
            for (int i = 0; i < meshFilters.Count; i++)
            {
                MeshFilter filter = meshFilters[i];
                Mesh mesh = filter.sharedMesh;
                if (mesh != null)
                {
                    MeshContent meshContent = GetMeshContent(mesh, filter.gameObject);
                    if (GameObjectUtility.AreStaticEditorFlagsSet(filter.gameObject, StaticEditorFlags.BatchingStatic))
                    {
                        meshContent.InStaticBatching.Add(filter.gameObject);
                    }
                }
                else
                {
                    GetMissingContent(filter.gameObject, "MeshFilter.Mesh");
                }
            }
        }
        private void SearchSkinnedMeshRenderer()
        {
            List<SkinnedMeshRenderer> skinnedMeshRenderers = FindAssets<SkinnedMeshRenderer>();
            for (int i = 0; i < skinnedMeshRenderers.Count; i++)
            {
                SkinnedMeshRenderer skinned = skinnedMeshRenderers[i];
                Mesh mesh = skinned.sharedMesh;
                if (mesh != null)
                {
                    MeshContent meshContent = GetMeshContent(mesh, skinned.gameObject);
                    meshContent.InSkinned.Add(skinned.gameObject);
                }
                else
                {
                    GetMissingContent(skinned.gameObject, "SkinnedMeshRenderer.Mesh");
                }
            }
        }
        private void SearchMonoScript()
        {
            if (_isIncludeMonoScript)
            {
                List<MonoBehaviour> scripts = FindAssets<MonoBehaviour>();
                BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
                for (int i = 0; i < scripts.Count; i++)
                {
                    if (scripts[i] != null)
                    {
                        GetMonoScriptContent(scripts[i]);

                        FieldInfo[] fields = scripts[i].GetType().GetFields(flags);
                        foreach (FieldInfo field in fields)
                        {
                            Type fieldType = field.FieldType;
                            if (fieldType == typeof(Sprite))
                            {
                                Sprite sprite = field.GetValue(scripts[i]) as Sprite;
                                if (sprite != null)
                                {
                                    GetTextureContent(sprite.texture, scripts[i].gameObject);
                                }
                                else
                                {
                                    GetMissingContent(scripts[i].gameObject, "MonoScript.Sprite");
                                }
                            }
                            else if (fieldType == typeof(Mesh))
                            {
                                Mesh mesh = field.GetValue(scripts[i]) as Mesh;
                                if (mesh != null)
                                {
                                    GetMeshContent(mesh, scripts[i].gameObject);
                                }
                                else
                                {
                                    GetMissingContent(scripts[i].gameObject, "MonoScript.Mesh");
                                }
                            }
                            else if (fieldType == typeof(Material))
                            {
                                Material material = field.GetValue(scripts[i]) as Material;
                                if (material != null)
                                {
                                    GetMaterialContent(material, scripts[i].gameObject);
                                }
                                else
                                {
                                    GetMissingContent(scripts[i].gameObject, "MonoScript.Material");
                                }
                            }
                        }
                    }
                }

                List<Transform> transforms = FindAssets<Transform>();
                for (int i = 0; i < transforms.Count; i++)
                {
                    Component[] components = transforms[i].GetComponents<Component>();
                    foreach (Component component in components)
                    {
                        if (component == null)
                        {
                            GetMissingContent(transforms[i].gameObject, "Missing Script");
                            break;
                        }
                    }
                }
            }
        }
        private void SearchTextureInMaterial()
        {
            foreach (var content in _materials)
            {
                Material material = content.Key;
                if (material != null)
                {
                    UnityObject[] dependencies = EditorUtility.CollectDependencies(new UnityObject[] { material });
                    foreach (var depend in dependencies)
                    {
                        if (depend is Texture)
                        {
                            TextureContent textureContent = GetTextureContent(depend as Texture, null);
                            textureContent.InMaterials.Add(material);
                            textureContent.InGameObjects.UnionWith(content.Value.InGameObjects);
                        }
                    }
                    if (material.HasProperty("_MainTex"))
                    {
                        if (material.mainTexture != null)
                        {
                            TextureContent textureContent = GetTextureContent(material.mainTexture, null);
                            textureContent.InMaterials.Add(material);
                            textureContent.InGameObjects.UnionWith(content.Value.InGameObjects);
                        }
                    }
                }
            }
        }

        private void GetTotalMeshVertices()
        {
            _totalMeshVertices = 0;
            foreach (var mesh in _meshes)
            {
                _totalMeshVertices += mesh.Value.Me.vertexCount * mesh.Value.InGameObjects.Count;
            }
        }
        #endregion

        #region Get Asset Function
        private MaterialContent GetMaterialContent(Material material, GameObject obj)
        {
            if (_materials.ContainsKey(material))
            {
                if (obj != null) _materials[material].InGameObjects.Add(obj);
                return _materials[material];
            }
            else
            {
                MaterialContent materialContent = new MaterialContent(material);
                if (obj != null) materialContent.InGameObjects.Add(obj);
                _materials.Add(material, materialContent);
                return materialContent;
            }
        }
        private TextureContent GetTextureContent(Texture texture, GameObject obj)
        {
            if (_textures.ContainsKey(texture))
            {
                if (obj != null) _textures[texture].InGameObjects.Add(obj);
                return _textures[texture];
            }
            else
            {
                TextureContent textureContent = new TextureContent(texture);
                if (obj != null) textureContent.InGameObjects.Add(obj);
                _textures.Add(texture, textureContent);
                return textureContent;
            }
        }
        private MeshContent GetMeshContent(Mesh mesh, GameObject obj)
        {
            if (_meshes.ContainsKey(mesh))
            {
                if (obj != null) _meshes[mesh].InGameObjects.Add(obj);
                return _meshes[mesh];
            }
            else
            {
                MeshContent meshContent = new MeshContent(mesh);
                if (obj != null) meshContent.InGameObjects.Add(obj);
                _meshes.Add(mesh, meshContent);
                return meshContent;
            }
        }
        private MonoScriptContent GetMonoScriptContent(MonoBehaviour mono)
        {
            Type type = mono.GetType();
            if (_scripts.ContainsKey(type))
            {
                _scripts[type].InGameObjects.Add(mono.gameObject);
                return _scripts[type];
            }
            else
            {
                MonoScriptContent monoScriptContent = new MonoScriptContent(mono);
                monoScriptContent.InGameObjects.Add(mono.gameObject);
                _scripts.Add(type, monoScriptContent);
                return monoScriptContent;
            }
        }
        private MissingContent GetMissingContent(GameObject target, string missingType)
        {
            if (_missing.ContainsKey(target))
            {
                _missing[target].MissingInfos.Add(missingType);
                return _missing[target];
            }
            else
            {
                MissingContent missingContent = new MissingContent(target);
                missingContent.MissingInfos.Add(missingType);
                _missing.Add(target, missingContent);
                return missingContent;
            }
        }

        private List<T> FindAssets<T>()
        {
            List<GameObject> roots = new List<GameObject>();
            GlobalTools.GetRootGameObjectsInAllScene(roots);
            if (Main.Current != null)
            {
                roots.Add(Main.Current.gameObject);
            }

            List<T> assets = new List<T>();
            for (int i = 0; i < roots.Count; i++)
            {
                assets.AddRange(roots[i].GetComponentsInChildren<T>(_isIncludeDisabled));
            }
            return assets;
        }
        #endregion

        /// <summary>
        /// 资产类型
        /// </summary>
        public enum AssetType
        {
            Material,
            Texture,
            Mesh,
            MonoScript,
            Missing
        }
    }
}