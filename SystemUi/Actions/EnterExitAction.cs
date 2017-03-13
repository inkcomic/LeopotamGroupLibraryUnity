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
    public struct EnterActionData {
        public GameObject Receiver;

        public string Group;

        public PointerEventData EventData;

        public EnterActionData (GameObject receiver, string group, PointerEventData eventData) {
            Receiver = receiver;
            Group = group;
            EventData = eventData;
        }
    }

    public struct ExitActionData {
        public GameObject Receiver;

        public string Group;

        public PointerEventData EventData;

        public ExitActionData (GameObject receiver, string group, PointerEventData eventData) {
            Receiver = receiver;
            Group = group;
            EventData = eventData;
        }
    }

    public class EnterExitAction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        [SerializeField]
        string _group;

        void IPointerEnterHandler.OnPointerEnter (PointerEventData eventData) {
            Singleton.Get<UnityEventBus> ()
                .Publish<EnterActionData> (new EnterActionData (gameObject, _group, eventData));
        }

        void IPointerExitHandler.OnPointerExit (PointerEventData eventData) {
            Singleton.Get<UnityEventBus> ()
                .Publish<ExitActionData> (new ExitActionData (gameObject, _group, eventData));
        }
    }
}