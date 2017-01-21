// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using UnityEngine;

namespace LeopotamGroup.SystemUi.DataBinding.Binders {
    public sealed class DataBindEnable : SingleTokenBinderBase {
        /// <summary>
        /// Target component for enable / disable. If null - current gameObject will be used.
        /// </summary>
        [SerializeField]
        Behaviour _target = null;

        public override void OnDataChanged (string token, object data) {
            var state = data is bool ? (bool) data : false;
            if (_target != null) {
                _target.enabled = state;
            } else {
                gameObject.SetActive (state);
            }
        }
    }
}