// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using LeopotamGroup.Common;
using UnityEngine;

#if !UNITY_WEBGL
namespace LeopotamGroup.Threading {
    /// <summary>
    /// Base class for processing data at background thread, singleton based.
    /// </summary>
    public abstract class BackgroundWorkerBase<T> : UnitySingletonBase {
        /// <summary>
        /// Is background thread started and inited.
        /// </summary>
        public bool IsWorkerStarted {
            get { lock (_inSyncObj) { return _isWorkerStarted; } }
        }

        /// <summary>
        /// Dont sleep after processing item in background thread.
        /// Processing speed can increase, but owerall performance can degrade.
        /// Disable by default.
        /// </summary>
        protected bool DontSleepAfterItemProcess {
            get { lock (_inSyncObj) { return _dontSleepAfterItemProcess; } }
            set { lock (_inSyncObj) { _dontSleepAfterItemProcess = value; } }
        }

        /// <summary>
        /// Should all items in outQueue be processed at on Update event.
        /// By default, one item per Update event will be processed.
        /// </summary>
        protected bool ProcessAllItemsAtUpdate;

        /// <summary>
        /// Length of input data queue.
        /// </summary>
        protected int InputQueueLength {
            get { lock (_inSyncObj) { return _inQueue.Count; } }
        }

        bool _isWorkerStarted;

        bool _dontSleepAfterItemProcess;

        object _inSyncObj = new object ();

        object _outSyncObj = new object ();

        List<T> _inQueue = new List<T> (64);

        List<T> _outQueue = new List<T> (64);

        Thread _thread;

        protected override void OnConstruct () {
            base.OnConstruct ();
            _thread = new Thread (OnBackgroundThreadProc);
            _thread.Start ();
        }

        protected override void OnDestruct () {
            try {
                if (_thread != null) {
                    _thread.Interrupt ();
                    _thread.Join (100);
                }
            } catch (Exception ex) { Debug.LogError (ex); }
            _thread = null;
            base.OnDestruct ();
        }

        protected virtual void Update () {
            OnWorkerProcessOutQueueAtForeground ();
        }

        /// <summary>
        /// Method for custom reaction on thread start. Important - will be called at background thread!
        /// </summary>
        protected virtual void OnWorkerStartInBackground () { }

        /// <summary>
        /// Method for custom reaction on thread stop. Important - will be called at background thread!
        /// </summary>
        protected virtual void OnWorkerStopInBackground () { }

        /// <summary>
        /// Method for processing item. Important - will be called at background thread!
        /// </summary>
        /// <param name="item">Item for processing.</param>
        /// <returns>Result of processing.</returns>
        protected abstract T OnWorkerTickInBackground (T item);

        /// <summary>
        /// Method for custom reaction on receiving result of background processing.
        /// </summary>
        /// <param name="result">Result of processing</param>
        protected abstract void OnResultFromWorker (T result);

        /// <summary>
        /// Method for run processing outQueue. Important - should be called in unity thread!
        /// </summary>
        protected void OnWorkerProcessOutQueueAtForeground () {
            lock (_outSyncObj) {
                var count = _outQueue.Count;
                if (count == 0) {
                    return;
                }
                var processAll = ProcessAllItemsAtUpdate;
                if (!processAll) {
                    count = 1;
                }
                for (var i = 0; i < count; i++) {
                    OnResultFromWorker (_outQueue[i]);
                }
                if (!processAll) {
                    _outQueue.RemoveAt (0);
                } else {
                    _outQueue.Clear ();
                }
            }
        }

        void OnBackgroundThreadProc () {
            lock (_inSyncObj) {
                _isWorkerStarted = true;
            }
            try {
                OnWorkerStartInBackground ();
                var dontSleep = false;
                T item = default (T);
                bool isFound;
                while (Thread.CurrentThread.IsAlive) {
                    lock (_inSyncObj) {
                        dontSleep = _dontSleepAfterItemProcess;
                        isFound = _inQueue.Count > 0;
                        if (isFound) {
                            item = _inQueue[0];
                            _inQueue.RemoveAt (0);
                        }
                    }
                    if (isFound) {
                        var result = OnWorkerTickInBackground (item);
                        lock (_outSyncObj) {
                            _outQueue.Add (result);
                        }
                    }
                    if (!isFound || !dontSleep) {
                        Thread.Sleep (1);
                    }
                }
            } catch { }
            lock (_inSyncObj) {
                _isWorkerStarted = false;
                _inQueue.Clear ();
            }
            lock (_outSyncObj) {
                _outQueue.Clear ();
            }
            OnWorkerStopInBackground ();
        }

        public bool EnqueueItem (T item) {
            lock (_inSyncObj) {
                if (!_isWorkerStarted) {
#if UNITY_EDITOR
                    Debug.LogWarning ("Worker not started");
#endif
                    return false;
                }
                _inQueue.Add (item);
            }
            return true;
        }
    }
}
#endif