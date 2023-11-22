using UnityEngine;
using UnityEngine.Playables;

namespace HT.Framework
{
    [CustomDebugger(typeof(PlayableDirector))]
    internal sealed class DebuggerPlayableDirector : DebuggerComponentBase
    {
        private PlayableDirector _target;

        public override void OnEnable()
        {
            _target = Target as PlayableDirector;
        }
        public override void OnDebuggerGUI()
        {
            GUI.contentColor = _target.enabled ? Color.white : Color.gray;

            _target.enabled = BoolField("Enabled", _target.enabled);
            ObjectFieldReadOnly("Playable", _target.playableAsset);
            _target.timeUpdateMode = (DirectorUpdateMode)EnumField("Update Method", _target.timeUpdateMode);
            _target.playOnAwake = BoolField("Play On Awake", _target.playOnAwake);
            _target.extrapolationMode = (DirectorWrapMode)EnumField("Wrap Mode", _target.extrapolationMode);
            StringFieldReadOnly("Initial Time", _target.initialTime.ToString());
        }
    }
}