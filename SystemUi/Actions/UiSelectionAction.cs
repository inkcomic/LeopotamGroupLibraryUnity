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
    /// Event data of UiSelectAction.
    /// </summary>
    public sealed class UiSelectActionData : UiActionDataBase { }

    /// <summary>
    /// Event data of UiDeselectAction.
    /// </summary>
    public sealed class UiDeselectActionData : UiActionDataBase { }

    /// <summary>
    /// Ui action for processing OnSelect / OnDeselect events.
    /// </summary>
    public sealed class UiSelectionAction : UiActionBase, ISelectHandler, IDeselectHandler {
        void IDeselectHandler.OnDeselect (BaseEventData eventData) {
            var action = UiActionDataBase.GetFromPool<UiDeselectActionData> (gameObject, GroupId, eventData);
            Singleton.Get<UnityEventBus> ().Publish<UiDeselectActionData> (action);
            action.RecycleToPool ();
        }

        void ISelectHandler.OnSelect (BaseEventData eventData) {
            var action = UiActionDataBase.GetFromPool<UiSelectActionData> (gameObject, GroupId, eventData);
            Singleton.Get<UnityEventBus> ().Publish<UiSelectActionData> (action);
            action.RecycleToPool ();
        }
    }
}