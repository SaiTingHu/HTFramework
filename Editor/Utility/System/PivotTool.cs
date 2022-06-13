using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace HT.Framework
{
    [EditorTool("Pivot Tool", typeof(Transform))]
    internal class PivotTool : EditorTool
    {
        private Transform _target;
        private List<Transform> _childs = new List<Transform>();
        private GUIContent _gc;

        public override GUIContent toolbarIcon
        {
            get
            {
                if (_gc == null)
                {
                    _gc = new GUIContent();
                    _gc.image = EditorGUIUtility.IconContent("ToolHandlePivot").image;
                    _gc.tooltip = "Pivot Tool";
                }
                return _gc;
            }
        }

        private void OnEnable()
        {
            EditorTools.activeToolChanged += ActiveToolChanged;
            Selection.selectionChanged += ActiveToolChanged;
        }
        private void OnDisable()
        {
            EditorTools.activeToolChanged -= ActiveToolChanged;
            Selection.selectionChanged -= ActiveToolChanged;
        }
        private void ActiveToolChanged()
        {
            if (!EditorTools.IsActiveTool(this))
                return;

            _target = target as Transform;
            _childs.Clear();
            for (int i = 0; i < _target.childCount; i++)
            {
                _childs.Add(_target.GetChild(i));
            }
        }

        public override void OnToolGUI(EditorWindow window)
        {
            if (_target == null)
                return;

            using (new Handles.DrawingScope())
            {
                Handles.Label(_target.position, "       Pivot");

                EditorGUI.BeginChangeCheck();
                Vector3 newValue = Handles.PositionHandle(_target.position, Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_target, "Move Pivot");
                    Vector3 dir = newValue - _target.position;
                    _target.position = newValue;
                    for (int i = 0; i < _childs.Count; i++)
                    {
                        Undo.RecordObject(_childs[i], "Move Pivot");
                        _childs[i].position -= dir;
                    }
                }
            }
        }
    }
}