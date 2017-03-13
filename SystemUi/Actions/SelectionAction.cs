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
    public struct SelectActionData {
        public GameObject Receiver;

        public string Group;

        public BaseEventData EventData;

        public SelectActionData (GameObject receiver, string group, BaseEventData eventData) {
            Receiver = receiver;
            Group = group;
            EventData = eventData;
        }
    }

    public struct DeselectActionData {
        public GameObject Receiver;

        public string Group;

        public BaseEventData EventData;

        public DeselectActionData (GameObject receiver, string group, BaseEventData eventData) {
            Receiver = receiver;
            Group = group;
            EventData = eventData;
        }
    }

    public class SelectionAction : MonoBehaviour, ISelectHandler, IDeselectHandler {
        [SerializeField]
        string _group;

        void IDeselectHandler.OnDeselect (BaseEventData eventData) {
            Singleton.Get<UnityEventBus> ()
                .Publish<DeselectActionData> (new DeselectActionData (gameObject, _group, eventData));
        }

        void ISelectHandler.OnSelect (BaseEventData eventData) {
            Singleton.Get<UnityEventBus> ()
                .Publish<SelectActionData> (new SelectActionData (gameObject, _group, eventData));
        }
    }
}