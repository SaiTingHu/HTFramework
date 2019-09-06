using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(Transform))]
    public sealed class TransformEditor : HTFEditor<Transform>
    {
        private static bool _showProperty = true;
        private static bool _showCopy = false;
        private static bool _showSetting = false;
        private static bool _copyQuaternion = false;
        
        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            #region Property
            GUILayout.BeginHorizontal("MeTransitionHead");
            GUILayout.Space(12);
            _showProperty = EditorGUILayout.Foldout(_showProperty, "Property", true);
            GUILayout.EndHorizontal();

            if (_showProperty)
            {
                GUILayout.BeginVertical("Box");

                GUILayout.BeginHorizontal();
                GUILayout.Label("Position", GUILayout.Width(80));
                Vector3 pos = EditorGUILayout.Vector3Field("", Target.position);
                if (pos != Target.position)
                {
                    Undo.RecordObject(Target, "Move " + Target.name);
                    Target.position = pos;
                    HasChanged();
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Rotation", GUILayout.Width(80));
                Vector3 rot = EditorGUILayout.Vector3Field("", Target.rotation.eulerAngles);
                if (rot != Target.rotation.eulerAngles)
                {
                    Undo.RecordObject(Target, "Rotate " + Target.name);
                    Target.rotation = Quaternion.Euler(rot);
                    HasChanged();
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Scale", GUILayout.Width(80));
                GUI.enabled = false;
                EditorGUILayout.Vector3Field("", Target.lossyScale);
                GUI.enabled = true;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("LocalPosition", GUILayout.Width(80));
                Vector3 localpos = EditorGUILayout.Vector3Field("", Target.localPosition);
                if (localpos != Target.localPosition)
                {
                    Undo.RecordObject(Target, "Move " + Target.name);
                    Target.localPosition = localpos;
                    HasChanged();
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("LocalRotation", GUILayout.Width(80));
                Vector3 localrot = EditorGUILayout.Vector3Field("", Target.localRotation.eulerAngles);
                if (localrot != Target.localRotation.eulerAngles)
                {
                    Undo.RecordObject(Target, "Rotate " + Target.name);
                    Target.localRotation = Quaternion.Euler(localrot);
                    HasChanged();
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("LocalScale", GUILayout.Width(80));
                Vector3 localsca = EditorGUILayout.Vector3Field("", Target.localScale);
                if (localsca != Target.localScale)
                {
                    Undo.RecordObject(Target, "Scale " + Target.name);
                    Target.localScale = localsca;
                    HasChanged();
                }
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
            }
            #endregion

            #region Copy
            GUILayout.BeginHorizontal("MeTransitionHead");
            GUILayout.Space(12);
            _showCopy = EditorGUILayout.Foldout(_showCopy, "Copy", true);
            GUILayout.EndHorizontal();

            if (_showCopy)
            {
                GUILayout.BeginVertical("Box");

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Copy Position", "MiniButtonLeft"))
                {
                    GUIUtility.systemCopyBuffer =
                        Target.position.x.ToString("F4") + "f," +
                        Target.position.y.ToString("F4") + "f," +
                        Target.position.z.ToString("F4") + "f";
                    GlobalTools.LogInfo("已复制：" + GUIUtility.systemCopyBuffer);
                }
                if (GUILayout.Button("Copy LocalPosition", "MiniButtonRight"))
                {
                    GUIUtility.systemCopyBuffer =
                        Target.localPosition.x.ToString("F4") + "f," +
                        Target.localPosition.y.ToString("F4") + "f," +
                        Target.localPosition.z.ToString("F4") + "f";
                    GlobalTools.LogInfo("已复制：" + GUIUtility.systemCopyBuffer);
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Copy Rotation", "MiniButtonLeft"))
                {
                    if (_copyQuaternion)
                    {
                        GUIUtility.systemCopyBuffer =
                        Target.rotation.x.ToString("F4") + "f," +
                        Target.rotation.y.ToString("F4") + "f," +
                        Target.rotation.z.ToString("F4") + "f," +
                        Target.rotation.w.ToString("F4") + "f";
                        GlobalTools.LogInfo("已复制：" + GUIUtility.systemCopyBuffer);
                    }
                    else
                    {
                        string x = Target.rotation.eulerAngles.x > 180 ? ((int)Target.rotation.eulerAngles.x - 360).ToString() : ((int)Target.rotation.eulerAngles.x).ToString();
                        string y = Target.rotation.eulerAngles.y > 180 ? ((int)Target.rotation.eulerAngles.y - 360).ToString() : ((int)Target.rotation.eulerAngles.y).ToString();
                        string z = Target.rotation.eulerAngles.z > 180 ? ((int)Target.rotation.eulerAngles.z - 360).ToString() : ((int)Target.rotation.eulerAngles.z).ToString();

                        GUIUtility.systemCopyBuffer = x + "f," + y + "f," + z + "f";
                        GlobalTools.LogInfo("已复制：" + GUIUtility.systemCopyBuffer);
                    }
                }
                if (GUILayout.Button("Copy LocalRotation", "MiniButtonRight"))
                {
                    if (_copyQuaternion)
                    {
                        GUIUtility.systemCopyBuffer =
                        Target.localRotation.x.ToString("F4") + "f," +
                        Target.localRotation.y.ToString("F4") + "f," +
                        Target.localRotation.z.ToString("F4") + "f," +
                        Target.localRotation.w.ToString("F4") + "f";
                        GlobalTools.LogInfo("已复制：" + GUIUtility.systemCopyBuffer);
                    }
                    else
                    {
                        string x = Target.localRotation.eulerAngles.x > 180 ? ((int)Target.localRotation.eulerAngles.x - 360).ToString() : ((int)Target.localRotation.eulerAngles.x).ToString();
                        string y = Target.localRotation.eulerAngles.y > 180 ? ((int)Target.localRotation.eulerAngles.y - 360).ToString() : ((int)Target.localRotation.eulerAngles.y).ToString();
                        string z = Target.localRotation.eulerAngles.z > 180 ? ((int)Target.localRotation.eulerAngles.z - 360).ToString() : ((int)Target.localRotation.eulerAngles.z).ToString();

                        GUIUtility.systemCopyBuffer = x + "f," + y + "f," + z + "f";
                        GlobalTools.LogInfo("已复制：" + GUIUtility.systemCopyBuffer);
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Copy Scale", "MiniButton"))
                {
                    GUIUtility.systemCopyBuffer =
                        Target.localScale.x.ToString("F4") + "f," +
                        Target.localScale.y.ToString("F4") + "f," +
                        Target.localScale.z.ToString("F4") + "f";
                    GlobalTools.LogInfo("已复制：" + GUIUtility.systemCopyBuffer);
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(10);

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Copy Name", "MiniButtonLeft"))
                {
                    GUIUtility.systemCopyBuffer = Target.name;
                    GlobalTools.LogInfo("已复制：" + GUIUtility.systemCopyBuffer);
                }
                if (GUILayout.Button("Copy FullName", "MiniButtonRight"))
                {
                    GUIUtility.systemCopyBuffer = Target.FullName();
                    GlobalTools.LogInfo("已复制：" + GUIUtility.systemCopyBuffer);
                }
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
            }
            #endregion

            #region Setting
            GUILayout.BeginHorizontal("MeTransitionHead");
            GUILayout.Space(12);
            _showSetting = EditorGUILayout.Foldout(_showSetting, "Setting", true);
            GUILayout.EndHorizontal();

            if (_showSetting)
            {
                GUILayout.BeginVertical("Box");

                GUILayout.BeginHorizontal();
                _copyQuaternion = GUILayout.Toggle(_copyQuaternion, "Copy Quaternion");
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
            }
            #endregion
        }
    }
}