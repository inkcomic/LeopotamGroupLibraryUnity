// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using LeopotamGroup.Common;
using LeopotamGroup.Math;
using LeopotamGroup.Serialization;
using LeopotamGroup.SystemUi.Widgets;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LeopotamGroup.SystemUi.Markup.Generators {
    static class ButtonNode {
        static readonly int HashedImage = "image".GetStableHashCode ();

        static readonly int HashedNormalColor = "normalColor".GetStableHashCode ();

        static readonly int HashedPressedColor = "pressedColor".GetStableHashCode ();

        static readonly int HashedFocusedColor = "focusedColor".GetStableHashCode ();

        static readonly int HashedDisabledColor = "disabledColor".GetStableHashCode ();

        static readonly int HashedDisabled = "disabled".GetStableHashCode ();

        /// <summary>
        /// Create "button" node.
        /// </summary>
        /// <param name="go">Gameobject holder.</param>
        /// <param name="node">Xml node.</param>
        /// <param name="container">markup container.</param>
        public static void Create (GameObject go, XmlNode node, MarkupContainer container) {
#if UNITY_EDITOR
            go.name = "button";
#endif
            var btn = go.AddComponent<Button> ();
            var img = go.AddComponent<Image> ();
            string attrValue;
            var transition = Button.Transition.ColorTint;

            attrValue = node.GetAttribute (HashedImage);
            if (!string.IsNullOrEmpty (attrValue)) {
                img.sprite = container.GetAtlasSprite (attrValue);
            }

            if (transition == Button.Transition.ColorTint) {
                var colors = btn.colors;
                attrValue = node.GetAttribute (HashedNormalColor);
                if (!string.IsNullOrEmpty (attrValue)) {
                    colors.normalColor = attrValue.Length >= 8 ? attrValue.ToColor32 () : attrValue.ToColor24 ();
                }
                attrValue = node.GetAttribute (HashedPressedColor);
                if (!string.IsNullOrEmpty (attrValue)) {
                    colors.pressedColor = attrValue.Length >= 8 ? attrValue.ToColor32 () : attrValue.ToColor24 ();
                }
                attrValue = node.GetAttribute (HashedFocusedColor);
                if (!string.IsNullOrEmpty (attrValue)) {
                    colors.highlightedColor = attrValue.Length >= 8 ? attrValue.ToColor32 () : attrValue.ToColor24 ();
                }
                attrValue = node.GetAttribute (HashedDisabledColor);
                if (!string.IsNullOrEmpty (attrValue)) {
                    colors.disabledColor = attrValue.Length >= 8 ? attrValue.ToColor32 () : attrValue.ToColor24 ();
                }
                btn.colors = colors;
            }

            btn.targetGraphic = img;
            btn.transition = transition;

            MarkupUtils.SetSize (go, node);
            MarkupUtils.SetOffset (go, node);
            MarkupUtils.SetHidden (go, node);

            attrValue = node.GetAttribute (HashedDisabled);
            var disabled = string.CompareOrdinal (attrValue, "true") == 0;

            btn.interactable = !disabled && MarkupUtils.ValidateInteractive (go, node);
        }
    }
}