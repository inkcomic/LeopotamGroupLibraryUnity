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
    /// Event data of UiScrollAction.
    /// </summary>
    public sealed class UiScrollActionData : UiActionDataPointerBase { }

    /// <summary>
    /// Ui action for processing OnScroll events.
    /// </summary>
    public sealed class UiScrollAction : UiActionBase, IScrollHandler {
        void IScrollHandler.OnScroll (PointerEventData eventData) {
            var action = UiActionDataBase.GetFromPool<UiScrollActionData> (gameObject, GroupId, eventData);
            Singleton.Get<UnityEventBus> ().Publish<UiScrollActionData> (action);
            action.RecycleToPool ();
        }
    }
}