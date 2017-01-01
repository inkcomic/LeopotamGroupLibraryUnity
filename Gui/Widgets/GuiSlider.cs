
// -------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// -------------------------------------------------------

using LeopotamGroup.Gui.Common;
using UnityEngine;

namespace LeopotamGroup.Gui.Widgets {
    /// <summary>
    /// Slider widget.
    /// </summary>
    public sealed class GuiSlider : MonoBehaviour {
        /// <summary>
        /// Normalized value of slider [0-1].
        /// </summary>
        /// <value>The value.</value>
        public float Value {
            get { return _value; }
            set {
                value = Mathf.Clamp01 (value);
                if (_value != value) {
                    _value = value;
                    _needUpdate = true;
                }
            }
        }

        /// <summary>
        /// Optional background sprite of slider.
        /// </summary>
        /// <value>The background.</value>
        public GuiSprite Background {
            get { return _background; }
            set {
                if (_background != value) {
                    if (Application.isPlaying) {
                        OnDisable ();
                    }
                    _background = value;
                    if (Application.isPlaying) {
                        OnEnable ();
                    }
                    _needUpdate = true;
                }
            }
        }

        /// <summary>
        /// Optional foreground sprite of slider.
        /// </summary>
        /// <value>The foreground.</value>
        public GuiSprite Foreground {
            get { return _foreground; }
            set {
                if (_foreground != value) {
                    _foreground = value;
                    _needUpdate = true;
                }
            }
        }

        [SerializeField]
        GuiSprite _background;

        [SerializeField]
        GuiSprite _foreground;

        [HideInInspector]
        [SerializeField]
        float _value;

        bool _needUpdate;

        void OnEnable () {
            if (_background != null) {
                var rcv = _background.GetComponent<GuiEventReceiver> ();
                if (rcv != null) {
                    rcv.OnPress.AddListener (OnPress);
                    rcv.OnDrag.AddListener (OnDrag);
                }
            }

            _needUpdate = true;
        }

        void OnDisable () {
            if (_background != null) {
                var rcv = _background.GetComponent<GuiEventReceiver> ();
                if (rcv != null) {
                    rcv.OnPress.RemoveListener (OnPress);
                    rcv.OnDrag.RemoveListener (OnDrag);
                }
            }
        }

        void OnPress (GuiEventReceiver rcv, GuiTouchEventArg args) {
            if (args.State) {
                OnDrag (rcv, args);
            }
        }

        void OnDrag (GuiEventReceiver rcv, GuiTouchEventArg args) {
            var offset = _background.transform.InverseTransformPoint (args.WorldPosition).x / (float) _background.Width;
            Value = Mathf.Clamp (offset, -0.5f, 0.5f) + 0.5f;
        }

        void LateUpdate () {
            if (_needUpdate) {
                _needUpdate = false;
                UpdateVisuals ();
            }
        }

        /// <summary>
        /// Force revalidate visuals.
        /// </summary>
        public void UpdateVisuals () {
            if (_background != null && _foreground) {
                _foreground.Width = Mathf.RoundToInt (_background.Width * _value);
                _foreground.UpdateVisuals (GuiDirtyType.Geometry);
                var p = _foreground.transform.parent;
                var pos = new Vector3 ((_value - _background.Width * (1f - _value)) * 0.5f, 0f, 0f);
                if (p != null) {
                    pos = p.InverseTransformPoint (_background.transform.TransformPoint (pos));
                }
                pos.z = _foreground.transform.localPosition.z;
                _foreground.transform.localPosition = pos;
            }
        }
    }
}