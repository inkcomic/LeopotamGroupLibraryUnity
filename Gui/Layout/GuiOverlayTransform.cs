
// -------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// -------------------------------------------------------

using LeopotamGroup.Common;
using LeopotamGroup.Gui.Common;
using UnityEngine;

namespace LeopotamGroup.Gui.Layout {
    /// <summary>
    /// GUI overlay helper for following to world transform.
    /// </summary>
    [ExecuteInEditMode]
    public sealed class GuiOverlayTransform : MonoBehaviourBase {
        /// <summary>
        /// 3D-world camera.
        /// </summary>
        public Camera WorldCamera = null;

        /// <summary>
        /// 3D-world target to follow.
        /// </summary>
        public Transform WorldTarget = null;

        void LateUpdate () {
            // cant cast WorldTarget to object - strange behaviour at editor.
            if (WorldTarget != null && (object) WorldCamera != null) {
                _cachedTransform.localPosition =
                    GuiSystem.Instance.GetOverlayPosition (WorldCamera, WorldTarget.position, _cachedTransform.parent);
            }
        }
    }
}