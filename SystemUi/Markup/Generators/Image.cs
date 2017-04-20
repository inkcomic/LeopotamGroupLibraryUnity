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

        static readonly int HashedColor = "color".GetStableHashCode ();

        /// <summary>
        /// Create "image" node.
        /// </summary>
        /// <param name="node">Xml node.</param>
        /// <param name="container">markup container.</param>
        public static GameObject Create (XmlNode node, MarkupContainer container) {
            var go = new GameObject ("image");
            var img = go.AddComponent<Image> ();
            var rt = go.GetComponent<RectTransform> ();

            var attrValue = node.GetAttribute (HashedPath);
            if (!string.IsNullOrEmpty (attrValue)) {
                img.sprite = container.GetAtlasSprite (attrValue);
            }

            attrValue = node.GetAttribute (HashedNativeSize);
            var ignoreSize = (object) img.sprite != null && string.CompareOrdinal (attrValue, "true") == 0;
            if (ignoreSize) {
                img.SetNativeSize ();
            }

            MarkupUtils.SetColor (img, node);
            MarkupUtils.SetRectTransformSize (rt, node, ignoreSize);
            MarkupUtils.SetDisabled (rt, node);
            img.raycastTarget = MarkupUtils.ValidateInteractive (rt, node);

            return go;
        }
    }
}