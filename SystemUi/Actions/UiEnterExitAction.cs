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
    /// Event data of UiEnterAction.
    /// </summary>
    public sealed class UiEnterActionData : UiActionDataPointerBase { }

    /// <summary>
    /// Event data of UiExitAction.
    /// </summary>
    public sealed class UiExitActionData : UiActionDataPointerBase { }

    /// <summary>
    /// Ui action for processing OnEnter / OnExit events.
    /// </summary>
    public sealed class UiEnterExitAction : UiActionBase, IPointerEnterHandler, IPointerExitHandler {
        void IPointerEnterHandler.OnPointerEnter (PointerEventData eventData) {
            var action = UiActionDataBase.GetFromPool<UiEnterActionData> (gameObject, GroupId, eventData);
            Singleton.Get<UnityEventBus> ().Publish<UiEnterActionData> (action);
            action.RecycleToPool ();
        }

        void IPointerExitHandler.OnPointerExit (PointerEventData eventData) {
            var action = UiActionDataBase.GetFromPool<UiExitActionData> (gameObject, GroupId, eventData);
            Singleton.Get<UnityEventBus> ().Publish<UiExitActionData> (action);
            action.RecycleToPool ();
        }
    }
}