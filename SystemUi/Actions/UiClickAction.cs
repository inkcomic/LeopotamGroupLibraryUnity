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
    /// Event data of UiClickAction.
    /// </summary>
    public sealed class UiClickActionData : UiActionDataPointerBase { }

    /// <summary>
    /// Ui action for processing OnClick events.
    /// </summary>
    public sealed class UiClickAction : UiActionBase, IPointerClickHandler {
        void IPointerClickHandler.OnPointerClick (PointerEventData eventData) {
            var action = UiActionDataBase.GetFromPool<UiClickActionData> (gameObject, GroupId, eventData);
            Singleton.Get<UnityEventBus> ().Publish<UiClickActionData> (action);
            action.RecycleToPool ();
        }
    }
}