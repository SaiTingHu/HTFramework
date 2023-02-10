using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 图片资产
    /// </summary>
    internal sealed class TextureContent : AssetContent
    {
        public Texture Tex { get; private set; }
        public TextureFormat Format { get; private set; }
        public bool IsKnown { get; private set; }
        public bool IsCrunched { get; private set; }
        public int MipMapCount { get; private set; }
        public string Detail { get; private set; }
        public int Size { get; private set; }
        public int AlarmLevel { get; private set; }
        public string AlarmMessage { get; private set; }
        public HashSet<Material> InMaterials { get; private set; } = new HashSet<Material>();

        public TextureContent(Texture tex)
        {
            Tex = tex;

            if (tex is Texture2D)
            {
                Format = tex.Cast<Texture2D>().format;
                IsKnown = true;
                MipMapCount = tex.Cast<Texture2D>().mipmapCount;
            }
            else if (tex is Cubemap)
            {
                Format = tex.Cast<Cubemap>().format;
                IsKnown = true;
                MipMapCount = tex.Cast<Cubemap>().mipmapCount;
            }
            else
            {
                Format = TextureFormat.ARGB32;
                IsKnown = false;
                MipMapCount = 1;
            }

            IsCrunched = Format.ToString().Contains("Crunched");
            Detail = GetDetail();
            Size = GetSize();
            AlarmLevel = 0;
            AlarmMessage = "";
        }
        public void RaiseAlarmLevel(string message)
        {
            AlarmLevel += 1;
            AlarmMessage += $"{AlarmLevel}.{message}\r\n";
        }
        public void ClearAlarmLevel()
        {
            AlarmLevel = 0;
            AlarmMessage = "";
        }
        private int GetSize()
        {
            if (Tex != null)
            {
                if (Tex is Cubemap)
                {
                    return Tex.width * Tex.height * 6;
                }
                else
                {
                    return Tex.width * Tex.height;
                }
            }
            else
            {
                return 0;
            }
        }
        private string GetDetail()
        {
            StringBuilder detail = new StringBuilder();
            detail.Append(Tex.width.ToString());
            detail.Append("x");
            detail.Append(Tex.height.ToString());
            if (Tex is Cubemap) detail.Append("x6");
            detail.Append(" - ");
            detail.Append(Format.ToString());
            return detail.ToString();
        }
    }
}