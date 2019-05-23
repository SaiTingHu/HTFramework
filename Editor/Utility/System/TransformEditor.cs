using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(Transform))]
    public sealed class TransformEditor : Editor
    {
        private Transform _transform;
        private static bool _showProperty = true;
        private static bool _showCopy = false;
        private static bool _showSetting = false;
        private static bool _copyQuaternion = false;

        private void OnEnable()
        {
            _transform = target as Transform;
        }

        public override void OnInspectorGUI()
        {
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
                Vector3 pos = EditorGUILayout.Vector3Field("", _transform.position);
                if (pos != _transform.position)
                {
                    Undo.RecordObject(_transform, "Move " + _transform.name);
                    _transform.position = pos;
                    this.HasChanged();
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Rotation", GUILayout.Width(80));
                Vector3 rot = EditorGUILayout.Vector3Field("", _transform.rotation.eulerAngles);
                if (rot != _transform.rotation.eulerAngles)
                {
                    Undo.RecordObject(_transform, "Rotate " + _transform.name);
                    _transform.rotation = Quaternion.Euler(rot);
                    this.HasChanged();
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Scale", GUILayout.Width(80));
                GUI.enabled = false;
                EditorGUILayout.Vector3Field("", _transform.lossyScale);
                GUI.enabled = true;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("LocalPosition", GUILayout.Width(80));
                Vector3 localpos = EditorGUILayout.Vector3Field("", _transform.localPosition);
                if (localpos != _transform.localPosition)
                {
                    Undo.RecordObject(_transform, "Move " + _transform.name);
                    _transform.localPosition = localpos;
                    this.HasChanged();
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("LocalRotation", GUILayout.Width(80));
                Vector3 localrot = EditorGUILayout.Vector3Field("", _transform.localRotation.eulerAngles);
                if (localrot != _transform.localRotation.eulerAngles)
                {
                    Undo.RecordObject(_transform, "Rotate " + _transform.name);
                    _transform.localRotation = Quaternion.Euler(localrot);
                    this.HasChanged();
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("LocalScale", GUILayout.Width(80));
                Vector3 localsca = EditorGUILayout.Vector3Field("", _transform.localScale);
                if (localsca != _transform.localScale)
                {
                    Undo.RecordObject(_transform, "Scale " + _transform.name);
                    _transform.localScale = localsca;
                    this.HasChanged();
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
                        _transform.position.x.ToString("F4") + "f," +
                        _transform.position.y.ToString("F4") + "f," +
                        _transform.position.z.ToString("F4") + "f";
                    GlobalTools.LogInfo("已复制：" + GUIUtility.systemCopyBuffer);
                }
                if (GUILayout.Button("Copy LocalPosition", "MiniButtonRight"))
                {
                    GUIUtility.systemCopyBuffer =
                        _transform.localPosition.x.ToString("F4") + "f," +
                        _transform.localPosition.y.ToString("F4") + "f," +
                        _transform.localPosition.z.ToString("F4") + "f";
                    GlobalTools.LogInfo("已复制：" + GUIUtility.systemCopyBuffer);
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Copy Rotation", "MiniButtonLeft"))
                {
                    if (_copyQuaternion)
                    {
                        GUIUtility.systemCopyBuffer =
                        _transform.rotation.x.ToString("F4") + "f," +
                        _transform.rotation.y.ToString("F4") + "f," +
                        _transform.rotation.z.ToString("F4") + "f," +
                        _transform.rotation.w.ToString("F4") + "f";
                        GlobalTools.LogInfo("已复制：" + GUIUtility.systemCopyBuffer);
                    }
                    else
                    {
                        string x = _transform.rotation.eulerAngles.x > 180 ? ((int)_transform.rotation.eulerAngles.x - 360).ToString() : ((int)_transform.rotation.eulerAngles.x).ToString();
                        string y = _transform.rotation.eulerAngles.y > 180 ? ((int)_transform.rotation.eulerAngles.y - 360).ToString() : ((int)_transform.rotation.eulerAngles.y).ToString();
                        string z = _transform.rotation.eulerAngles.z > 180 ? ((int)_transform.rotation.eulerAngles.z - 360).ToString() : ((int)_transform.rotation.eulerAngles.z).ToString();

                        GUIUtility.systemCopyBuffer = x + "f," + y + "f," + z + "f";
                        GlobalTools.LogInfo("已复制：" + GUIUtility.systemCopyBuffer);
                    }
                }
                if (GUILayout.Button("Copy LocalRotation", "MiniButtonRight"))
                {
                    if (_copyQuaternion)
                    {
                        GUIUtility.systemCopyBuffer =
                        _transform.localRotation.x.ToString("F4") + "f," +
                        _transform.localRotation.y.ToString("F4") + "f," +
                        _transform.localRotation.z.ToString("F4") + "f," +
                        _transform.localRotation.w.ToString("F4") + "f";
                        GlobalTools.LogInfo("已复制：" + GUIUtility.systemCopyBuffer);
                    }
                    else
                    {
                        string x = _transform.localRotation.eulerAngles.x > 180 ? ((int)_transform.localRotation.eulerAngles.x - 360).ToString() : ((int)_transform.localRotation.eulerAngles.x).ToString();
                        string y = _transform.localRotation.eulerAngles.y > 180 ? ((int)_transform.localRotation.eulerAngles.y - 360).ToString() : ((int)_transform.localRotation.eulerAngles.y).ToString();
                        string z = _transform.localRotation.eulerAngles.z > 180 ? ((int)_transform.localRotation.eulerAngles.z - 360).ToString() : ((int)_transform.localRotation.eulerAngles.z).ToString();

                        GUIUtility.systemCopyBuffer = x + "f," + y + "f," + z + "f";
                        GlobalTools.LogInfo("已复制：" + GUIUtility.systemCopyBuffer);
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Copy Scale", "MiniButton"))
                {
                    GUIUtility.systemCopyBuffer =
                        _transform.localScale.x.ToString("F4") + "f," +
                        _transform.localScale.y.ToString("F4") + "f," +
                        _transform.localScale.z.ToString("F4") + "f";
                    GlobalTools.LogInfo("已复制：" + GUIUtility.systemCopyBuffer);
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(10);

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Copy Name", "MiniButtonLeft"))
                {
                    GUIUtility.systemCopyBuffer = _transform.name;
                    GlobalTools.LogInfo("已复制：" + GUIUtility.systemCopyBuffer);
                }
                if (GUILayout.Button("Copy FullName", "MiniButtonRight"))
                {
                    List<Transform> transforms = new List<Transform>();
                    Transform transform = _transform;
                    transforms.Add(transform);
                    while (transform.parent)
                    {
                        transform = transform.parent;
                        transforms.Add(transform);
                    }

                    string name = "";
                    name += transforms[transforms.Count - 1].name;
                    for (int i = transforms.Count - 2; i >= 0; i--)
                    {
                        name += "/" + transforms[i].name;
                    }

                    GUIUtility.systemCopyBuffer = name;
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
