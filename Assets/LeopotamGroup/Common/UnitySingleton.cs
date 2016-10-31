//-------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2016 Leopotam <leopotam@gmail.com>
//-------------------------------------------------------

using UnityEngine;

namespace LeopotamGroup.Common {
    /// <summary>
    /// Attribute for locking usage of UnitySingleton-based classes only at named scenes.
    /// </summary>
    [System.AttributeUsage (System.AttributeTargets.Class, AllowMultiple = true)]
    [System.Diagnostics.Conditional ("UNITY_EDITOR")]
    sealed class UnitySingletonAllowedSceneAttribute : System.Attribute {
        public string Name;

        public UnitySingletonAllowedSceneAttribute (string name) {
            Name = name;
        }
    }

    /// <summary>
    /// Singleton pattern, unity version.
    /// </summary>
    public abstract class UnitySingleton<T> : MonoBehaviour where T : MonoBehaviour {
        static T _instance;

        /// <summary>
        /// Get singleton instance.
        /// </summary>
        /// <value>Instance.</value>
        public static T Instance {
            get {
                // Workaround for slow checking "_instance == null" operation
                // (unity issue, overrided equality operators for additional internal checking).
                if ((System.Object) _instance == null) {
#if UNITY_EDITOR
                    if (!Application.isPlaying) {
                        throw new UnityException (typeof (T).Name + " singleton can be used only at PLAY mode");
                    }
#endif
                    _instance = Object.FindObjectOfType<T> ();
                    if ((System.Object) _instance == null) {
                        _instance = new GameObject (
#if UNITY_EDITOR
                            "_SINGLETON_" + typeof (T).Name
#endif
                        ).AddComponent<T> ();
                    }
                }

                return _instance;
            }
        }

        void Awake () {
            if ((System.Object) _instance != null && _instance != this) {
                DestroyImmediate (gameObject);
                return;
            }
                
#if UNITY_EDITOR
            // check for allowed scenes if possible.
            var attrs = GetType ().GetCustomAttributes (typeof (UnitySingletonAllowedSceneAttribute), true);
            if (attrs != null && attrs.Length > 0) {
                var sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name;
                int i;
                for (i = attrs.Length - 1; i >= 0; i--) {
                    if (System.Text.RegularExpressions.Regex.IsMatch (sceneName, (attrs[i] as UnitySingletonAllowedSceneAttribute).Name)) {
                        break;
                    }
                }
                if (i == -1) {
                    throw new UnityException (string.Format ("\"{0}\" singleton cant be used at scene \"{1}\"", GetType ().Name, sceneName));
                }
            }
#endif

            _instance = this as T;

            OnConstruct ();
        }

        void OnDestroy () {
            if (_instance == this) {
                _instance = null;
                OnDestruct ();
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

        /// <summary>
        /// Save checking for singleton instance availability.
        /// </summary>
        /// <returns>Instance exists.</returns>
        public static bool IsInstanceCreated () {
            return (System.Object) _instance != null;
        }

        /// <summary>
        /// Force validate instance.
        /// </summary>
        public void Validate () {
        }
    }
}