
// -------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2016 Leopotam <leopotam@gmail.com>
// -------------------------------------------------------

using System.Collections.Generic;
using System;
using UnityEngine;

namespace LeopotamGroup.Common {
    /// <summary>
    /// Attribute for locking usage of UnitySingletonBase-classes at specified scenes only.
    /// </summary>
    [System.AttributeUsage (System.AttributeTargets.Class, AllowMultiple = true)]
    [System.Diagnostics.Conditional ("UNITY_EDITOR")]
    sealed class UnitySingletonSceneFilterAttribute : Attribute {
        public string Name;

        public UnitySingletonSceneFilterAttribute (string name) {
            Name = name;
        }
    }

    /// <summary>
    /// Base class for singleton pattern, unity version.
    /// Warning: You cant touch singleton instance at any Awake() method!
    /// </summary>
    public abstract class UnitySingletonBase : MonoBehaviour {
        void Awake () {
            var type = GetType ();
            if (Singleton.IsTypeRegistered (type)) {
                DestroyImmediate (gameObject);
                return;
            }

#if UNITY_EDITOR

            // check for allowed scenes if possible.
            var attrs = GetType ().GetCustomAttributes (typeof (UnitySingletonSceneFilterAttribute), true);
            if (attrs != null && attrs.Length > 0) {
                var sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name;
                var i = attrs.Length - 1;
                for (; i >= 0; i--) {
                    if (System.Text.RegularExpressions.Regex.IsMatch (
                            sceneName,
                            (attrs[i] as UnitySingletonSceneFilterAttribute).Name)) {
                        break;
                    }
                }
                if (i == -1) {
                    throw new UnityException (
                              string.Format ("\"{0}\" singleton cant be used at scene \"{1}\"", type.Name, sceneName));
                }
            }
#endif

            Singleton.Register (this, type);

            OnConstruct ();
        }

        void OnDestroy () {
            if (Singleton.IsInstanceRegistered (this)) {
                OnDestruct ();
                Singleton.Unregister (this);
            }
        }

        /// <summary>
        /// Replacement of Awake method, will be raised only once for singleton.
        /// Dont use Awake method in inherited classes!
        /// </summary>
        protected virtual void OnConstruct () {
        }

        /// <summary>
        /// Replacement of OnDestroy method, will be raised only once for singleton.
        /// Dont use OnDestroy method in inherited classes!
        /// </summary>
        protected virtual void OnDestruct () {
        }
    }

    public static class Singleton {
        static readonly Dictionary<Type, object> _instancesPool =
            new Dictionary<Type, object> (32, new KeysComparer ());

        static readonly List<Type> _removeCache = new List<Type> (32);

        /// <summary>
        /// Get count of registered singletons.
        /// </summary>
        /// <value>The count.</value>
        public static int Count {
            get { return _instancesPool.Count; }
        }

        /// <summary>
        /// Safe check for instance-as-singleton availability.
        /// </summary>
        /// <param name="instance">Instance for checl.</param>
        public static bool IsInstanceRegistered (object instance) {
            if (instance != null) {
                foreach (var pair in _instancesPool) {
                    if (pair.Value == instance) {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool IsTypeRegistered<T> () {
            return IsTypeRegistered (typeof (T));
        }

        /// <summary>
        /// Safe check for type-as-singleton availability.
        /// </summary>
        /// <param name="type">Type for check.</param>
        public static bool IsTypeRegistered (Type type) {
            return type != null && _instancesPool.ContainsKey (type);
        }

        /// <summary>
        /// Get singleton instance of generic type.
        /// Warning: You cant touch singleton instance at any Awake() method!
        /// </summary>
        public static T Get<T> () where T : class, new() {
            var type = typeof (T);
            object retVal;
            if (_instancesPool.TryGetValue (type, out retVal)) {
                return (T) retVal;
            }

#if UNITY_EDITOR
            if (!Application.isPlaying) {
                throw new UnityException (
                          string.Format ("UnitySingleton<{0}> can be used only at PLAY mode", type.Name));
            }
#endif

            // special case for unity components.
            if (type.IsSubclassOf (typeof (Component))) {
                new GameObject (
#if UNITY_EDITOR
                    "_SINGLETON_" + type.Name
#endif
                    ).AddComponent (type);
            } else {
                _instancesPool[type] = new T ();
            }

            return (T) _instancesPool[type];
        }

        /// <summary>
        /// Register instance as singleton of specified type. Instance type should be inheritated from specified type.
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="targetType">Target type.</param>
        /// <param name="forceRegister">Force unregister old instance and destroy if exists, then register new
        // one.</param>
        public static void Register (object instance, Type targetType = null, bool forceRegister = false) {
            if ((object) instance == null) {
                throw new UnityException (
                          string.Format (
                              "Cant register instance of \"{0}\" as singleton of type \"{1}\" - instance is null",
                              instance.GetType ().Name, targetType != null ? targetType.Name : "null"));
            }
            var instanceType = instance.GetType ();
            targetType = targetType ?? instanceType;

            if (instanceType != targetType && !instanceType.IsSubclassOf (targetType)) {
                throw new UnityException (
                          string.Format (
                              "Cant register instance of \"{0}\" as singleton of type \"{1}\" - invalid target type",
                              instanceType.Name, targetType != null ? targetType.Name : "null"));
            }
            if (IsTypeRegistered (targetType)) {
                if (!forceRegister) {
                    throw new UnityException (
                              string.Format (
                                  "Cant register instance of \"{0}\" as singleton of type \"{1}\" - target type already registered",
                                  instanceType.Name, targetType.Name));
                }

                // special case for unity components.
                if (targetType.IsSubclassOf (typeof (Component))) {
                    UnityEngine.Object.DestroyImmediate ((_instancesPool[targetType] as UnitySingletonBase).gameObject);
                }
            }
            _instancesPool[targetType] = instance;
        }

        /// <summary>
        /// Unregister singleton instance for all types if exists.
        /// </summary>
        /// <param name="instance">Instance.</param>
        public static void Unregister (object instance) {
            if (instance != null) {
                foreach (var pair in _instancesPool) {
                    if (pair.Value == instance) {
                        _removeCache.Add (pair.Key);
                    }
                }
                for (var i = _removeCache.Count - 1; i >= 0; i--) {
                    _instancesPool.Remove (_removeCache[i]);
                }
                _removeCache.Clear ();
            }
        }

        class KeysComparer : IEqualityComparer<Type> {
            public bool Equals (Type x, Type y) {
                return x == y;
            }

            public int GetHashCode (Type obj) {
                return obj.GetHashCode ();
            }
        }
    }
}