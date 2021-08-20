using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(UIAnimation))]
    internal sealed class UIAnimationInspector : HTFEditor<UIAnimation>
    {
        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            PropertyField(nameof(UIAnimation.TheAnimationType), "Animation Type");
            PropertyField(nameof(UIAnimation.TheEase), "Ease");
            PropertyField(nameof(UIAnimation.TheLoopType), "Loop Type");

            if (Target.TheAnimationType == UIAnimationType.Move)
            {
                PropertyField(nameof(UIAnimation.TheTargetVector3), "Target Position");
            }
            else if (Target.TheAnimationType == UIAnimationType.Rotate)
            {
                PropertyField(nameof(UIAnimation.TheRotateMode), "Rotate Mode");
                PropertyField(nameof(UIAnimation.TheTargetVector3), "Target Rotation");
            }
            else if (Target.TheAnimationType == UIAnimationType.Scale)
            {
                PropertyField(nameof(UIAnimation.TheTargetVector3), "Target Scale");
            }
            else if (Target.TheAnimationType == UIAnimationType.Color)
            {
                PropertyField(nameof(UIAnimation.TheTargetColor), "Target Color");
            }
            else if (Target.TheAnimationType == UIAnimationType.Transparency)
            {
                PropertyField(nameof(UIAnimation.TheTargetFloat), "Target Transparency");
            }

            PropertyField(nameof(UIAnimation.TheTime), "Time");
            PropertyField(nameof(UIAnimation.TheLoops), "Loops");
            PropertyField(nameof(UIAnimation.PlayOnStart), "Play On Start");
        }
        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            GUI.enabled = !Target.IsPlaying && !Target.IsPause;
            if (GUILayout.Button("Play"))
            {
                Target.Play();
            }
            GUI.enabled = Target.IsPlaying && !Target.IsPause;
            if (GUILayout.Button("Pause"))
            {
                Target.Pause();
            }
            GUI.enabled = !Target.IsPlaying && Target.IsPause;
            if (GUILayout.Button("Resume"))
            {
                Target.Resume();
            }
            GUI.enabled = Target.IsPlaying;
            if (GUILayout.Button("Stop"))
            {
                Target.Stop();
            }
            GUI.enabled = true;
        }
    }
}