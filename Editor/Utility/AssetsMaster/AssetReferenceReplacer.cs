using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 资产引用替换器
    /// </summary>
    internal sealed class AssetReferenceReplacer : HTFEditorWindow
    {
        /// <summary>
        /// 添加资产到监测列表中
        /// </summary>
        /// <param name="assetPath">资产路径</param>
        public static void AddAssetByPath(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath) || !assetPath.EndsWith(".mat"))
            {
                Log.Error("资产引用替换器（AssetReferenceReplacer）：目前只支持处理材质资产（Material）。");
                return;
            }

            Material material = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
            AddAssetByObject(material);
        }
        /// <summary>
        /// 添加资产到监测列表中
        /// </summary>
        /// <param name="asset">资产</param>
        public static void AddAssetByObject(Object asset)
        {
            AssetReferenceReplacer window = GetWindow<AssetReferenceReplacer>();
            window.titleContent.image = EditorGUIUtility.IconContent("Custom").image;
            window.titleContent.text = "Asset Reference Replacer";
            window.AddMaterial(asset as Material);
            window.Show();
        }

        private AssetsMaster.AssetType _assetType = AssetsMaster.AssetType.Material;
        private Material _replaceTargetMaterial;
        private List<Material> _materials = new List<Material>();
        private Vector2 _materialScroll;
        private List<DefaultAsset> _searchFolders = new List<DefaultAsset>();
        private bool _hasPrefab = true;
        private bool _hasScene = true;
        private Vector2 _searchFoldersScroll;

        protected override string HelpUrl => "https://wanderer.blog.csdn.net/article/details/171351058";

        protected override void OnTitleGUI()
        {
            base.OnTitleGUI();

            GUILayout.FlexibleSpace();
        }
        protected override void OnBodyGUI()
        {
            base.OnBodyGUI();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Asset Type", GUILayout.Width(100));
            GUI.enabled = false;
            EditorGUILayout.EnumPopup(_assetType);
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            switch (_assetType)
            {
                case AssetsMaster.AssetType.Material:
                    OnMaterialGUI();
                    break;
            }

            OnSearchFoldersGUI();

            GUILayout.BeginHorizontal();
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Replace", EditorGlobalTools.Styles.LargeButton))
            {
                if (EditorUtility.DisplayDialog("Prompt", "Are you sure you want to replace all asset references in [Assets List] with the [Replace Target] reference? This cannot be undone!", "Yes", "No"))
                {
                    switch (_assetType)
                    {
                        case AssetsMaster.AssetType.Material:
                            string result = ReplaceAllMaterialsReference();
                            StringValueEditor.OpenWindow(this, result, "Asset Reference Replace Result", true, null);
                            EditorUtility.ClearProgressBar();
                            break;
                    }
                }
            }
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();
        }
        private void OnMaterialGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Replace Target", GUILayout.Width(100));
            GUI.color = _replaceTargetMaterial != null ? Color.white : Color.gray;
            _replaceTargetMaterial = EditorGUILayout.ObjectField(_replaceTargetMaterial, typeof(Material), false) as Material;
            GUI.color = Color.white;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Assets List", GUILayout.Width(100));
            EditorGUILayout.LabelField(_materials.Count.ToString(), GUILayout.Width(60));
            GUILayout.FlexibleSpace();
            GUI.color = Color.green;
            if (GUILayout.Button("", "OL Plus", GUILayout.Width(20), GUILayout.Height(20)))
            {
                _materials.Add(null);
            }
            GUI.color = Color.white;
            GUILayout.Space(5);
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("Box");
            _materialScroll = GUILayout.BeginScrollView(_materialScroll);

            for (int i = 0; i < _materials.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUI.color = _materials[i] != null ? Color.white : Color.gray;
                GUILayout.Label((i + 1).ToString(), GUILayout.Width(20));
                _materials[i] = EditorGUILayout.ObjectField(_materials[i], typeof(Material), false) as Material;
                GUI.color = Color.red;
                if (GUILayout.Button("", "OL Minus", GUILayout.Width(20), GUILayout.Height(20)))
                {
                    _materials.RemoveAt(i);
                    i--;
                }
                GUI.color = Color.white;
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }
        private void OnSearchFoldersGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Search Folders", GUILayout.Width(100));
            EditorGUILayout.LabelField(_searchFolders.Count.ToString(), GUILayout.Width(60));
            GUI.color = _hasPrefab ? Color.white : Color.gray;
            _hasPrefab = EditorGUILayout.ToggleLeft("Prefab", _hasPrefab, GUILayout.Width(60));
            GUI.color = _hasScene ? Color.white : Color.gray;
            _hasScene = EditorGUILayout.ToggleLeft("Scene", _hasScene, GUILayout.Width(60));
            GUILayout.FlexibleSpace();
            GUI.color = Color.green;
            if (GUILayout.Button("", "OL Plus", GUILayout.Width(20), GUILayout.Height(20)))
            {
                _searchFolders.Add(null);
            }
            GUI.color = Color.white;
            GUILayout.Space(5);
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("Box");
            _searchFoldersScroll = GUILayout.BeginScrollView(_searchFoldersScroll);

            for (int i = 0; i < _searchFolders.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUI.color = _searchFolders[i] != null ? Color.white : Color.gray;
                GUILayout.Label((i + 1).ToString(), GUILayout.Width(20));
                _searchFolders[i] = EditorGUILayout.ObjectField(_searchFolders[i], typeof(DefaultAsset), false) as DefaultAsset;
                GUI.color = Color.red;
                if (GUILayout.Button("", "OL Minus", GUILayout.Width(20), GUILayout.Height(20)))
                {
                    _searchFolders.RemoveAt(i);
                    i--;
                }
                GUI.color = Color.white;
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        /// <summary>
        /// 添加材质资产到监测列表中
        /// </summary>
        private void AddMaterial(Material material)
        {
            if (material == null)
                return;

            if (!_materials.Contains(material))
            {
                _materials.Add(material);
            }
        }
        /// <summary>
        /// 替换所有材质资产的引用
        /// </summary>
        private string ReplaceAllMaterialsReference()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("开始替换材质资产的引用：");

            if (_replaceTargetMaterial == null)
            {
                sb.Append("<color=yellow>Replace Target 为空！</color>");
                return sb.ToString();
            }

            if (_materials.Count <= 0)
            {
                sb.Append("<color=yellow>Assets List 为空！</color>");
                return sb.ToString();
            }

            if (_searchFolders.Count <= 0)
            {
                sb.Append("<color=yellow>Search Folders 为空！</color>");
                return sb.ToString();
            }

            //获取替换目标材质的GUID
            string targetMaterialGuid = null;
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(_replaceTargetMaterial, out targetMaterialGuid, out long targetLocalId);
            if (string.IsNullOrEmpty(targetMaterialGuid))
            {
                sb.Append("<color=yellow>Replace Target 的 Guid 为空！</color>");
                return sb.ToString();
            }

            //获取所有旧材质的GUID
            Dictionary<string, Material> oldMaterialsGuid = new Dictionary<string, Material>();
            for (int i = 0; i < _materials.Count; i++)
            {
                EditorUtility.DisplayProgressBar("Replace All Materials Reference", $"Search Old Materials Guid......（{i}/{_materials.Count}）", ((float)i) / _materials.Count);

                Material material = _materials[i];
                if (material == null)
                    continue;

                if (material == _replaceTargetMaterial)
                    continue;

                string oldGuid = null;
                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(material, out oldGuid, out long oldLocalId);
                if (string.IsNullOrEmpty(oldGuid))
                    continue;

                if (!oldMaterialsGuid.ContainsKey(oldGuid))
                    oldMaterialsGuid.Add(oldGuid, material);
            }

            //所有需要搜索的文件夹
            List<string> foldersList = new List<string>();
            for (int i = 0; i < _searchFolders.Count; i++)
            {
                EditorUtility.DisplayProgressBar("Replace All Materials Reference", $"Search Prefab And Scene......（{i}/{_searchFolders.Count}）", ((float)i) / _searchFolders.Count);

                if (_searchFolders[i] == null)
                    continue;

                foldersList.Add(AssetDatabase.GetAssetPath(_searchFolders[i]));
            }
            string[] folders = foldersList.ToArray();
            if (folders.Length <= 0)
            {
                sb.Append("<color=yellow>Search Folders 为空！</color>");
                return sb.ToString();
            }

            //获取所有【预制体】及【场景】资产，材质资产目前仅可被这两种资产引用
            string[] prefabGuids = _hasPrefab ? AssetDatabase.FindAssets("t:Prefab", folders) : new string[0];
            string[] sceneAssetGuids = _hasScene ? AssetDatabase.FindAssets("t:Scene", folders) : new string[0];
            if (prefabGuids.Length <= 0 && sceneAssetGuids.Length <= 0)
            {
                sb.Append("<color=yellow>当前项目工程中不存在任意【预制体】及【场景】资产，或未启用相关资产类型的检索！</color>");
                return sb.ToString();
            }

            //替换所有【预制体】资产中的材质
            List<ReplaceMaterialResult> results = new List<ReplaceMaterialResult>();
            if (_hasPrefab)
            {
                for (int i = 0; i < prefabGuids.Length; i++)
                {
                    EditorUtility.DisplayProgressBar("Replace All Materials Reference", $"Replace Materials In Prefab......（{i}/{prefabGuids.Length}）", ((float)i) / prefabGuids.Length);

                    ReplaceMaterialResult result = ReplaceMaterialsReference(prefabGuids[i], oldMaterialsGuid, targetMaterialGuid);
                    if (result != null) results.Add(result);
                }
            }
            //替换所有【场景】资产中的材质
            if (_hasScene)
            {
                for (int i = 0; i < sceneAssetGuids.Length; i++)
                {
                    EditorUtility.DisplayProgressBar("Replace All Materials Reference", $"Replace Materials In Scene......（{i}/{sceneAssetGuids.Length}）", ((float)i) / sceneAssetGuids.Length);

                    ReplaceMaterialResult result = ReplaceMaterialsReference(sceneAssetGuids[i], oldMaterialsGuid, targetMaterialGuid);
                    if (result != null) results.Add(result);
                }
            }

            //输出替换结果
            sb.Append($"<color=yellow>已完成替换 {results.Count} 个资产。</color>\r\n\r\n");
            for (int i = 0; i < results.Count; i++)
            {
                ReplaceMaterialResult result = results[i];
                sb.Append($"{result.AssetPath.Hyperlink(result.AssetPath)}：\r\n");

                foreach (var replaceCount in result.ReplaceCount)
                {
                    sb.Append($"替换 <color=cyan>{replaceCount.Key.name}</color> 为 <color=cyan>{_replaceTargetMaterial.name}</color>，共替换 <color=cyan>{replaceCount.Value}</color> 处。\r\n");
                }
                sb.Append("\r\n");
            }
            return sb.ToString();
        }
        private ReplaceMaterialResult ReplaceMaterialsReference(string assetGuid, Dictionary<string, Material> oldMaterialsGuid,string targetMaterialGuid)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
            if (string.IsNullOrEmpty(assetPath))
                return null;

            string fullPath = PathToolkit.ProjectPath + assetPath;
            if (!File.Exists(fullPath))
                return null;

            ReplaceMaterialResult result = new ReplaceMaterialResult(assetPath);
            string tempPath = fullPath + ".tmp";
            using (StreamReader reader = new StreamReader(fullPath, Encoding.UTF8))
            {
                using (StreamWriter writer = new StreamWriter(tempPath, false, Encoding.UTF8))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        foreach (var oldMaterial in oldMaterialsGuid)
                        {
                            string oldGuid = oldMaterial.Key;
                            Material oldMat = oldMaterial.Value;
                            string newLine = line.Replace(oldGuid, targetMaterialGuid);
                            if (line != newLine)
                            {
                                result.RecordReplaceNumber(oldMat);
                                line = newLine;
                            }
                        }
                        writer.WriteLine(line);
                    }
                }
            }

            if (result.HasAnyReplace)
            {
                File.Delete(fullPath);
                File.Move(tempPath, fullPath);
                AssetDatabase.ImportAsset(assetPath);
                return result;
            }
            else
            {
                File.Delete(tempPath);
                return null;
            }
        }

        /// <summary>
        /// 材质资产的替换结果
        /// </summary>
        private class ReplaceMaterialResult
        {
            /// <summary>
            /// 目标资产
            /// </summary>
            public string AssetPath;
            /// <summary>
            /// 已替换材质的次数
            /// </summary>
            public Dictionary<Material, int> ReplaceCount;

            /// <summary>
            /// 是否替换了至少一次材质
            /// </summary>
            public bool HasAnyReplace
            {
                get
                {
                    return ReplaceCount.Count > 0;
                }
            }

            public ReplaceMaterialResult(string assetPath)
            {
                AssetPath = assetPath;
                ReplaceCount = new Dictionary<Material, int>();
            }

            /// <summary>
            /// 记录一次材质替换
            /// </summary>
            public void RecordReplaceNumber(Material material)
            {
                if (ReplaceCount.ContainsKey(material))
                {
                    ReplaceCount[material] += 1;
                }
                else
                {
                    ReplaceCount.Add(material, 1);
                }
            }
        }
    }
}