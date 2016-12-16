
// -------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2016 Leopotam <leopotam@gmail.com>
// -------------------------------------------------------

#if LGL_UNSAFE

using System;

namespace LeopotamGroup.Collections {
    /// <summary>
    /// GC-free string, heap-based, 4095 characters at max.
    /// </summary>
    [Serializable]
    public sealed class FastString {
        const int Capacity = 4096;

        /// <summary>
        /// Max allowed length.
        /// </summary>
        public const int MaxLength = Capacity - 1;

        readonly string _str = new string ('\0', Capacity);

        int _length;

        bool _isDirty = true;

        /// <summary>
        /// Get current length.
        /// </summary>
        /// <value>The length.</value>
        public int Length {
            get { return _length; }
        }

        void UpdateLength () {
            unsafe {
                fixed (char* ptr = _str) {
                    var pi = (int*) ptr;
                    pi[-1] = _length;
                    ptr[_length] = '\0';
                }
            }
        }

        /// <summary>
        /// Default constructor with empty string data.
        /// </summary>
        public FastString () {
        }

        /// <summary>
        /// Constructor with specified default string data.
        /// </summary>
        /// <param name="rhs">Init string data.</param>
        public FastString (string rhs) {
            Append (rhs);
        }

        /// <summary>
        /// Return final string, no GC allocation.
        /// </summary>
        public override string ToString () {
            if (_isDirty) {
                _isDirty = false;
                UpdateLength ();
            }

            return _str;
        }

        /// <summary>
        /// Clear string data.
        /// </summary>
        public void Clear () {
            _length = 0;
            _isDirty = true;
        }

        /// <summary>
        /// Get / set character at specified position.
        /// </summary>
        /// <param name="idx">Index.</param>
        public char this[int idx] {
            get { return _str[idx]; }
            set {
                if (idx < 0 || idx >= _length) {
                    throw new IndexOutOfRangeException ();
                }
                unsafe {
                    fixed (char* ptr = _str) {
                        ptr[idx] = value;
                    }
                }
            }
        }

        /// <summary>
        /// Append specified string to end of current string.
        /// </summary>
        /// <param name="rhs">String to append.</param>
        public FastString Append (string rhs) {
            if (rhs != null) {
                if (rhs.Length + _length >= Capacity) {
                    throw new OutOfMemoryException ("Result string too long");
                }
                unsafe {
                    fixed (char* ptr = _str) {
                        for (int i = 0, iMax = rhs.Length; i < iMax; i++, _length++) {
                            ptr[_length] = rhs[i];
                        }
                    }
                }
                _isDirty = true;
            }

            return this;
        }

        /// <summary>
        /// Append specified character to end of current string.
        /// </summary>
        /// <param name="rhs">Character to append.</param>
        public FastString Append (char rhs) {
            if (_length + 1 >= Capacity) {
                throw new OutOfMemoryException ("Result string too long");
            }
            unsafe {
                fixed (char* ptr = _str) {
                    ptr[_length] = rhs;
                    _length++;
                }
            }
            _isDirty = true;

            return this;
        }

        /// <summary>
        /// Append specified FastString to end of current string.
        /// </summary>
        /// <param name="rhs">String to append.</param>
        public FastString Append (FastString rhs) {
            return rhs != null ? Append (rhs.ToString ()) : this;
        }

        /// <summary>
        /// Trim current string with specified amount of characters from end.
        /// </summary>
        /// <param name="amount">Amount.</param>
        public FastString Trim (int amount) {
            if (amount < 0 || amount > _length) {
                throw new IndexOutOfRangeException ();
            }
            if (amount > 0) {
                _length -= amount;
                _isDirty = true;
            }

            return this;
        }
    }
}

#endif