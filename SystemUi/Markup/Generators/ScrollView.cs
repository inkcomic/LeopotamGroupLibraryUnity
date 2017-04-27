// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using System.Globalization;
using LeopotamGroup.Math;
using LeopotamGroup.Serialization;
using LeopotamGroup.SystemUi.Actions;
using LeopotamGroup.SystemUi.Widgets;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LeopotamGroup.SystemUi.Markup.Generators {
    static class ScrollViewNode {
        public const string ViewportName = "viewport";

        public const string ContentName = "content";

        static readonly int HashedContentSize = "contentSize".GetStableHashCode ();

        static readonly int HashedOnChange = "onChange".GetStableHashCode ();

        /// <summary>
        /// Create "scrollView" node. If children supported - GameObject container for them should be returned.
        /// </summary>
        /// <param name="widget">Ui widget.</param>
        /// <param name="node">Xml node.</param>
        /// <param name="container">Markup container.</param>
        public static RectTransform Create (RectTransform widget, XmlNode node, MarkupContainer container) {
#if UNITY_EDITOR
            widget.name = "scrollView";
#endif
            // GameObject go1;
            // Image img;
            RectTransform rt;
            float amount;
            string attrValue;

            var anchorMin = Vector2.zero;
            var anchorMax = Vector2.one;
            var offsetMin = Vector3.zero;
            var offsetMax = Vector3.zero;

            var scrollView = widget.gameObject.AddComponent<ScrollRect> ();
            var theme = MarkupUtils.GetTheme (node, container);

            // viewport.
            rt = MarkupUtils.CreateUiObject (ViewportName, widget);
            rt.gameObject.AddComponent<RectMask2D> ();
            rt.gameObject.AddComponent<NonVisualWidget> ().raycastTarget = false;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            scrollView.viewport = rt;

            // content.
            rt = MarkupUtils.CreateUiObject (ContentName, rt);
            attrValue = node.GetAttribute (HashedContentSize);
            if (!string.IsNullOrEmpty (attrValue)) {
                var parts = MarkupUtils.SplitAttrValue (attrValue);
                if (parts.Length > 0 && !string.IsNullOrEmpty (parts[0])) {
                    if (float.TryParse (parts[0], NumberStyles.Float, MathExtensions.UnifiedNumberFormat, out amount)) {
                        amount *= 0.5f;
                        offsetMin.x = -amount;
                        offsetMax.x = amount;
                        anchorMin.x = 0.5f;
                        anchorMax.x = 0.5f;
                    }
                }
                if (parts.Length > 1 && !string.IsNullOrEmpty (parts[1])) {
                    if (float.TryParse (parts[1], NumberStyles.Float, MathExtensions.UnifiedNumberFormat, out amount)) {
                        amount *= 0.5f;
                        offsetMin.y = -amount;
                        offsetMax.y = amount;
                        anchorMin.y = 0.5f;
                        anchorMax.y = 0.5f;
                    }
                }
            }
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.offsetMin = offsetMin;
            rt.offsetMax = offsetMax;
            scrollView.content = rt;

            // TODO: scrollbars.

            attrValue = node.GetAttribute (HashedOnChange);
            if (!string.IsNullOrEmpty (attrValue)) {
                widget.gameObject.AddComponent<NonVisualWidget> ();
                widget.gameObject.AddComponent<UiScrollViewAction> ().SetGroup (attrValue);
            }

            scrollView.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
            scrollView.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;

            MarkupUtils.SetSize (widget, node);
            MarkupUtils.SetRotation (widget, node);
            MarkupUtils.SetOffset (widget, node);
            MarkupUtils.SetHidden (widget, node);

            return rt;
        }
    }
}