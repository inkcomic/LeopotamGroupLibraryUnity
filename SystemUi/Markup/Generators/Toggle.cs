// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using LeopotamGroup.Math;
using LeopotamGroup.Serialization;
using LeopotamGroup.SystemUi.Actions;
using LeopotamGroup.SystemUi.Widgets;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LeopotamGroup.SystemUi.Markup.Generators {
    static class ToggleNode {
        public const string BackgroundImageName = "background";

        public const string ForegroundImageName = "foreground";

        public const string ContentName = "content";

        static readonly int HashedGroup = "group".GetStableHashCode ();

        static readonly int HashedCheck = "check".GetStableHashCode ();

        static readonly int HashedOnChange = "onChange".GetStableHashCode ();

        /// <summary>
        /// Create "toggle" node. If children supported - GameObject container for them should be returned.
        /// </summary>
        /// <param name="go">Gameobject holder.</param>
        /// <param name="node">Xml node.</param>
        /// <param name="container">Markup container.</param>
        public static GameObject Create (GameObject go, XmlNode node, MarkupContainer container) {
#if UNITY_EDITOR
            go.name = "toggle";
#endif
            GameObject go1;
            Image img;
            RectTransform rt;
            Vector2 size;
            string attrValue;

            var toggle = go.AddComponent<Toggle> ();
            var theme = MarkupUtils.GetTheme (node, container);

            var isInteractive = false;

            // background.
            go1 = new GameObject (BackgroundImageName);
            go1.layer = go.layer;
            go1.hideFlags = HideFlags.DontSave;
            img = go1.AddComponent<Image> ();
            rt = img.rectTransform;
            rt.SetParent (go.transform, false);
            img.sprite = theme.GetToggleSprite (MarkupTheme.ToggleState.Background);
            img.type = Image.Type.Sliced;
            img.raycastTarget = false;
            size = theme.GetToggleSize (MarkupTheme.ToggleState.Background);
            rt.anchorMin = new Vector2 (0f, 0.5f);
            rt.anchorMax = new Vector2 (0f, 0.5f);
            rt.offsetMin = new Vector2 (0f, -size.y * 0.5f);
            rt.offsetMax = new Vector2 (size.x, size.y * 0.5f);
            toggle.targetGraphic = img;

            // foreground.
            go1 = new GameObject (ForegroundImageName);
            go1.layer = go.layer;
            go1.hideFlags = HideFlags.DontSave;
            go1.transform.SetParent (rt, false);
            img = go1.AddComponent<Image> ();
            rt = img.rectTransform;
            rt.anchorMin = new Vector2 (0.5f, 0.5f);
            rt.anchorMax = new Vector2 (0.5f, 0.5f);
            rt.sizeDelta = theme.GetToggleSize (MarkupTheme.ToggleState.Foreground);
            img.sprite = theme.GetToggleSprite (MarkupTheme.ToggleState.Foreground);
            img.type = Image.Type.Sliced;
            img.raycastTarget = false;
            toggle.graphic = img;

            // content.
            go1 = new GameObject (ContentName);
            go1.layer = go.layer;
            go1.hideFlags = HideFlags.DontSave;
            rt = go1.AddComponent<RectTransform> ();
            rt.SetParent (go.transform, false);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.right * size.x;
            rt.offsetMax = Vector2.zero;

            attrValue = node.GetAttribute (HashedGroup);
            if (!string.IsNullOrEmpty (attrValue)) {
                var groupGO = container.GetNamedNode (attrValue);
                if ((object) groupGO != null) {
                    toggle.group = groupGO.GetComponent<ToggleGroup> ();
                }
            }

            attrValue = node.GetAttribute (HashedCheck);
            if (string.CompareOrdinal (attrValue, "true") == 0) {
                toggle.isOn = true;
            }

            attrValue = node.GetAttribute (HashedOnChange);
            if (!string.IsNullOrEmpty (attrValue)) {
                go.AddComponent<NonVisualWidget> ();
                go.gameObject.AddComponent<UiToggleAction> ().SetGroup (attrValue);
                isInteractive = true;
            }

            toggle.transition = Selectable.Transition.None;
            toggle.interactable = isInteractive;

            MarkupUtils.SetSize (go, node);
            MarkupUtils.SetRotation (go, node);
            MarkupUtils.SetOffset (go, node);
            MarkupUtils.SetHidden (go, node);

            return go1;
        }
    }
}