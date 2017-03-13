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
    public struct ScrollActionData {
        public GameObject Receiver;

        public string Group;

        public PointerEventData EventData;

        public ScrollActionData (GameObject receiver, string group, PointerEventData eventData) {
            Receiver = receiver;
            Group = group;
            EventData = eventData;
        }

        public bool IsGroupValid (string group) {
            return string.CompareOrdinal (Group, group) == 0;
        }
    }

    public class ScrollAction : MonoBehaviour, IScrollHandler {
        [SerializeField]
        string _group;

        void IScrollHandler.OnScroll (PointerEventData eventData) {
            Singleton.Get<UnityEventBus> ()
                .Publish<ScrollActionData> (new ScrollActionData (gameObject, _group, eventData));
        }
    }
}