
// -------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// -------------------------------------------------------

using System;
using UnityEngine;

namespace LeopotamGroup.Common {
    /// <summary>
    /// Unity class extensions.
    /// </summary>
    public static class UnityExtensions {
        /// <summary>
        /// Broadcast method with data to all active GameObjects.
        /// </summary>
        /// <param name="method">Method name.</param>
        /// <param name="data">Optional data.</param>
        public static void BroadcastToAll (string method, object data = null) {
            var list = UnityEngine.Object.FindObjectsOfType<MonoBehaviour> ();
            for (var i = list.Length - 1; i >= 0; i--) {
                list[i].SendMessage (method, data, SendMessageOptions.DontRequireReceiver);
            }
        }

        /// <summary>
        /// Ensure that GameObject have component.
        /// </summary>
        /// <returns>Wanted component.</returns>
        /// <param name="go">Target GameObject.</param>
        /// <typeparam name="T">Any unity-based component.</typeparam>
        public static T EnsureGetComponent<T> (this GameObject go) where T : Component {
            if (go != null) {
                var c = go.GetComponent<T> ();
                if ((object) c == null) {
                    c = go.AddComponent<T> ();
                }
                return c;
            }
            return null;
        }

        /// <summary>
        /// Find GameObject with name in recursive hierarchy.
        /// </summary>
        /// <returns>Transform of found GameObject.</returns>
        /// <param name="target">Root of search.</param>
        /// <param name="name">Name to search.</param>
        public static Transform FindRecursive (this Transform target, string name) {
            if (target == null || string.CompareOrdinal (target.name, name) == 0) {
                return target;
            }
            Transform retVal = null;
            for (var i = target.childCount - 1; i >= 0; i--) {
                retVal = target.GetChild (i).FindRecursive (name);
                if (retVal != null) {
                    break;
                }
            }
            return retVal;
        }

        /// <summary>
        /// Convert hex string "rrggbb" to Color.
        /// </summary>
        /// <returns>Color.</returns>
        /// <param name="text">"rrggbb" string.</param>
        public static Color ToColor24 (this string text) {
            try {
                var data = Convert.ToInt32 (text.Length > 6 ? text.Substring (0, 6) : text, 16);
                return new Color (
                    ((data >> 16) & 0xff) / 255f,
                    ((data >> 8) & 0xff) / 255f,
                    (data & 0xff) / 255f,
                    1f);
            } catch {
                return Color.black;
            }
        }

        /// <summary>
        /// Convert hex string "rrggbbaa" to Color.
        /// </summary>
        /// <returns>Color.</returns>
        /// <param name="text">"rrggbbaa" string.</param>
        public static Color ToColor32 (this string text) {
            try {
                var data = Convert.ToInt32 (text.Length > 8 ? text.Substring (0, 8) : text, 16);
                return new Color (
                    ((data >> 24) & 0xff) / 255f,
                    ((data >> 16) & 0xff) / 255f,
                    ((data >> 8) & 0xff) / 255f,
                    (data & 0xff) / 255f);
            } catch {
                return Color.black;
            }
        }

        /// <summary>
        /// Convert color to hex string "rrggbb".
        /// </summary>
        /// <param name="color">Color.</param>
        public static string ToHexString24 (this Color color) {
            var data = ((int) (color.r * 255f) << 16) + ((int) (color.g * 255f) << 8) + (int) (color.b * 255f);
            return data.ToString ("x6");
        }

        /// <summary>
        /// Convert color to hex string "rrggbbaa".
        /// </summary>
        /// <param name="color">Color.</param>
        public static string ToHexString32 (this Color color) {
            var data = ((int) (color.r * 255f) << 24) +
                       ((int) (color.g * 255f) << 16) +
                       ((int) (color.b * 255f) << 8) +
                       (int) (color.a * 255f);
            return data.ToString ("x8");
        }
    }
}