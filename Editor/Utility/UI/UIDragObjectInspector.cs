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

            PropertyField("DragTarget");

            GUI.enabled = Target.DragTarget;

            PropertyField("DragButton");
            PropertyField("Mode");
            PropertyField("Horizontal");
            if (Target.Horizontal)
            {
                PropertyField("HorizontalLimit");

                if (Target.HorizontalLimit)
                {
                    PropertyField("Left");
                    PropertyField("Right");
                }
            }
            PropertyField("Vertical");
            if (Target.Vertical)
            {
                PropertyField("VerticalLimit");

                if (Target.VerticalLimit)
                {
                    PropertyField("Up");
                    PropertyField("Down");
                }
            }

            GUI.enabled = true;
        }
    }
}