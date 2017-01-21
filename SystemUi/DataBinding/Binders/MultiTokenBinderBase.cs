// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using LeopotamGroup.Common;
using System;
using UnityEngine;

namespace LeopotamGroup.SystemUi.DataBinding.Binders {
    public abstract class MultiTokenBinderBase : MonoBehaviour, IDataBinder {
        [SerializeField]
        string[] _tokens = null;

        [SerializeField]
        string _mask = "{0}";

        string[] _dataCache;

        void Start () {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty (_mask)) {
                throw new UnityException ("Mask cant be empty");
            }
#endif
            var isDirty = false;
            if (_tokens != null) {
                var storage = Singleton.Get<DataStorage> ();
                for (var i = 0; i < _tokens.Length; i++) {
#if UNITY_EDITOR
                    if (string.IsNullOrEmpty (_tokens[i])) {
                        throw new UnityException ("Token cant be null");
                    }
#endif
                    isDirty = true;
                    storage.Subscribe (_tokens[i], this);
                }

                if (isDirty) {
                    _dataCache = new string[_tokens.Length];
                    // get cached data.
                    object data;
                    for (var i = 0; i < _tokens.Length; i++) {
                        data = storage.GetData (_tokens[i]);
                        _dataCache[i] = data != null ? data.ToString () : string.Empty;
                    }
                }
            }
            enabled = isDirty;
        }

        void OnDestroy () {
            if (Singleton.IsTypeRegistered<DataStorage> ()) {
                if (_tokens != null) {
                    var storage = Singleton.Get<DataStorage> ();
                    for (var i = _tokens.Length - 1; i >= 0; i--) {
#if UNITY_EDITOR
                        if (string.IsNullOrEmpty (_tokens[i])) {
                            throw new UnityException ("Token cant be null");
                        }
#endif
                        storage.Unsubscribe (_tokens[i], this);
                    }
                }
            }
        }

        void LateUpdate () {
            UpdateData (string.CompareOrdinal (_mask, "{0}") == 0 ? _dataCache[0] : string.Format (_mask, _dataCache));
            enabled = false;
        }

        protected abstract void UpdateData (string data);

        public void OnDataChanged (string token, object data) {
            enabled = true;
            _dataCache[Array.IndexOf (_tokens, token)] = data.ToString ();
        }
    }
}