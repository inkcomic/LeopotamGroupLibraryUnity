// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

namespace LeopotamGroup.SystemUi.DataBinding.Binders {
    [RequireComponent (typeof (Text))]
    public sealed class DataBindText : MultiTokenBinderBase {
        Text _target;
        protected override void UpdateData (string data) {
            if ((object) _target == null) {
                _target = GetComponent<Text> ();
            }
            _target.text = data;
        }
    }
}