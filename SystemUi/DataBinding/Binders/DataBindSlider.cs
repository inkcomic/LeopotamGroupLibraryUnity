// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

// ReSharper disable RedundantCast.0

namespace LeopotamGroup.SystemUi.DataBinding.Binders {
    /// <summary>
    /// Data binding of progress value for any Slider component.
    /// </summary>
    [RequireComponent (typeof (Slider))]
    public sealed class DataBindSlider : AbstractBinderBase {
        Slider _target;

        public override void OnBindedDataChanged (object data) {
            if ((object) _target == null) {
                _target = GetComponent<Slider> ();
            }
            _target.value = GetValueAsNumber (data);
        }
    }
}