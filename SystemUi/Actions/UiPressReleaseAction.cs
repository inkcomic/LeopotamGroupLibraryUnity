// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using LeopotamGroup.Common;
using LeopotamGroup.Events;
using UnityEngine.EventSystems;

namespace LeopotamGroup.SystemUi.Actions {
    /// <summary>
    /// Event data of UiPressAction.
    /// </summary>
    public sealed class UiPressActionData : UiActionDataPointerBase { }

    /// <summary>
    /// Event data of UiReleaseAction.
    /// </summary>
    public sealed class UiReleaseActionData : UiActionDataPointerBase { }

    /// <summary>
    /// Ui action for processing OnPress / OnRelease events.
    /// </summary>
    public sealed class UiPressReleaseAction : UiActionBase, IPointerDownHandler, IPointerUpHandler {
        void IPointerDownHandler.OnPointerDown (PointerEventData eventData) {
            var action = UiActionDataBase.GetFromPool<UiPressActionData> (gameObject, GroupId, eventData);
            Singleton.Get<UnityEventBus> ().Publish<UiPressActionData> (action);
            action.RecycleToPool ();
        }

        void IPointerUpHandler.OnPointerUp (PointerEventData eventData) {
            var action = UiActionDataBase.GetFromPool<UiReleaseActionData> (gameObject, GroupId, eventData);
            Singleton.Get<UnityEventBus> ().Publish<UiReleaseActionData> (action);
            action.RecycleToPool ();
        }
    }
}