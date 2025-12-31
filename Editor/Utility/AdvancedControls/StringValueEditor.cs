using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 字符串编辑器
    /// </summary>
    public sealed class StringValueEditor : HTFEditorWindow
    {
        private static readonly Regex HtmlTagsRegex = new Regex(@"<[^>]*>");
        private static readonly Regex UnicodeRegex = new Regex(@"\\u([a-fA-F0-9]{4})");

        /// <summary>
        /// 打开字符串编辑器
        /// </summary>
        /// <param name="parasitifer">宿主窗口</param>
        /// <param name="value">字符串</param>
        /// <param name="title">窗口显示标题</param>
        /// <param name="editEnd">编辑完成回调（如果为空，将保存到剪贴板）</param>
        public static void OpenWindow(EditorWindow parasitifer, string value, string title, HTFAction<string> editEnd)
        {
            StringValueEditor valueEditor = GetWindow<StringValueEditor>();
            valueEditor.titleContent.image = EditorGUIUtility.IconContent("UnityEditor.ConsoleWindow").image;
            valueEditor.titleContent.text = "String Editor";
            valueEditor._parasitifer = parasitifer;
            valueEditor._value = value;
            valueEditor._title = title;
            valueEditor._editEnd = editEnd;
            valueEditor.Show();
        }

        private EditorWindow _parasitifer;
        private bool _isShowRichText = false;
        private string _value;
        private string _title;
        private HTFAction<string> _editEnd;
        private GUIStyle _valueGS;
        private Vector2 _scroll;

        protected override string HelpUrl => "https://wanderer.blog.csdn.net/article/details/155063207";

        protected override void OnEnable()
        {
            base.OnEnable();

            _valueGS = new GUIStyle(EditorStyles.textArea);
            _valueGS.wordWrap = true;
        }
        protected override void OnTitleGUI()
        {
            base.OnTitleGUI();

            GUILayout.Label(_title);
            GUILayout.FlexibleSpace();

            _isShowRichText = GUILayout.Toggle(_isShowRichText, "RichText", EditorStyles.toolbarButton);
            if (GUILayout.Button("Html", EditorStyles.toolbarPopup))
            {
                GenericMenu gm = new GenericMenu();
                gm.AddItem(new GUIContent("Set Bold"), false, () =>
                {
                    SetHtmlBold();
                });
                gm.AddItem(new GUIContent("Set Italic"), false, () =>
                {
                    SetHtmlItalic();
                });
                gm.AddItem(new GUIContent("Set Hyperlink"), false, () =>
                {
                    SetHtmlHyperlink();
                });
                gm.AddItem(new GUIContent("Set Color/Red"), false, () =>
                {
                    SetHtmlColor("red");
                });
                gm.AddItem(new GUIContent("Set Color/Green"), false, () =>
                {
                    SetHtmlColor("green");
                });
                gm.AddItem(new GUIContent("Set Color/Blue"), false, () =>
                {
                    SetHtmlColor("blue");
                });
                gm.AddItem(new GUIContent("Set Color/White"), false, () =>
                {
                    SetHtmlColor("white");
                });
                gm.AddItem(new GUIContent("Set Color/Black"), false, () =>
                {
                    SetHtmlColor("black");
                });
                gm.AddItem(new GUIContent("Set Color/Yellow"), false, () =>
                {
                    SetHtmlColor("yellow");
                });
                gm.AddItem(new GUIContent("Set Color/Cyan"), false, () =>
                {
                    SetHtmlColor("cyan");
                });
                gm.AddItem(new GUIContent("Set Color/Magenta"), false, () =>
                {
                    SetHtmlColor("magenta");
                });
                gm.AddItem(new GUIContent("Set Color/Gray"), false, () =>
                {
                    SetHtmlColor("gray");
                });
                gm.AddItem(new GUIContent("Set Size"), false, () =>
                {
                    SetHtmlSize();
                });
                gm.AddSeparator("");
                gm.AddItem(new GUIContent("Clear selected tags"), false, () =>
                {
                    ClearSelectedHtmlTags();
                });
                gm.AddItem(new GUIContent("Clear all tags"), false, () =>
                {
                    ClearAllHtmlTags();
                });
                gm.ShowAsContext();
            }
            if (GUILayout.Button("Markdown", EditorStyles.toolbarPopup))
            {
                GenericMenu gm = new GenericMenu();
                gm.AddItem(new GUIContent("Set Bold"), false, () =>
                {
                    SetMarkdownBold();
                });
                gm.AddItem(new GUIContent("Set Italic"), false, () =>
                {
                    SetMarkdownItalic();
                });
                gm.AddItem(new GUIContent("Set Hyperlink"), false, () =>
                {
                    SetMarkdownHyperlink();
                });
                gm.AddItem(new GUIContent("Set Strikethrough"), false, () =>
                {
                    SetMarkdownStrikethrough();
                });
                gm.AddItem(new GUIContent("Set Code Block"), false, () =>
                {
                    SetMarkdownCodeBlock();
                });
                gm.AddItem(new GUIContent("Set Code Fragment"), false, () =>
                {
                    SetMarkdownCodeFragment();
                });
                gm.AddItem(new GUIContent("Set Embedded Image"), false, () =>
                {
                    SetMarkdownEmbeddedImage();
                });
                gm.AddItem(new GUIContent("Add Table"), false, () =>
                {
                    AddMarkdownTable();
                });
                gm.AddSeparator("");
                gm.AddItem(new GUIContent("Add Object"), false, () =>
                {
                    AddMarkdownObject();
                });
                gm.AddItem(new GUIContent("Add Menu"), false, () =>
                {
                    AddMarkdownMenu();
                });
                gm.AddItem(new GUIContent("Add Custom Action"), false, () =>
                {
                    AddMarkdownCustomAction();
                });
                gm.ShowAsContext();
            }
            if (GUILayout.Button("Json", EditorStyles.toolbarPopup))
            {
                GenericMenu gm = new GenericMenu();
                gm.AddItem(new GUIContent("Format"), false, () =>
                {
                    JsonFormat();
                });
                gm.AddSeparator("");
                gm.AddItem(new GUIContent("JSON.cn"), false, () =>
                {
                    Application.OpenURL("https://www.json.cn");
                });
                gm.AddItem(new GUIContent("BEJSON"), false, () =>
                {
                    Application.OpenURL("https://www.bejson.com/");
                });
                gm.ShowAsContext();
            }
            if (GUILayout.Button("$", EditorStyles.toolbarPopup))
            {
                GenericMenu gm = new GenericMenu();
                gm.AddItem(new GUIContent("Set Superscript"), false, () =>
                {
                    SetSuperscript();
                });
                gm.ShowAsContext();
            }
        }
        protected override void OnBodyGUI()
        {
            base.OnBodyGUI();

            _scroll = GUILayout.BeginScrollView(_scroll);
            _valueGS.richText = _isShowRichText;
            _value = EditorGUILayout.TextArea(_value, _valueGS, GUILayout.ExpandHeight(true));
            GUILayout.EndScrollView();

            GUILayout.BeginHorizontal();
            string save = _editEnd != null ? "Save" : "Save to Clipboard";
            if (GUILayout.Button(save))
            {
                if (_editEnd != null)
                {
                    _editEnd?.Invoke(_value);
                }
                else
                {
                    GUIUtility.systemCopyBuffer = _value;
                }
                Close();
            }
            GUILayout.EndHorizontal();
        }
        private void Update()
        {
            if (_parasitifer == null || EditorApplication.isCompiling)
            {
                Close();
            }
        }

        /// <summary>
        /// 设置当前选中文本为粗体
        /// </summary>
        private void SetHtmlBold()
        {
            GUIUtility.systemCopyBuffer = null;
            SimulateKeyboardEvent("^c");

            EditorApplication.delayCall += () =>
            {
                if (!string.IsNullOrEmpty(GUIUtility.systemCopyBuffer))
                {
                    string content = GUIUtility.systemCopyBuffer;
                    content = $"<b>{content}</b>";
                    GUIUtility.systemCopyBuffer = content;

                    SimulateKeyboardEvent("^v");
                }
            };
        }
        /// <summary>
        /// 设置当前选中文本为斜体
        /// </summary>
        private void SetHtmlItalic()
        {
            GUIUtility.systemCopyBuffer = null;
            SimulateKeyboardEvent("^c");

            EditorApplication.delayCall += () =>
            {
                if (!string.IsNullOrEmpty(GUIUtility.systemCopyBuffer))
                {
                    string content = GUIUtility.systemCopyBuffer;
                    content = $"<i>{content}</i>";
                    GUIUtility.systemCopyBuffer = content;

                    SimulateKeyboardEvent("^v");
                }
            };
        }
        /// <summary>
        /// 设置当前选中文本为超链接
        /// </summary>
        private void SetHtmlHyperlink()
        {
            GUIUtility.systemCopyBuffer = null;
            SimulateKeyboardEvent("^c");

            EditorApplication.delayCall += () =>
            {
                if (!string.IsNullOrEmpty(GUIUtility.systemCopyBuffer))
                {
                    string content = GUIUtility.systemCopyBuffer;
                    content = $"<a href=\"在这里输入超链接地址\">{content}</a>";
                    GUIUtility.systemCopyBuffer = content;

                    SimulateKeyboardEvent("^v");
                }
            };
        }
        /// <summary>
        /// 设置当前选中文本的颜色
        /// </summary>
        private void SetHtmlColor(string color)
        {
            GUIUtility.systemCopyBuffer = null;
            SimulateKeyboardEvent("^c");

            EditorApplication.delayCall += () =>
            {
                if (!string.IsNullOrEmpty(GUIUtility.systemCopyBuffer))
                {
                    string content = GUIUtility.systemCopyBuffer;
                    content = $"<color=\"{color}\">{content}</color>";
                    GUIUtility.systemCopyBuffer = content;

                    SimulateKeyboardEvent("^v");
                }
            };
        }
        /// <summary>
        /// 设置当前选中文本的大小
        /// </summary>
        private void SetHtmlSize()
        {
            GUIUtility.systemCopyBuffer = null;
            SimulateKeyboardEvent("^c");

            EditorApplication.delayCall += () =>
            {
                if (!string.IsNullOrEmpty(GUIUtility.systemCopyBuffer))
                {
                    string content = GUIUtility.systemCopyBuffer;
                    content = $"<size=20>{content}</size>";
                    GUIUtility.systemCopyBuffer = content;

                    SimulateKeyboardEvent("^v");
                }
            };
        }
        /// <summary>
        /// 清空当前选中文本的Html标签
        /// </summary>
        private void ClearSelectedHtmlTags()
        {
            GUIUtility.systemCopyBuffer = null;
            SimulateKeyboardEvent("^c");

            EditorApplication.delayCall += () =>
            {
                if (!string.IsNullOrEmpty(GUIUtility.systemCopyBuffer))
                {
                    string content = GUIUtility.systemCopyBuffer;
                    content = HtmlTagsRegex.Replace(content, string.Empty);
                    GUIUtility.systemCopyBuffer = content;

                    SimulateKeyboardEvent("^v");
                }
            };
        }
        /// <summary>
        /// 清空所有的Html标签
        /// </summary>
        private void ClearAllHtmlTags()
        {
            if (!string.IsNullOrEmpty(_value))
            {
                _value = HtmlTagsRegex.Replace(_value, string.Empty);
            }
            GUI.FocusControl(null);

            EditorApplication.delayCall += () =>
            {
                SimulateKeyboardEvent("[esc]");
            };
        }

        /// <summary>
        /// 设置当前选中文本为粗体
        /// </summary>
        private void SetMarkdownBold()
        {
            GUIUtility.systemCopyBuffer = null;
            SimulateKeyboardEvent("^c");

            EditorApplication.delayCall += () =>
            {
                if (!string.IsNullOrEmpty(GUIUtility.systemCopyBuffer))
                {
                    string content = GUIUtility.systemCopyBuffer;
                    content = $"**{content}**";
                    GUIUtility.systemCopyBuffer = content;

                    SimulateKeyboardEvent("^v");
                }
            };
        }
        /// <summary>
        /// 设置当前选中文本为斜体
        /// </summary>
        private void SetMarkdownItalic()
        {
            GUIUtility.systemCopyBuffer = null;
            SimulateKeyboardEvent("^c");

            EditorApplication.delayCall += () =>
            {
                if (!string.IsNullOrEmpty(GUIUtility.systemCopyBuffer))
                {
                    string content = GUIUtility.systemCopyBuffer;
                    content = $"*{content}*";
                    GUIUtility.systemCopyBuffer = content;

                    SimulateKeyboardEvent("^v");
                }
            };
        }
        /// <summary>
        /// 设置当前选中文本为超链接
        /// </summary>
        private void SetMarkdownHyperlink()
        {
            GUIUtility.systemCopyBuffer = null;
            SimulateKeyboardEvent("^c");

            EditorApplication.delayCall += () =>
            {
                if (!string.IsNullOrEmpty(GUIUtility.systemCopyBuffer))
                {
                    string content = GUIUtility.systemCopyBuffer;
                    content = $"[{content}](在这里输入超链接地址)";
                    GUIUtility.systemCopyBuffer = content;

                    SimulateKeyboardEvent("^v");
                }
            };
        }
        /// <summary>
        /// 设置当前选中文本为删除线
        /// </summary>
        private void SetMarkdownStrikethrough()
        {
            GUIUtility.systemCopyBuffer = null;
            SimulateKeyboardEvent("^c");

            EditorApplication.delayCall += () =>
            {
                if (!string.IsNullOrEmpty(GUIUtility.systemCopyBuffer))
                {
                    string content = GUIUtility.systemCopyBuffer;
                    content = $"~~{content}~~";
                    GUIUtility.systemCopyBuffer = content;

                    SimulateKeyboardEvent("^v");
                }
            };
        }
        /// <summary>
        /// 设置当前选中文本为代码块
        /// </summary>
        private void SetMarkdownCodeBlock()
        {
            GUIUtility.systemCopyBuffer = null;
            SimulateKeyboardEvent("^c");

            EditorApplication.delayCall += () =>
            {
                if (!string.IsNullOrEmpty(GUIUtility.systemCopyBuffer))
                {
                    string content = GUIUtility.systemCopyBuffer;
                    content = $"`{content}`";
                    GUIUtility.systemCopyBuffer = content;

                    SimulateKeyboardEvent("^v");
                }
            };
        }
        /// <summary>
        /// 设置当前选中文本为代码片段
        /// </summary>
        private void SetMarkdownCodeFragment()
        {
            GUIUtility.systemCopyBuffer = null;
            SimulateKeyboardEvent("^c");

            EditorApplication.delayCall += () =>
            {
                if (!string.IsNullOrEmpty(GUIUtility.systemCopyBuffer))
                {
                    string content = GUIUtility.systemCopyBuffer;
                    content = $"\r\n```\r\n{content}\r\n```\r\n";
                    GUIUtility.systemCopyBuffer = content;

                    SimulateKeyboardEvent("^v");
                }
            };
        }
        /// <summary>
        /// 设置当前选中文本为嵌入图像
        /// </summary>
        private void SetMarkdownEmbeddedImage()
        {
            GUIUtility.systemCopyBuffer = null;
            SimulateKeyboardEvent("^c");

            EditorApplication.delayCall += () =>
            {
                if (!string.IsNullOrEmpty(GUIUtility.systemCopyBuffer))
                {
                    string content = GUIUtility.systemCopyBuffer;
                    content = $"![{content}](在这里输入图像路径)";
                    GUIUtility.systemCopyBuffer = content;

                    SimulateKeyboardEvent("^v");
                }
            };
        }
        /// <summary>
        /// 在当前光标位置插入一个表格
        /// </summary>
        private void AddMarkdownTable()
        {
            EditorApplication.delayCall += () =>
            {
                string content = "\r\n|Name|Value|\r\n|:---:|:---:|\r\n|电脑|￥8500|\r\n|手机|￥4000|\r\n|汽车|￥200000|\r\n";
                GUIUtility.systemCopyBuffer = content;

                SimulateKeyboardEvent("^v");
            };
        }
        /// <summary>
        /// 在当前光标位置插入一个引用对象
        /// </summary>
        private void AddMarkdownObject()
        {
            EditorApplication.delayCall += () =>
            {
                string content = "{Object}(在这里输入对象Assets开头的路径)";
                GUIUtility.systemCopyBuffer = content;

                SimulateKeyboardEvent("^v");
            };
        }
        /// <summary>
        /// 在当前光标位置插入一个执行菜单
        /// </summary>
        private void AddMarkdownMenu()
        {
            EditorApplication.delayCall += () =>
            {
                string content = "{Menu}(在这里输入编辑器中的菜单路径)";
                GUIUtility.systemCopyBuffer = content;

                SimulateKeyboardEvent("^v");
            };
        }
        /// <summary>
        /// 在当前光标位置插入一个自定义操作
        /// </summary>
        private void AddMarkdownCustomAction()
        {
            EditorApplication.delayCall += () =>
            {
                string content = "{Custom}(在这里输入参数)";
                GUIUtility.systemCopyBuffer = content;

                SimulateKeyboardEvent("^v");
            };
        }

        /// <summary>
        /// Json格式化
        /// </summary>
        private void JsonFormat()
        {
            JsonData jsonData = JsonToolkit.StringToJson(_value);
            if (jsonData != null)
            {
                StringBuilder builder = new StringBuilder();
                JsonWriter writer = new JsonWriter(builder);
                writer.PrettyPrint = true;
                writer.IndentValue = 4;

                JsonMapper.ToJson(jsonData, writer);
                string unicodeString = builder.ToString();
                _value = DecodeUnicode(unicodeString);
                GUI.FocusControl(null);

                EditorApplication.delayCall += () =>
                {
                    SimulateKeyboardEvent("[esc]");
                };
            }
        }
        /// <summary>
        /// 将字符串中的Unicode转义序列（如 \uXXXX）解码为对应的字符
        /// </summary>
        private string DecodeUnicode(string unicodeString)
        {
            return UnicodeRegex.Replace(unicodeString, m =>
            {
                string hexValue = m.Groups[1].Value;
                int code = int.Parse(hexValue, NumberStyles.HexNumber);
                return ((char)code).ToString();
            });
        }

        /// <summary>
        /// 设置为上标
        /// </summary>
        private void SetSuperscript()
        {
            GUIUtility.systemCopyBuffer = null;
            SimulateKeyboardEvent("^c");

            EditorApplication.delayCall += () =>
            {
                if (!string.IsNullOrEmpty(GUIUtility.systemCopyBuffer))
                {
                    string content = GUIUtility.systemCopyBuffer;
                    content = ReplaceAllSuperscript(content);
                    GUIUtility.systemCopyBuffer = content;

                    SimulateKeyboardEvent("^v");
                }
            };
        }
        /// <summary>
        /// 替换字符串中所有字符为上标（存在上标格式的字符）
        /// </summary>
        private string ReplaceAllSuperscript(string content)
        {
            char[] chars = content.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                switch (chars[i])
                {
                    case '0': chars[i] = '⁰'; break;
                    case '1': chars[i] = '¹'; break;
                    case '2': chars[i] = '²'; break;
                    case '3': chars[i] = '³'; break;
                    case '4': chars[i] = '⁴'; break;
                    case '5': chars[i] = '⁵'; break;
                    case '6': chars[i] = '⁶'; break;
                    case '7': chars[i] = '⁷'; break;
                    case '8': chars[i] = '⁸'; break;
                    case '9': chars[i] = '⁹'; break;
                    case '+': chars[i] = '⁺'; break;
                    case '-': chars[i] = '⁻'; break;
                    case '=': chars[i] = '⁼'; break;
                    case '(': chars[i] = '⁽'; break;
                    case ')': chars[i] = '⁾'; break;
                    case 'a': chars[i] = 'ᵃ'; break;
                    case 'b': chars[i] = 'ᵇ'; break;
                    case 'n': chars[i] = 'ⁿ'; break;
                    case 'i': chars[i] = 'ⁱ'; break;
                    case 'x': chars[i] = 'ˣ'; break;
                    default: break;
                }
            }
            return new string(chars);
        }

        /// <summary>
        /// 模拟键盘事件
        /// </summary>
        private void SimulateKeyboardEvent(string eventName)
        {
            Event e = Event.KeyboardEvent(eventName);
            SendEvent(e);
        }
    }
}