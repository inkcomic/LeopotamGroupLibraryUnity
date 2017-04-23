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
    static class SliderNode {
        static readonly int HashedHandle = "handle".GetStableHashCode ();

        static readonly int HashedRtl = "rtl".GetStableHashCode ();

        static readonly int HashedRange = "range".GetStableHashCode ();

        static readonly int HashedValue = "value".GetStableHashCode ();

        static readonly int HashedOnChange = "onChange".GetStableHashCode ();

        /// <summary>
        /// Create "slider" node.
        /// </summary>
        /// <param name="go">Gameobject holder.</param>
        /// <param name="node">Xml node.</param>
        /// <param name="container">Markup container.</param>
        public static void Create (GameObject go, XmlNode node, MarkupContainer container) {
#if UNITY_EDITOR
            go.name = "slider";
#endif
            var slider = go.AddComponent<Slider> ();
            var theme = MarkupUtils.GetTheme (node, container);
            Image img;
            RectTransform rt;

            var direction = Slider.Direction.LeftToRight;
            var minValue = 0f;
            var maxValue = 1f;
            var useInts = false;
            var dataValue = 0f;
            var isInteractive = false;

            // background.
            img = new GameObject ().AddComponent<Image> ();
            rt = img.rectTransform;
            rt.gameObject.layer = go.layer;
            rt.SetParent (slider.transform, false);
            img.sprite = theme.GetSliderSprite (MarkupTheme.SliderState.Background);
            img.color = theme.GetSliderColor (MarkupTheme.SliderState.Background);
            img.type = Image.Type.Sliced;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;

            // foreground.
            img = new GameObject ().AddComponent<Image> ();
            rt = img.rectTransform;
            rt.gameObject.layer = go.layer;
            rt.SetParent (slider.transform, false);
            img.sprite = theme.GetSliderSprite (MarkupTheme.SliderState.Foreground);
            img.color = theme.GetSliderColor (MarkupTheme.SliderState.Foreground);
            img.type = Image.Type.Sliced;
            img.raycastTarget = false;
            rt.sizeDelta = Vector2.zero;
            slider.fillRect = rt;

            string attrValue;
            attrValue = node.GetAttribute (HashedHandle);
            var useHandle = string.CompareOrdinal (attrValue, "true") == 0;
            if (useHandle) {
                var handle = new GameObject ().AddComponent<RectTransform> ();
                handle.gameObject.layer = go.layer;
                handle.SetParent (slider.transform, false);
                slider.handleRect = handle;

                img = new GameObject ().AddComponent<Image> ();
                img.raycastTarget = false;
                img.gameObject.layer = go.layer;
                img.sprite = theme.GetSliderSprite (MarkupTheme.SliderState.Handle);
                img.color = theme.GetSliderColor (MarkupTheme.SliderState.Handle);
                img.type = Image.Type.Sliced;
                img.SetNativeSize ();
                img.transform.SetParent (handle, false);
                handle.sizeDelta = img.rectTransform.sizeDelta;
            }

            attrValue = node.GetAttribute (HashedRtl);
            if (string.CompareOrdinal (attrValue, "true") == 0) {
                direction = Slider.Direction.RightToLeft;
            }

            float amount;
            attrValue = node.GetAttribute (HashedRange);
            if (!string.IsNullOrEmpty (attrValue)) {
                var parts = attrValue.Split (';');
                if (parts.Length > 0 && !string.IsNullOrEmpty (parts[0])) {
                    if (float.TryParse (parts[0], NumberStyles.Float, MathExtensions.UnifiedNumberFormat, out amount)) {
                        minValue = amount;
                    }
                }
                if (parts.Length > 1 && !string.IsNullOrEmpty (parts[1])) {
                    if (float.TryParse (parts[1], NumberStyles.Float, MathExtensions.UnifiedNumberFormat, out amount)) {
                        maxValue = amount;
                    }
                }
                if (parts.Length > 2 && string.CompareOrdinal (parts[2], "true") == 0) {
                    useInts = true;
                }
            }

            attrValue = node.GetAttribute (HashedValue);
            if (!string.IsNullOrEmpty (attrValue)) {
                if (float.TryParse (attrValue, NumberStyles.Float, MathExtensions.UnifiedNumberFormat, out amount)) {
                    dataValue = amount;
                }
            }

            attrValue = node.GetAttribute (HashedOnChange);
            if (!string.IsNullOrEmpty (attrValue)) {
                go.gameObject.AddComponent<UiSliderAction> ().SetGroup (attrValue);
                isInteractive = true;
            }

            slider.minValue = minValue;
            slider.maxValue = maxValue;
            slider.wholeNumbers = useInts;
            slider.value = dataValue;
            slider.direction = direction;

            slider.transition = Selectable.Transition.None;

            MarkupUtils.SetSize (go, node);
            MarkupUtils.SetRotation (go, node);
            MarkupUtils.SetOffset (go, node);
            MarkupUtils.SetHidden (go, node);

            slider.interactable = useHandle && isInteractive;
        }
    }
}