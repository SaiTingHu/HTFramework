using UnityEditor;

namespace HT.Framework
{
    [CustomEditor(typeof(UIAnimation))]
    internal sealed class UIAnimationInspector : HTFEditor<UIAnimation>
    {
        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            PropertyField("TheAnimationType", "Animation Type");
            PropertyField("TheEase", "Ease");
            PropertyField("TheLoopType", "Loop Type");

            if (Target.TheAnimationType == UIAnimationType.Move)
            {
                PropertyField("TheTargetVector3", "Target Position");
            }
            else if (Target.TheAnimationType == UIAnimationType.Rotate)
            {
                PropertyField("TheRotateMode", "Rotate Mode");
                PropertyField("TheTargetVector3", "Target Rotation");
            }
            else if (Target.TheAnimationType == UIAnimationType.Scale)
            {
                PropertyField("TheTargetVector3", "Target Scale");
            }
            else if (Target.TheAnimationType == UIAnimationType.Color)
            {
                PropertyField("TheTargetColor", "Target Color");
            }
            else if (Target.TheAnimationType == UIAnimationType.Transparency)
            {
                PropertyField("TheTargetFloat", "Target Transparency");
            }

            PropertyField("TheTime", "Time");
            PropertyField("TheLoops", "Loops");
            PropertyField("PlayOnStart");
        }
    }
}