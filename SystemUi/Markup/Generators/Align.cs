// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using LeopotamGroup.Math;
using LeopotamGroup.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LeopotamGroup.SystemUi.Markup.Generators {
    static class AlignNode {
        static readonly int HashedSide = "side".GetStableHashCode ();

        /// <summary>
        /// Create "align" node.
        /// </summary>
        /// <param name="node">Xml node.</param>
        /// <param name="container">markup container.</param>
        public static GameObject Create (XmlNode node, MarkupContainer container) {
            var go = new GameObject ("align");
            var rt = go.AddComponent<RectTransform> ();
            var offset = Vector2.one * 0.5f;
            var attrValue = node.GetAttribute (HashedSide);
            if (!string.IsNullOrEmpty (attrValue)) {
                var parts = attrValue.Split (';');
                for (int i = 0; i < parts.Length; i++) {
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

            MarkupUtils.SetDisabled (rt, node);
            
            return go;
        }
    }
}