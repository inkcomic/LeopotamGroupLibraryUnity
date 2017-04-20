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

        /// <summary>
        /// Create "text" node.
        /// </summary>
        /// <param name="node">Xml node.</param>
        /// <param name="container">markup container.</param>
        public static GameObject Create (XmlNode node, MarkupContainer container) {
            var go = new GameObject ("text");
            var txt = go.AddComponent<Text> ();
            var rt = go.GetComponent<RectTransform> ();
            string attrValue;
            string font = null;
            var align = TextAnchor.MiddleCenter;
            var color = Color.black;

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

            txt.text = node.Value;
            txt.alignment = align;
            txt.font = container.GetFont (font);
            txt.color = color;

            MarkupUtils.SetColor (txt, node);
            MarkupUtils.SetRectTransformSize (rt, node);
            MarkupUtils.SetDisabled (rt, node);
            txt.raycastTarget = MarkupUtils.ValidateInteractive (rt, node);

            return go;
        }
    }
}