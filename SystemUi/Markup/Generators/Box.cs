// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using LeopotamGroup.Math;
using LeopotamGroup.Serialization;
using LeopotamGroup.SystemUi.Widgets;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LeopotamGroup.SystemUi.Markup.Generators {
    static class BoxNode {
        /// <summary>
        /// Create "box" node.
        /// </summary>
        /// <param name="node">Xml node.</param>
        /// <param name="container">markup container.</param>
        public static GameObject Create (XmlNode node, MarkupContainer container) {
            var go = new GameObject ("box");
            var rt = go.AddComponent<RectTransform> ();
            
            MarkupUtils.SetRectTransformSize (rt, node);
            MarkupUtils.SetDisabled (rt, node);
            
            if (MarkupUtils.ValidateInteractive (rt, node)) {
                go.AddComponent<NonVisualWidget> ();
            }

            return go;
        }
    }
}