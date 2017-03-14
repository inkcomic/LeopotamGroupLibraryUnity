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
    /// Event data of UiDropAction.
    /// </summary>
    public sealed class UiDropActionData : UiActionDataPointerBase { }

    /// <summary>
    /// Ui action for processing OnDrop events.
    /// </summary>
    public sealed class UiDropAction : UiActionBase, IDropHandler {
        void IDropHandler.OnDrop (PointerEventData eventData) {
            var action = UiActionDataBase.GetFromPool<UiDropActionData> (gameObject, GroupId, eventData);
            Singleton.Get<UnityEventBus> ().Publish<UiDropActionData> (action);
            action.RecycleToPool ();
        }
    }
}