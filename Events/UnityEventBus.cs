// -------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// -------------------------------------------------------

using System;
using LeopotamGroup.Common;

namespace LeopotamGroup.Events {
    /// <summary>
    /// Event bus singleton, local for unity scene.
    /// </summary>
    public class UnityEventBus : UnitySingletonBase {
        protected EventBus EventBus;

        protected override void OnConstruct () {
            base.OnConstruct ();
            EventBus = new EventBus ();
        }

        protected override void OnDestruct () {
            UnsubscribeAndClearAllEvents ();
            base.OnDestruct ();
        }

        /// <summary>
        /// Subscribe callback to be raised on specific event.
        /// </summary>
        /// <param name="eventAction">Callback.</param>
        /// <param name="insertAsFirst">Is callback should be raised first in sequence.</param>
        public void Subscribe<T> (Func<T, bool> eventAction, bool insertAsFirst = false) {
            EventBus.Subscribe (eventAction, insertAsFirst);
        }

        /// <summary>
        /// Unsubscribe callback.
        /// </summary>
        /// <param name="eventAction">Event action.</param>
        /// <param name="keepEvent">GC optimization - clear only callback list and keep event for future use.</param>
        public void Unsubscribe<T> (Func<T, bool> eventAction, bool keepEvent = false) {
            EventBus.Unsubscribe (eventAction, keepEvent);
        }

        /// <summary>
        /// Unsubscribe all callbacks from event.
        /// </summary>
        /// <param name="keepEvent">GC optimization - clear only callback list and keep event for future use.</param>
        public void UnsubscribeAll<T> (bool keepEvent = false) {
            EventBus.UnsubscribeAll<T> (keepEvent);
        }

        /// <summary>
        /// Unsubscribe all listeneres and clear all events.
        /// </summary>
        public void UnsubscribeAndClearAllEvents () {
            EventBus.UnsubscribeAndClearAllEvents ();
        }

        /// <summary>
        /// Publish event.
        /// </summary>
        /// <param name="eventMessage">Event message.</param>
        public void Publish<T> (T eventMessage) {
            EventBus.Publish (eventMessage);
        }
    }
}