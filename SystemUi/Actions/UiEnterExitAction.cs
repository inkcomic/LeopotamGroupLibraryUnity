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
    /// Event data of UiEnterAction.
    /// </summary>
    public struct UiEnterActionData {
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
    /// Event data of UiExitAction.
    /// </summary>
    public struct UiExitActionData {
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
    /// Ui action for processing OnEnter / OnExit events.
    /// </summary>
    public sealed class UiEnterExitAction : UiActionBase, IPointerEnterHandler, IPointerExitHandler {
        void IPointerEnterHandler.OnPointerEnter (PointerEventData eventData) {
            if (Singleton.IsTypeRegistered<UnityEventBus> ()) {
                var action = new UiEnterActionData ();
                action.GroupId = GroupId;
                action.Sender = gameObject;
                action.EventData = eventData;
                Singleton.Get<UnityEventBus> ().Publish<UiEnterActionData> (action);
            }
        }

        void IPointerExitHandler.OnPointerExit (PointerEventData eventData) {
            if (Singleton.IsTypeRegistered<UnityEventBus> ()) {
                var action = new UiExitActionData ();
                action.GroupId = GroupId;
                action.Sender = gameObject;
                action.EventData = eventData;
                Singleton.Get<UnityEventBus> ().Publish<UiExitActionData> (action);
            }
        }
    }
}