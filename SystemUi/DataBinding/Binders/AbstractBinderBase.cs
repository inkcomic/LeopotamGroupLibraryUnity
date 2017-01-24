// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using System;
using LeopotamGroup.Common;
using UnityEngine;

namespace LeopotamGroup.SystemUi.DataBinding.Binders {
    /// <summary>
    /// Base class for all binders, provide helpers, automatic subscription in 2 ways.
    /// </summary>
    public abstract class AbstractBinderBase : MonoBehaviour, IDataBinder {
        [SerializeField]
        string _source = null;

        [SerializeField]
        string _property = null;

        /// <summary>
        /// Receive events only in enabled state or always.
        /// </summary>
        /// <returns></returns>
        protected virtual bool ProcessEventsOnlyWhenEnabled { get { return true; } }

        public string BindedSource { get { return _source; } }

        public string BindedProperty { get { return _property; } }

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

        /// <summary>
        /// Convert object to float value. If cant - zero will be returned.
        /// </summary>
        /// <param name="obj">Object to convert.</param>
        protected float GetValueAsNumber (object obj) {
            if (obj != null && IsTypeNumeric (obj.GetType ())) {
                return Convert.ToSingle (obj);
            }
            return 0f;
        }

        /// <summary>
        /// Convert object to bool value. If cant - false will be returned.
        /// </summary>
        /// <param name="obj">Object to convert.</param>
        protected bool GetValueAsBool (object obj) {
            return System.Math.Abs (GetValueAsNumber (obj)) > 0f;
        }

        /// <summary>
        /// Convert object to string value.
        /// </summary>
        /// <param name="obj">Object to convert.</param>
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
            if (!string.IsNullOrEmpty (_source) && !string.IsNullOrEmpty (_property)) {
                var storage = Singleton.Get<DataStorage> ();
                storage.Subscribe (this);
                OnBindedDataChanged (storage.GetData (_source, _property));
            }
        }

        void Unsubscribe () {
            if (!string.IsNullOrEmpty (_source) && !string.IsNullOrEmpty (_property)) {
                if (Singleton.IsTypeRegistered<DataStorage> ()) {
                    Singleton.Get<DataStorage> ().Unsubscribe (this);
                }
            }
        }

        /// <summary>
        /// Raise on binded property changes.
        /// </summary>
        /// <param name="data">New value.</param>
        public abstract void OnBindedDataChanged (object data);
    }
}