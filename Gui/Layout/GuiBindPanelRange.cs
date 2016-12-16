
// -------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2016 Leopotam <leopotam@gmail.com>
// -------------------------------------------------------

using LeopotamGroup.Gui.Common;
using UnityEngine;

namespace LeopotamGroup.Gui.Layout {
    [ExecuteInEditMode]
    [RequireComponent (typeof (GuiPanel))]
    public sealed class GuiBindPanelRange : MonoBehaviour {
        /// <summary>
        /// Binding will be done only once, then component will be disabled.
        /// </summary>
        public bool Once = true;

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

        GuiPanel _panel;

        void OnEnable () {
            _panel = GetComponent<GuiPanel> ();
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
            _panel.ClipWidth = (int) (GuiSystem.Instance.ScreenHeight * GuiSystem.Instance.Camera.aspect * Horizontal);
            _panel.ClipHeight = (int) (GuiSystem.Instance.ScreenHeight * Vertical);
        }
    }
}