using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace HT.Framework
{
    public sealed class MeshRendererBatch : EditorWindow
    {
        private GameObject _root;
        private Material _mat;
        private LightProbeUsage _lightProbe;
        private ReflectionProbeUsage _reflectionProbe;
        private ShadowCastingMode _castShadows;
        private bool _receiveShadows;

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("批处理Root下的MeshRenderer组件", MessageType.Info);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Root：", GUILayout.Width(120));
            _root = EditorGUILayout.ObjectField(_root, typeof(GameObject), true) as GameObject;
            EditorGUILayout.EndHorizontal();

            GUI.enabled = _root;

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Replace Material：", GUILayout.Width(120));
            _mat = EditorGUILayout.ObjectField(_mat, typeof(Material), true) as Material;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Light Probes：", GUILayout.Width(120));
            _lightProbe = (LightProbeUsage)EditorGUILayout.EnumPopup(_lightProbe);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Reflection Probes：", GUILayout.Width(120));
            _reflectionProbe = (ReflectionProbeUsage)EditorGUILayout.EnumPopup(_reflectionProbe);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Cast Shadows：", GUILayout.Width(120));
            _castShadows = (ShadowCastingMode)EditorGUILayout.EnumPopup(_castShadows);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Receive Shadows：", GUILayout.Width(120));
            _receiveShadows = EditorGUILayout.Toggle(_receiveShadows);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Set"))
            {
                MeshRenderer[] renderers = _root.GetComponentsInChildren<MeshRenderer>();
                for (int i = 0; i < renderers.Length; i++)
                {
                    if (_mat)
                    {
                        Material[] materials = new Material[renderers[i].sharedMaterials.Length];
                        for (int j = 0; j < materials.Length; j++)
                        {
                            materials[j] = _mat;
                        }
                        renderers[i].sharedMaterials = materials;
                    }

                    renderers[i].lightProbeUsage = _lightProbe;
                    renderers[i].reflectionProbeUsage = _reflectionProbe;
                    renderers[i].shadowCastingMode = _castShadows;
                    renderers[i].receiveShadows = _receiveShadows;
                }
                
                GlobalTools.LogInfo("[" + _root.name + "] 设置完成！共设置了 " + renderers.Length + " 处！");
            }
            EditorGUILayout.EndHorizontal();

            GUI.enabled = true;
        }
    }
}
