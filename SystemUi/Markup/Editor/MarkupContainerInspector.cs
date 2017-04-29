// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using UnityEditor;
using UnityEngine;

namespace LeopotamGroup.SystemUi.Markup.UnityEditors {
    [CanEditMultipleObjects]
    [CustomEditor (typeof (MarkupContainer), true)]
    sealed class MarkupContainerInspector : Editor {
        public override void OnInspectorGUI () {
            DrawDefaultInspector ();

            if (!Application.isPlaying) {
                if (GUILayout.Button ("Remove visuals")) {
                    MarkupContainer container;
                    foreach (var item in targets) {
                        container = item as MarkupContainer;
                        container.Clear ();
                    }
                }
                if (GUILayout.Button ("Create visuals")) {
                    MarkupContainer container;
                    foreach (var item in targets) {
                        container = item as MarkupContainer;
                        container.Clear ();
                        container.CreateVisuals ();
                    }
                }
            }
        }
    }
}