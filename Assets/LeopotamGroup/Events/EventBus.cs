﻿//-------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2016 Leopotam <leopotam@gmail.com>
//-------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;

namespace LeopotamGroup.Events {
    /// <summary>
    /// EventBus implementation.
    /// </summary>
    public sealed class EventBus {
        readonly object _syncObj = new object ();

        readonly Dictionary<Type, object> _events = new Dictionary<Type, object> ();

        readonly HashSet<Type> _eventsInCall = new HashSet<Type> ();

        readonly List<object> _currentEventCallList = new List<object> ();

        /// <summary>
        /// Subscribe callback to be raised on specific event.
        /// </summary>
        /// <param name="eventAction">Callback. Should returns state - is event interrupted / should not be processed by next callbacks or not.</param>
        /// <param name="insertAsFirst">Is callback should be raised first in sequence.</param>
        public void Subscribe<T> (Func<T, bool> eventAction, bool insertAsFirst = false) where T : class {
            if (eventAction == null) {
                return;
            }
            var eventType = typeof (T);
            lock (_syncObj) {
                if (!_events.ContainsKey (eventType)) {
                    _events[eventType] = new List<Func<T, bool>> ();
                }
                var list = _events[eventType] as List<Func<T, bool>>;
                if (list == null) {
                    Debug.LogError ("Cant subscribe to event: " + eventType.Name);
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
        public void Unsubscribe<T> (Func<T, bool> eventAction, bool keepEvent = false) where T : class {
            if (eventAction == null) {
                return;
            }
            var eventType = typeof (T);
            lock (_syncObj) {
                if (_events.ContainsKey (eventType)) {
                    var list = _events[eventType] as List<Func<T, bool>>;
                    if (list != null) {
                        var id = list.IndexOf (eventAction);
                        if (id != -1) {
                            list.RemoveAt (id);

                            if (list.Count == 0) {
                                _events.Remove (eventType);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Unsubscribes all callbacks from event.
        /// </summary>
        /// <param name="keepEvent">GC optimization - clear only callback list and keep event for future use.</param>
        public void UnsubscribeAll<T> (bool keepEvent = false) where T : class {
            var eventType = typeof (T);
            lock (_syncObj) {
                if (_events.ContainsKey (eventType)) {
                    if (keepEvent) {
                        (_events[eventType] as List<Func<T, bool>>).Clear ();
                    } else {
                        _events.Remove (eventType);
                    }
                }
            }
        }

        /// <summary>
        /// Unsubscribes all events.
        /// </summary>
        public void UnsubscribeAllEvents () {
            lock (_syncObj) {
                _events.Clear ();
            }
        }

        /// <summary>
        /// Publish event.
        /// </summary>
        /// <param name="eventMessage">Event message.</param>
        public void Publish<T> (T eventMessage) where T : class {
            if (eventMessage == null) {
                return;
            }
            var eventType = typeof (T);
            List<Func<T, bool>> list = null;
            lock (_syncObj) {
                if (_eventsInCall.Contains (eventType)) {
                    Debug.LogError ("Already in calling of " + eventType.Name);
                    return;
                }
                if (_events.ContainsKey (eventType)) {
                    list = _events[eventType] as List<Func<T, bool>>;
                }
                if (list != null) {
                    _eventsInCall.Add (eventType);
                }
            }
            if (list != null) {
                int i;
                var iMax = list.Count;
                for (i = 0; i < iMax; i++) {
                    _currentEventCallList.Add (list[i]);
                }
                try {
                    for (i = 0; i < iMax; i++) {
                        if ((_currentEventCallList[i] as Func<T, bool>) (eventMessage)) {
                            // Event was interrupted / processed, we can exit.
                            return;
                        }
                    }
                } finally {
                    _currentEventCallList.Clear ();
                    lock (_syncObj) {
                        _eventsInCall.Remove (eventType);
                    }
                }
            }
        }
    }
}