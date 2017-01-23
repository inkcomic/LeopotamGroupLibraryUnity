// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

// ReSharper disable RedundantCast.0

namespace LeopotamGroup.SystemUi.DataBinding.Binders {
    [RequireComponent (typeof (Text))]
    public sealed class DataBindText : AbstractBinderBase {
        Text _target;

        public override void OnDataChanged (string token, object data) {
            if ((object) _target == null) {
                _target = GetComponent<Text> ();
            }
            _target.text = GetValueAsString (data);
        }
    }
}