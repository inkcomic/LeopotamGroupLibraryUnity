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
    /// Event data of UiBeginDragAction.
    /// </summary>
    public sealed class UiBeginDragActionData : UiActionDataPointerBase { }

    /// <summary>
    /// Event data of UiDragAction.
    /// </summary>
    public sealed class UiDragActionData : UiActionDataPointerBase { }

    /// <summary>
    /// Event data of UiEndDragAction.
    /// </summary>
    public sealed class UiEndDragActionData : UiActionDataPointerBase { }

    /// <summary>
    /// Ui action for processing OnBeginDrag / OnDrag / OnEndDrag events.
    /// </summary>
    public sealed class UiDragAction : UiActionBase, IBeginDragHandler, IDragHandler, IEndDragHandler {
        void IBeginDragHandler.OnBeginDrag (PointerEventData eventData) {
            var action = UiActionDataBase.GetFromPool<UiBeginDragActionData> (gameObject, GroupId, eventData);
            Singleton.Get<UnityEventBus> ().Publish<UiBeginDragActionData> (action);
            action.RecycleToPool ();
        }

        void IDragHandler.OnDrag (PointerEventData eventData) {
            var action = UiActionDataBase.GetFromPool<UiDragActionData> (gameObject, GroupId, eventData);
            Singleton.Get<UnityEventBus> ().Publish<UiDragActionData> (action);
            action.RecycleToPool ();
        }

        void IEndDragHandler.OnEndDrag (PointerEventData eventData) {
            var action = UiActionDataBase.GetFromPool<UiEndDragActionData> (gameObject, GroupId, eventData);
            Singleton.Get<UnityEventBus> ().Publish<UiEndDragActionData> (action);
            action.RecycleToPool ();
        }
    }
}