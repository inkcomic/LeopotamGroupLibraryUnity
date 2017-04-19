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

namespace LeopotamGroup.SystemUi.Markup {
    /// <summary>
    /// Default widgets.
    /// </summary>
    static class StandardGenerators {
        public static readonly int HashedSide = "side".GetStableHashCode ();

        public static readonly int HashedAlign = "align".GetStableHashCode ();

        public static readonly int HashedOffset = "offset".GetStableHashCode ();

        public static readonly int HashedSize = "size".GetStableHashCode ();

        public static readonly int HashedSrc = "src".GetStableHashCode ();

        public static readonly int HashedNativeSize = "nativeSize".GetStableHashCode ();

        public static readonly int HashedDisabled = "disabled".GetStableHashCode ();

        public static readonly int HashedOnClick = "onClick".GetStableHashCode ();

        public static readonly int HashedOnBeginDrag = "onBeginDrag".GetStableHashCode ();

        public static readonly int HashedOnDrag = "onDrag".GetStableHashCode ();

        public static readonly int HashedOnEndDrag = "onEndDrag".GetStableHashCode ();

        public static readonly int HashedOnScroll = "onScroll".GetStableHashCode ();

        public static readonly int HashedOnDrop = "onDrop".GetStableHashCode ();

        public static readonly int HashedOnPressRelease = "onPressRelease".GetStableHashCode ();

        public static readonly int HashedOnEnterExit = "onEnterExit".GetStableHashCode ();

        public static readonly int HashedOnSelection = "onSelection".GetStableHashCode ();

        /// <summary>
        /// Create "ui" node.
        /// </summary>
        /// <param name="node">Xml node.</param>
        /// <param name="container">markup container.</param>
        public static GameObject CreateUi (XmlNode node, MarkupContainer container) {
            var go = new GameObject ("ui");
            var canvas = go.AddComponent<Canvas> ();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.pixelPerfect = false;

            var scaler = go.AddComponent<CanvasScaler> ();
            var refData = node.GetAttribute ("base".GetStableHashCode ());
            if (refData != null) {
                var refWidth = 1024;
                var refHeight = 768;
                var refBalance = 1f;
                try {
                    var parts = refData.Split (';');
                    var w = int.Parse (parts[0]);
                    var h = int.Parse (parts[1]);
                    var b = Mathf.Clamp01 (float.Parse (parts[2], MathExtensions.UnifiedNumberFormat));
                    refWidth = w;
                    refHeight = h;
                    refBalance = b;
                } catch { }
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2 (refWidth, refHeight);
                scaler.matchWidthOrHeight = refBalance;
            }

            go.AddComponent<GraphicRaycaster> ();

            var es = GameObject.FindObjectOfType<EventSystem> ();
            if ((object) es == null) {
                es = new GameObject ("EventSystem").AddComponent<EventSystem> ();
                es.gameObject.AddComponent<StandaloneInputModule> ();
            }

            return go;
        }

        /// <summary>
        /// Create "box" node.
        /// </summary>
        /// <param name="node">Xml node.</param>
        /// <param name="container">markup container.</param>
        public static GameObject CreateBox (XmlNode node, MarkupContainer container) {
            var go = new GameObject ("box");
            var rt = go.AddComponent<RectTransform> ();
            SetRectTransformSize (rt, node);
            SetDisabled (rt, node);
            if (ValidateInteractive (rt, node)) {
                go.AddComponent<NonVisualWidget> ();
            }
            return go;
        }

        /// <summary>
        /// Create "align" node.
        /// </summary>
        /// <param name="node">Xml node.</param>
        /// <param name="container">markup container.</param>
        public static GameObject CreateAlign (XmlNode node, MarkupContainer container) {
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

            SetDisabled (rt, node);
            return go;
        }

        /// <summary>
        /// Create "image" node.
        /// </summary>
        /// <param name="node">Xml node.</param>
        /// <param name="container">markup container.</param>
        public static GameObject CreateImage (XmlNode node, MarkupContainer container) {
            var go = new GameObject ("image");
            var img = go.AddComponent<Image> ();
            var rt = go.GetComponent<RectTransform> ();

            img.raycastTarget = false;

            var attrValue = node.GetAttribute (HashedSrc);
            if (!string.IsNullOrEmpty (attrValue)) {
                img.sprite = container.GetAtlasSprite (attrValue);
            }

            attrValue = node.GetAttribute (HashedNativeSize);
            var ignoreSize = (object) img.sprite != null && string.CompareOrdinal (attrValue, "true") == 0;
            if (ignoreSize) {
                img.SetNativeSize ();
            }

            SetRectTransformSize (rt, node, ignoreSize);
            SetDisabled (rt, node);
            if (ValidateInteractive (rt, node)) {
                img.raycastTarget = true;
            }
            return go;
        }

        /// <summary>
        /// Create "grid" node.
        /// </summary>
        /// <param name="node">Xml node.</param>
        /// <param name="container">markup container.</param>
        public static GameObject CreateGrid (XmlNode node, MarkupContainer container) {
            var go = new GameObject ("grid");
            var grid = go.AddComponent<GridLayoutGroup> ();
            var rt = go.GetComponent<RectTransform> ();

            SetRectTransformSize (rt, node);
            SetDisabled (rt, node);
            return go;
        }

        /// <summary>
        /// Process "disable"-attribute of node.
        /// </summary>
        /// <param name="node">Xml node.</param>
        /// <param name="container">markup container.</param>
        public static void SetDisabled (RectTransform rt, XmlNode node) {
            var attrValue = node.GetAttribute (HashedDisabled);
            if (string.CompareOrdinal (attrValue, "true") == 0) {
                rt.gameObject.SetActive (false);
            }
        }

        /// <summary>
        /// Process interactive callbacks of node.
        /// </summary>
        /// <param name="node">Xml node.</param>
        /// <param name="container">markup container.</param>
        public static bool ValidateInteractive (RectTransform rt, XmlNode node) {
            var isInteractive = false;
            string attrValue;
            attrValue = node.GetAttribute (HashedOnClick);
            if (!string.IsNullOrEmpty (attrValue)) {
                rt.gameObject.AddComponent<UiClickAction> ().SetGroup (attrValue);
                isInteractive = true;
            }
            attrValue = node.GetAttribute (HashedOnDrag);
            if (!string.IsNullOrEmpty (attrValue)) {
                rt.gameObject.AddComponent<UiDragAction> ().SetGroup (attrValue);
                isInteractive = true;
            }
            attrValue = node.GetAttribute (HashedOnPressRelease);
            if (!string.IsNullOrEmpty (attrValue)) {
                rt.gameObject.AddComponent<UiPressReleaseAction> ().SetGroup (attrValue);
                isInteractive = true;
            }
            attrValue = node.GetAttribute (HashedOnScroll);
            if (!string.IsNullOrEmpty (attrValue)) {
                rt.gameObject.AddComponent<UiScrollAction> ().SetGroup (attrValue);
                isInteractive = true;
            }
            attrValue = node.GetAttribute (HashedOnDrop);
            if (!string.IsNullOrEmpty (attrValue)) {
                rt.gameObject.AddComponent<UiDropAction> ().SetGroup (attrValue);
                isInteractive = true;
            }
            attrValue = node.GetAttribute (HashedOnEnterExit);
            if (!string.IsNullOrEmpty (attrValue)) {
                rt.gameObject.AddComponent<UiEnterExitAction> ().SetGroup (attrValue);
                isInteractive = true;
            }
            attrValue = node.GetAttribute (HashedOnSelection);
            if (!string.IsNullOrEmpty (attrValue)) {
                rt.gameObject.AddComponent<UiSelectionAction> ().SetGroup (attrValue);
                isInteractive = true;
            }
            return isInteractive;
        }

        /// <summary>
        /// Process "offset" / "size" attributes of node.
        /// </summary>
        /// <param name="node">Xml node.</param>
        /// <param name="container">markup container.</param>
        public static void SetRectTransformSize (RectTransform rt, XmlNode node, bool ignoreSize = false) {
            var anchorMin = Vector2.zero;
            var anchorMax = Vector2.one;
            var offsetMin = Vector3.zero;
            var offsetMax = offsetMin;
            var point = Vector3.zero;
            string amountStr;
            float amount;
            string attrValue;

            if (!ignoreSize) {
                attrValue = node.GetAttribute ("size".GetStableHashCode ());
                if (!string.IsNullOrEmpty (attrValue)) {
                    int percentIdx;
                    var parts = attrValue.Split (';');
                    if (parts.Length > 0 && !string.IsNullOrEmpty (parts[0])) {
                        amountStr = parts[0];
                        percentIdx = amountStr.IndexOf ('%');
                        if (percentIdx != -1) {
                            // relative.
                            if (float.TryParse (
                                    amountStr.Substring (0, percentIdx),
                                    NumberStyles.Float,
                                    MathExtensions.UnifiedNumberFormat,
                                    out amount)) {
                                amount *= 0.01f * 0.5f;
                                anchorMin.x = 0.5f - amount;
                                anchorMax.x = 0.5f + amount;
                            }
                        } else {
                            // absolute.
                            if (float.TryParse (amountStr, NumberStyles.Float, MathExtensions.UnifiedNumberFormat, out amount)) {
                                amount *= 0.5f;
                                anchorMin.x = 0.5f;
                                anchorMax.x = 0.5f;
                                offsetMin.x = -amount;
                                offsetMax.x = amount;
                            }
                        }
                    }
                    if (parts.Length > 1 && !string.IsNullOrEmpty (parts[1])) {
                        amountStr = parts[1];
                        percentIdx = amountStr.IndexOf ('%');
                        if (percentIdx != -1) {
                            // relative.
                            if (float.TryParse (
                                    amountStr.Substring (0, percentIdx),
                                    NumberStyles.Float,
                                    MathExtensions.UnifiedNumberFormat,
                                    out amount)) {
                                amount *= 0.01f * 0.5f;
                                anchorMin.y = 0.5f - amount;
                                anchorMax.y = 0.5f + amount;
                            }
                        } else {
                            // absolute.
                            if (float.TryParse (amountStr, NumberStyles.Float, MathExtensions.UnifiedNumberFormat, out amount)) {
                                amount *= 0.5f;
                                anchorMin.y = 0.5f;
                                anchorMax.y = 0.5f;
                                offsetMin.y = -amount;
                                offsetMax.y = amount;
                            }
                        }
                    }
                }
            }

            attrValue = node.GetAttribute ("offset".GetStableHashCode ());
            if (!string.IsNullOrEmpty (attrValue)) {
                var parts = attrValue.Split (';');
                if (parts.Length > 0 && !string.IsNullOrEmpty (parts[0])) {
                    if (float.TryParse (parts[0], NumberStyles.Float, MathExtensions.UnifiedNumberFormat, out amount)) {
                        point.x = amount;
                    }
                }
                if (parts.Length > 1 && !string.IsNullOrEmpty (parts[1])) {
                    if (float.TryParse (parts[1], NumberStyles.Float, MathExtensions.UnifiedNumberFormat, out amount)) {
                        point.y = amount;
                    }
                }
            }

            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.offsetMin = offsetMin;
            rt.offsetMax = offsetMax;
            rt.localPosition = point;
        }
    }
}