using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;
using static HT.Framework.MarkdownSpriteAsset;

namespace HT.Framework
{
    /// <summary>
    /// 支持解析并显示Markdown的Text
    /// </summary>
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    [AddComponentMenu("HTFramework/UI/Markdown Text")]
    public sealed class MarkdownText : Text, IPointerClickHandler
    {
        /// <summary>
        /// Awake时自动解析
        /// </summary>
        public bool IsParseInAwake = true;
        /// <summary>
        /// 是否显示超链接下划线
        /// </summary>
        public bool IsHyperlinkUnderline = false;
        /// <summary>
        /// 表格行高度
        /// </summary>
        public float TableRowHeight = 20;
        /// <summary>
        /// 嵌入图像的资源
        /// </summary>
        public MarkdownSpriteAsset SpriteAssets;
        /// <summary>
        /// 表格模板
        /// </summary>
        public MarkdownTable TableTemplate;
        /// <summary>
        /// 点击超链接事件（参数1：超链接名称，参数2：超链接Url）
        /// </summary>
        public UnityEvent<string, string> OnClickHyperlink;
        /// <summary>
        /// 点击嵌入的图像事件（参数1：图像）
        /// </summary>
        public UnityEvent<Sprite> OnClickEmbedTexture;

        /// <summary>
        /// 原始文本
        /// </summary>
        public string RawText { get; private set; }
        /// <summary>
        /// 纯净文本
        /// </summary>
        public string PureText { get; private set; }
        /// <summary>
        /// 所有的表格标记
        /// </summary>
        public List<TableMark> AllTableMarks { get; private set; } = new List<TableMark>();
        /// <summary>
        /// 所有嵌入图像标记
        /// </summary>
        public List<EmbedTextureMark> AllEmbedTextureMarks { get; private set; } = new List<EmbedTextureMark>();
        /// <summary>
        /// 所有超链接标记
        /// </summary>
        public List<HyperlinkMark> AllHyperlinkMarks { get; private set; } = new List<HyperlinkMark>();
        
        protected override void Awake()
        {
            base.Awake();

            if (IsParseInAwake)
            {
                ParseRawText();
            }
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();

            ClearParseContent();
        }
        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            base.OnPopulateMesh(toFill);

            OnPopulateTableMark(toFill);
            OnPopulateHyperlinkBoxes(toFill);
            OnPopulateEmbedTexture(toFill);
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            Vector2 anchoredPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out anchoredPos);

            for (int i = 0; i < AllHyperlinkMarks.Count; i++)
            {
                if (AllHyperlinkMarks[i].IsContains(anchoredPos))
                {
                    OnClickHyperlink.Invoke(AllHyperlinkMarks[i].Name, AllHyperlinkMarks[i].Url);
                    return;
                }
            }
        }

        #region Parser
        private static readonly Regex TableRegex = new Regex(@"[:]?[-]{3,}[:]?", RegexOptions.Singleline);
        private static readonly Regex EmphasizeRegex = new Regex(@"([*_]{1,3})([^*_]+?)([*_]{1,3})", RegexOptions.Singleline);
        private static readonly Regex EmbedTextureRegex = new Regex(@"!\[(.+?)\]\((.+?)\)", RegexOptions.Singleline);
        private static readonly Regex HyperlinkRegex = new Regex(@"\[(.+?)\]\((https?://.+?)\)", RegexOptions.Singleline);
        private const int Title1FontSize = 24;
        private const int Title2FontSize = 20;
        private const int Title3FontSize = 16;
        private const int Title4FontSize = 12;
        private const int Title5FontSize = 8;
        private const int Title6FontSize = 4;

        private StringBuilder _richTextBuilder = new StringBuilder();
        private StringBuilder _pureTextBuilder = new StringBuilder();
        private int _richTextCount;
        private int _pureTextCount;

        private StringBuilder _richTextBlock = new StringBuilder();
        private StringBuilder _pureTextBlock = new StringBuilder();
        private TableMark _tableMark;
        private List<EmbedTextureMark> _embedTextureMarksLine = new List<EmbedTextureMark>();
        private List<HyperlinkMark> _hyperlinkMarksLine = new List<HyperlinkMark>();

        /// <summary>
        /// 清空解析后的内容
        /// </summary>
        public void ClearParseContent()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
                return;
#endif
            RawText = null;
            PureText = null;
            if (Main.m_ReferencePool)
            {
                Main.m_ReferencePool.Despawns(AllTableMarks);
                Main.m_ReferencePool.Despawns(AllEmbedTextureMarks);
                Main.m_ReferencePool.Despawns(AllHyperlinkMarks);
            }
        }
        /// <summary>
        /// 解析原始文本
        /// </summary>
        /// <param name="onParseEnd">解析结束回调</param>
        public Coroutine ParseRawText(HTFAction onParseEnd = null)
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                onParseEnd?.Invoke();
                return null;
            }
#endif
            ClearParseContent();

            if (string.IsNullOrEmpty(text))
            {
                onParseEnd?.Invoke();
                return null;
            }

            return Main.Current.StartCoroutine(ParseRawText(text, onParseEnd));
        }
        private IEnumerator ParseRawText(string content, HTFAction onParseEnd)
        {
            _richTextBuilder.Clear();
            _pureTextBuilder.Clear();
            _richTextCount = 0;
            _pureTextCount = 0;
            _contentSizeFitter = GetComponent<ContentSizeFitter>();

            string[] rawTexts = content.Split('\n');
            for (int i = 0; i < rawTexts.Length; i++)
            {
                string rawText = rawTexts[i].Trim();
                string richText = null;
                string pureText = null;
                _embedTextureMarksLine.Clear();
                _hyperlinkMarksLine.Clear();

                if (!string.IsNullOrEmpty(rawText))
                {
                    //解析代码块
                    bool isCode = false;
                    Parse_Code(rawText, out isCode);
                    if (isCode)
                        continue;

                    //解析表格
                    bool isTable = false;
                    bool isSign = false;
                    Parse_Table(rawText, out richText, out pureText, out isTable, out isSign);
                    if (isTable && isSign)
                        continue;

                    if (!isTable)
                    {
                        Parse_TableDone();

                        //解析强调文本
                        Parse_Emphasize(rawText, out richText, out pureText);
                        //解析标题
                        Parse_Title(richText, pureText, out richText, out pureText);
                        //解析嵌入图像
                        yield return Parse_EmbedTexture(richText, pureText, (r, p) => { richText = r; pureText = p; });
                        //解析超链接
                        Parse_Hyperlink(richText, pureText, out richText, out pureText);
                    }

                    if (!string.IsNullOrEmpty(richText))
                    {
                        _richTextBuilder.Append(richText);
                        _richTextCount += richText.Length;
                    }
                    if (!string.IsNullOrEmpty(pureText))
                    {
                        _pureTextBuilder.Append(pureText);
                        _pureTextCount += pureText.LengthIgnoreBlank();
                    }
                }

                if (i != (rawTexts.Length - 1))
                {
                    _richTextBuilder.Append('\n');
                    _pureTextBuilder.Append('\n');
                    _richTextCount += 1;
                }
                AllEmbedTextureMarks.AddRange(_embedTextureMarksLine);
                AllHyperlinkMarks.AddRange(_hyperlinkMarksLine);
            }
            Parse_TableDone();

            RawText = content;
            text = _richTextBuilder.ToString();
            PureText = _pureTextBuilder.ToString();
            onParseEnd?.Invoke();
        }
        /// <summary>
        /// 解析代码块
        /// </summary>
        private void Parse_Code(string rawText, out bool isCode)
        {
            isCode = rawText.StartsWith("```");
        }
        /// <summary>
        /// 解析表格
        /// </summary>
        private void Parse_Table(string rawText, out string richText, out string pureText, out bool isTable, out bool isSign)
        {
            if (rawText.StartsWith('|') && rawText.EndsWith('|'))
            {
                string[] values = rawText.Split('|');
                if (values.Length >= 3)
                {
                    if (_tableMark == null)
                    {
                        _tableMark = Main.m_ReferencePool.Spawn<TableMark>();
                        _tableMark.RichStartIndex = _richTextCount;
                        _tableMark.PureStartIndex = _pureTextCount;
                        _tableMark.Width = rectTransform.rect.width;
                        _tableMark.RowHeight = TableRowHeight;
                    }
                    
                    if (_tableMark.Signs.Count == 0 && IsTableSign(values))
                    {
                        for (int i = 1; i < (values.Length - 1); i++) _tableMark.Signs.Add(values[i]);

                        richText = null;
                        pureText = null;
                        isTable = true;
                        isSign = true;
                        return;
                    }
                    else
                    {
                        string[] row = new string[values.Length - 2];
                        for (int i = 1; i < (values.Length - 1); i++) row[i - 1] = values[i];
                        _tableMark.Rows.Add(row);

                        _tableMark.RichQuadIndexs.Add(_richTextCount);
                        _tableMark.PureQuadIndexs.Add(_pureTextCount);
                        richText = $"<quad size={TableRowHeight}/>";
                        pureText = "?";
                        isTable = true;
                        isSign = false;
                        return;
                    }
                }
            }

            richText = null;
            pureText = null;
            isTable = false;
            isSign = false;
        }
        /// <summary>
        /// 解析表格完成
        /// </summary>
        private void Parse_TableDone()
        {
            if (_tableMark != null)
            {
                if (TableTemplate)
                {
                    _tableMark.Generate(rectTransform, TableTemplate);
                    AllTableMarks.Add(_tableMark);
                }
                else
                {
                    Main.m_ReferencePool.Despawn(_tableMark);
                }
                _tableMark = null;
            }
        }
        /// <summary>
        /// 解析强调文本
        /// </summary>
        private void Parse_Emphasize(string rawText, out string richText, out string pureText)
        {
            int index = 0;
            _richTextBlock.Clear();
            _pureTextBlock.Clear();
            foreach (Match match in EmphasizeRegex.Matches(rawText))
            {
                if (match.Groups.Count == 4)
                {
                    //存储前置内容
                    string frontValue = rawText.Substring(index, match.Index - index);
                    _richTextBlock.Append(frontValue);
                    _pureTextBlock.Append(frontValue);

                    //存储强调文本内容
                    string keywordStart = match.Groups[1].Value;
                    string text = match.Groups[2].Value;
                    string keywordEnd = match.Groups[3].Value;
                    if (keywordStart == "***" && keywordEnd == "***")
                    {
                        _richTextBlock.Append("<b><i>");
                        _richTextBlock.Append(text);
                        _richTextBlock.Append("</i></b>");

                        _pureTextBlock.Append(text);
                    }
                    else if (keywordStart == "___" && keywordEnd == "___")
                    {
                        _richTextBlock.Append("<b><i>");
                        _richTextBlock.Append(text);
                        _richTextBlock.Append("</i></b>");

                        _pureTextBlock.Append(text);
                    }
                    else if (keywordStart == "**" && keywordEnd == "**")
                    {
                        _richTextBlock.Append("<b>");
                        _richTextBlock.Append(text);
                        _richTextBlock.Append("</b>");

                        _pureTextBlock.Append(text);
                    }
                    else if (keywordStart == "__" && keywordEnd == "__")
                    {
                        _richTextBlock.Append("<b>");
                        _richTextBlock.Append(text);
                        _richTextBlock.Append("</b>");

                        _pureTextBlock.Append(text);
                    }
                    else if (keywordStart == "*" && keywordEnd == "*")
                    {
                        _richTextBlock.Append("<i>");
                        _richTextBlock.Append(text);
                        _richTextBlock.Append("</i>");

                        _pureTextBlock.Append(text);
                    }
                    else if (keywordStart == "_" && keywordEnd == "_")
                    {
                        _richTextBlock.Append("<i>");
                        _richTextBlock.Append(text);
                        _richTextBlock.Append("</i>");

                        _pureTextBlock.Append(text);
                    }
                    else
                    {
                        _richTextBlock.Append(keywordStart);
                        _richTextBlock.Append(text);
                        _richTextBlock.Append(keywordEnd);

                        _pureTextBlock.Append(keywordStart);
                        _pureTextBlock.Append(text);
                        _pureTextBlock.Append(keywordEnd);
                    }

                    index = match.Index + match.Length;
                }
            }
            //存储后置内容
            string backValue = rawText.Substring(index);
            _richTextBlock.Append(backValue);
            _pureTextBlock.Append(backValue);
            richText = _richTextBlock.ToString();
            pureText = _pureTextBlock.ToString();
        }
        /// <summary>
        /// 解析标题
        /// </summary>
        private void Parse_Title(string oldRichText, string oldPureText, out string richText, out string pureText)
        {
            if (oldRichText.StartsWith("######"))
            {
                richText = $"<size={fontSize + Title6FontSize}>{oldRichText.Remove(0, 6)}</size>";
                pureText = oldPureText.Remove(0, 6);
            }
            else if (oldRichText.StartsWith("#####"))
            {
                richText = $"<size={fontSize + Title5FontSize}>{oldRichText.Remove(0, 5)}</size>";
                pureText = oldPureText.Remove(0, 5);
            }
            else if (oldRichText.StartsWith("####"))
            {
                richText = $"<size={fontSize + Title4FontSize}>{oldRichText.Remove(0, 4)}</size>";
                pureText = oldPureText.Remove(0, 4);
            }
            else if (oldRichText.StartsWith("###"))
            {
                richText = $"<size={fontSize + Title3FontSize}>{oldRichText.Remove(0, 3)}</size>";
                pureText = oldPureText.Remove(0, 3);
            }
            else if (oldRichText.StartsWith("##"))
            {
                richText = $"<size={fontSize + Title2FontSize}>{oldRichText.Remove(0, 2)}</size>";
                pureText = oldPureText.Remove(0, 2);
            }
            else if (oldRichText.StartsWith("#"))
            {
                richText = $"<size={fontSize + Title1FontSize}>{oldRichText.Remove(0, 1)}</size>";
                pureText = oldPureText.Remove(0, 1);
            }
            else
            {
                richText = oldRichText;
                pureText = oldPureText;
            }
        }
        /// <summary>
        /// 解析嵌入图像
        /// </summary>
        private IEnumerator Parse_EmbedTexture(string oldRichText, string oldPureText, HTFAction<string, string> onParseEnd)
        {
            MatchCollection richMatchs = EmbedTextureRegex.Matches(oldRichText);
            MatchCollection pureMatchs = EmbedTextureRegex.Matches(oldPureText);
            if (richMatchs.Count != pureMatchs.Count || (richMatchs.Count == 0 && pureMatchs.Count == 0))
            {
                onParseEnd?.Invoke(oldRichText, oldPureText);
                yield break;
            }

            int richIndex = 0;
            int pureIndex = 0;
            _richTextBlock.Clear();
            _pureTextBlock.Clear();
            for (int i = 0; i < pureMatchs.Count; i++)
            {
                Match richMatch = richMatchs[i];
                Match pureMatch = pureMatchs[i];
                if (richMatch.Groups.Count == 3 && pureMatch.Groups.Count == 3 && (pureMatch.Groups[2].Value.StartsWith("http") || pureMatch.Groups[2].Value.StartsWith("id:")))
                {
                    EmbedTextureMark embedTextureMark = Main.m_ReferencePool.Spawn<EmbedTextureMark>();
                    embedTextureMark.Name = pureMatch.Groups[1].Value;
                    embedTextureMark.Url = pureMatch.Groups[2].Value;
                    embedTextureMark.IsNetwork = embedTextureMark.Url.StartsWith("http");

                    //存储前置内容
                    _richTextBlock.Append(oldRichText.Substring(richIndex, richMatch.Index - richIndex));
                    _pureTextBlock.Append(oldPureText.Substring(pureIndex, pureMatch.Index - pureIndex));

                    Sprite sprite = null;
                    int size = fontSize;
                    if (embedTextureMark.IsNetwork)
                    {
                        yield return LoadUrlTexture(embedTextureMark.Url, (texture) =>
                        {
                            if (texture != null)
                            {
                                sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                                size = Mathf.Min(Mathf.Max(texture.width, texture.height), (int)(rectTransform.rect.width - 10));
                            }
                        });
                    }
                    else
                    {
                        if (SpriteAssets != null)
                        {
                            string id = embedTextureMark.Url.Replace("id:", "");
                            SpriteTarget spriteTarget = SpriteAssets.SpriteTargets.Find((s) => { return s.ID == id; });
                            if (spriteTarget != null && spriteTarget.Target != null)
                            {
                                sprite = spriteTarget.Target;
                                size = Mathf.Min(Mathf.Max(sprite.texture.width, sprite.texture.height), (int)(rectTransform.rect.width - 10));
                            }
                        }
                    }

                    embedTextureMark.Generate(rectTransform, sprite, size);
                    embedTextureMark.EmbedButton.onClick.AddListener(() =>
                    {
                        OnClickEmbedTexture.Invoke(embedTextureMark.EmbedSprite);
                    });

                    //存储嵌入图像内容
                    embedTextureMark.RichStartIndex = _richTextCount + _richTextBlock.Length;
                    _richTextBlock.Append($"<quad size={embedTextureMark.Size}/>");
                    embedTextureMark.PureStartIndex = _pureTextCount + _pureTextBlock.LengthIgnoreBlank();
                    _pureTextBlock.Append("?");

                    richIndex = richMatch.Index + richMatch.Length;
                    pureIndex = pureMatch.Index + pureMatch.Length;

                    _embedTextureMarksLine.Add(embedTextureMark);
                }
            }
            //存储后置内容
            _richTextBlock.Append(oldRichText.Substring(richIndex));
            _pureTextBlock.Append(oldPureText.Substring(pureIndex));
            onParseEnd?.Invoke(_richTextBlock.ToString(), _pureTextBlock.ToString());
        }
        /// <summary>
        /// 解析超链接
        /// </summary>
        private void Parse_Hyperlink(string oldRichText, string oldPureText, out string richText, out string pureText)
        {
            MatchCollection richMatchs = HyperlinkRegex.Matches(oldRichText);
            MatchCollection pureMatchs = HyperlinkRegex.Matches(oldPureText);
            if (richMatchs.Count != pureMatchs.Count || (richMatchs.Count == 0 && pureMatchs.Count == 0))
            {
                richText = oldRichText;
                pureText = oldPureText;
                return;
            }
            
            int richIndex = 0;
            int pureIndex = 0;
            _richTextBlock.Clear();
            _pureTextBlock.Clear();
            for (int i = 0; i < pureMatchs.Count; i++)
            {
                Match richMatch = richMatchs[i];
                Match pureMatch = pureMatchs[i];
                if (richMatch.Groups.Count == 3 && pureMatch.Groups.Count == 3)
                {
                    HyperlinkMark hyperlinkMark = Main.m_ReferencePool.Spawn<HyperlinkMark>();
                    hyperlinkMark.Name = pureMatch.Groups[1].Value;
                    hyperlinkMark.Url = pureMatch.Groups[2].Value;

                    //存储前置内容
                    _richTextBlock.Append(oldRichText.Substring(richIndex, richMatch.Index - richIndex));
                    _pureTextBlock.Append(oldPureText.Substring(pureIndex, pureMatch.Index - pureIndex));

                    //存储超链接文本内容
                    _richTextBlock.Append("<color=blue>");
                    hyperlinkMark.RichStartIndex = _richTextCount + _richTextBlock.Length;
                    _richTextBlock.Append(hyperlinkMark.Name);
                    hyperlinkMark.RichEndIndex = _richTextCount + _richTextBlock.Length - 1;
                    _richTextBlock.Append("</color>");
                    hyperlinkMark.PureStartIndex = _pureTextCount + _pureTextBlock.LengthIgnoreBlank();
                    _pureTextBlock.Append(hyperlinkMark.Name);
                    hyperlinkMark.PureEndIndex = _pureTextCount + _pureTextBlock.LengthIgnoreBlank() - 1;

                    int richChangedIndex = (12 - 1) + (8 - hyperlinkMark.Url.Length - 3);
                    int pureChangedIndex = (0 - 1) + (0 - hyperlinkMark.Url.Length - 3);
                    UpdateEmbedTextureMarksIndex(richMatch.Index, pureMatch.Index, richChangedIndex, pureChangedIndex);

                    richIndex = richMatch.Index + richMatch.Length;
                    pureIndex = pureMatch.Index + pureMatch.Length;

                    _hyperlinkMarksLine.Add(hyperlinkMark);
                }
            }
            //存储后置内容
            _richTextBlock.Append(oldRichText.Substring(richIndex));
            _pureTextBlock.Append(oldPureText.Substring(pureIndex));
            richText = _richTextBlock.ToString();
            pureText = _pureTextBlock.ToString();
        }

        /// <summary>
        /// 是否为表格的标记行
        /// </summary>
        private bool IsTableSign(string[] row)
        {
            for (int i = 1; i < (row.Length - 1); i++)
            {
                string value = row[i].Trim();
                if (!TableRegex.IsMatch(value))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 下载网络链接图片
        /// </summary>
        private IEnumerator LoadUrlTexture(string url, HTFAction<Texture2D> callback)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.downloadHandler = new DownloadHandlerTexture(true);
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    callback?.Invoke(request.downloadHandler.Cast<DownloadHandlerTexture>().texture);
                }
                else
                {
                    Log.Warning("下载网络图片失败：" + request.error);

                    callback?.Invoke(null);
                }
            }
        }
        /// <summary>
        /// 更新所有嵌入图片标记的索引
        /// </summary>
        private void UpdateEmbedTextureMarksIndex(int richIndex, int pureIndex, int richChangedIndex, int pureChangedIndex)
        {
            for (int i = 0; i < _embedTextureMarksLine.Count; i++)
            {
                if (_embedTextureMarksLine[i].RichStartIndex > richIndex && _embedTextureMarksLine[i].PureStartIndex > pureIndex)
                {
                    _embedTextureMarksLine[i].RichStartIndex += richChangedIndex;
                    _embedTextureMarksLine[i].PureStartIndex += pureChangedIndex;
                }
            }
        }
        #endregion

        #region PopulateMesh
        private ContentSizeFitter _contentSizeFitter;
        private UIVertex[] _underlineVertices = new UIVertex[4];

        /// <summary>
        /// 填充表格内容
        /// </summary>
        private void OnPopulateTableMark(VertexHelper toFill)
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
                return;
#endif
            //填充表格区域
            bool isCrossBorder = IsContentCrossBorder();
            for (int i = 0; i < AllTableMarks.Count; i++)
            {
                TableMark tableMark = AllTableMarks[i];

                int startVertIndex = isCrossBorder ? (tableMark.RichStartIndex * 4) : (tableMark.PureStartIndex * 4);
                if (startVertIndex < toFill.currentVertCount)
                {
                    UIVertex vertex = new UIVertex();
                    toFill.PopulateUIVertex(ref vertex, startVertIndex);
                    float x = vertex.position.x + (rectTransform.pivot.x - 0.5f) * rectTransform.rect.width;
                    float y = vertex.position.y + (rectTransform.pivot.y - 0.5f) * rectTransform.rect.height;
                    tableMark.Table.IsShow = true;
                    tableMark.Table.Width = rectTransform.rect.width;
                    tableMark.Pos = new Vector2(x, y);

                    for (int j = 0; j < tableMark.RichQuadIndexs.Count; j++)
                    {
                        int startIndex = isCrossBorder ? (tableMark.RichQuadIndexs[j] * 4) : (tableMark.PureQuadIndexs[j] * 4);
                        if ((startIndex + 3) < toFill.currentVertCount)
                        {
                            toFill.PopulateUIVertex(ref vertex, startIndex);
                            vertex.uv0 = vertex.uv1 = vertex.uv2 = vertex.uv3 = Vector4.zero;
                            toFill.SetUIVertex(vertex, startIndex);

                            toFill.PopulateUIVertex(ref vertex, startIndex + 1);
                            vertex.uv0 = vertex.uv1 = vertex.uv2 = vertex.uv3 = Vector4.zero;
                            toFill.SetUIVertex(vertex, startIndex + 1);

                            toFill.PopulateUIVertex(ref vertex, startIndex + 2);
                            vertex.uv0 = vertex.uv1 = vertex.uv2 = vertex.uv3 = Vector4.zero;
                            toFill.SetUIVertex(vertex, startIndex + 2);

                            toFill.PopulateUIVertex(ref vertex, startIndex + 3);
                            vertex.uv0 = vertex.uv1 = vertex.uv2 = vertex.uv3 = Vector4.zero;
                            toFill.SetUIVertex(vertex, startIndex + 3);
                        }
                    }
                }
                else
                {
                    tableMark.Table.IsShow = false;
                }
            }
        }
        /// <summary>
        /// 填充超链接包围盒
        /// </summary>
        private void OnPopulateHyperlinkBoxes(VertexHelper toFill)
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
                return;
#endif
            //填充包围盒
            bool isCrossBorder = IsContentCrossBorder();
            for (int i = 0; i < AllHyperlinkMarks.Count; i++)
            {
                HyperlinkMark hyperlinkMark = AllHyperlinkMarks[i];
                hyperlinkMark.UnderlinePoss.Clear();
                hyperlinkMark.Boxes.Clear();

                int startVertIndex = isCrossBorder ? (hyperlinkMark.RichStartIndex * 4) : (hyperlinkMark.PureStartIndex * 4);
                int endVertIndex = isCrossBorder ? (hyperlinkMark.RichEndIndex * 4) : (hyperlinkMark.PureEndIndex * 4);
                float underlinePos = 0;
                int lineIndex = 0;
                UIVertex vertex = new UIVertex();
                for (int j = startVertIndex; j <= endVertIndex; j += 4)
                {
                    if ((j + 3) < toFill.currentVertCount)
                    {
                        toFill.PopulateUIVertex(ref vertex, j + 3);
                        Vector2 pos = vertex.position;
                        toFill.PopulateUIVertex(ref vertex, j + 2);
                        float width = vertex.position.x - pos.x;
                        toFill.PopulateUIVertex(ref vertex, j + 1);
                        float height = vertex.position.y - pos.y;
                        if (!width.Approximately(0) && !height.Approximately(0))
                        {
                            hyperlinkMark.Boxes.Add(new Rect(pos.x, pos.y, width, height));
                            //计算下划线位置
                            if (IsHyperlinkUnderline)
                            {
                                if (hyperlinkMark.Boxes.Count == 1)
                                {
                                    underlinePos = pos.y;
                                }
                                else
                                {
                                    if (IsSameRow(hyperlinkMark.Boxes[^1], hyperlinkMark.Boxes[^2]))
                                    {
                                        if (pos.y < underlinePos)
                                        {
                                            underlinePos = pos.y;
                                            for (int l = lineIndex; l < hyperlinkMark.UnderlinePoss.Count; l++) hyperlinkMark.UnderlinePoss[l] = underlinePos;
                                        }
                                    }
                                    else
                                    {
                                        if (pos.y < underlinePos)
                                        {
                                            underlinePos = pos.y;
                                            lineIndex = hyperlinkMark.Boxes.Count - 1;
                                        }
                                    }
                                }
                            }
                            hyperlinkMark.UnderlinePoss.Add(underlinePos);
                        }
                    }
                }
            }

            //创建下划线
            if (IsHyperlinkUnderline && AllHyperlinkMarks.Count > 0)
            {
                using (TextGenerator underline = new TextGenerator())
                {
                    underline.Populate("—", GetGenerationSettings(rectTransform.rect.size));
                    IList<UIVertex> vertexs = underline.verts;
                    if (vertexs.Count == 4)
                    {
                        float height = vertexs[0].position.y - vertexs[3].position.y;
                        for (int i = 0; i < AllHyperlinkMarks.Count; i++)
                        {
                            for (int j = 0; j < AllHyperlinkMarks[i].Boxes.Count; j++)
                            {
                                Rect rect = AllHyperlinkMarks[i].Boxes[j];
                                float x = rect.position.x;
                                float y = AllHyperlinkMarks[i].UnderlinePoss[j] + height * 0.5f;
                                float width = rect.width;

                                for (int v = 0; v < 4; v++)
                                {
                                    _underlineVertices[v] = vertexs[v];
                                    _underlineVertices[v].color = Color.blue;
                                }
                                _underlineVertices[0].position = new Vector3(x, y);
                                _underlineVertices[1].position = _underlineVertices[0].position + new Vector3(width, 0, 0);
                                _underlineVertices[2].position = _underlineVertices[1].position + new Vector3(0, -height, 0);
                                _underlineVertices[3].position = _underlineVertices[0].position + new Vector3(0, -height, 0);
                                toFill.AddUIVertexQuad(_underlineVertices);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 填充嵌入图片内容
        /// </summary>
        private void OnPopulateEmbedTexture(VertexHelper toFill)
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
                return;
#endif
            //填充图片区域
            bool isCrossBorder = IsContentCrossBorder();
            for (int i = 0; i < AllEmbedTextureMarks.Count; i++)
            {
                EmbedTextureMark embedTextureMark = AllEmbedTextureMarks[i];

                int startVertIndex = isCrossBorder ? (embedTextureMark.RichStartIndex * 4) : (embedTextureMark.PureStartIndex * 4);
                if (startVertIndex < toFill.currentVertCount)
                {
                    UIVertex vertex = new UIVertex();
                    toFill.PopulateUIVertex(ref vertex, startVertIndex);
                    float x = vertex.position.x + (rectTransform.pivot.x - 0.5f) * rectTransform.rect.width;
                    float y = vertex.position.y + (rectTransform.pivot.y - 0.5f) * rectTransform.rect.height;
                    embedTextureMark.EmbedImage.IsShow = true;
                    embedTextureMark.Pos = new Vector2(x, y);

                    if ((startVertIndex + 3) < toFill.currentVertCount)
                    {
                        vertex.uv0 = vertex.uv1 = vertex.uv2 = vertex.uv3 = Vector4.zero;
                        toFill.SetUIVertex(vertex, startVertIndex);

                        toFill.PopulateUIVertex(ref vertex, startVertIndex + 1);
                        vertex.uv0 = vertex.uv1 = vertex.uv2 = vertex.uv3 = Vector4.zero;
                        toFill.SetUIVertex(vertex, startVertIndex + 1);

                        toFill.PopulateUIVertex(ref vertex, startVertIndex + 2);
                        vertex.uv0 = vertex.uv1 = vertex.uv2 = vertex.uv3 = Vector4.zero;
                        toFill.SetUIVertex(vertex, startVertIndex + 2);

                        toFill.PopulateUIVertex(ref vertex, startVertIndex + 3);
                        vertex.uv0 = vertex.uv1 = vertex.uv2 = vertex.uv3 = Vector4.zero;
                        toFill.SetUIVertex(vertex, startVertIndex + 3);
                    }
                }
                else
                {
                    embedTextureMark.EmbedImage.IsShow = false;
                }
            }
        }

        /// <summary>
        /// 文本内容是否越过边界
        /// </summary>
        private bool IsContentCrossBorder()
        {
            if (_contentSizeFitter != null && _contentSizeFitter.verticalFit == ContentSizeFitter.FitMode.PreferredSize)
                return false;

            TextGenerationSettings settings = GetGenerationSettings(new Vector2(rectTransform.rect.width, 0.0f));
            float height = cachedTextGeneratorForLayout.GetPreferredHeight(m_Text, settings) / pixelsPerUnit;
            return height >= rectTransform.rect.height && verticalOverflow != VerticalWrapMode.Overflow;
        }
        /// <summary>
        /// 两个包围盒是否在同一行中
        /// </summary>
        private bool IsSameRow(Rect rect1, Rect rect2)
        {
            float value = (rect1.height > rect2.height) ? (rect1.height * 0.8f) : (rect2.height * 0.8f);
            return Mathf.Abs(rect1.center.y - rect2.center.y) < value;
        }
        #endregion

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!UnityEditor.EditorApplication.isPlaying)
                return;

            for (int i = 0; i < AllHyperlinkMarks.Count; i++)
            {
                for (int j = 0; j < AllHyperlinkMarks[i].Boxes.Count; j++)
                {
                    Rect rect = AllHyperlinkMarks[i].Boxes[j];
                    rect.position = transform.localToWorldMatrix.MultiplyPoint(rect.position);
                    rect.width *= transform.localScale.x;
                    rect.height *= transform.localScale.y;
                    DrawRect(rect, Color.cyan);
                }
            }
        }
        private void DrawRect(Rect rect, Color color)
        {
            Gizmos.color = color;

            Vector2 v1 = rect.position;
            Vector2 v2 = rect.position + new Vector2(rect.width, 0);
            Vector2 v3 = rect.position + new Vector2(rect.width, rect.height);
            Vector2 v4 = rect.position + new Vector2(0, rect.height);

            Gizmos.DrawLine(v1, v2);
            Gizmos.DrawLine(v2, v3);
            Gizmos.DrawLine(v3, v4);
            Gizmos.DrawLine(v4, v1);
        }
#endif

        /// <summary>
        /// 表格标记
        /// </summary>
        public class TableMark : IReference
        {
            public int RichStartIndex;
            public int PureStartIndex;
            public float Width;
            public float RowHeight;
            public List<string> Signs = new List<string>();
            public List<string[]> Rows = new List<string[]>();
            public List<int> RichQuadIndexs = new List<int>();
            public List<int> PureQuadIndexs = new List<int>();
            public MarkdownTable Table;

            /// <summary>
            /// 表格位置
            /// </summary>
            public Vector2 Pos
            {
                get
                {
                    return Table != null ? Table.Rect.anchoredPosition : Vector2.zero;
                }
                set
                {
                    if (Table != null)
                    {
                        Table.Rect.anchoredPosition = value;
                    }
                }
            }

            /// <summary>
            /// 生成
            /// </summary>
            public void Generate(RectTransform parent, MarkdownTable template)
            {
                GameObject obj = Main.CloneGameObject(template.gameObject, true);
                obj.name = "表格";
                obj.transform.SetParent(parent);
                obj.transform.localRotation = Quaternion.identity;
                obj.transform.localScale = Vector3.one;
                Table = obj.GetComponent<MarkdownTable>();
                Table.Generate(this);
            }
            public void Reset()
            {
                Signs.Clear();
                Rows.Clear();
                RichQuadIndexs.Clear();
                PureQuadIndexs.Clear();
                if (Table != null)
                {
                    Main.Kill(Table.gameObject);
                    Table = null;
                }
            }
        }
        /// <summary>
        /// 嵌入图像标记
        /// </summary>
        public class EmbedTextureMark : IReference
        {
            public string Name;
            public string Url;
            public bool IsNetwork;
            public int RichStartIndex;
            public int PureStartIndex;
            public Sprite EmbedSprite;
            public MarkdownImage EmbedImage;
            public Button EmbedButton;
            public int Size;

            /// <summary>
            /// 图像位置
            /// </summary>
            public Vector2 Pos
            {
                get
                {
                    return EmbedImage.rectTransform.anchoredPosition;
                }
                set
                {
                    EmbedImage.rectTransform.anchoredPosition = value;
                }
            }

            /// <summary>
            /// 生成
            /// </summary>
            public void Generate(RectTransform parent, Sprite sprite, int size)
            {
                EmbedSprite = sprite;
                Size = size;

                GameObject obj = new GameObject(Name);
                obj.transform.SetParent(parent);
                obj.transform.localRotation = Quaternion.identity;
                obj.transform.localScale = Vector3.one;
                EmbedImage = obj.AddComponent<MarkdownImage>();
                EmbedImage.rectTransform.sizeDelta = new Vector2(Size, Size);
                EmbedImage.rectTransform.pivot = new Vector2(0, 1);
                EmbedImage.sprite = EmbedSprite;
                EmbedButton = obj.AddComponent<Button>();
                EmbedButton.targetGraphic = EmbedImage;
                EmbedButton.transition = Selectable.Transition.None;
            }
            public void Reset()
            {
                if (IsNetwork && EmbedSprite != null)
                {
                    Main.Kill(EmbedSprite);
                    EmbedSprite = null;
                }
                if (EmbedImage != null)
                {
                    Main.Kill(EmbedImage.gameObject);
                    EmbedImage = null;
                    EmbedButton = null;
                }
            }
        }
        /// <summary>
        /// 超链接标记
        /// </summary>
        public class HyperlinkMark : IReference
        {
            public string Name;
            public string Url;
            public int RichStartIndex;
            public int RichEndIndex;
            public int PureStartIndex;
            public int PureEndIndex;
            public List<float> UnderlinePoss = new List<float>();
            public List<Rect> Boxes = new List<Rect>();

            /// <summary>
            /// 超链接包围盒是否包含指定点
            /// </summary>
            /// <param name="pos">指定点</param>
            public bool IsContains(Vector2 pos)
            {
                for (int i = 0; i < Boxes.Count; i++)
                {
                    if (Boxes[i].Contains(pos))
                    {
                        return true;
                    }
                }
                return false;
            }
            public void Reset()
            {
                UnderlinePoss.Clear();
                Boxes.Clear();
            }
        }
    }
}