using DG.Tweening;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [InternalSettingItem(HTFrameworkModule.Controller)]
    internal sealed class SettingItemController : SettingItemBase
    {
        private ControllerManager _controllerManager;

        public override string Name
        {
            get
            {
                return "Controller";
            }
        }

        public override void OnBeginSetting()
        {
            base.OnBeginSetting();

            GameObject controllerManager = GameObject.Find("HTFramework/Controller");
            if (controllerManager)
            {
                _controllerManager = controllerManager.GetComponent<ControllerManager>();
            }
        }
        public override void OnSettingGUI()
        {
            base.OnSettingGUI();

            if (_controllerManager)
            {
                GUILayout.BeginHorizontal();
                _controllerManager.DefaultControlMode = (ControlMode)EditorGUILayout.EnumPopup("Default ControlMode", _controllerManager.DefaultControlMode);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                _controllerManager.DefaultEase = (Ease)EditorGUILayout.EnumPopup("Default Ease", _controllerManager.DefaultEase);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                _controllerManager.DefaultAutoPlay = (AutoPlay)EditorGUILayout.EnumPopup("Default Auto Play", _controllerManager.DefaultAutoPlay);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                _controllerManager.IsAutoKill = EditorGUILayout.Toggle("Tweener Auto Kill", _controllerManager.IsAutoKill);
                GUILayout.EndHorizontal();

                if (GUI.changed)
                {
                    HasChanged(_controllerManager);
                }
            }
        }
        public override void OnReset()
        {
            base.OnReset();

            if (_controllerManager)
            {
                _controllerManager.DefaultControlMode = ControlMode.FreeControl;
                _controllerManager.DefaultEase = Ease.Linear;
                _controllerManager.DefaultAutoPlay = AutoPlay.All;
                _controllerManager.IsAutoKill = true;

                HasChanged(_controllerManager);
            }
        }
    }
}