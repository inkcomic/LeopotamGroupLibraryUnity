//-------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2016 Leopotam <leopotam@gmail.com>
//-------------------------------------------------------

using LeopotamGroup.Gui.Common;
using LeopotamGroup.Gui.Widgets;
using UnityEngine;

namespace LeopotamGroup.Gui.Layout {
    [ExecuteInEditMode]
    [RequireComponent (typeof (GuiEventReceiver))]
    public sealed class GuiBindEventReceiverSize : MonoBehaviour {
        /// <summary>
        /// Binding will be done only once, then component will be disabled.
        /// </summary>
        public bool Once = true;

        /// <summary>
        /// Target GuiWidget to binding. If null - screen borders will be used.
        /// </summary>
        public GuiWidget Target = null;

        /// <summary>
        /// Horizontal multiplier.
        /// </summary>
        [Range (0f, 4f)]
        public float Horizontal = 1f;

        /// <summary>
        /// Vertical multiplier.
        /// </summary>
        [Range (0f, 4f)]
        public float Vertical = 1f;

        GuiEventReceiver _receiver;

        void OnEnable () {
            _receiver = GetComponent <GuiEventReceiver> ();
            Validate ();
        }

        void LateUpdate () {
            Validate ();
            if (Once && Application.isPlaying) {
                enabled = false;
            }
        }

        /// <summary>
        /// Force revalidate size.
        /// </summary>
        public void Validate () {
            Horizontal = Mathf.Clamp (Horizontal, 0f, 4f);
            Vertical = Mathf.Clamp (Vertical, 0f, 4f);
            if (Target != null) {
                _receiver.Width = (int) (Target.Width * Horizontal);
                _receiver.Height = (int) (Target.Height * Vertical);
            } else {
                _receiver.Width = (int) (GuiSystem.Instance.ScreenHeight * GuiSystem.Instance.Camera.aspect * Horizontal);
                _receiver.Height = (int) (GuiSystem.Instance.ScreenHeight * Vertical);
            }
        }
    }
}