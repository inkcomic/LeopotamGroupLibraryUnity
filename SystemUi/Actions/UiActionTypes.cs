// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using LeopotamGroup.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LeopotamGroup.SystemUi.Actions {
    /// <summary>
    /// Base class for data of ui-event for passing through events pipeline.
    /// Dont forget - you cant use provided instance of this class at your callbacks,
    /// it will be automatically recycled after event will be processed.
    /// </summary>
    public abstract class UiActionDataBase {
        /// <summary>
        /// Holder of action-emitter of event.
        /// </summary>
        public GameObject Receiver { get; private set; }

        /// <summary>
        /// Logical group for filtering events.
        /// </summary>
        public int GroupId { get; private set; }

        /// <summary>
        /// Event data from uGui.
        /// </summary>
        public BaseEventData EventData { get; private set; }

        static Dictionary<Type, FastList<object>> _pool = new Dictionary<Type, FastList<object>> (16);

        void Init (GameObject receiver, int groupId, BaseEventData rawEventData) {
            Receiver = receiver;
            GroupId = groupId;
            EventData = rawEventData;
        }

        /// <summary>
        /// Force recycle instance to internal pool. Dont touch it if you dont understand how it works!
        /// </summary>
        public void RecycleToPool () {
            FastList<object> list;
            if (!_pool.TryGetValue (GetType (), out list)) {
                list = new FastList<object> (2);
                _pool[GetType ()] = list;
            }
            Init (null, 0, null);
            list.Add (this);
        }

        /// <summary>
        /// Request and initialize instance of required event data from internal pool.
        /// Dont touch it if you dont understand how it works!
        /// </summary>
        /// <param name="receiver">Holder of action.</param>
        /// <param name="groupId">Logical group of action.</param>
        /// <param name="rawEventData">Event data of action from uGui.</param>
        public static T GetFromPool<T> (GameObject receiver, int groupId, BaseEventData rawEventData) where T : UiActionDataBase, new () {
            FastList<object> list;
            if (!_pool.TryGetValue (typeof (T), out list)) {
                list = new FastList<object> (2);
                _pool[typeof (T)] = list;
            }
            if (list.Count > 0) {
                var inst = list[list.Count - 1];
                list.RemoveLast ();
                (inst as UiActionDataBase).Init (receiver, groupId, rawEventData);
                return inst as T;
            }
            var newInst = new T ();
            (newInst as UiActionDataBase).Init (receiver, groupId, rawEventData);
            return newInst;
        }
    }

    /// <summary>
    /// Base class for ui action.
    /// </summary>
    public abstract class UiActionBase : MonoBehaviour {
        [SerializeField]
        string _group;

        protected int GroupId { get; private set; }

        void Awake () {
            SetGroup (_group);
        }

        public void SetGroup (string group) {
            GroupId = group != null ? group.GetHashCode () : 0;
        }
    }

    /// <summary>
    /// Helper class for data of ui-event data with autocasting uGui data to PointerEventData.
    /// </summary>
    public abstract class UiActionDataPointerBase : UiActionDataBase {
        /// <summary>
        /// Event data from uGui.
        /// </summary>
        public new PointerEventData EventData { get { return base.EventData as PointerEventData; } }
    }
}