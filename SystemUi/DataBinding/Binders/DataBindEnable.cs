// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using UnityEngine;

namespace LeopotamGroup.SystemUi.DataBinding.Binders {
    public sealed class DataBindEnable : AbstractBinderBase {
        /// <summary>
        /// Target component for enable / disable. If null - current gameObject will be used.
        /// </summary>
        [SerializeField]
        Behaviour _target = null;

        protected override bool ProcessEventsOnlyWhenEnabled { get { return false; } }

        public override void OnDataChanged (string token, object data) {
            var state = GetValueAsBool (data);
            if (_target != null) {
                _target.enabled = state;
            } else {
                gameObject.SetActive (state);
            }
        }
    }
}