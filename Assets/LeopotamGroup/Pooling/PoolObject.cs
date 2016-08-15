//-------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2016 Leopotam <leopotam@gmail.com>
//-------------------------------------------------------

using LeopotamGroup.Common;
using UnityEngine;

namespace LeopotamGroup.Pooling {
    public interface IPoolObject {
        /// <summary>
        /// Pool container - spawner of this instance, should be set only once.
        /// </summary>
        /// <value>The pool container.</value>
        PoolContainer PoolContainer { get; set; }

        /// <summary>
        /// Transform of spawned instance, can be null if you dont need it.
        /// </summary>
        Transform PoolTransform { get; }

        /// <summary>
        /// Recycle this instance.
        /// </summary>
        void PoolRecycle ();
    }

    /// <summary>
    /// Helper for PoolContainer.
    /// </summary>
    public sealed class PoolObject : MonoBehaviourBase, IPoolObject {
#region IPoolObject implementation

        public PoolContainer PoolContainer { get; set; }

        public Transform PoolTransform { get { return transform; } }

        public void PoolRecycle () {
            if ((System.Object) PoolContainer != null) {
                PoolContainer.Recycle (this);
            }
        }

#endregion
    }
}