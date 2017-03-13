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
    public struct BeginDragActionData {
        public GameObject Receiver;

        public string Group;

        public PointerEventData EventData;

        public BeginDragActionData (GameObject receiver, string group, PointerEventData eventData) {
            Receiver = receiver;
            Group = group;
            EventData = eventData;
        }
    }

    public struct DragActionData {
        public GameObject Receiver;

        public string Group;

        public PointerEventData EventData;

        public DragActionData (GameObject receiver, string group, PointerEventData eventData) {
            Receiver = receiver;
            Group = group;
            EventData = eventData;
        }
    }

    public struct EndDragActionData {
        public GameObject Receiver;

        public string Group;

        public PointerEventData EventData;

        public EndDragActionData (GameObject receiver, string group, PointerEventData eventData) {
            Receiver = receiver;
            Group = group;
            EventData = eventData;
        }
    }

    public class DragAction : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
        [SerializeField]
        string _group;

        void IBeginDragHandler.OnBeginDrag (PointerEventData eventData) {
            Singleton.Get<UnityEventBus> ()
                .Publish<BeginDragActionData> (new BeginDragActionData (gameObject, _group, eventData));
        }

        void IDragHandler.OnDrag (PointerEventData eventData) {
            Singleton.Get<UnityEventBus> ()
                .Publish<DragActionData> (new DragActionData (gameObject, _group, eventData));
        }

        void IEndDragHandler.OnEndDrag (PointerEventData eventData) {
            Singleton.Get<UnityEventBus> ()
                .Publish<EndDragActionData> (new EndDragActionData (gameObject, _group, eventData));
        }
    }
}