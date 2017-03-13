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
    public struct PressActionData {
        public GameObject Receiver;

        public string Group;

        public PointerEventData EventData;

        public PressActionData (GameObject receiver, string group, PointerEventData eventData) {
            Receiver = receiver;
            Group = group;
            EventData = eventData;
        }
    }

    public struct ReleaseActionData {
        public GameObject Receiver;

        public string Group;

        public PointerEventData EventData;

        public ReleaseActionData (GameObject receiver, string group, PointerEventData eventData) {
            Receiver = receiver;
            Group = group;
            EventData = eventData;
        }
    }

    public class PressReleaseAction : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
        [SerializeField]
        string _group;

        void IPointerDownHandler.OnPointerDown (PointerEventData eventData) {
            Singleton.Get<UnityEventBus> ()
                .Publish<PressActionData> (new PressActionData (gameObject, _group, eventData));
        }

        void IPointerUpHandler.OnPointerUp (PointerEventData eventData) {
            Singleton.Get<UnityEventBus> ()
                .Publish<ReleaseActionData> (new ReleaseActionData (gameObject, _group, eventData));
        }
    }
}