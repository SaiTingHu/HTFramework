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

        private List<Material> _materialsSort = new List<Material>();
        private List<Texture> _texturesSort = new List<Texture>();
        private List<Mesh> _meshesSort = new List<Mesh>();
        private List<Type> _scriptsSort = new List<Type>();
        private List<GameObject> _missingSort = new List<GameObject>();
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

        private GUIContent _sortGC;
        private GUIContent _meshGC;
        private GUIContent _scriptGC;
        private GUIContent _scriptObjGC;
        private GUIContent _prefabGC;
        private GUIContent _alarmGC;

        private string _filterName = "";
        private int _totalMeshVertices = 0;
        private int _textureAlarmCount = 0;

        public bool IsMarkTextureWidth = true;
        public bool IsMarkTextureHeight = true;
        public bool IsMarkTextureCrunched = true;
        public bool IsMarkTextureMipMap = true;
        public int TextureWidthUpper = 1024;
        public int TextureHeightUpper = 1024;
        public int TextureMipMapUpper = 1;
        #endregion

        #region Lifecycle Function
        protected override void OnEnable()
        {
            base.OnEnable();

            _sortGC = new GUIContent();
            _sortGC.image = EditorGUIUtility.IconContent("Audio Mixer").image;
            _sortGC.tooltip = "Sort";
            _meshGC = EditorGUIUtility.IconContent("Mesh Icon");
            _scriptGC = EditorGUIUtility.IconContent("cs Script Icon");
            _scriptObjGC = EditorGUIUtility.IconContent("ScriptableObject Icon");
            _prefabGC = EditorGUIUtility.IconContent("Prefab Icon");
            _alarmGC = new GUIContent();
            _alarmGC.image = EditorGUIUtility.IconContent("console.warnicon.sml").image;

            EditorApplication.playModeStateChanged += OnPlayModeStateChange;

            ClearAssets();
        }
        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChange;
        }

        protected override string HelpUrl => "https://wanderer.blog.csdn.net/article/details/107974865";

        /// <summary>
        /// 当编辑器播放状态改变
        /// </summary>
        private void OnPlayModeStateChange(PlayModeStateChange state)
        {
            ClearAssets();
            GUI.changed = true;
        }
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
            if (GUILayout.Button("Clear", EditorStyles.toolbarPopup))
            {
                ClearAssets();
                Resources.UnloadUnusedAssets();
            }
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
        /// <summary>
        /// 头部GUI
        /// </summary>
        private void OnHeaderGUI()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Toggle(_type == AssetType.Material, "Material", EditorGlobalTools.Styles.LargeButtonLeft))
            {
                _type = AssetType.Material;
            }
            if (GUILayout.Toggle(_type == AssetType.Texture, "Texture", EditorGlobalTools.Styles.LargeButtonMid))
            {
                _type = AssetType.Texture;
            }
            if (GUILayout.Toggle(_type == AssetType.Mesh, "Mesh", EditorGlobalTools.Styles.LargeButtonMid))
            {
                _type = AssetType.Mesh;
            }
            if (_missing.Count <= 0)
            {
                GUI.enabled = _isIncludeMonoScript;
                if (GUILayout.Toggle(_type == AssetType.MonoScript, "MonoScript", EditorGlobalTools.Styles.LargeButtonRight))
                {
                    _type = AssetType.MonoScript;
                }
                GUI.enabled = true;
            }
            else
            {
                GUI.enabled = _isIncludeMonoScript;
                if (GUILayout.Toggle(_type == AssetType.MonoScript, "MonoScript", EditorGlobalTools.Styles.LargeButtonMid))
                {
                    _type = AssetType.MonoScript;
                }
                GUI.enabled = true;
                GUI.backgroundColor = Color.red;
                if (GUILayout.Toggle(_type == AssetType.Missing, "Missing", EditorGlobalTools.Styles.LargeButtonRight))
                {
                    _type = AssetType.Missing;
                }
                GUI.backgroundColor = Color.white;
            }
            GUILayout.EndHorizontal();

            if (_missing.Count > 0)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox("Some gameObjects are missing asset!", MessageType.Error);
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Search:", GUILayout.Width(60));
            _filterName = EditorGUILayout.TextField("", _filterName, EditorGlobalTools.Styles.SearchTextField, GUILayout.Width(200));
            if (GUILayout.Button("", _filterName != "" ? EditorGlobalTools.Styles.SearchCancelButton : EditorGlobalTools.Styles.SearchCancelButtonEmpty))
            {
                _filterName = "";
                GUI.FocusControl(null);
            }
            GUILayout.FlexibleSpace();
            if (_type == AssetType.Texture)
            {
                GUI.color = _textureAlarmCount > 0 ? Color.yellow : Color.white;
                if (GUILayout.Button("Alarm Value [" + _textureAlarmCount + "]", EditorStyles.popup))
                {
                    TextureAlarmValueSetter.ShowWindow(this, new Vector2(position.x + position.width + 20, position.y + 100));
                }
                GUI.color = Color.white;
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
        }
        /// <summary>
        /// 材质列表GUI
        /// </summary>
        private void OnMaterialGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.Label(_materials.Count.ToString(), GUILayout.Width(_previewWidth));
            if (GUILayout.Button(_sortGC, EditorGlobalTools.Styles.IconButton, GUILayout.Width(20), GUILayout.Height(20)))
            {
                GenericMenu gm = new GenericMenu();
                gm.AddItem(new GUIContent("Ascending"), false, () =>
                {
                    _materialsSort.Sort((a, b) => { return string.Compare(a.name, b.name); });
                });
                gm.AddItem(new GUIContent("Descending"), false, () =>
                {
                    _materialsSort.Sort((a, b) => { return string.Compare(b.name, a.name); });
                });
                gm.ShowAsContext();
            }
            GUILayout.Label("Name", GUILayout.Width(175));
            if (GUILayout.Button(_sortGC, EditorGlobalTools.Styles.IconButton, GUILayout.Width(20), GUILayout.Height(20)))
            {
                GenericMenu gm = new GenericMenu();
                gm.AddItem(new GUIContent("Ascending"), false, () =>
                {
                    _materialsSort.Sort((a, b) => { return string.Compare(_materials[a].ShaderName, _materials[b].ShaderName); });
                });
                gm.AddItem(new GUIContent("Descending"), false, () =>
                {
                    _materialsSort.Sort((a, b) => { return string.Compare(_materials[b].ShaderName, _materials[a].ShaderName); });
                });
                gm.ShowAsContext();
            }
            GUILayout.Label("Shader", GUILayout.Width(175));
            if (GUILayout.Button(_sortGC, EditorGlobalTools.Styles.IconButton, GUILayout.Width(20), GUILayout.Height(20)))
            {
                GenericMenu gm = new GenericMenu();
                gm.AddItem(new GUIContent("Ascending"), false, () =>
                {
                    _materialsSort.Sort((a, b) => { return _materials[a].TextureCount - _materials[b].TextureCount; });
                });
                gm.AddItem(new GUIContent("Descending"), false, () =>
                {
                    _materialsSort.Sort((a, b) => { return _materials[b].TextureCount - _materials[a].TextureCount; });
                });
                gm.ShowAsContext();
            }
            GUILayout.Label("Texture", GUILayout.Width(115));
            if (GUILayout.Button(_sortGC, EditorGlobalTools.Styles.IconButton, GUILayout.Width(20), GUILayout.Height(20)))
            {
                GenericMenu gm = new GenericMenu();
                gm.AddItem(new GUIContent("Ascending"), false, () =>
                {
                    _materialsSort.Sort((a, b) => { return _materials[a].RenderQueue - _materials[b].RenderQueue; });
                });
                gm.AddItem(new GUIContent("Descending"), false, () =>
                {
                    _materialsSort.Sort((a, b) => { return _materials[b].RenderQueue - _materials[a].RenderQueue; });
                });
                gm.ShowAsContext();
            }
            GUILayout.Label("RenderQueue", GUILayout.Width(115));
            if (GUILayout.Button(_sortGC, EditorGlobalTools.Styles.IconButton, GUILayout.Width(20), GUILayout.Height(20)))
            {
                GenericMenu gm = new GenericMenu();
                gm.AddItem(new GUIContent("Ascending"), false, () =>
                {
                    _materialsSort.Sort((a, b) => { return _materials[a].InGameObjects.Count - _materials[b].InGameObjects.Count; });
                });
                gm.AddItem(new GUIContent("Descending"), false, () =>
                {
                    _materialsSort.Sort((a, b) => { return _materials[b].InGameObjects.Count - _materials[a].InGameObjects.Count; });
                });
                gm.ShowAsContext();
            }
            GUILayout.Label("In GameObject", GUILayout.Width(115));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);
            _materialGUIScroll = GUILayout.BeginScrollView(_materialGUIScroll);

            string filter = _filterName.ToLower();
            for (int i = 0; i < _materialsSort.Count; i++)
            {
                if (_materialsSort[i] != null && _materialsSort[i].name.ToLower().Contains(filter))
                {
                    MaterialContent content = _materials[_materialsSort[i]];
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(AssetPreview.GetAssetPreview(content.Mat), EditorGlobalTools.Styles.IconButton, GUILayout.Width(_previewWidth), GUILayout.Height(_previewHeight)))
                    {
                        Selection.activeObject = content.Mat;
                        EditorGUIUtility.PingObject(content.Mat);
                    }
                    GUILayout.Label(content.Mat.name, GUILayout.Width(200));
                    GUILayout.Label(content.ShaderName, GUILayout.Width(200));
                    GUILayout.Label(content.TextureCount.ToString(), GUILayout.Width(140));
                    GUILayout.Label(content.RenderQueue.ToString(), GUILayout.Width(140));
                    GUI.enabled = content.InGameObjects.Count > 0;
                    if (GUILayout.Button(content.InGameObjects.Count + " GameObject", GUILayout.Width(140)))
                    {
                        Selection.objects = content.InGameObjects.ToArray();
                    }
                    GUI.enabled = true;
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }
        /// <summary>
        /// 贴图列表GUI
        /// </summary>
        private void OnTextureGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.Label(_textures.Count.ToString(), GUILayout.Width(_previewWidth));
            if (GUILayout.Button(_sortGC, EditorGlobalTools.Styles.IconButton, GUILayout.Width(20), GUILayout.Height(20)))
            {
                GenericMenu gm = new GenericMenu();
                gm.AddItem(new GUIContent("Ascending"), false, () =>
                {
                    _texturesSort.Sort((a, b) => { return string.Compare(a.name, b.name); });
                });
                gm.AddItem(new GUIContent("Descending"), false, () =>
                {
                    _texturesSort.Sort((a, b) => { return string.Compare(b.name, a.name); });
                });
                gm.AddSeparator("");
                gm.AddItem(new GUIContent("Alarm Value"), false, () =>
                {
                    _texturesSort.Sort((a, b) => { return _textures[b].AlarmLevel - _textures[a].AlarmLevel; });
                });
                gm.ShowAsContext();
            }
            GUILayout.Label("Name", GUILayout.Width(175));
            if (GUILayout.Button(_sortGC, EditorGlobalTools.Styles.IconButton, GUILayout.Width(20), GUILayout.Height(20)))
            {
                GenericMenu gm = new GenericMenu();
                gm.AddItem(new GUIContent("Ascending"), false, () =>
                {
                    _texturesSort.Sort((a, b) => { return _textures[a].Size - _textures[b].Size; });
                });
                gm.AddItem(new GUIContent("Descending"), false, () =>
                {
                    _texturesSort.Sort((a, b) => { return _textures[b].Size - _textures[a].Size; });
                });
                gm.ShowAsContext();
            }
            GUILayout.Label("Detail", GUILayout.Width(175));
            if (GUILayout.Button(_sortGC, EditorGlobalTools.Styles.IconButton, GUILayout.Width(20), GUILayout.Height(20)))
            {
                GenericMenu gm = new GenericMenu();
                gm.AddItem(new GUIContent("Ascending"), false, () =>
                {
                    _texturesSort.Sort((a, b) => { return Convert.ToInt32(_textures[a].IsCrunched) - Convert.ToInt32(_textures[b].IsCrunched); });
                });
                gm.AddItem(new GUIContent("Descending"), false, () =>
                {
                    _texturesSort.Sort((a, b) => { return Convert.ToInt32(_textures[b].IsCrunched) - Convert.ToInt32(_textures[a].IsCrunched); });
                });
                gm.ShowAsContext();
            }
            GUILayout.Label("Crunched", GUILayout.Width(95));
            if (GUILayout.Button(_sortGC, EditorGlobalTools.Styles.IconButton, GUILayout.Width(20), GUILayout.Height(20)))
            {
                GenericMenu gm = new GenericMenu();
                gm.AddItem(new GUIContent("Ascending"), false, () =>
                {
                    _texturesSort.Sort((a, b) => { return _textures[a].MipMapCount - _textures[b].MipMapCount; });
                });
                gm.AddItem(new GUIContent("Descending"), false, () =>
                {
                    _texturesSort.Sort((a, b) => { return _textures[b].MipMapCount - _textures[a].MipMapCount; });
                });
                gm.ShowAsContext();
            }
            GUILayout.Label("MipMaps", GUILayout.Width(95));
            if (GUILayout.Button(_sortGC, EditorGlobalTools.Styles.IconButton, GUILayout.Width(20), GUILayout.Height(20)))
            {
                GenericMenu gm = new GenericMenu();
                gm.AddItem(new GUIContent("Ascending"), false, () =>
                {
                    _texturesSort.Sort((a, b) => { return _textures[a].InMaterials.Count - _textures[b].InMaterials.Count; });
                });
                gm.AddItem(new GUIContent("Descending"), false, () =>
                {
                    _texturesSort.Sort((a, b) => { return _textures[b].InMaterials.Count - _textures[a].InMaterials.Count; });
                });
                gm.ShowAsContext();
            }
            GUILayout.Label("In Material", GUILayout.Width(75));
            if (GUILayout.Button(_sortGC, EditorGlobalTools.Styles.IconButton, GUILayout.Width(20), GUILayout.Height(20)))
            {
                GenericMenu gm = new GenericMenu();
                gm.AddItem(new GUIContent("Ascending"), false, () =>
                {
                    _texturesSort.Sort((a, b) => { return _textures[a].InGameObjects.Count - _textures[b].InGameObjects.Count; });
                });
                gm.AddItem(new GUIContent("Descending"), false, () =>
                {
                    _texturesSort.Sort((a, b) => { return _textures[b].InGameObjects.Count - _textures[a].InGameObjects.Count; });
                });
                gm.ShowAsContext();
            }
            GUILayout.Label("In GameObject", GUILayout.Width(115));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);
            _textureGUIScroll = GUILayout.BeginScrollView(_textureGUIScroll);

            string filter = _filterName.ToLower();
            for (int i = 0; i < _texturesSort.Count; i++)
            {
                if (_texturesSort[i] != null && _texturesSort[i].name.ToLower().Contains(filter))
                {
                    TextureContent content = _textures[_texturesSort[i]];
                    GUILayout.BeginHorizontal();
                    GUI.enabled = content.IsKnown;
                    if (GUILayout.Button(AssetPreview.GetAssetPreview(content.Tex), EditorGlobalTools.Styles.IconButton, GUILayout.Width(_previewWidth), GUILayout.Height(_previewHeight)))
                    {
                        Selection.activeObject = content.Tex;
                        EditorGUIUtility.PingObject(content.Tex);
                    }
                    GUILayout.Label(content.Tex.name, GUILayout.Width(200));
                    GUILayout.Label(content.Detail, GUILayout.Width(200));
                    GUILayout.Toggle(content.IsCrunched, "", GUILayout.Width(120));
                    GUILayout.Toggle(content.MipMapCount > 1, "", GUILayout.Width(20));
                    GUILayout.Label(content.MipMapCount + "MipMap", GUILayout.Width(100));
                    GUI.enabled = content.IsKnown && content.InMaterials.Count > 0;
                    if (GUILayout.Button(content.InMaterials.Count + " Material", GUILayout.Width(100)))
                    {
                        Selection.objects = content.InMaterials.ToArray();
                    }
                    GUI.enabled = content.IsKnown;
                    if (GUILayout.Button(content.InGameObjects.Count + " GameObject", GUILayout.Width(140)))
                    {
                        Selection.objects = content.InGameObjects.ToArray();
                    }
                    if (content.AlarmLevel > 0)
                    {
                        _alarmGC.tooltip = content.AlarmMessage;
                        GUILayout.Box(_alarmGC, EditorStyles.label, GUILayout.Width(20), GUILayout.Height(20));
                    }
                    GUILayout.FlexibleSpace();
                    GUI.enabled = true;
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }
        /// <summary>
        /// 网格列表GUI
        /// </summary>
        private void OnMeshGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.Label(_meshes.Count.ToString(), GUILayout.Width(_previewWidth));
            if (GUILayout.Button(_sortGC, EditorGlobalTools.Styles.IconButton, GUILayout.Width(20), GUILayout.Height(20)))
            {
                GenericMenu gm = new GenericMenu();
                gm.AddItem(new GUIContent("Ascending"), false, () =>
                {
                    _meshesSort.Sort((a, b) => { return string.Compare(a.name, b.name); });
                });
                gm.AddItem(new GUIContent("Descending"), false, () =>
                {
                    _meshesSort.Sort((a, b) => { return string.Compare(b.name, a.name); });
                });
                gm.ShowAsContext();
            }
            GUILayout.Label("Name", GUILayout.Width(175));
            if (GUILayout.Button(_sortGC, EditorGlobalTools.Styles.IconButton, GUILayout.Width(20), GUILayout.Height(20)))
            {
                GenericMenu gm = new GenericMenu();
                gm.AddItem(new GUIContent("Ascending"), false, () =>
                {
                    _meshesSort.Sort((a, b) => { return _meshes[a].VertexCount - _meshes[b].VertexCount; });
                });
                gm.AddItem(new GUIContent("Descending"), false, () =>
                {
                    _meshesSort.Sort((a, b) => { return _meshes[b].VertexCount - _meshes[a].VertexCount; });
                });
                gm.ShowAsContext();
            }
            GUILayout.Label("VertexCount [" + _totalMeshVertices + " verts]", GUILayout.Width(175));
            if (GUILayout.Button(_sortGC, EditorGlobalTools.Styles.IconButton, GUILayout.Width(20), GUILayout.Height(20)))
            {
                GenericMenu gm = new GenericMenu();
                gm.AddItem(new GUIContent("Ascending"), false, () =>
                {
                    _meshesSort.Sort((a, b) => { return _meshes[a].InStaticBatching.Count - _meshes[b].InStaticBatching.Count; });
                });
                gm.AddItem(new GUIContent("Descending"), false, () =>
                {
                    _meshesSort.Sort((a, b) => { return _meshes[b].InStaticBatching.Count - _meshes[a].InStaticBatching.Count; });
                });
                gm.ShowAsContext();
            }
            GUILayout.Label("In StaticBatching", GUILayout.Width(115));
            if (GUILayout.Button(_sortGC, EditorGlobalTools.Styles.IconButton, GUILayout.Width(20), GUILayout.Height(20)))
            {
                GenericMenu gm = new GenericMenu();
                gm.AddItem(new GUIContent("Ascending"), false, () =>
                {
                    _meshesSort.Sort((a, b) => { return _meshes[a].InSkinned.Count - _meshes[b].InSkinned.Count; });
                });
                gm.AddItem(new GUIContent("Descending"), false, () =>
                {
                    _meshesSort.Sort((a, b) => { return _meshes[b].InSkinned.Count - _meshes[a].InSkinned.Count; });
                });
                gm.ShowAsContext();
            }
            GUILayout.Label("In Skinned", GUILayout.Width(115));
            if (GUILayout.Button(_sortGC, EditorGlobalTools.Styles.IconButton, GUILayout.Width(20), GUILayout.Height(20)))
            {
                GenericMenu gm = new GenericMenu();
                gm.AddItem(new GUIContent("Ascending"), false, () =>
                {
                    _meshesSort.Sort((a, b) => { return _meshes[a].InGameObjects.Count - _meshes[b].InGameObjects.Count; });
                });
                gm.AddItem(new GUIContent("Descending"), false, () =>
                {
                    _meshesSort.Sort((a, b) => { return _meshes[b].InGameObjects.Count - _meshes[a].InGameObjects.Count; });
                });
                gm.ShowAsContext();
            }
            GUILayout.Label("In GameObject", GUILayout.Width(115));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);
            _meshGUIScroll = GUILayout.BeginScrollView(_meshGUIScroll);

            string filter = _filterName.ToLower();
            for (int i = 0; i < _meshesSort.Count; i++)
            {
                if (_meshesSort[i] != null && _meshesSort[i].name.ToLower().Contains(filter))
                {
                    MeshContent content = _meshes[_meshesSort[i]];
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(_meshGC, EditorGlobalTools.Styles.IconButton, GUILayout.Width(_previewWidth), GUILayout.Height(_previewHeight)))
                    {
                        Selection.activeObject = content.Me;
                        EditorGUIUtility.PingObject(content.Me);
                    }
                    GUILayout.Label(content.Me.name, GUILayout.Width(200));
                    GUILayout.Label(content.VertexCount + "x" + content.InGameObjects.Count + " verts", GUILayout.Width(200));
                    GUI.enabled = content.InStaticBatching.Count > 0;
                    if (GUILayout.Button(content.InStaticBatching.Count + " Static Batching", GUILayout.Width(140)))
                    {
                        Selection.objects = content.InStaticBatching.ToArray();
                    }
                    GUI.enabled = content.InSkinned.Count > 0;
                    if (GUILayout.Button(content.InSkinned.Count + " Skinned Mesh", GUILayout.Width(140)))
                    {
                        Selection.objects = content.InSkinned.ToArray();
                    }
                    GUI.enabled = true;
                    if (GUILayout.Button(content.InGameObjects.Count + " GameObject", GUILayout.Width(140)))
                    {
                        Selection.objects = content.InGameObjects.ToArray();
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }
        /// <summary>
        /// 脚本列表GUI
        /// </summary>
        private void OnMonoScriptGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.Label(_scripts.Count.ToString(), GUILayout.Width(_previewWidth));
            if (GUILayout.Button(_sortGC, EditorGlobalTools.Styles.IconButton, GUILayout.Width(20), GUILayout.Height(20)))
            {
                GenericMenu gm = new GenericMenu();
                gm.AddItem(new GUIContent("Ascending"), false, () =>
                {
                    _scriptsSort.Sort((a, b) => { return string.Compare(_scripts[a].Script.name, _scripts[b].Script.name); });
                });
                gm.AddItem(new GUIContent("Descending"), false, () =>
                {
                    _scriptsSort.Sort((a, b) => { return string.Compare(_scripts[b].Script.name, _scripts[a].Script.name); });
                });
                gm.ShowAsContext();
            }
            GUILayout.Label("Name", GUILayout.Width(175));
            if (GUILayout.Button(_sortGC, EditorGlobalTools.Styles.IconButton, GUILayout.Width(20), GUILayout.Height(20)))
            {
                GenericMenu gm = new GenericMenu();
                gm.AddItem(new GUIContent("Ascending"), false, () =>
                {
                    _scriptsSort.Sort((a, b) => { return string.Compare(_scripts[a].Assembly, _scripts[b].Assembly); });
                });
                gm.AddItem(new GUIContent("Descending"), false, () =>
                {
                    _scriptsSort.Sort((a, b) => { return string.Compare(_scripts[b].Assembly, _scripts[a].Assembly); });
                });
                gm.ShowAsContext();
            }
            GUILayout.Label("Assembly", GUILayout.Width(175));
            if (GUILayout.Button(_sortGC, EditorGlobalTools.Styles.IconButton, GUILayout.Width(20), GUILayout.Height(20)))
            {
                GenericMenu gm = new GenericMenu();
                gm.AddItem(new GUIContent("Ascending"), false, () =>
                {
                    _scriptsSort.Sort((a, b) => { return _scripts[a].InGameObjects.Count - _scripts[b].InGameObjects.Count; });
                });
                gm.AddItem(new GUIContent("Descending"), false, () =>
                {
                    _scriptsSort.Sort((a, b) => { return _scripts[b].InGameObjects.Count - _scripts[a].InGameObjects.Count; });
                });
                gm.ShowAsContext();
            }
            GUILayout.Label("In GameObject", GUILayout.Width(115));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);
            _monoScriptGUIScroll = GUILayout.BeginScrollView(_monoScriptGUIScroll);

            string filter = _filterName.ToLower();
            for (int i = 0; i < _scriptsSort.Count; i++)
            {
                MonoScriptContent content = _scripts[_scriptsSort[i]];
                if (content.Script && content.Script.name.ToLower().Contains(filter))
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(content.IsMono ? _scriptGC : _scriptObjGC, EditorGlobalTools.Styles.IconButton, GUILayout.Width(_previewWidth), GUILayout.Height(_previewHeight)))
                    {
                        Selection.activeObject = content.Script;
                        EditorGUIUtility.PingObject(content.Script);
                    }
                    GUILayout.Label(content.Script.name, GUILayout.Width(200));
                    GUILayout.Label(content.Assembly, GUILayout.Width(200));
                    if (GUILayout.Button(content.InGameObjects.Count + " GameObject", GUILayout.Width(140)))
                    {
                        Selection.objects = content.InGameObjects.ToArray();
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }
        /// <summary>
        /// 丢失资产列表GUI
        /// </summary>
        private void OnMissingGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.Label(_missing.Count.ToString(), GUILayout.Width(_previewWidth));
            if (GUILayout.Button(_sortGC, EditorGlobalTools.Styles.IconButton, GUILayout.Width(20), GUILayout.Height(20)))
            {
                GenericMenu gm = new GenericMenu();
                gm.AddItem(new GUIContent("Ascending"), false, () =>
                {
                    _missingSort.Sort((a, b) => { return string.Compare(a.name, b.name); });
                });
                gm.AddItem(new GUIContent("Descending"), false, () =>
                {
                    _missingSort.Sort((a, b) => { return string.Compare(b.name, a.name); });
                });
                gm.ShowAsContext();
            }
            GUILayout.Label("Name", GUILayout.Width(175));
            if (GUILayout.Button(_sortGC, EditorGlobalTools.Styles.IconButton, GUILayout.Width(20), GUILayout.Height(20)))
            {
                GenericMenu gm = new GenericMenu();
                gm.AddItem(new GUIContent("Ascending"), false, () =>
                {
                    _missingSort.Sort((a, b) => { return _missing[a].MissingInfos.Count - _missing[b].MissingInfos.Count; });
                });
                gm.AddItem(new GUIContent("Descending"), false, () =>
                {
                    _missingSort.Sort((a, b) => { return _missing[b].MissingInfos.Count - _missing[a].MissingInfos.Count; });
                });
                gm.ShowAsContext();
            }
            GUILayout.Label("Missing", GUILayout.Width(100));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);
            _missingGUIScroll = GUILayout.BeginScrollView(_missingGUIScroll);

            string filter = _filterName.ToLower();
            for (int i = 0; i < _missingSort.Count; i++)
            {
                if (_missingSort[i] != null && _missingSort[i].name.ToLower().Contains(filter))
                {
                    MissingContent content = _missing[_missingSort[i]];
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(_prefabGC, EditorGlobalTools.Styles.IconButton, GUILayout.Width(_previewWidth), GUILayout.Height(_previewHeight)))
                    {
                        Selection.activeObject = content.Target;
                        EditorGUIUtility.PingObject(content.Target);
                    }
                    GUILayout.Label(content.Target.name, GUILayout.Width(200));
                    GUI.color = Color.red;
                    foreach (var item in content.MissingInfos)
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
            ClearAssets();

            SearchRenderer();
            SearchLightmap();
            SearchUGUI();
            SearchMeshFilter();
            SearchSkinnedMeshRenderer();
            SearchMonoScript();
            SearchTextureInMaterial();

            GetTotalMeshVertices();
        }
        /// <summary>
        /// 清空所有资产
        /// </summary>
        public void ClearAssets()
        {
            _type = AssetType.Material;
            _filterName = "";
            _totalMeshVertices = 0;
            _textureAlarmCount = 0;

            _materialsSort.Clear();
            _texturesSort.Clear();
            _meshesSort.Clear();
            _scriptsSort.Clear();
            _missingSort.Clear();
            _materials.Clear();
            _textures.Clear();
            _meshes.Clear();
            _scripts.Clear();
            _missing.Clear();
        }

        /// <summary>
        /// 检索所有渲染器
        /// </summary>
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
                        GetMissingContent(renderer.gameObject, renderer.GetType().Name + ".Material");
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
        /// <summary>
        /// 检索所有光照贴图
        /// </summary>
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
        /// <summary>
        /// 检索所有UGUI元素
        /// </summary>
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
        /// <summary>
        /// 检索所有网格组件
        /// </summary>
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
        /// <summary>
        /// 检索所有蒙皮网格组件
        /// </summary>
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
        /// <summary>
        /// 检索所有脚本
        /// </summary>
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
                            if (fieldType == typeof(Sprite) || fieldType.IsSubclassOf(typeof(Sprite)))
                            {
                                Sprite sprite = field.GetValue(scripts[i]) as Sprite;
                                if (sprite != null)
                                {
                                    GetTextureContent(sprite.texture, scripts[i].gameObject);
                                }
                                else
                                {
                                    GetMissingContent(scripts[i].gameObject, scripts[i].GetType().Name + ".Sprite");
                                }
                            }
                            else if (fieldType == typeof(Mesh) || fieldType.IsSubclassOf(typeof(Mesh)))
                            {
                                Mesh mesh = field.GetValue(scripts[i]) as Mesh;
                                if (mesh != null)
                                {
                                    GetMeshContent(mesh, scripts[i].gameObject);
                                }
                                else
                                {
                                    GetMissingContent(scripts[i].gameObject, scripts[i].GetType().Name + ".Mesh");
                                }
                            }
                            else if (fieldType == typeof(Material) || fieldType.IsSubclassOf(typeof(Material)))
                            {
                                Material material = field.GetValue(scripts[i]) as Material;
                                if (material != null)
                                {
                                    GetMaterialContent(material, scripts[i].gameObject);
                                }
                                else
                                {
                                    GetMissingContent(scripts[i].gameObject, scripts[i].GetType().Name + ".Material");
                                }
                            }
                            else if (fieldType == typeof(ScriptableObject) || fieldType.IsSubclassOf(typeof(ScriptableObject)))
                            {
                                ScriptableObject scriptObj = field.GetValue(scripts[i]) as ScriptableObject;
                                if (scriptObj != null)
                                {
                                    GetMonoScriptContent(scriptObj, scripts[i].gameObject);
                                }
                                else
                                {
                                    GetMissingContent(scripts[i].gameObject, scripts[i].GetType().Name + ".ScriptableObject");
                                }
                            }
                        }
                    }
                }

                //检索所有Missing脚本
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
        /// <summary>
        /// 检索所有材质中的贴图
        /// </summary>
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
                            content.Value.AddTextureDependence();
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

        /// <summary>
        /// 获取网格总顶点数
        /// </summary>
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
        /// <summary>
        /// 获取材质资产
        /// </summary>
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
                _materialsSort.Add(material);
                return materialContent;
            }
        }
        /// <summary>
        /// 获取贴图资产
        /// </summary>
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
                _texturesSort.Add(texture);

                bool isAlarm = false;
                if (IsMarkTextureWidth)
                {
                    if (texture.width > TextureWidthUpper)
                    {
                        textureContent.RaiseAlarmLevel("Width > " + TextureWidthUpper);
                        isAlarm = true;
                    }
                }
                if (IsMarkTextureHeight)
                {
                    if (texture.height > TextureHeightUpper)
                    {
                        textureContent.RaiseAlarmLevel("Height > " + TextureHeightUpper);
                        isAlarm = true;
                    }
                }
                if (IsMarkTextureCrunched)
                {
                    if (!textureContent.IsCrunched)
                    {
                        textureContent.RaiseAlarmLevel("Not enabled crunched");
                        isAlarm = true;
                    }
                }
                if (IsMarkTextureMipMap)
                {
                    if (textureContent.MipMapCount > TextureMipMapUpper)
                    {
                        textureContent.RaiseAlarmLevel("MipMap too much");
                        isAlarm = true;
                    }
                }
                if (isAlarm)
                {
                    _textureAlarmCount += 1;
                }

                return textureContent;
            }
        }
        /// <summary>
        /// 获取网格资产
        /// </summary>
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
                _meshesSort.Add(mesh);
                return meshContent;
            }
        }
        /// <summary>
        /// 获取脚本资产
        /// </summary>
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
                _scriptsSort.Add(type);
                return monoScriptContent;
            }
        }
        /// <summary>
        /// 获取脚本资产
        /// </summary>
        private MonoScriptContent GetMonoScriptContent(ScriptableObject scriptObj, GameObject obj)
        {
            Type type = scriptObj.GetType();
            if (_scripts.ContainsKey(type))
            {
                _scripts[type].InGameObjects.Add(obj);
                return _scripts[type];
            }
            else
            {
                MonoScriptContent monoScriptContent = new MonoScriptContent(scriptObj);
                monoScriptContent.InGameObjects.Add(obj);
                _scripts.Add(type, monoScriptContent);
                _scriptsSort.Add(type);
                return monoScriptContent;
            }
        }
        /// <summary>
        /// 获取丢失的资产
        /// </summary>
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
                _missingSort.Add(target);
                return missingContent;
            }
        }

        /// <summary>
        /// 查找指定类型的资产，在当前打开的场景中
        /// </summary>
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