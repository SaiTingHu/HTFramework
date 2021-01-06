using UnityEngine;

namespace HT.Framework
{
    [CustomDebugger(typeof(ParticleSystem))]
    internal sealed class DebuggerParticleSystem : DebuggerComponentBase
    {
        private ParticleSystem _target;
        private ParticleSystem.MainModule _mainModule;

        public override void OnEnable()
        {
            _target = Target as ParticleSystem;
            _mainModule = _target.main;
        }
        public override void OnDebuggerGUI()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(_target.isPlaying ? "Pause" : "Play"))
            {
                if (_target.isPlaying)
                    _target.Pause();
                else
                    _target.Play();
            }
            if (GUILayout.Button("Restart"))
            {
                _target.Stop();
                _target.Play();
            }
            if (GUILayout.Button("Stop"))
            {
                _target.Stop();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Particles: " + _target.particleCount);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _mainModule.loop = GUILayout.Toggle(_mainModule.loop, "Loop");
            GUILayout.EndHorizontal();
        }
    }
}