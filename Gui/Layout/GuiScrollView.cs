//-------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2016 Leopotam <leopotam@gmail.com>
//-------------------------------------------------------

using System.Collections.Generic;
using LeopotamGroup.Common;
using LeopotamGroup.Gui.Widgets;
using UnityEngine;

namespace LeopotamGroup.Gui.Layout {
    public class GuiScrollView : MonoBehaviourBase {
        static readonly List<GuiWidget> _widgetsBuf = new List<GuiWidget> ();

        Vector3 _min;

        Vector3 _max;

        Vector3 _size;

        GuiPanel _panel;

        void OnEnable () {
            ValidateBounds ();
            Validate ();
        }

        void ValidatePanel () {
            if (_panel == null) {
                _panel = GuiPanel.GetPanel (transform);
            }
        }

        bool ClipPosition (ref Vector3 pos) {
            if (!enabled) {
                return false;
            }
                
            if (_panel.ClipType != LeopotamGroup.Gui.Common.GuiPanelClipType.Range) {
                return false;
            }

            var worldClip = _panel.WorldClipRect;
            var tr = transform;
            var posOffset = pos - tr.position;
            var worldMin = tr.TransformPoint (_min) + posOffset;
            var worldMax = tr.TransformPoint (_max) + posOffset;
            var worldCenter = (worldMin + worldMax) * 0.5f;

            bool isClipped = false;
            float offset;
            // horizontal clip.
            if (_panel.ClipWidth < _size.x) {
                offset = worldClip.x - worldMin.x;
                if (offset < 0f) {
                    pos.x += offset;
                    isClipped = true;
                } else {
                    offset = worldClip.z - worldMax.x;
                    if (offset > 0f) {
                        pos.x += offset;
                        isClipped = true;
                    }
                }
            } else {
                // need center content.
                pos.x += (worldClip.x + worldClip.z) * 0.5f - worldCenter.x;
                isClipped = true;
            }

            // vertical clip.
            if (_panel.ClipHeight < _size.y) {
                offset = worldClip.y - worldMin.y;
                if (offset < 0f) {
                    pos.y += offset;
                    isClipped = true;
                } else {
                    offset = worldClip.w - worldMax.y;
                    if (offset > 0f) {
                        pos.y += offset;
                        isClipped = true;
                    }
                }
            } else {
                // need center content.
                pos.y += (worldClip.y + worldClip.w) * 0.5f - worldCenter.y;
                isClipped = true;
            }

            return isClipped;
        }

        /// <summary>
        /// Revalidate internal bounds cache. No reposition for new bounds!
        /// </summary>
        public void ValidateBounds () {
            var bounds = new Bounds ();
            gameObject.GetComponentsInChildren (false, _widgetsBuf);
            Transform tr;
            for (int i = _widgetsBuf.Count - 1; i >= 0; i--) {
                tr = _widgetsBuf[i].transform;
                bounds.Encapsulate (tr.TransformPoint (new Vector3 (-_widgetsBuf[i].Width * 0.5f, -_widgetsBuf[i].Height * 0.5f, 0f)));
                bounds.Encapsulate (tr.TransformPoint (new Vector3 (_widgetsBuf[i].Width * 0.5f, _widgetsBuf[i].Height * 0.5f, 0f)));
            }
            _widgetsBuf.Clear ();

            _min = transform.InverseTransformPoint (bounds.min);
            _max = transform.InverseTransformPoint (bounds.max);
            _size = _max - _min;
        }

        /// <summary>
        /// Validate view position for respect cached bounds. No force recalculation for bounds!
        /// </summary>
        public void Validate () {
            if (enabled) {
                var pos = transform.position;
                ValidatePanel ();
                if (ClipPosition (ref pos)) {
                    transform.position = pos;
                }
            }
        }

        /// <summary>
        /// Scroll view with relative offset.
        /// </summary>
        public void ScrollRelative (Vector2 delta) {
            ValidatePanel ();

            var pos = transform.position;
            pos.x += delta.x;
            pos.y += delta.y;
            ClipPosition (ref pos);
            transform.position = pos;
        }
    }
}