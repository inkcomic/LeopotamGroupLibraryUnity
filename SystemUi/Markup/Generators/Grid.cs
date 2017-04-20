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
    static class GridNode {
        /// <summary>
        /// Create "grid" node.
        /// </summary>
        /// <param name="node">Xml node.</param>
        /// <param name="container">markup container.</param>
        public static GameObject Create (XmlNode node, MarkupContainer container) {
            var go = new GameObject ("grid");
            var grid = go.AddComponent<GridLayoutGroup> ();
            var rt = go.GetComponent<RectTransform> ();

            MarkupUtils.SetRectTransformSize (rt, node);
            MarkupUtils.SetDisabled (rt, node);
            
            return go;
        }
    }
}