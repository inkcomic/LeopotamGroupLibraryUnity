// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

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
    public class PoolObject : MonoBehaviourBase, IPoolObject {
        public virtual PoolContainer PoolContainer { get; set; }

        public virtual Transform PoolTransform {
            get { return transform; }
        }

        public virtual void PoolRecycle () {
            if ((object) PoolContainer != null) {
                PoolContainer.Recycle (this);
            }
        }
    }
}