﻿using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(MouseRotation))]
    public sealed class MouseRotationInspector : ModuleEditor
    {
        private MouseRotation _mouseRotation;

        protected override void OnEnable()
        {
            _mouseRotation = target as MouseRotation;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            Toggle(_mouseRotation.IsCanOnUGUI, out _mouseRotation.IsCanOnUGUI, "Is Can Control On UGUI");
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("HelpBox");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Offset:", "BoldLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("Box");
            GUILayout.Label("X");
            FloatField(_mouseRotation.OffsetX, out _mouseRotation.OffsetX);
            GUILayout.Label("Y");
            FloatField(_mouseRotation.OffsetY, out _mouseRotation.OffsetY);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();


            GUILayout.BeginVertical("HelpBox");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Speed:", "BoldLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("Box");
            GUILayout.Label("X");
            FloatField(_mouseRotation.XSpeed, out _mouseRotation.XSpeed);
            GUILayout.Label("Y");
            FloatField(_mouseRotation.YSpeed, out _mouseRotation.YSpeed);
            GUILayout.Label("M");
            FloatField(_mouseRotation.MSpeed, out _mouseRotation.MSpeed);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();


            GUILayout.BeginVertical("HelpBox");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Angle Limit:", "BoldLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("Box");
            GUILayout.Label("Y min");
            FloatField(_mouseRotation.YMinAngleLimit, out _mouseRotation.YMinAngleLimit);
            GUILayout.Label("Y max");
            FloatField(_mouseRotation.YMaxAngleLimit, out _mouseRotation.YMaxAngleLimit);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();


            GUILayout.BeginVertical("HelpBox");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Distance:", "BoldLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("Box");
            GUILayout.Label("D");
            FloatField(_mouseRotation.Distance, out _mouseRotation.Distance);
            Button(DistanceAdd, "", "OL Plus", GUILayout.Width(15));
            Button(DistanceSubtract, "", "OL Minus", GUILayout.Width(15));
            GUILayout.Label("Min");
            FloatField(_mouseRotation.MinDistance, out _mouseRotation.MinDistance);
            GUILayout.Label("Max");
            FloatField(_mouseRotation.MaxDistance, out _mouseRotation.MaxDistance);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();


            GUILayout.BeginVertical("HelpBox");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Damping:", "BoldLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("Box");
            Toggle(_mouseRotation.NeedDamping, out _mouseRotation.NeedDamping, "Need Damping");
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();


            GUILayout.BeginVertical("HelpBox");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Camera Position Limit:", "BoldLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("Box");

            GUILayout.BeginHorizontal();
            Toggle(_mouseRotation.NeedLimit, out _mouseRotation.NeedLimit, "Need Limit");
            GUILayout.EndHorizontal();

            if (_mouseRotation.NeedLimit)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("X Min");
                FloatField(_mouseRotation.XMinLimit, out _mouseRotation.XMinLimit);
                GUILayout.Label("X Max");
                FloatField(_mouseRotation.XMaxLimit, out _mouseRotation.XMaxLimit);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Y Min");
                FloatField(_mouseRotation.YMinLimit, out _mouseRotation.YMinLimit);
                GUILayout.Label("Y Max");
                FloatField(_mouseRotation.YMaxLimit, out _mouseRotation.YMaxLimit);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Z Min");
                FloatField(_mouseRotation.ZMinLimit, out _mouseRotation.ZMinLimit);
                GUILayout.Label("Z Max");
                FloatField(_mouseRotation.ZMaxLimit, out _mouseRotation.ZMaxLimit);
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();

            GUILayout.EndVertical();


            GUILayout.BeginVertical("HelpBox");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Angle:", "BoldLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("Box");
            GUILayout.Label("X");
            FloatField(_mouseRotation.X, out _mouseRotation.X);
            Button(XAdd, "", "OL Plus", GUILayout.Width(15));
            Button(XSubtract, "", "OL Minus", GUILayout.Width(15));
            GUILayout.Label("Y");
            FloatField(_mouseRotation.Y, out _mouseRotation.Y);
            Button(YAdd, "", "OL Plus", GUILayout.Width(15));
            Button(YSubtract, "", "OL Minus", GUILayout.Width(15));
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();


            GUILayout.BeginHorizontal();
            GUI.backgroundColor = Color.cyan;
            if (GUILayout.Button("Copy Angle"))
            {
                GUIUtility.systemCopyBuffer =
                        _mouseRotation.X.ToString("F2") + "f," +
                        _mouseRotation.Y.ToString("F2") + "f," +
                        _mouseRotation.Distance.ToString("F2") + "f";
            }
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();
        }

        private void DistanceAdd()
        {
            _mouseRotation.Distance += 0.1f;
        }
        private void DistanceSubtract()
        {
            _mouseRotation.Distance -= 0.1f;
        }
        private void XAdd()
        {
            _mouseRotation.X += 2;
        }
        private void XSubtract()
        {
            _mouseRotation.X -= 2;
        }
        private void YAdd()
        {
            _mouseRotation.Y += 2;
        }
        private void YSubtract()
        {
            _mouseRotation.Y -= 2;
        }
    }
}