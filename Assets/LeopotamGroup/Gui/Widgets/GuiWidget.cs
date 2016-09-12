//-------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2016 Leopotam <leopotam@gmail.com>
//-------------------------------------------------------

using System;
using LeopotamGroup.Common;
using LeopotamGroup.Gui.Common;
using LeopotamGroup.Gui.Layout;
using UnityEngine;

namespace LeopotamGroup.Gui.Widgets {
    /// <summary>
    /// Base gui widget.
    /// </summary>
    [ExecuteInEditMode]
    public abstract class GuiWidget : MonoBehaviourBase {
        /// <summary>
        /// Will be raised on each geometry update.
        /// </summary>
        public event Action<GuiWidget> OnGeometryUpdated = delegate {};

        /// <summary>
        /// Enable dirty state for specified types.
        /// </summary>
        /// <param name="changes">Specified dirty types.</param>
        public virtual void SetDirty (GuiDirtyType changes) {
            _dirtyState |= changes;
        }

        /// <summary>
        /// Color of widget.
        /// </summary>
        public Color Color {
            get { return _color; }
            set {
                if (value != _color) {
                    _color = value;
                    SetDirty (GuiDirtyType.Geometry);
                }
            }
        }

        /// <summary>
        /// Width of widget.
        /// </summary>
        public int Width {
            get { return _width; }
            set {
                if (value >= 0 && value != _width) {
                    _width = value;
                    SetDirty (GuiDirtyType.Geometry);
                }
            }
        }

        /// <summary>
        /// height of widget.
        /// </summary>
        public int Height {
            get { return _height; }
            set {
                if (value >= 0 && value != _height) {
                    _height = value;
                    SetDirty (GuiDirtyType.Geometry);
                }
            }
        }

        /// <summary>
        /// Order of widgets inside GuiPanel for proper sorting. Positive values brings panel closer to camera.
        /// </summary>
        public int Depth {
            get { return _depth; }
            set {
                if (value != _depth) {
                    _depth = value;
                    SetDirty (GuiDirtyType.Depth);
                }
            }
        }

        /// <summary>
        /// Is visual widget visible.
        /// </summary>
        public bool IsVisible { get { return _meshRenderer != null && _meshRenderer.enabled; } }

        /// <summary>
        /// Gets parent GuiPanel. If not exists - will be created at root GameObject.
        /// </summary>
        /// <value>The panel.</value>
        public GuiPanel Panel {
            get {
                if (_visualPanel == null) {
                    _visualPanel = GuiPanel.GetPanel (transform);
                    _visualPanel.OnChanged += OnPanelChanged;
                }
                return _visualPanel;
            }
        }

        /// <summary>
        /// Widget depth limit, in negative and positive directions.
        /// </summary>
        public const int DepthLimit = 49;

        const float DepthSlice = 0.5f;

        protected MeshRenderer _meshRenderer;

        GuiPanel _visualPanel;

        [HideInInspector]
        [SerializeField]
        int _width = 64;

        [HideInInspector]
        [SerializeField]
        int _height = 64;

        [HideInInspector]
        [SerializeField]
        Color _color = Color.white;

        [HideInInspector]
        [SerializeField]
        int _depth;

        GuiDirtyType _dirtyState;

        protected virtual void OnDisable () {
            if (_meshRenderer != null) {
                _meshRenderer.enabled = false;
            }
            ResetPanel ();
        }

        void OnPanelChanged (GuiPanel panel) {
            if (_visualPanel != panel) {
                ResetPanel ();
            } else {
                UpdateVisuals (GuiDirtyType.None);
            }
        }

        void LateUpdate () {
            var transformChanged = _cachedTransform.hasChanged;
            if (transformChanged || _dirtyState != GuiDirtyType.None) {
                if (transformChanged) {
                    _cachedTransform.hasChanged = false;
                }

                var changes = _dirtyState;
                _dirtyState = GuiDirtyType.None;
                UpdateVisuals (changes);

                // Panel clipping additional check.
                if (changes != GuiDirtyType.None) {
                    UpdateVisuals (GuiDirtyType.None);
                }
            }
        }

        /// <summary>
        /// Force reset cached parent panel reference.
        /// </summary>
        public void ResetPanel () {
            SetDirty (GuiDirtyType.Panel);
            if (_visualPanel) {
                _visualPanel.OnChanged -= OnPanelChanged;
            }
            _visualPanel = null;
        }

        /// <summary>
        /// Force revalidate visuals.
        /// </summary>
        /// <returns>true, if visuals were updated.</returns>
        /// <param name="changes">What should be revalidate.</param>
        public virtual bool UpdateVisuals (GuiDirtyType changes) {
            if (_meshRenderer == null) {
                return false;
            }

            // Special case for checking panel clipping.
            if (changes == GuiDirtyType.None && Panel.ClipType == GuiPanelClipType.Range) {
                var halfSize = new Vector3 (_width * 0.5f, _height * 0.5f, 0f);
                var min = _cachedTransform.TransformPoint (-halfSize);
                var max = _cachedTransform.TransformPoint (halfSize);
                if (!Panel.IsRectInside (min, max)) {
                    _meshRenderer.enabled = false;
                    return false;
                }
            }

            if (!_meshRenderer.enabled) {
                _meshRenderer.enabled = true;
            }

            if ((changes & GuiDirtyType.Depth) != GuiDirtyType.None) {
                var panelTrans = Panel.transform;
                var pos = panelTrans.InverseTransformPoint (_cachedTransform.position);
                pos.z = -DepthSlice * Depth;
                _cachedTransform.position = panelTrans.TransformPoint (pos);
            }

            if ((changes & GuiDirtyType.Geometry) != GuiDirtyType.None) {
                OnGeometryUpdated (this);
            }
            return true;
        }

        /// <summary>
        /// Bake transform scale to widget size.
        /// </summary>
        public virtual void BakeScale () {
            var scale = transform.localScale;
            if (scale.x < 0f) {
                scale.x = -scale.x;
            }
            if (scale.y < 0f) {
                scale.y = -scale.y;
            }
            Width = (int) (Width * scale.x);
            Height = (int) (Height * scale.y);
            transform.localScale = Vector3.one;
        }
    }
}