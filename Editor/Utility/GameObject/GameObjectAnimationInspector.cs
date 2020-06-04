using UnityEditor;

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
    }
}