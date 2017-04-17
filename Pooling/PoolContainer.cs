// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using System;
using LeopotamGroup.Collections;
using UnityEngine;

namespace LeopotamGroup.Pooling {
    /// <summary>
    /// Pool container. Supports spawning of named prefab from Resources folder.
    /// </summary>
    public sealed class PoolContainer : MonoBehaviour {
        [SerializeField]
        string _prefabPath = "UnknownPrefab";

        [SerializeField]
        Transform _itemsRoot;

        FastStack<IPoolObject> _store = new FastStack<IPoolObject> (32);

        UnityEngine.Object _cachedAsset;

        Vector3 _cachedScale;

        bool _needToAddPoolObject;

        Type _overridedType;

        bool LoadPrefab () {
            var go = Resources.Load<GameObject> (_prefabPath);
            if (go == null) {
                Debug.LogWarning ("Cant load asset " + _prefabPath, gameObject);
                return false;
            }
            _cachedAsset = go.GetComponent (typeof (IPoolObject));
            _needToAddPoolObject = (object) _cachedAsset == null;
            if (_needToAddPoolObject) {
                _cachedAsset = go;
            } else {
                if (_cachedAsset.GetType () != _overridedType) {
                    Debug.LogWarning ("Prefab already contains another IPoolObject-component", gameObject);
                    return false;
                }
            }

            _cachedScale = go.transform.localScale;

            _store.UseCastToObjectComparer (true);

            return true;
        }

        /// <summary>
        /// Get new instance of prefab from pool.
        /// </summary>
        public IPoolObject Get () {
            bool isNew;
            return Get (out isNew);
        }

        /// <summary>
        /// Get new instance of prefab from pool.
        /// </summary>
        /// <param name="isNew">Is instance was created during this call.</param>
        public IPoolObject Get (out bool isNew) {
            if ((object) _cachedAsset == null) {
                if (!LoadPrefab ()) {
                    isNew = true;
                    return null;
                }
            }

            IPoolObject obj;
            if (_store.Count > 0) {
                obj = _store.Pop ();
                isNew = false;
            } else {
                obj = _needToAddPoolObject ?
                    (IPoolObject) ((GameObject) Instantiate (_cachedAsset)).AddComponent (_overridedType) :
                    (IPoolObject) Instantiate (_cachedAsset);
                obj.PoolContainer = this;
                var tr = obj.PoolTransform;
                if ((object) tr != null) {
                    tr.gameObject.SetActive (false);
                    tr.SetParent (_itemsRoot, false);
                    tr.localScale = _cachedScale;
                }
                isNew = true;
            }

            return obj;
        }

        /// <summary>
        /// Recycle specified instance to pool.
        /// </summary>
        /// <param name="obj">Instance to recycle.</param>
        public void Recycle (IPoolObject obj) {
            if ((object) obj != null) {
#if UNITY_EDITOR
                if ((object) obj.PoolContainer != (object) this) {
                    Debug.LogWarning ("Invalid obj to recycle", (UnityEngine.Object) obj);
                    return;
                }
#endif
                var tr = obj.PoolTransform;
                if ((object) tr != null) {
                    tr.gameObject.SetActive (false);
                }
                if (!_store.Contains (obj)) {
                    _store.Push (obj);
                }
            }
        }

        /// <summary>
        /// /// Creates new pool container for specified prefab.
        /// </summary>
        /// <returns>Created pool container.</returns>
        /// <param name="prefabPath">Prefab path at Resources folder.</param>
        /// <param name="itemsRoot">Root for new items.</param>
        /// <param name="overridedType">Overrided type of pool object.
        /// If null - PoolObject-type will be used or exist IPool component on prefab.</param>
        public static PoolContainer CreatePool<T> (string prefabPath, Transform itemsRoot = null) where T : IPoolObject {
            return CreatePool (prefabPath, itemsRoot, typeof (T));
        }

        /// <summary>
        /// Creates new pool container for specified prefab.
        /// </summary>
        /// <returns>Created pool container.</returns>
        /// <param name="prefabPath">Prefab path at Resources folder.</param>
        /// <param name="itemsRoot">Root for new items.</param>
        /// <param name="overridedType">Overrided type of pool object.
        /// If null - PoolObject-type will be used or exist IPool component on prefab.</param>
        public static PoolContainer CreatePool (string prefabPath, Transform itemsRoot = null, System.Type overridedType = null) {
            if (string.IsNullOrEmpty (prefabPath)) {
                return null;
            }
            var container =
                new GameObject (
#if UNITY_EDITOR
                    "_POOL_" + prefabPath
#endif
                ).AddComponent<PoolContainer> ();
            container._prefabPath = prefabPath;
            container._itemsRoot = itemsRoot;
            container._overridedType = overridedType ?? typeof (PoolObject);
            return container;
        }
    }
}