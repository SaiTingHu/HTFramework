using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(GameObjectAnimation))]
    internal sealed class GameObjectAnimationInspector : HTFEditor<GameObjectAnimation>
    {
        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            PropertyField("TheAnimationType", "Animation Type");
            PropertyField("TheEase", "Ease");
            PropertyField("TheLoopType", "Loop Type");
            
            if (Target.TheAnimationType == GameObjectAnimationType.Move)
            {
                PropertyField("TheTargetVector3", "Target Position");
            }
            else if (Target.TheAnimationType == GameObjectAnimationType.Rotate)
            {
                PropertyField("TheRotateMode", "Rotate Mode");
                PropertyField("TheTargetVector3", "Target Rotation");
            }
            else if (Target.TheAnimationType == GameObjectAnimationType.Scale)
            {
                PropertyField("TheTargetVector3", "Target Scale");
            }

            PropertyField("TheTime", "Time");
            PropertyField("TheLoops", "Loops");
            PropertyField("PlayOnStart");
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