//-------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2016 Leopotam <leopotam@gmail.com>
//-------------------------------------------------------

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
            // cant cast WorldTarget to System.Object - strange behaviour at editor.
            if (WorldTarget != null && (System.Object) WorldCamera != null) {
                _cachedTransform.localPosition = GuiSystem.Instance.GetOverlayPosition (WorldCamera, WorldTarget.position, _cachedTransform.parent);
            }
        }
    }
}