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
    static class UiNode {
        /// <summary>
        /// Create "ui" node.
        /// </summary>
        /// <param name="node">Xml node.</param>
        /// <param name="container">markup container.</param>
        public static GameObject Create (XmlNode node, MarkupContainer container) {
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
    }
}