// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using System.Globalization;
using LeopotamGroup.Math;
using LeopotamGroup.Serialization;
using LeopotamGroup.SystemUi.Layouts;
using UnityEngine;

namespace LeopotamGroup.SystemUi.Markup.Generators {
    static class TableNode {
        static readonly int HashedItemsInRow = "itemsInRow".GetStableHashCode ();

        static readonly int HashedCellHeight = "cellHeight".GetStableHashCode ();

        /// <summary>
        /// Create "table" node. If children supported - GameObject container for them should be returned.
        /// </summary>
        /// <param name="widget">Ui widget.</param>
        /// <param name="node">Xml node.</param>
        /// <param name="container">Markup container.</param>
        public static RectTransform Create (RectTransform widget, XmlNode node, MarkupContainer container) {
#if UNITY_EDITOR
            widget.name = "table";
#endif
            var table = widget.gameObject.AddComponent<FixedTableLayout> ();
            var itemsInRow = 1;
            var cellHeight = 0f;

            var attrValue = node.GetAttribute (HashedItemsInRow);
            if (!string.IsNullOrEmpty (attrValue)) {
                int.TryParse (attrValue, out itemsInRow);
            }

            attrValue = node.GetAttribute (HashedCellHeight);
            if (!string.IsNullOrEmpty (attrValue)) {
                float.TryParse (attrValue, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out cellHeight);
            }

            table.ItemsInRow = itemsInRow;
            table.CellHeight = cellHeight;

            MarkupUtils.SetSize (widget, node);
            MarkupUtils.SetRotation (widget, node);
            MarkupUtils.SetOffset (widget, node);
            MarkupUtils.SetHidden (widget, node);

            return widget;
        }
    }
}