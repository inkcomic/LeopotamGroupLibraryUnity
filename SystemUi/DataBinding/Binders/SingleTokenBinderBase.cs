// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using LeopotamGroup.Common;
using UnityEngine;

namespace LeopotamGroup.SystemUi.DataBinding.Binders {
    public abstract class SingleTokenBinderBase : MonoBehaviour, IDataBinder {
        [SerializeField]
        string _token = null;

        void Awake () {
            if (!string.IsNullOrEmpty (_token)) {
                var storage = Singleton.Get<DataStorage> ();
                storage.Subscribe (_token, this);
                OnDataChanged (_token, storage.GetData (_token));
            }
        }

        void OnDestroy () {
            if (Singleton.IsTypeRegistered<DataStorage> ()) {
                if (!string.IsNullOrEmpty (_token)) {
                    var storage = Singleton.Get<DataStorage> ();
                    storage.Unsubscribe (_token, this);
                }
            }
        }

        public abstract void OnDataChanged (string token, object data);
    }
}