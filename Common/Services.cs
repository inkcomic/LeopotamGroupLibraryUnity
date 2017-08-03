// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;

namespace LeopotamGroup.Common {
    /// <summary>
    /// Attribute for locking usage of UnityServiceBase-classes at specified scenes only.
    /// </summary>
    [AttributeUsage (AttributeTargets.Class, AllowMultiple = true)]
    [System.Diagnostics.Conditional ("UNITY_EDITOR")]
    sealed class UnityServiceFilterAttribute : Attribute {
        public string Name;

        public UnityServiceFilterAttribute (string name) {
            Name = name;
        }
    }

    /// <summary>
    /// UnityMonoBehaviour base class for service locator pattern.
    /// Warning: Touching services at any Awake() method will lead to undefined behaviour!
    /// </summary>
    public abstract class UnityServiceBase : MonoBehaviour {
        void Awake () {
            var type = GetType ();
            if (Services.IsTypeRegistered (type)) {
                DestroyImmediate (this);
                return;
            }
#if UNITY_EDITOR
            // check for allowed scenes if possible.
            var attrs = GetType ().GetCustomAttributes (typeof (UnityServiceFilterAttribute), true);
            if (attrs.Length > 0) {
                var sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name;
                var i = attrs.Length - 1;
                for (; i >= 0; i--) {
                    if (System.Text.RegularExpressions.Regex.IsMatch (
                            sceneName, ((UnityServiceFilterAttribute) attrs[i]).Name)) {
                        break;
                    }
                }
                if (i == -1) {
                    throw new UnityException (
                        string.Format ("\"{0}\" service cant be used at scene \"{1}\"", type.Name, sceneName));
                }
            }
#endif
            Services.Register (this);
            OnCreateService ();
        }

        void OnDestroy () {
            if (Services.IsTypeRegistered (GetType ())) {
                OnDestroyService ();
                Services.Unregister (this);
            }
        }

        /// <summary>
        /// Replacement of Awake method, will be raised only once for singleton.
        /// Dont use Awake method in inherited classes!
        /// </summary>
        protected abstract void OnCreateService ();

        /// <summary>
        /// Replacement of OnDestroy method, will be raised only once for singleton.
        /// Dont use OnDestroy method in inherited classes!
        /// </summary>
        protected abstract void OnDestroyService ();
    }

    /// <summary>
    /// Services locator.
    /// </summary>
    public static class Services {
        static readonly Dictionary<int, object> _instancesPool = new Dictionary<int, object> (64);

        /// <summary>
        /// Safe check for type-as-service availability.
        /// </summary>
        public static bool IsTypeRegistered<T> () {
            return IsTypeRegistered (typeof (T));
        }

        /// <summary>
        /// Safe check for type-as-service availability.
        /// </summary>
        /// <param name="type">Type for check.</param>
        public static bool IsTypeRegistered (Type type) {
            return type != null && _instancesPool.ContainsKey (type.GetHashCode ());
        }

        /// <summary>
        /// Get service instance of generic type.
        /// Warning: Touching services at any Awake() method will lead to undefined behaviour!
        /// </summary>
        /// <param name="lazyInitialization">Should new service will be created if not exists or not.
        /// If false and not exists - Exception.</param>
        public static T Get<T> (bool lazyInitialization = false) where T : class, new () {
            var type = typeof (T);
            var hash = type.GetHashCode ();
            object retVal;
            if (_instancesPool.TryGetValue (hash, out retVal)) {
                return retVal as T;
            }

            if (!lazyInitialization) {
                throw new UnityException (string.Format ("Services.Get<{0}>() instance not exists", type.Name));
            }

            // Special case for unity scriptable objects.
            if (type.IsSubclassOf (typeof (ScriptableObject))) {
                var list = Resources.FindObjectsOfTypeAll (type);
                if (list == null || list.Length == 0) {
                    throw new UnityException (
                        string.Format ("Services.Get<{0}>() can be used only with exists / loaded asset of this type", type.Name));
                }
                var obj = list[0] as T;
                _instancesPool[hash] = obj;
                return obj;
            }

            // Not allow any unity components except wrapped to special class.
#if UNITY_EDITOR
            if (type.IsSubclassOf (typeof (Component)) && !type.IsSubclassOf (typeof (UnityServiceBase))) {
                throw new UnityException (string.Format ("\"{0}\" - invalid type, should be UnityServiceBase-based", type.Name));
            }
#endif

            // Ready to lazy initialization.

            // Special case for UnitySingletonBase components.
            if (type.IsSubclassOf (typeof (UnityServiceBase))) {
#if UNITY_EDITOR
                if (!Application.isPlaying) {
                    throw new UnityException (string.Format ("Services.Get<{0}>() can be used only at PLAY mode", type.Name));
                }
#endif
                new GameObject (
#if UNITY_EDITOR
                    "_SINGLETON_" + type.Name
#endif
                ).AddComponent (type);
            } else {
                _instancesPool[hash] = new T ();
            }

            return _instancesPool[hash] as T;
        }

        /// <summary>
        /// Register instance as service.
        /// </summary>
        /// <param name="service">Service instance.</param>
        public static void Register (object service) {
            if (service == null) {
                throw new UnityException ("Cant register null instance as service");
            }
            var type = service.GetType ();
            var hash = type.GetHashCode ();

            if (IsTypeRegistered (type)) {
                throw new UnityException (string.Format (
                    "Cant register \"{0}\" as service - type already registered", type.Name));
            }
            _instancesPool[hash] = service;
        }

        /// <summary>
        /// Unregister service instance for all types if exists.
        /// </summary>
        /// <param name="service">Service instance.</param>
        public static void Unregister (object service) {
            if (service != null) {
                var hash = service.GetType ().GetHashCode ();
                if (_instancesPool.ContainsKey (hash)) {
                    _instancesPool.Remove (hash);
                }
            }
        }
    }
}