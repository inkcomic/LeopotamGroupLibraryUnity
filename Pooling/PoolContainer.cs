// -------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// -------------------------------------------------------

using LeopotamGroup.Collections;
using UnityEngine;

// ReSharper disable RedundantCast
// ReSharper disable RedundantCast.0

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

        Object _cachedAsset;

        Vector3 _cachedScale;

        bool _needToAddPoolObject;

        bool LoadPrefab () {
            var go = Resources.Load<GameObject> (_prefabPath);
            if (go == null) {
                Debug.LogWarning ("Cant load asset " + _prefabPath);
                return false;
            }
            _cachedAsset = go.GetComponent (typeof (IPoolObject));
            _needToAddPoolObject = (object) _cachedAsset == null;
            if (_needToAddPoolObject) {
                _cachedAsset = go;
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
                    ((GameObject) Instantiate (_cachedAsset)).AddComponent<PoolObject> () :
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
                    Debug.LogWarning ("Invalid obj to recycle", (Object) obj);
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
        /// Creates new pool container for specified prefab.
        /// </summary>
        /// <returns>Created pool container.</returns>
        /// <param name="prefabPath">Prefab path at Resources folder.</param>
        /// <param name="itemsRoot">Root for new items.</param>
        public static PoolContainer CreatePool (string prefabPath, Transform itemsRoot = null) {
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
            return container;
        }
    }
}