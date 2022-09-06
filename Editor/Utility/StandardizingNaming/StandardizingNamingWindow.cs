using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace HT.Framework
{
    /// <summary>
    /// 标准化命名-配置窗口
    /// </summary>
    internal sealed class StandardizingNamingWindow : HTFEditorWindow
    {
        private readonly string _defaultDataPath = "Assets/StandardizingNaming.asset";
        private string _dataPath;
        private StandardizingNamingData _data;

        protected override bool IsEnableTitleGUI => false;

        protected override void OnEnable()
        {
            base.OnEnable();

            _dataPath = EditorPrefs.GetString(EditorPrefsTable.StandardizingNaming_Config, _defaultDataPath);
            _data = AssetDatabase.LoadAssetAtPath<StandardizingNamingData>(_dataPath);
        }
        protected override void OnBodyGUI()
        {
            base.OnBodyGUI();
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Data", GUILayout.Width(100));
            EditorGUI.BeginChangeCheck();
            StandardizingNamingData data = EditorGUILayout.ObjectField(_data, typeof(StandardizingNamingData), false) as StandardizingNamingData;
            if (EditorGUI.EndChangeCheck())
            {
                _data = data;
                _dataPath = AssetDatabase.GetAssetPath(_data);
                EditorPrefs.SetString(EditorPrefsTable.StandardizingNaming_Config, _dataPath);
            }
            if (_data == null)
            {
                if (GUILayout.Button("Create", GUILayout.Width(60)))
                {
                    _data = CreateNamingData();
                    _dataPath = AssetDatabase.GetAssetPath(_data);
                    EditorPrefs.SetString(EditorPrefsTable.StandardizingNaming_Config, _dataPath);
                }
            }
            EditorGUILayout.EndHorizontal();

            if (_data != null)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Name Match", GUILayout.Width(100));
                EditorGUI.BeginChangeCheck();
                string nameMatch = EditorGUILayout.TextField(_data.NameMatch);
                if (EditorGUI.EndChangeCheck())
                {
                    _data.NameMatch = nameMatch;
                    HasChanged(_data);
                }
                EditorGUILayout.EndHorizontal();
            }

            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Sure"))
            {
                Close();
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 创建命名配置
        /// </summary>
        private StandardizingNamingData CreateNamingData()
        {
            StandardizingNamingData data = CreateInstance<StandardizingNamingData>();
            GenerateNamingTemplate(data);
            AssetDatabase.CreateAsset(data, _defaultDataPath);
            AssetDatabase.SaveAssets();
            EditorGUIUtility.PingObject(data);
            return data;
        }
        /// <summary>
        /// 生成命名模板
        /// </summary>
        private void GenerateNamingTemplate(StandardizingNamingData data)
        {
            data.HierarchyNamingSigns.Add(new NamingSign(typeof(ScrollRect), "Scr_" + data.NameMatch));
            data.HierarchyNamingSigns.Add(new NamingSign(typeof(Button), "Btn_" + data.NameMatch));
            data.HierarchyNamingSigns.Add(new NamingSign(typeof(InputField), "Ifd_" + data.NameMatch));
            data.HierarchyNamingSigns.Add(new NamingSign(typeof(Dropdown), "Dpd_" + data.NameMatch));
            data.HierarchyNamingSigns.Add(new NamingSign(typeof(Toggle), "Tog_" + data.NameMatch));
            data.HierarchyNamingSigns.Add(new NamingSign(typeof(Slider), "Sld_" + data.NameMatch));
            data.HierarchyNamingSigns.Add(new NamingSign(typeof(Scrollbar), "Scb_" + data.NameMatch));
            data.HierarchyNamingSigns.Add(new NamingSign(typeof(Text), "Txt_" + data.NameMatch));
            data.HierarchyNamingSigns.Add(new NamingSign(typeof(Image), "Img_" + data.NameMatch));
            data.HierarchyNamingSigns.Add(new NamingSign(typeof(RawImage), "RImg_" + data.NameMatch));

            data.HierarchyNamingSigns.Add(new NamingSign(typeof(Camera), "Camera_" + data.NameMatch));
            data.HierarchyNamingSigns.Add(new NamingSign(typeof(ParticleSystem), "Effects_" + data.NameMatch));
            data.HierarchyNamingSigns.Add(new NamingSign(typeof(Light), "Light_" + data.NameMatch));
            data.HierarchyNamingSigns.Add(new NamingSign(typeof(SpriteRenderer), "Sprite_" + data.NameMatch));
            data.HierarchyNamingSigns.Add(new NamingSign(typeof(TextMesh), "TMesh_" + data.NameMatch));
            data.HierarchyNamingSigns.Add(new NamingSign(typeof(MeshRenderer), "Mesh_" + data.NameMatch));
            data.HierarchyNamingSigns.Add(new NamingSign(typeof(Collider), "Collider_" + data.NameMatch));
            data.HierarchyNamingSigns.Add(new NamingSign(typeof(Collider2D), "Collider2D_" + data.NameMatch));

            data.ProjectNamingSigns.Add(new NamingSign(typeof(Material), "Mat_" + data.NameMatch));
            data.ProjectNamingSigns.Add(new NamingSign(typeof(Texture2D), "Tex_" + data.NameMatch));
            data.ProjectNamingSigns.Add(new NamingSign(typeof(AudioClip), "Audio_" + data.NameMatch));
        }
    }
}