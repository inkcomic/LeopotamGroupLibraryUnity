//-------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2016 Leopotam <leopotam@gmail.com>
//-------------------------------------------------------

using LeopotamGroup.Gui.Widgets;
using LeopotamGroup.Tweening;
using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LeopotamGroup.Gui.Tweeners {
    [RequireComponent (typeof (GuiSprite))]
    public class GuiTweenSpriteName : TweeningBase {
        /// <summary>
        /// Target GuiSprite. If null on start - GuiSprite on current gameobject will be used.
        /// </summary>
        public GuiSprite Target = null;

        /// <summary>
        /// Name mask. Regular expression.
        /// </summary>
        public string NameMask = ".*";

        /// <summary>
        /// Reset sprite size on each frame.
        /// </summary>
        public bool ResetSize = false;

        readonly List<string> _filteredNames = new List<string> (8);

        float _nextFrameTime;

        float _frameTime;

        protected override void OnInit () {
            if (Target == null) {
                Target = GetComponent <GuiSprite> ();
            }
            if (Target == null) {
                Destroy (this);
                return;
            }
            ResetNames ();
            _nextFrameTime = -1f;
            _frameTime = _filteredNames.Count > 0 ? 0.5f / (float) _filteredNames.Count : 0f;
        }

        protected override void OnUpdateValue () {
            if (Target != null && _frameTime > 0) {
                if (CurrentTimeRaw >= _nextFrameTime) {
                    var newName = _filteredNames[Mathf.RoundToInt (Mathf.Lerp (0, _filteredNames.Count - 1, Value))];
                    Target.SpriteName = newName;
                    if (ResetSize) {
                        Target.ResetSize ();
                    }
                    _nextFrameTime = CurrentTimeRaw + _frameTime;
                }
            }
        }

        public void ResetNames () {
            _filteredNames.Clear ();
            if (Target.SpriteAtlas != null) {
                var spriteNames = Target.SpriteAtlas.GetSpriteNames ();
                for (int i = 0, iMax = spriteNames.Length; i < iMax; i++) {
                    if (Regex.IsMatch (spriteNames[i], NameMask)) {
                        _filteredNames.Add (spriteNames[i]);
                    }
                }
            } else {
                enabled = false;
            }
        }

        /// <summary>
        /// Begin tweening.
        /// </summary>
        /// <param name="nameMask">Sprite names mask.</param>
        /// <param name="resetSize">Reset sprite size on each frame.</param>
        /// <param name="time">Time for tweening.</param>
        public GuiTweenSpriteName Begin (string nameMask, bool resetSize, float time) {
            enabled = false;
            if (NameMask != nameMask) {
                NameMask = nameMask;
                ResetNames ();
            }
            TweenTime = time;
            ResetSize = resetSize;
            enabled = true;
            return this;
        }

        /// <summary>
        /// Begin tweening at specified GameObject.
        /// </summary>
        /// <param name="go">Holder of tweener.</param>
        /// <param name="nameMask">Sprite names mask.</param>
        /// <param name="resetSize">Reset sprite size on each frame.</param>
        /// <param name="time">Time for tweening.</param>
        public static GuiTweenSpriteName Begin (GameObject go, string nameMask, bool resetSize, float time) {
            var tweener = Get<GuiTweenSpriteName> (go);
            if (tweener != null) {
                tweener.Begin (nameMask, resetSize, time);
            }
            return tweener;
        }
    }
}