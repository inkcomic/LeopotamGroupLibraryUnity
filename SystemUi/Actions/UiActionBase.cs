// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using LeopotamGroup.Common;
using LeopotamGroup.Events;
using UnityEngine;

namespace LeopotamGroup.SystemUi.Actions {
    /// <summary>
    /// Base class for ui action.
    /// </summary>
    public abstract class UiActionBase : MonoBehaviour {
        [SerializeField]
        string _group;

        protected int GroupId { get; private set; }

        protected virtual void Awake () {
            SetGroup (_group);
        }

        protected virtual void Start () {
            // Force create eventbus object.
            Services.Get<UnityEventBus> (true);
        }

        protected void SendActionData<T> (T data) {
            if (Services.IsTypeRegistered<UnityEventBus> ()) {
                Services.Get<UnityEventBus> ().Publish<T> (data);
            }
        }

        public void SetGroup (string group) {
            _group = group;
            GroupId = _group.GetUiActionGroupId ();
        }
    }
}