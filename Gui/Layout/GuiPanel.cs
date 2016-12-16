
// -------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2016 Leopotam <leopotam@gmail.com>
// -------------------------------------------------------

using LeopotamGroup.Common;
using LeopotamGroup.Gui.Common;
using LeopotamGroup.Gui.Widgets;
using System.Collections.Generic;
using UnityEngine;

namespace LeopotamGroup.Gui.Layout {
    /// <summary>
    /// Container for holding GuiWidget-s.
    /// </summary>
    [ExecuteInEditMode]
    public sealed class GuiPanel : MonoBehaviourBase {
        /// <summary>
        /// Clipping type of children. Not realized yet.
        /// </summary>
        public GuiPanelClipType ClipType {
            get { return _clipType; }
            set {
                if (value != _clipType) {
                    _clipType = value;
                    _isChanged = true;
                }
            }
        }

        /// <summary>
        /// Width of widget.
        /// </summary>
        public int ClipWidth {
            get { return _clipWidth; }
            set {
                if (value >= 0 && value != _clipWidth) {
                    _clipWidth = value;
                    _isChanged = true;
                }
            }
        }

        /// <summary>
        /// height of widget.
        /// </summary>
        public int ClipHeight {
            get { return _clipHeight; }
            set {
                if (value >= 0 && value != _clipHeight) {
                    _clipHeight = value;
                    _isChanged = true;
                }
            }
        }

        /// <summary>
        /// Order of panel for proper sorting. Positive values brings panel closer to camera.
        /// </summary>
        /// <value>The depth.</value>
        public int Depth {
            get { return _depth; }
            set {
                if (value != _depth) {
                    _depth = value;
                    _isChanged = true;
                }
            }
        }

        /// <summary>
        /// Clip bounds in world space (xMin, yMin, xMax, yMax).
        /// </summary>
        public Vector4 WorldClipRect { get; private set; }

        const float PanelDepthSlice = 50f;

        const string OpaqueShaderName = "Hidden/LeopotamGroup/Gui/Opaque";

        const string StandardShaderName = "Hidden/LeopotamGroup/Gui/Standard";

        const string FontShaderName = "Hidden/LeopotamGroup/Gui/Font";

        [HideInInspector]
        [SerializeField]
        int _depth;

        [HideInInspector]
        [SerializeField]
        GuiPanelClipType _clipType = GuiPanelClipType.None;

        [HideInInspector]
        [SerializeField]
        int _clipWidth = 100;

        [HideInInspector]
        [SerializeField]
        int _clipHeight = 100;

        readonly Dictionary<GuiAtlas, Material> _mtrlCache = new Dictionary<GuiAtlas, Material> ();

        readonly Dictionary<Font, Material> _fontCache = new Dictionary<Font, Material> ();

        readonly List<GuiWidget> _onChangeListeners = new List<GuiWidget> (32);

        readonly List<GuiWidget> _onChangedListenersCallingList = new List<GuiWidget> (32);

        bool _isChanged;

        void OnEnable () {
            InvalidateClipData (transform.position);
            _isChanged = true;
        }

        void OnDisable () {
            RaiseOnChanged (null);
#if UNITY_EDITOR
            if (_onChangeListeners.Count > 0) {
                Debug.LogWarning ("GuiPanel: memory leak for unsubscribed widgets", this.gameObject);
            }
#endif
        }

        void RaiseOnChanged (GuiPanel panel) {
            _onChangedListenersCallingList.Clear ();
            _onChangedListenersCallingList.AddRange (_onChangeListeners);
            for (var i = _onChangedListenersCallingList.Count - 1; i >= 0; i--) {
                _onChangeListeners[i].ValidatePanel (panel);
            }
            _onChangedListenersCallingList.Clear ();
        }

        void InvalidateClipData (Vector3 worldPos) {
            var halfSize = new Vector3 (_clipWidth * 0.5f, _clipHeight * 0.5f, 0f);
            var min = worldPos - halfSize;
            var max = worldPos + halfSize;
            WorldClipRect = new Vector4 (min.x, min.y, max.x, max.y);
        }

        void LateUpdate () {
            if (_clipType == GuiPanelClipType.Range && _cachedTransform.hasChanged) {
                _isChanged = true;
                _cachedTransform.hasChanged = false;
            }
            if (_isChanged) {
                _isChanged = false;
                UpdateVisuals ();
            }
        }

        void UpdateMaterial (Material mtrl) {
            switch (_clipType) {
            case GuiPanelClipType.Range:
                mtrl.EnableKeyword (GuiConsts.ShaderKeyWordClipRange);
                mtrl.SetVector (GuiConsts.ShaderParamClipData, WorldClipRect);
                break;
            default:
                mtrl.DisableKeyword (GuiConsts.ShaderKeyWordClipRange);
                break;
            }
        }

        /// <summary>
        /// Add OnChange event listener.
        /// </summary>
        /// <param name="listener">Listener.</param>
        public void AddOnChangeListener (GuiWidget listener) {
            if ((object) listener != null && !_onChangeListeners.Contains (listener)) {
                _onChangeListeners.Add (listener);
            }
        }

        /// <summary>
        /// Remove OnChange event listener.
        /// </summary>
        /// <param name="listener">Listener.</param>
        public void RemoveOnChangeListener (GuiWidget listener) {
            if ((object) listener != null) {
                var idx = _onChangeListeners.IndexOf (listener);
                if (idx != -1) {
                    _onChangeListeners.RemoveAt (idx);
                }
            }
        }

        /// <summary>
        /// Is (X,Y) point inside panel clipping range. Will return true if no clipping.
        /// </summary>
        /// <param name="x">Point X coordinate.</param>
        /// <param name="y">Point y coordinate.</param>
        public bool IsPointInside (float x, float y) {
            if (_clipType == GuiPanelClipType.None) {
                return true;
            }
            return x <= WorldClipRect.z && x >= WorldClipRect.x && y <= WorldClipRect.w && y >= WorldClipRect.y;
        }

        /// <summary>
        /// Is (min,max) rect inside panel clipping range. Will return true if no clipping.
        /// </summary>
        /// <param name="min">Left bottom corner of rect.</param>
        /// <param name="max">Right top corner of rect.</param>
        public bool IsRectInside (Vector2 min, Vector2 max) {
            if (_clipType == GuiPanelClipType.None) {
                return true;
            }
            return min.x <= WorldClipRect.z && max.x >= WorldClipRect.x && min.y <= WorldClipRect.w && max.y >= WorldClipRect.y;
        }

        /// <summary>
        /// Force revalidate internal atlases, fonts and children.
        /// </summary>
        public void UpdateVisuals () {
            var pos = GuiSystem.Instance.CameraTransform.InverseTransformPoint (_cachedTransform.position);
            pos.z = -_depth * PanelDepthSlice;
            var worldPos = GuiSystem.Instance.CameraTransform.TransformPoint (pos);
            _cachedTransform.position = worldPos;

            InvalidateClipData (worldPos);

            foreach (var texPair in _mtrlCache) {
                UpdateMaterial (texPair.Value);
            }
            foreach (var texPair in _fontCache) {
                UpdateMaterial (texPair.Value);
            }
            RaiseOnChanged (this);
        }

        /// <summary>
        /// Reset materials cache. All dependent widgets should be invalidate manually!
        /// </summary>
        public void ResetMaterialCache () {
            foreach (var item in _mtrlCache) {
                DestroyImmediate (item.Value);
            }
            _mtrlCache.Clear ();
        }

        /// <summary>
        /// Get cached material for GuiAtlas.
        /// </summary>
        /// <param name="atlas">GuiAtlas instance.</param>
        public Material GetMaterial (GuiAtlas atlas) {
            if (atlas == null) {
                return null;
            }
            Material mtrl;
            if (!_mtrlCache.ContainsKey (atlas)) {
                mtrl = new Material (Shader.Find (atlas.AlphaTexture != null ? StandardShaderName : OpaqueShaderName));
                mtrl.mainTexture = atlas.ColorTexture;
                mtrl.SetTexture ("_AlphaTex", atlas.AlphaTexture);
                mtrl.hideFlags = HideFlags.DontSave | HideFlags.HideInInspector;
                _mtrlCache[atlas] = mtrl;
            } else {
                mtrl = _mtrlCache[atlas];
            }
            UpdateMaterial (mtrl);
            return mtrl;
        }

        /// <summary>
        /// Get cached font material.
        /// </summary>
        /// <param name="font">Font instance.</param>
        public Material GetFontMaterial (Font font) {
            if (font == null) {
                return null;
            }
            Material mtrl;
            if (!_fontCache.ContainsKey (font)) {
                mtrl = new Material (Shader.Find (FontShaderName));
                mtrl.mainTexture = font.material.mainTexture;
                mtrl.hideFlags = HideFlags.DontSave | HideFlags.HideInInspector;
                _fontCache[font] = mtrl;
            } else {
                mtrl = _fontCache[font];
            }
            UpdateMaterial (mtrl);
            return mtrl;
        }

        /// <summary>
        /// Get closest parent panel for transform. if not found - new panel will be created at root GameObject.
        /// </summary>
        /// <returns>The panel.</returns>
        /// <param name="obj">Widget.</param>
        public static GuiPanel GetPanel (Transform obj) {
            if (obj == null) {
                return null;
            }
            var panel = obj.GetComponentInParent<GuiPanel> ();
            if (panel == null) {
                panel = obj.transform.root.gameObject.AddComponent<GuiPanel> ();
            }
            return panel;
        }

        public delegate void OnChangeHandler (GuiPanel panel);
    }
}