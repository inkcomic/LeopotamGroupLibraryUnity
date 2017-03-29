// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using LeopotamGroup.Collections;
using UnityEngine;

namespace LeopotamGroup.Events {
    /// <summary>
    /// EventBus implementation.
    /// </summary>
    public sealed class EventBus {
        readonly object _syncObj = new object ();

        readonly Dictionary<Type, object> _events = new Dictionary<Type, object> (32);

        // faster than hashset / dictionary on small amount of items.
        readonly FastList<Type> _eventsInCall = new FastList<Type> (32);

        readonly Dictionary<Type, FastList<object>> _eventSpecificListCaches = new Dictionary<Type, FastList<object>> (32);

        public EventBus () {
            _eventsInCall.UseCastToObjectComparer (true);
        }

        /// <summary>
        /// Subscribe callback to be raised on specific event.
        /// </summary>
        /// <param name="eventAction">Callback. Should returns state - is event interrupted / should not be processed by
        /// next callbacks or not.</param>
        /// <param name="insertAsFirst">Is callback should be raised first in sequence.</param>
        public void Subscribe<T> (Func<T, bool> eventAction, bool insertAsFirst = false) {
            if (eventAction == null) {
                return;
            }
            var eventType = typeof (T);
            lock (_syncObj) {
                if (!_events.ContainsKey (eventType)) {
                    _events[eventType] = new FastList<Func<T, bool>> (16);
                    _eventSpecificListCaches[eventType] = new FastList<object> (16);
                }
                var list = _events[eventType] as FastList<Func<T, bool>>;
                if (list == null) {
#if UNITY_EDITOR
                    Debug.LogError ("Cant subscribe to event: " + eventType.Name);
#endif
                    return;
                }
                if (!list.Contains (eventAction)) {
                    if (insertAsFirst) {
                        list.Insert (0, eventAction);
                    } else {
                        list.Add (eventAction);
                    }
                }
            }
        }

        /// <summary>
        /// Unsubscribe callback.
        /// </summary>
        /// <param name="eventAction">Event action.</param>
        /// <param name="keepEvent">GC optimization - clear only callback list and keep event for future use.</param>
        public void Unsubscribe<T> (Func<T, bool> eventAction, bool keepEvent = false) {
            if (eventAction == null) {
                return;
            }
            var eventType = typeof (T);
            lock (_syncObj) {
                if (_events.ContainsKey (eventType)) {
                    var list = _events[eventType] as FastList<Func<T, bool>>;
                    if (list != null) {
                        var id = list.IndexOf (eventAction);
                        if (id != -1) {
                            list.RemoveAt (id);
                            if (list.Count == 0 && !keepEvent) {
                                _events.Remove (eventType);
                                _eventSpecificListCaches.Remove (eventType);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Unsubscribe all callbacks from event.
        /// </summary>
        /// <param name="keepEvent">GC optimization - clear only callback list and keep event for future use.</param>
        public void UnsubscribeAll<T> (bool keepEvent = false) {
            var eventType = typeof (T);
            lock (_syncObj) {
                if (_events.ContainsKey (eventType)) {
                    if (keepEvent) {
                        ((FastList<Func<T, bool>>) _events[eventType]).Clear ();
                    } else {
                        _events.Remove (eventType);
                        _eventSpecificListCaches.Remove (eventType);
                    }
                }
            }
        }

        /// <summary>
        /// Unsubscribe all listeneres and clear all events.
        /// </summary>
        public void UnsubscribeAndClearAllEvents () {
            lock (_syncObj) {
                _events.Clear ();
                _eventSpecificListCaches.Clear ();
            }
        }

        /// <summary>
        /// Publish event.
        /// </summary>
        /// <param name="eventMessage">Event message.</param>
        public void Publish<T> (T eventMessage) {
            var eventType = typeof (T);
            FastList<Func<T, bool>> list = null;
            lock (_syncObj) {
                if (_eventsInCall.Contains (eventType)) {
                    Debug.LogError ("Already in calling of " + eventType.Name);
                    return;
                }
                object objList;
                if (_events.TryGetValue (eventType, out objList)) {
                    list = (FastList<Func<T, bool>>) objList;

                    // kept for no new GC alloc, but empty.
                    if (list.Count == 0) {
                        list = null;
                    }
                }
                if (list != null) {
                    _eventsInCall.Add (eventType);
                }
            }
            if (list != null) {
                var cacheList = _eventSpecificListCaches[eventType];
                int i;
                int iMax;
                var listData = list.GetData (out iMax);
                cacheList.Reserve (iMax, true, false);
                var cacheListData = cacheList.GetData ();
                // we cant use direct copy because cached list dont know T-generic type of event.
                for (i = 0; i < iMax; i++) {
                    cacheListData[i] = listData[i];
                }
                try {
                    for (i = 0; i < iMax; i++) {
                        if (((Func<T, bool>) cacheListData[i]) (eventMessage)) {
                            // Event was interrupted / processed, we can exit.
                            return;
                        }
                    }
                } finally {
                    cacheList.Clear ();
                    lock (_syncObj) {
                        _eventsInCall.Remove (eventType);
                    }
                }
            }
        }
    }
}