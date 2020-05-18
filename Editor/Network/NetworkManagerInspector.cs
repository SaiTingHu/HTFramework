using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(NetworkManager))]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/103575999")]
    internal sealed class NetworkManagerInspector : InternalModuleInspector<NetworkManager>
    {
        private Dictionary<Type, ProtocolChannelBase> _protocolChannels;

        protected override string Intro
        {
            get
            {
                return "Network Manager, implementing basic network client with socket!";
            }
        }

        protected override void OnRuntimeEnable()
        {
            base.OnRuntimeEnable();

            _protocolChannels = Target.GetType().GetField("_protocolChannels", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Target) as Dictionary<Type, ProtocolChannelBase>;
        }

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            TextField(Target.ServerIP, out Target.ServerIP, "Server IP");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            IntField(Target.ServerPort, out Target.ServerPort, "Server Port");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            TextField(Target.ClientIP, out Target.ClientIP, "Client IP");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            IntField(Target.ClientPort, out Target.ClientPort, "Client Port");
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Enabled Channels:");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            for (int i = 0; i < Target.ChannelTypes.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label((i + 1) + "." + Target.ChannelTypes[i]);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Edit", EditorStyles.miniButtonLeft))
                {
                    MonoScriptToolkit.OpenMonoScript(Target.ChannelTypes[i]);
                }
                if (GUILayout.Button("Delete", EditorStyles.miniButtonRight))
                {
                    Undo.RecordObject(target, "Delete Channel");
                    Target.ChannelTypes.RemoveAt(i);
                    HasChanged();
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Channel", EditorGlobalTools.Styles.MiniPopup))
            {
                GenericMenu gm = new GenericMenu();
                List<Type> types = ReflectionToolkit.GetTypesInRunTimeAssemblies(type =>
                {
                    return type.IsSubclassOf(typeof(ProtocolChannelBase));
                });
                for (int i = 0; i < types.Count; i++)
                {
                    int j = i;
                    if (Target.ChannelTypes.Contains(types[j].FullName))
                    {
                        gm.AddDisabledItem(new GUIContent(types[j].FullName));
                    }
                    else
                    {
                        gm.AddItem(new GUIContent(types[j].FullName), false, () =>
                        {
                            Undo.RecordObject(target, "Add Channel");
                            Target.ChannelTypes.Add(types[j].FullName);
                            HasChanged();
                        });
                    }
                }
                gm.ShowAsContext();
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Enabled Channels:");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            foreach (var channel in _protocolChannels)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(channel.Key.FullName + " [" + channel.Value.Protocol.ToString() + "]");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label("IsConnect:" + channel.Value.IsConnect.ToString());
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
        }
    }
}