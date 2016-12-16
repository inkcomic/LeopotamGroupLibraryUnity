
// -------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2016 Leopotam <leopotam@gmail.com>
// -------------------------------------------------------

using LeopotamGroup.Gui.Common;
using LeopotamGroup.Gui.Layout;
using UnityEngine;

namespace LeopotamGroup.Gui.Interaction {
    [RequireComponent (typeof (GuiEventReceiver))]
    public sealed class GuiDragScrollView : MonoBehaviour {
        public GuiScrollView ScrollView = null;

        GuiEventReceiver _eventReceiver;

        void Awake () {
            _eventReceiver = GetComponent<GuiEventReceiver> ();
        }

        void OnEnable () {
            _eventReceiver.OnDrag.AddListener (OnDrag);
            _eventReceiver.OnPress.AddListener (OnPress);
            if (ScrollView == null) {
                ScrollView = GetComponentInParent<GuiScrollView> ();
            }
            if (ScrollView == null) {
                enabled = false;
            }
        }

        void OnDisable () {
            _eventReceiver.OnDrag.RemoveListener (OnDrag);
            _eventReceiver.OnPress.RemoveListener (OnPress);
        }

        void OnDrag (GuiEventReceiver receiver, GuiTouchEventArg args) {
            ScrollView.ScrollRelative (args.Delta);
        }

        void OnPress (GuiEventReceiver receiver, GuiTouchEventArg args) {
            if (!args.State) {
                ScrollView.Validate ();
            }
        }
    }
}