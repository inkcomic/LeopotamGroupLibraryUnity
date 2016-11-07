//-------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2016 Leopotam <leopotam@gmail.com>
//-------------------------------------------------------

using System;
using LeopotamGroup.Common;
using UnityEngine;

namespace LeopotamGroup.FX {
    /// <summary>
    /// Fade manager for Camera with tag "MainCamera".
    /// </summary>
    sealed class FadeManager : UnitySingleton<FadeManager> {
        /// <summary>
        /// Callback for extensions.
        /// </summary>
        public event Action<float> OnRender = delegate {};

        static Material _mtrl;

        bool _fadeAudio;

        float _opaque;

        float _startValue;

        float _endValue;

        float _fadeTime;

        float _time;

        Action _callback;

        int _cameraIndex;

        protected override void OnConstruct () {
            if (_mtrl == null) {
                _mtrl = new Material (Shader.Find ("Hidden/LeopotamGroup/FX/ScreenFade"));
                _mtrl.hideFlags = HideFlags.DontSave;
            }

            SetFade (0f);
        }

        void LateUpdate () {
            _cameraIndex = 0;
            if (_fadeTime <= 0f) {
                return;
            }
            _time = Mathf.Clamp01 (_time + Time.deltaTime / _fadeTime);

            _opaque = Mathf.Lerp (_startValue, _endValue, _time);
            if (_fadeAudio) {
                AudioListener.volume = 1f - _opaque;
            }

            if (_time >= 1f) {
                if (_opaque <= 0f) {
                    enabled = false;
                }
                if (_callback != null) {
                    var cb = _callback;
                    _callback = null;
                    cb ();
                }
            }
        }

        void OnRenderObject () {
            if (_opaque <= 0f) {
                return;
            }

            _cameraIndex++;
            if (_cameraIndex == Camera.allCamerasCount) {
                GL.PushMatrix ();
                GL.LoadOrtho ();
                _mtrl.SetPass (0);
                GL.Begin (GL.QUADS);
                GL.Color (Color.Lerp (Color.clear, Color.black, _opaque));
                GL.Vertex3 (0f, 0f, 0f);
                GL.Vertex3 (0f, 1f, 0f);
                GL.Vertex3 (1f, 1f, 0f);
                GL.Vertex3 (1f, 0f, 0f);
                GL.End ();
                GL.PopMatrix ();

                OnRender (_opaque);
            }
        }

        /// <summary>
        /// Set current fade status.
        /// </summary>
        /// <param name="opaque">Opaque value [0, 1], 0 - full transparent, 1 - full opaque.</param>
        /// <param name="fadeAudio">Fade audio sources too.</param>
        public void SetFade (float opaque, bool fadeAudio = false) {
            _opaque = Mathf.Clamp01 (opaque);
            if (fadeAudio) {
                AudioListener.volume = 1f - _opaque;
            }
            _fadeTime = 0f;
            enabled = _opaque > 0f;
        }

        /// <summary>
        /// Start fading to target fade status.
        /// </summary>
        /// <param name="toOpaque">Target opaque status.</param>
        /// <param name="time">Time of fading.</param>
        /// <param name="onSuccess">Optional callback on success ending of fading.</param>
        /// <param name="fadeAudio">Fade audio too.</param>
        public void StartFadeTo (float toOpaque, float time, Action onSuccess = null, bool fadeAudio = false) {
            _fadeAudio = fadeAudio;
            _startValue = _opaque;
            _endValue = toOpaque;
            _fadeTime = time;
            _callback = onSuccess;
            _time = 0f;
            enabled = _startValue != _endValue;
        }
    }
}