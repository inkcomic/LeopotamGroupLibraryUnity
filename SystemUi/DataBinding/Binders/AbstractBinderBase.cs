// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using System;
using LeopotamGroup.Common;
using UnityEngine;

namespace LeopotamGroup.SystemUi.DataBinding.Binders {
    public abstract class AbstractBinderBase : MonoBehaviour, IDataBinder {
        [SerializeField]
        string _token = null;

        /// <summary>
        /// Receive events only in enabled state or always.
        /// </summary>
        /// <returns></returns>
        protected virtual bool ProcessEventsOnlyWhenEnabled { get { return true; } }

        static bool IsTypeNumeric (Type type) {
            switch (Type.GetTypeCode (type)) {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                case TypeCode.Boolean:
                    return true;
                default:
                    return false;
            }
        }

        protected float GetValueAsNumber (object obj) {
            if (obj != null && IsTypeNumeric (obj.GetType ())) {
                return Convert.ToSingle (obj);
            }
            return 0f;
        }

        protected bool GetValueAsBool (object obj) {
            return System.Math.Abs (GetValueAsNumber (obj)) > 0f;
        }

        protected string GetValueAsString (object obj) {
            return obj != null ? obj.ToString () : null;
        }

        void Awake () {
            if (!ProcessEventsOnlyWhenEnabled) {
                Subscribe ();
            }
        }

        void OnEnable () {
            if (ProcessEventsOnlyWhenEnabled) {
                Subscribe ();
            }
        }

        void OnDisable () {
            if (ProcessEventsOnlyWhenEnabled) {
                Unsubscribe ();
            }
        }

        void OnDestroy () {
            if (!ProcessEventsOnlyWhenEnabled) {
                Unsubscribe ();
            }
        }

        void Subscribe () {
            if (!string.IsNullOrEmpty (_token)) {
                var storage = Singleton.Get<DataStorage> ();
                storage.Subscribe (_token, this);
                OnDataChanged (_token, storage.GetData (_token));
            }
        }

        void Unsubscribe () {
            if (!string.IsNullOrEmpty (_token)) {
                if (Singleton.IsTypeRegistered<DataStorage> ()) {
                    Singleton.Get<DataStorage> ().Unsubscribe (_token, this);
                }
            }
        }

        public abstract void OnDataChanged (string token, object data);
    }
}