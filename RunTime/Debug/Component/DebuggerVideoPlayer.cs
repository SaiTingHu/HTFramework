using UnityEngine;
using UnityEngine.Video;

namespace HT.Framework
{
    [CustomDebugger(typeof(VideoPlayer))]
    internal sealed class DebuggerVideoPlayer : DebuggerComponentBase
    {
        private VideoPlayer _target;

        public override void OnEnable()
        {
            _target = Target as VideoPlayer;
        }
        public override void OnDebuggerGUI()
        {
            GUI.contentColor = _target.enabled ? Color.white : Color.gray;

            _target.enabled = BoolField("Enabled", _target.enabled);
            _target.source = (VideoSource)EnumField("Source", _target.source);
            if (_target.source == VideoSource.VideoClip)
            {
                ObjectFieldReadOnly("Video Clip", _target.clip);
            }
            else if (_target.source == VideoSource.Url)
            {
                StringFieldReadOnly("URL", _target.url);
            }
            _target.playOnAwake = BoolField("Play On Awake", _target.playOnAwake);
            _target.waitForFirstFrame = BoolField("Wait For First Frame", _target.waitForFirstFrame);
            _target.isLooping = BoolField("Loop", _target.isLooping);
            _target.skipOnDrop = BoolField("Skip On Drop", _target.skipOnDrop);
            FloatFieldReadOnly("Playback Speed", _target.playbackSpeed);
            _target.aspectRatio = (VideoAspectRatio)EnumField("Aspect Ratio", _target.aspectRatio);
        }
    }
}