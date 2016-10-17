//-------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2016 Leopotam <leopotam@gmail.com>
//-------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;

namespace LeopotamGroup.Collections {
    /// <summary>
    /// List class replacement without additional checks and public access to internal array data.
    /// </summary>
    [Serializable]
    public class FastList<T> : IList<T> {
        /// <summary>
        /// Get items count.
        /// </summary>
        public int Count { get { return _count; } }

        /// <summary>
        /// Get collection capacity.
        /// </summary>
        public int Capacity { get { return _capacity; } }

        /// <summary>
        /// Get / set item at specified index.
        /// </summary>
        /// <param name="index">Index.</param>
        public T this [int index] {
            get { return _items[index]; }
            set { _items[index] = value; }
        }

        const int InitCapacity = 4;

        readonly bool _isNullable;

        T[] _items;

        int _count;

        int _capacity;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public FastList () : this (InitCapacity) {
        }

        /// <summary>
        /// Constructor with capacity initialization.
        /// </summary>
        /// <param name="capacity">Capacity on start.</param>
        public FastList (int capacity) {
            var type = typeof (T);
            _isNullable = !type.IsValueType || (Nullable.GetUnderlyingType (type) != null);
            _capacity = capacity > InitCapacity ? capacity : InitCapacity;
            _count = 0;
            _items = new T[_capacity];
        }

        /// <summary>
        /// Add new item to end of collection.
        /// </summary>
        /// <param name="item">New item.</param>
        public void Add (T item) {
            if (_count == _capacity) {
                if (_capacity > 0) {
                    _capacity <<= 1;
                } else {
                    _capacity = InitCapacity;
                }
                var items = new T[_capacity];
                Array.Copy (_items, items, _count);
                _items = items;
            }
            _items[_count] = item;
            _count++;
        }

        /// <summary>
        /// Set internal data, use it on your own risk!
        /// </summary>
        /// <param name="data">Data.</param>
        /// <param name="count">Count.</param>
        public void AssignData (T[] data, int count) {
            if (data == null) {
                throw new NullReferenceException ("data");
            }
            _items = data;
            _count = count >= 0 ? count : 0;
            _capacity = _items.Length;
        }

        /// <summary>
        /// Clear collection without release memory for performance optimization. Similar as Clear(true) call for reference T-type.
        /// </summary>
        public void Clear () {
            if (_isNullable) {
                for (var i = _count - 1; i >= 0; i--) {
                    _items[i] = default(T);
                }
            }
            _count = 0;
        }

        /// <summary>
        /// Clear collection without release memory for performance optimization. 
        /// </summary>
        /// <param name="forceSetDefaultValues">Is new items should be set to their default values (False useful for optimization).</param>
        public void Clear (bool forceSetDefaultValues) {
            if (forceSetDefaultValues) {
                for (var i = _count - 1; i >= 0; i--) {
                    _items[i] = default(T);
                }
            }
            _count = 0;
        }

        /// <summary>
        /// Is collection contains specified item.
        /// </summary>
        /// <param name="item">Item to check.</param>
        public bool Contains (T item) {
            return Array.IndexOf<T> (_items, item) != -1;
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="array">Array.</param>
        /// <param name="arrayIndex">Array index.</param>
        public void CopyTo (T[] array, int arrayIndex) {
            Array.Copy (_items, 0, array, arrayIndex, _count);
        }

        /// <summary>
        /// Add new items with default values to end of collection.
        /// </summary>
        /// <param name="amount">Amount of new items.</param>
        /// <param name="clearCollection">Is collection should be cleared before.</param>
        /// <param name="forceSetDefaultValues">Is new items should be set to their default values (False useful for optimization).</param>
        public void FillWithEmpty (int amount, bool clearCollection = false, bool forceSetDefaultValues = true) {
            if (amount <= 0) {
                return;
            }

            if (clearCollection) {
                _count = 0;
            }
            Reserve (amount, clearCollection, forceSetDefaultValues);
            _count += amount;
        }

        /// <summary>
        /// Get index of specified item.
        /// </summary>
        /// <returns>Found index or -1.</returns>
        /// <param name="item">Item to check.</param>
        public int IndexOf (T item) {
            return Array.IndexOf<T> (_items, item);
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="index">Index.</param>
        /// <param name="item">Item.</param>
        public void Insert (int index, T item) {
            throw new NotImplementedException ();
        }

        /// <summary>
        /// Is collection readonly (for compatibility to IList).
        /// </summary>
        public bool IsReadOnly { get { return false; } }

        /// <summary>
        /// Get internal data, use it on your own risk!
        /// Dont forget, length of result array equals Capacity, not Count!
        /// Can be used for external implementation any other methods.
        /// </summary>
        /// <returns>The data.</returns>
        public T[] GetData () {
            return _items;
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<T> GetEnumerator () {
            throw new NotImplementedException ();
        }

        IEnumerator IEnumerable.GetEnumerator () {
            throw new NotImplementedException ();
        }

        /// <summary>
        /// Try to remove specified item.
        /// </summary>
        /// <param name="item">Item to remove.</param>
        public bool Remove (T item) {
            int id = Array.IndexOf (_items, item);
            if (id != -1) {
                RemoveAt (id);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Remove item from collection at index.
        /// </summary>
        /// <param name="id">Index of item to remove.</param>
        public void RemoveAt (int id) {
            if (id >= 0 && id < _count) {
                Array.Copy (_items, id + 1, _items, id, _count - id);
            }
        }

        /// <summary>
        /// Try to remove last item in collection.
        /// </summary>
        /// <returns><c>true</c>, if last was removed, <c>false</c> otherwise.</returns>
        /// <param name="forceSetDefaultValues">Is new items should be set to their default values (False useful for optimization).</param>
        public bool RemoveLast (bool forceSetDefaultValues = true) {
            if (_count > 0) {
                _count--;
                if (forceSetDefaultValues) {
                    _items[_count] = default(T);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Reserve the specified amount of items, absolute or relative.
        /// </summary>
        /// <param name="amount">Amount.</param>
        /// <param name="totalAmount">Is amount value means - total items amount at collection or relative otherwise.</param>
        /// <param name="forceSetDefaultValues">Is new items should be set to their default values (False useful for optimization).</param>
        public void Reserve (int amount, bool totalAmount = false, bool forceSetDefaultValues = true) {
            if (amount <= 0) {
                return;
            }
            var start = totalAmount ? 0 : _count;
            var newCount = start + amount;
            if (newCount > _capacity) {
                if (_capacity <= 0) {
                    _capacity = InitCapacity;
                }
                while (_capacity < newCount) {
                    _capacity <<= 1;
                }
                var items = new T[_capacity];
                Array.Copy (_items, items, _count);
                _items = items;
            }
            if (forceSetDefaultValues) {
                for (var i = _count; i < newCount; i++) {
                    _items[i] = default(T);
                }
            }
        }

        /// <summary>
        /// Reverse items order in collection.
        /// </summary>
        public void Reverse () {
            if (_count > 0) {
                Array.Reverse (_items, 0, _count);
            }
        }
    }
}