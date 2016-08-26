//-------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2016 Leopotam <leopotam@gmail.com>
//-------------------------------------------------------

using System;

namespace LeopotamGroup.Events {
    /// <summary>
    /// Global event bus.
    /// </summary>
    public sealed class GlobalEventBus {
        public static readonly GlobalEventBus Instance = new GlobalEventBus ();

        readonly EventBus _eventBus;

        GlobalEventBus () {
            _eventBus = new EventBus ();
        }

        /// <summary>
        /// Subscribe callback to be raised on specific event.
        /// </summary>
        /// <param name="eventAction">Callback.</param>
        /// <param name="insertAsFirst">Is callback should be raised first in sequence.</param>
        public void Subscribe<T> (Func<T, bool> eventAction, bool insertAsFirst = false) where T : class {
            _eventBus.Subscribe (eventAction, insertAsFirst);
        }

        /// <summary>
        /// Unsubscribe callback.
        /// </summary>
        /// <param name="eventAction">Event action.</param>
        /// <param name="keepEvent">GC optimization - clear only callback list and keep event for future use.</param>
        public void Unsubscribe<T> (Func<T, bool> eventAction, bool keepEvent = false) where T : class {
            _eventBus.Unsubscribe (eventAction, keepEvent);
        }

        /// <summary>
        /// Unsubscribes all callbacks from event.
        /// </summary>
        /// <param name="keepEvent">GC optimization - clear only callback list and keep event for future use.</param>
        public void UnsubscribeAll<T> (bool keepEvent = false) where T : class {
            _eventBus.UnsubscribeAll<T> (keepEvent);
        }

        /// <summary>
        /// Unsubscribes all events.
        /// </summary>
        public void UnsubscribeAllEvents () {
            _eventBus.UnsubscribeAllEvents ();
        }

        /// <summary>
        /// Publish event.
        /// </summary>
        /// <param name="eventMessage">Event message.</param>
        public void Publish<T> (T eventMessage) where T : class {
            _eventBus.Publish (eventMessage);
        }
    }
}