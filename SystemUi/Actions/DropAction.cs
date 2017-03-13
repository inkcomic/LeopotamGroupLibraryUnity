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
    public struct DropActionData {
        public GameObject Receiver;

        public string Group;

        public PointerEventData EventData;

        public DropActionData (GameObject receiver, string group, PointerEventData eventData) {
            Receiver = receiver;
            Group = group;
            EventData = eventData;
        }

        public bool IsGroupValid (string group) {
            return string.CompareOrdinal (Group, group) == 0;
        }
    }

    public class DropAction : MonoBehaviour, IDropHandler {
        [SerializeField]
        string _group;

        void IDropHandler.OnDrop (PointerEventData eventData) {
            Singleton.Get<UnityEventBus> ()
                .Publish<DropActionData> (new DropActionData (gameObject, _group, eventData));
        }
    }
}