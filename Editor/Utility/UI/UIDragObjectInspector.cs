using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(UIDragObject))]
    internal sealed class UIDragObjectInspector : HTFEditor<UIDragObject>
    {
        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            PropertyField(nameof(UIDragObject.DragTarget));

            GUI.enabled = Target.DragTarget;

            PropertyField(nameof(UIDragObject.DragButton));
            PropertyField(nameof(UIDragObject.Horizontal));
            if (Target.Horizontal)
            {
                PropertyField(nameof(UIDragObject.HorizontalLimit));

                if (Target.HorizontalLimit)
                {
                    PropertyField(nameof(UIDragObject.Left));
                    PropertyField(nameof(UIDragObject.Right));
                }
            }
            PropertyField(nameof(UIDragObject.Vertical));
            if (Target.Vertical)
            {
                PropertyField(nameof(UIDragObject.VerticalLimit));

                if (Target.VerticalLimit)
                {
                    PropertyField(nameof(UIDragObject.Up));
                    PropertyField(nameof(UIDragObject.Down));
                }
            }

            GUI.enabled = true;
        }
    }
}