// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using LeopotamGroup.Common;
using LeopotamGroup.Math;
using LeopotamGroup.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LeopotamGroup.SystemUi.Markup.Generators {
    static class TextNode {
        static readonly int HashedFontName = "fontName".GetStableHashCode ();

        static readonly int HashedFontSize = "fontSize".GetStableHashCode ();

        static readonly int HashedFontStyle = "fontStyle".GetStableHashCode ();

        /// <summary>
        /// Create "text" node.
        /// </summary>
        /// <param name="go">Gameobject holder.</param>
        /// <param name="node">Xml node.</param>
        /// <param name="container">Markup container.</param>
        public static void Create (GameObject go, XmlNode node, MarkupContainer container) {
#if UNITY_EDITOR
            go.name = "text";
#endif
            var txt = go.AddComponent<Text> ();
            string attrValue;
            string font = null;
            var align = TextAnchor.MiddleCenter;
            var color = Color.black;
            var style = FontStyle.Normal;

            attrValue = node.GetAttribute (HashedFontName);
            if (!string.IsNullOrEmpty (attrValue)) {
                font = attrValue;
            }

            attrValue = node.GetAttribute (HashedFontSize);
            if (!string.IsNullOrEmpty (attrValue)) {
                int fontSize;
                if (int.TryParse (attrValue, out fontSize)) {
                    txt.fontSize = fontSize;
                }
            }

            attrValue = node.GetAttribute (HashedFontStyle);
            if (!string.IsNullOrEmpty (attrValue)) {
                var parts = attrValue.Split (';');
                for (var i = 0; i < parts.Length; i++) {
                    switch (parts[i]) {
                        case "bold":
                            style |= FontStyle.Bold;
                            break;
                        case "italic":
                            style |= FontStyle.Italic;
                            break;
                    }
                }
            }

            txt.text = node.Value;
            txt.alignment = align;
            txt.font = container.GetFont (font);
            txt.color = color;
            txt.fontStyle = style;

            MarkupUtils.SetColor (txt, node);
            MarkupUtils.SetSize (go, node);
            MarkupUtils.SetRotation (go, node);
            MarkupUtils.SetOffset (go, node);
            MarkupUtils.SetMask (go, node);
            MarkupUtils.SetHidden (go, node);
            txt.raycastTarget = MarkupUtils.ValidateInteractive (go, node);
        }
    }
}