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
    public struct ClickActionData {
        public GameObject Receiver;

        public string Group;

        public PointerEventData EventData;

        public ClickActionData (GameObject receiver, string group, PointerEventData eventData) {
            Receiver = receiver;
            Group = group;
            EventData = eventData;
        }
    }

    public class ClickAction : MonoBehaviour, IPointerClickHandler {
        [SerializeField]
        string _group;

        void IPointerClickHandler.OnPointerClick (PointerEventData eventData) {
            Singleton.Get<UnityEventBus> ()
                .Publish<ClickActionData> (new ClickActionData (gameObject, _group, eventData));
        }
    }
}