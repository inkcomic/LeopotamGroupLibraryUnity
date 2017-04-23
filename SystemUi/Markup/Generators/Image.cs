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
    static class ImageNode {
        static readonly int HashedPath = "path".GetStableHashCode ();

        static readonly int HashedNativeSize = "nativeSize".GetStableHashCode ();

        /// <summary>
        /// Create "image" node.
        /// </summary>
        /// <param name="go">Gameobject holder.</param>
        /// <param name="node">Xml node.</param>
        /// <param name="container">Markup container.</param>
        public static void Create (GameObject go, XmlNode node, MarkupContainer container) {
#if UNITY_EDITOR
            go.name = "image";
#endif
            var img = go.AddComponent<Image> ();

            var attrValue = node.GetAttribute (HashedPath);
            if (!string.IsNullOrEmpty (attrValue)) {
                img.sprite = container.GetAtlasSprite (attrValue);
            }

            attrValue = node.GetAttribute (HashedNativeSize);
            var ignoreSize = (object) img.sprite != null && string.CompareOrdinal (attrValue, "true") == 0;
            if (ignoreSize) {
                img.SetNativeSize ();
            } else {
                MarkupUtils.SetSize (go, node);
            }

            MarkupUtils.SetColor (img, node);
            MarkupUtils.SetRotation (go, node);
            MarkupUtils.SetOffset (go, node);
            MarkupUtils.SetMask (go, node);
            MarkupUtils.SetHidden (go, node);
            img.raycastTarget = MarkupUtils.ValidateInteractive (go, node);
        }
    }
}