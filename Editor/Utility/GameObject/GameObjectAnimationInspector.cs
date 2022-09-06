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

            PropertyField(nameof(GameObjectAnimation.TheAnimationType), "Animation Type");
            PropertyField(nameof(GameObjectAnimation.TheEase), "Ease");
            PropertyField(nameof(GameObjectAnimation.TheLoopType), "Loop Type");
            
            if (Target.TheAnimationType == GameObjectAnimationType.Move)
            {
                PropertyField(nameof(GameObjectAnimation.TheTargetVector3), "Target Position");
            }
            else if (Target.TheAnimationType == GameObjectAnimationType.Rotate)
            {
                PropertyField(nameof(GameObjectAnimation.TheRotateMode), "Rotate Mode");
                PropertyField(nameof(GameObjectAnimation.TheTargetVector3), "Target Rotation");
            }
            else if (Target.TheAnimationType == GameObjectAnimationType.Scale)
            {
                PropertyField(nameof(GameObjectAnimation.TheTargetVector3), "Target Scale");
            }

            PropertyField(nameof(GameObjectAnimation.TheTime), "Time");
            PropertyField(nameof(GameObjectAnimation.TheLoops), "Loops");
            PropertyField(nameof(GameObjectAnimation.PlayOnStart), "Play On Start");
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