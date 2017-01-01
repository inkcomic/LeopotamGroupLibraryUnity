
// -------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// -------------------------------------------------------

#if LGL_UNSAFE

using System.Runtime.InteropServices;
using System;

namespace LeopotamGroup.Collections {
    /// <summary>
    /// GC-free string, stack-based, 255 characters at max.
    /// </summary>
    [Serializable]
    [StructLayout (LayoutKind.Explicit, Size = 514)]
    public struct StackString : IComparable<string>, IComparable<StackString> {
        [FieldOffset (0)]
        char _data;

        /// <summary>
        /// Length of string.
        /// </summary>
        [FieldOffset (512)]
        public byte Length;

        // 513-byte not used, added just for word-alignment in memory.

        /// <summary>
        /// Get / set character at index position.
        /// </summary>
        /// <param name="idx">Index position.</param>
        public char this[byte idx] {
            get {
                unsafe {
                    fixed (char* ptr = &_data) {
                        return ptr[idx];
                    }
                }
            }
            set {
                unsafe {
                    fixed (char* p = &_data) {
                        p[idx] = value;
                    }
                }
            }
        }

        /// <summary>
        /// Constructor with specified default string data.
        /// </summary>
        /// <param name="rhs">Init string data.</param>
        public StackString (string rhs) {
            _data = '\0';
            Length = 0;
            Append (rhs);
        }

        /// <summary>
        /// Append specified string to end of current string.
        /// </summary>
        /// <param name="rhs">String to append.</param>
        public void Append (string rhs) {
            if (rhs != null) {
                if (rhs.Length + Length >= 256) {
                    throw new ArgumentOutOfRangeException (rhs);
                }
                unsafe {
                    fixed (char* ptr = &_data) {
                        for (int i = 0, iMax = rhs.Length; i < iMax; i++, Length++) {
                            ptr[Length] = rhs[i];
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Return final string with GC memory allocation.
        /// </summary>
        public override string ToString () {
            unsafe {
                fixed (char* ptr = &_data) {
                    return new string (ptr, 0, Length);
                }
            }
        }

        /// <summary>
        /// Compare with string.
        /// </summary>
        /// <param name="rhs">Comparing string.</param>
        public int CompareTo (string rhs) {
            if (rhs == null) {
                return 1;
            }
            var rhsLength = rhs.Length;
            var len = rhsLength > Length ? Length : rhsLength;
            var res = 0;
            unsafe {
                fixed (char* ptr = &_data) {
                    for (var i = 0; i < len; i++) {
                        res = ptr[i].CompareTo (rhs[i]);
                        if (res != 0) {
                            break;
                        }
                    }
                    if (res != 0) {
                        return res;
                    }
                }
            }
            return len - rhsLength;
        }

        /// <summary>
        /// Compare with StackString.
        /// </summary>
        /// <param name="rhs">Comparing StackString.</param>
        public int CompareTo (StackString rhs) {
            var rhsLength = rhs.Length;
            var len = rhsLength > Length ? Length : rhsLength;
            var res = 0;
            unsafe {
                fixed (char* ptr = &_data) {
                    char* rhsPtr = &(rhs._data);
                    for (var i = 0; i < len; i++) {
                        res = ptr[i].CompareTo (rhsPtr[i]);
                        if (res != 0) {
                            break;
                        }
                    }
                }
                if (res != 0) {
                    return res;
                }
            }
            return len - rhsLength;
        }
    }
}

#endif