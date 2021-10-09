using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(NetworkManager))]
    [GiteeURL("https://gitee.com/SaiTingHu/HTFramework")]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/103575999")]
    internal sealed class NetworkManagerInspector : InternalModuleInspector<NetworkManager, INetworkHelper>
    {
        private GUIContent _addGC;
        private GUIContent _removeGC;
        private GUIContent _editGC;
        private SerializedProperty _channelTypes;
        private ReorderableList _channelTypeList;

        protected override string Intro => "Network Manager, help you implementing basic network client with socket!";

        protected override void OnDefaultEnable()
        {
            base.OnDefaultEnable();

            _addGC = new GUIContent();
            _addGC.image = EditorGUIUtility.IconContent("d_Toolbar Plus More").image;
            _addGC.tooltip = "Add a new channel";
            _removeGC = new GUIContent();
            _removeGC.image = EditorGUIUtility.IconContent("d_Toolbar Minus").image;
            _removeGC.tooltip = "Remove select channel";
            _editGC = new GUIContent();
            _editGC.image = EditorGUIUtility.IconContent("d_editicon.sml").image;
            _editGC.tooltip = "Edit channel script";

            _channelTypes = GetProperty("ChannelTypes");
            _channelTypeList = new ReorderableList(serializedObject, _channelTypes, true, true, false, false);
            _channelTypeList.drawHeaderCallback = (Rect rect) =>
            {
                Rect sub = rect;
                sub.Set(rect.x, rect.y, 200, rect.height);
                GUI.Label(sub, "Enabled Channels:");

                if (!EditorApplication.isPlaying)
                {
                    sub.Set(rect.x + rect.width - 40, rect.y - 2, 20, 20);
                    if (GUI.Button(sub, _addGC, "InvisibleButton"))
                    {
                        GenericMenu gm = new GenericMenu();
                        List<Type> types = ReflectionToolkit.GetTypesInRunTimeAssemblies(type =>
                        {
                            return type.IsSubclassOf(typeof(ProtocolChannelBase)) && !type.IsAbstract;
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

                    sub.Set(rect.x + rect.width - 20, rect.y - 2, 20, 20);
                    GUI.enabled = _channelTypeList.index >= 0 && _channelTypeList.index < Target.ChannelTypes.Count;
                    if (GUI.Button(sub, _removeGC, "InvisibleButton"))
                    {
                        Undo.RecordObject(target, "Delete Channel");
                        Target.ChannelTypes.RemoveAt(_channelTypeList.index);
                        HasChanged();
                    }
                    GUI.enabled = true;
                }
            };
            _channelTypeList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                if (index >= 0 && index < Target.ChannelTypes.Count)
                {
                    Rect subrect = rect;
                    subrect.Set(rect.x, rect.y + 2, rect.width, 16);
                    GUI.Label(subrect, Target.ChannelTypes[index]);

                    int size = 20;
                    if (isActive && isFocused)
                    {
                        subrect.Set(rect.x + rect.width - size, rect.y, 20, 20);
                        if (GUI.Button(subrect, _editGC, "InvisibleButton"))
                        {
                            MonoScriptToolkit.OpenMonoScript(Target.ChannelTypes[index]);
                        }
                        size += 20;
                    }
                }
            };
            _channelTypeList.drawElementBackgroundCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                if (Event.current.type == EventType.Repaint)
                {
                    GUIStyle gUIStyle = (index % 2 != 0) ? "CN EntryBackEven" : "Box";
                    gUIStyle = (!isActive && !isFocused) ? gUIStyle : "RL Element";
                    rect.x += 2;
                    rect.width -= 6;
                    gUIStyle.Draw(rect, false, isActive, isActive, isFocused);
                }
            };
        }
        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUI.enabled = !EditorApplication.isPlaying;

            PropertyField(nameof(NetworkManager.ServerIP), "Server IP");
            PropertyField(nameof(NetworkManager.ServerPort), "Server Port");
            PropertyField(nameof(NetworkManager.ClientIP), "Client IP");
            PropertyField(nameof(NetworkManager.ClientPort), "Client Port");
            
            _channelTypeList.DoLayoutList();

            GUI.enabled = true;
        }
        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Enabled Channels:");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            foreach (var channel in _helper.ProtocolChannels)
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