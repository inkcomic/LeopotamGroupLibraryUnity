// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

namespace LeopotamGroup.SystemUi.DataBinding.Binders {
    [RequireComponent (typeof (Toggle))]
    public sealed class DataBindToggle : AbstractBinderBase {
        Toggle _target;

        public override void OnDataChanged (string token, object data) {
            if ((object) _target == null) {
                _target = GetComponent<Toggle> ();
            }
            _target.isOn = GetValueAsBool (data);
        }
    }
}