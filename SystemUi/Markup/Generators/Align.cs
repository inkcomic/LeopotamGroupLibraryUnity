// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using LeopotamGroup.Math;
using LeopotamGroup.Serialization;
using UnityEngine;

namespace LeopotamGroup.SystemUi.Markup.Generators {
    static class AlignNode {
        static readonly int HashedSide = "side".GetStableHashCode ();

        /// <summary>
        /// Create "align" node. If children supported - GameObject container for them should be returned.
        /// </summary>
        /// <param name="widget">Ui widget.</param>
        /// <param name="node">Xml node.</param>
        /// <param name="container">Markup container.</param>
        public static RectTransform Create (RectTransform widget, XmlNode node, MarkupContainer container) {
#if UNITY_EDITOR
            widget.name = "align";
#endif
            var rt = widget.GetComponent<RectTransform> ();
            var offset = Vector2.one * 0.5f;
            var attrValue = node.GetAttribute (HashedSide);
            if (!string.IsNullOrEmpty (attrValue)) {
                var parts = MarkupUtils.SplitAttrValue (attrValue);
                for (var i = 0; i < parts.Length; i++) {
                    switch (parts[i]) {
                        case "left":
                            offset.x = 0f;
                            break;
                        case "right":
                            offset.x = 1f;
                            break;
                        case "top":
                            offset.y = 1f;
                            break;
                        case "down":
                            offset.y = 0f;
                            break;
                    }
                }
            }

            rt.anchorMin = offset;
            rt.anchorMax = offset;
            rt.offsetMin = -Vector2.one * 0.5f;
            rt.offsetMax = -rt.offsetMin;

            MarkupUtils.SetHidden (widget, node);

            return widget;
        }
    }
}