// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using LeopotamGroup.Common;
using LeopotamGroup.Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LeopotamGroup.SystemUi.Actions {
    /// <summary>
    /// Event data of UiPressAction.
    /// </summary>
    public struct UiPressActionData {
        /// <summary>
        /// Logical group for filtering events.
        /// </summary>
        public int GroupId;

        /// <summary>
        /// Event sender.
        /// </summary>
        public GameObject Sender;

        /// <summary>
        /// Event data from uGui.
        /// </summary>
        public PointerEventData EventData;
    }

    /// <summary>
    /// Event data of UiReleaseAction.
    /// </summary>
    public struct UiReleaseActionData {
        /// <summary>
        /// Logical group for filtering events.
        /// </summary>
        public int GroupId;

        /// <summary>
        /// Event sender.
        /// </summary>
        public GameObject Sender;

        /// <summary>
        /// Event data from uGui.
        /// </summary>
        public PointerEventData EventData;
    }

    /// <summary>
    /// Ui action for processing OnPress / OnRelease events.
    /// </summary>
    public sealed class UiPressReleaseAction : UiActionBase, IPointerDownHandler, IPointerUpHandler {
        void IPointerDownHandler.OnPointerDown (PointerEventData eventData) {
            if (Singleton.IsTypeRegistered<UnityEventBus> ()) {
                var action = new UiPressActionData ();
                action.GroupId = GroupId;
                action.Sender = gameObject;
                action.EventData = eventData;
                Singleton.Get<UnityEventBus> ().Publish<UiPressActionData> (action);
            }
        }

        void IPointerUpHandler.OnPointerUp (PointerEventData eventData) {
            if (Singleton.IsTypeRegistered<UnityEventBus> ()) {
                var action = new UiReleaseActionData ();
                action.GroupId = GroupId;
                action.Sender = gameObject;
                action.EventData = eventData;
                Singleton.Get<UnityEventBus> ().Publish<UiReleaseActionData> (action);
            }
        }
    }
}