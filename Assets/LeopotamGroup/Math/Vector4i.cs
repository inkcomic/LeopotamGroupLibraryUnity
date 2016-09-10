//-------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2016 Leopotam <leopotam@gmail.com>
//-------------------------------------------------------

using System;
using UnityEngine;

namespace LeopotamGroup.Math {
    /// <summary>
    /// Vector4 struct with int32 fields.
    /// </summary>
    [Serializable]
    public struct Vector4i {
        /// <summary>
        /// X field.
        /// </summary>
        public int x;

        /// <summary>
        /// Y field.
        /// </summary>
        public int y;

        /// <summary>
        /// Z field.
        /// </summary>
        public int z;

        /// <summary>
        /// W field.
        /// </summary>
        public int w;

        /// <summary>
        /// Static value of Vector4i(0, 0, 0). No protection from external property changes, dont be stupid to do this!
        /// </summary>
        public static readonly Vector4i zero = new Vector4i (0, 0, 0, 0);

        /// <summary>
        /// Static value of Vector4i(1, 1, 1). No protection from external property changes, dont be stupid to do this!
        /// </summary>
        public static readonly Vector4i one = new Vector4i (1, 1, 1, 1);

        /// <summary>
        /// Initialization with custom values for X/Y/Z.
        /// </summary>
        /// <param name="inX">X value.</param>
        /// <param name="inY">Y value.</param>
        /// <param name="inZ">Z value.</param>
        /// <param name="inW">W value.</param>
        public Vector4i (int inX, int inY, int inZ, int inW) {
            x = inX;
            y = inY;
            z = inZ;
            w = inW;
        }

        /// <summary>
        /// Initialization from Vector2i instance.
        /// </summary>
        public Vector4i (Vector2i v) {
            x = v.x;
            y = v.y;
            z = 0;
            w = 0;
        }

        /// <summary>
        /// Initialization from Vector3i instance.
        /// </summary>
        public Vector4i (Vector3i v) {
            x = v.x;
            y = v.y;
            z = v.z;
            w = 0;
        }

        /// <summary>
        /// Initialization from Vector2 instance.
        /// </summary>
        public Vector4i (Vector2 v) {
            x = Mathf.RoundToInt (v.x);
            y = Mathf.RoundToInt (v.y);
            z = 0;
            w = 0;
        }

        /// <summary>
        /// Initialization from Vector3 instance.
        /// </summary>
        public Vector4i (Vector3 v) {
            x = Mathf.RoundToInt (v.x);
            y = Mathf.RoundToInt (v.y);
            z = Mathf.RoundToInt (v.z);
            w = 0;
        }

        /// <summary>
        /// Initialization from Vector4 instance.
        /// </summary>
        public Vector4i (Vector4 v) {
            x = Mathf.RoundToInt (v.x);
            y = Mathf.RoundToInt (v.y);
            z = Mathf.RoundToInt (v.z);
            w = Mathf.RoundToInt (v.w);
        }

        /// <summary>
        /// Get hash code.
        /// </summary>
        public override int GetHashCode () {
            return base.GetHashCode ();
        }

        /// <summary>
        /// Is instance equals with specified one.
        /// </summary>
        /// <param name="rhs">Specified instance for comparation.</param>
        public override bool Equals (object rhs) {
            if (!(rhs is Vector4i)) {
                return false;
            }
            return this == (Vector4i) rhs;
        }

        /// <summary>
        /// Return formatted X/Y/Z values.
        /// </summary>
        public override string ToString () {
            return string.Format ("({0}, {1}, {2}, {3})", x, y, z, w);
        }

        /// <summary>
        /// Combine new Vector4i from min values of two vectors.
        /// </summary>
        /// <param name="lhs">First vector.</param>
        /// <param name="rhs">Second vector.</param>
        public static Vector4i Min (Vector4i lhs, Vector4i rhs) {
            return new Vector4i (lhs.x < rhs.x ? lhs.x : rhs.x, lhs.x < rhs.x ? lhs.y : rhs.y, lhs.z < rhs.z ? lhs.z : rhs.z, lhs.w < rhs.w ? lhs.w : rhs.w);
        }

        /// <summary>
        /// Combine new Vector4i from max values of two vectors.
        /// </summary>
        /// <param name="lhs">First vector.</param>
        /// <param name="rhs">Second vector.</param>
        public static Vector4i Max (Vector4i lhs, Vector4i rhs) {
            return new Vector4i (lhs.x > rhs.x ? lhs.x : rhs.x, lhs.x > rhs.x ? lhs.y : rhs.y, lhs.z > rhs.z ? lhs.z : rhs.z, lhs.w > rhs.w ? lhs.w : rhs.w);
        }

        /// <summary>
        /// Return clamped version of specified vector with min/max range.
        /// </summary>
        /// <param name="value">Source vector.</param>
        /// <param name="min">Min value.</param>
        /// <param name="max">Max value.</param>
        public static Vector4i Clamp (Vector4i value, Vector4i min, Vector4i max) {
            return new Vector4i (Mathf.Clamp (value.x, min.x, max.x), Mathf.Clamp (value.y, min.y, max.y), Mathf.Clamp (value.z, min.z, max.z), Mathf.Clamp (value.w, min.w, max.w));
        }

        public static bool operator == (Vector4i lhs, Vector4i rhs) {
            return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z && lhs.w == rhs.w;
        }

        public static bool operator != (Vector4i lhs, Vector4i rhs) {
            return !(lhs == rhs);
        }

        public static Vector4i operator - (Vector4i lhs) {
            return new Vector4i (-lhs.x, -lhs.y, -lhs.z, -lhs.w);
        }

        public static Vector4i operator - (Vector4i lhs, Vector4i rhs) {
            return new Vector4i (lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z, lhs.w - rhs.w);
        }

        public static Vector4i operator + (Vector4i lhs, Vector4i rhs) {
            return new Vector4i (lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z, lhs.w + rhs.w);
        }

        public static Vector4i operator * (Vector4i lhs, int rhs) {
            return new Vector4i (lhs.x * rhs, lhs.y * rhs, lhs.z * rhs, lhs.w * rhs);
        }

        public static Vector4i operator * (Vector4i lhs, float rhs) {
            return new Vector4i (Mathf.RoundToInt (lhs.x * rhs), Mathf.RoundToInt (lhs.y * rhs), Mathf.RoundToInt (lhs.z * rhs), Mathf.RoundToInt (lhs.w * rhs));
        }

        public static Vector4i operator * (Vector4i lhs, Vector4i rhs) {
            return new Vector4i (lhs.x * rhs.x, lhs.y * rhs.y, lhs.z * rhs.z, lhs.w * rhs.w);
        }

        public static Vector4i operator / (Vector4i lhs, Vector4i rhs) {
            return new Vector4i (lhs.x / rhs.x, lhs.y / rhs.y, lhs.z / rhs.z, lhs.w / rhs.w);
        }

        public static Vector4i operator / (Vector4i lhs, int rhs) {
            return new Vector4i (lhs.x / rhs, lhs.y / rhs, lhs.z / rhs, lhs.w / rhs);
        }

        public static implicit operator Vector2 (Vector4i lhs) {
            return new Vector2 (lhs.x, lhs.y);
        }

        public static implicit operator Vector3 (Vector4i lhs) {
            return new Vector3 (lhs.x, lhs.y, lhs.z);
        }

        public static implicit operator Vector4 (Vector4i lhs) {
            return new Vector4 (lhs.x, lhs.y, lhs.z, lhs.w);
        }

        public static implicit operator Vector4i (Vector2 lhs) {
            return new Vector4i (lhs);
        }

        public static implicit operator Vector4i (Vector3 lhs) {
            return new Vector4i (lhs);
        }

        public static implicit operator Vector4i (Vector4 lhs) {
            return new Vector4i (lhs);
        }

        public static implicit operator Vector4i (Vector2i lhs) {
            return new Vector4i (lhs);
        }

        public static implicit operator Vector4i (Vector3i lhs) {
            return new Vector4i (lhs);
        }
    }
}