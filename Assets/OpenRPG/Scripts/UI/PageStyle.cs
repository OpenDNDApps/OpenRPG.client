using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ORC
{
    [CreateAssetMenu(fileName = nameof(PageStyle), menuName = kBaseScriptablePath + nameof(PageStyle))]
    public class PageStyle : OrcScriptableObject
    {
        public string Title;
        public PageBackgrounds SolidBackground => TransparentBackgrounds.Find(x => x.ID.Equals("solid"));
        public List<PageBackgrounds> TransparentBackgrounds = new List<PageBackgrounds>();

        public List<Sprite> Images = new List<Sprite>();

        [ContextMenu("ParseImages")]
        public void ParseImages()
        {
            foreach (Sprite image in Images)
            {
                if (TransparentBackgrounds.Exists(item => item.ID.Equals(image.name)))
                    continue;

                var style = image.name.Split('_')[0];
                var transparency = PageTransparency.None;
                switch (style)
                {
                    case "b":  transparency = PageTransparency.Bottom; break;
                    case "t":  transparency = PageTransparency.Top; break;
                    case "bt": transparency = PageTransparency.TopBottom; break;
                    case "lr": transparency = PageTransparency.LeftRight; break;
                    case "tl": transparency = PageTransparency.TopLeft; break;
                    case "tr": transparency = PageTransparency.TopRight; break;  
                    case "bl": transparency = PageTransparency.BottomLeft; break;
                    case "br": transparency = PageTransparency.BottomRight; break;
                    case "ch": transparency = PageTransparency.Horizontal; break;
                    case "cv": transparency = PageTransparency.Center; break;
                    case "l":  transparency = PageTransparency.Left; break;
                    case "r":  transparency = PageTransparency.Right; break;
                    case "tb": transparency = PageTransparency.TopBottom; break;
                }

                if (transparency == PageTransparency.None)
                {
                    Debug.LogError($"Style '{style}' is not supported.");
                    continue;
                }

                TransparentBackgrounds.Add(new PageBackgrounds
                {
                    ID = image.name,
                    Background = image,
                    Transparency = transparency
                });
            }
        }
    }
    
    [Serializable]
    public class PageBackgrounds
    {
        public string ID;
        public string SubID => ID.Split('_').Last();
        public PageTransparency Transparency;
        public Sprite Background;
    }
        
    [Serializable, Flags]
    public enum PageTransparency
    {
        None,
        Top = 1 << 1,
        Bottom = 1 << 2,
        Left = 1 << 3,
        Right = 1 << 4,
        Center = 1 << 5,
        Horizontal = 1 << 6,
        
        TopLeft = Top | Left,
        TopRight = Top | Right,
        TopBottom = Top | Bottom,
        BottomLeft = Bottom | Left,
        BottomRight = Bottom | Right,
        LeftRight = Left | Right,
    }
}
