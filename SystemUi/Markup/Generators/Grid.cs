// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using System.Globalization;
using LeopotamGroup.Math;
using LeopotamGroup.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LeopotamGroup.SystemUi.Markup.Generators {
    static class GridNode {
        static readonly int HashedFlip = "flip".GetStableHashCode ();

        static readonly int HashedCellSize = "cellSize".GetStableHashCode ();

        /// <summary>
        /// Create "grid" node.
        /// </summary>
        /// <param name="node">Xml node.</param>
        /// <param name="container">markup container.</param>
        public static GameObject Create (XmlNode node, MarkupContainer container) {
            var go = new GameObject ("grid");
            var grid = go.AddComponent<GridLayoutGroup> ();
            var rt = go.GetComponent<RectTransform> ();

            grid.childAlignment = TextAnchor.MiddleCenter;
            var flipX = false;
            var flipY = false;
            var cellSize = Vector2.zero;

            var attrValue = node.GetAttribute (HashedFlip);
            if (!string.IsNullOrEmpty (attrValue)) {
                var parts = attrValue.Split (';');
                flipX = parts.Length > 0 && string.CompareOrdinal (parts[0], "true") == 0;
                flipY = parts.Length > 1 && string.CompareOrdinal (parts[1], "true") == 0;
            }

            float amount;
            attrValue = node.GetAttribute (HashedCellSize);
            if (!string.IsNullOrEmpty (attrValue)) {
                var parts = attrValue.Split (';');
                if (parts.Length > 0 && !string.IsNullOrEmpty (parts[0])) {
                    if (float.TryParse (parts[0], NumberStyles.Float, MathExtensions.UnifiedNumberFormat, out amount)) {
                        cellSize.x = amount;
                    }
                }
                if (parts.Length > 1 && !string.IsNullOrEmpty (parts[1])) {
                    if (float.TryParse (parts[1], NumberStyles.Float, MathExtensions.UnifiedNumberFormat, out amount)) {
                        cellSize.y = amount;
                    }
                }
            }

            grid.cellSize = cellSize;
            grid.startCorner = (GridLayoutGroup.Corner) ((flipX ? 1 : 0) | (flipY ? 2 : 0));

            MarkupUtils.SetRectTransformSize (rt, node);
            MarkupUtils.SetDisabled (rt, node);

            return go;
        }
    }
}