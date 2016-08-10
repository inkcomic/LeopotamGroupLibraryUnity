//-------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2016 Leopotam <leopotam@gmail.com>
//-------------------------------------------------------

using LeopotamGroup.Gui.Common.UnityEditors;
using LeopotamGroup.Gui.Widgets;
using UnityEditor;

namespace LeopotamGroup.Gui.Widgets.UnityEditors {
    [CustomEditor (typeof (GuiButton))]
    sealed class GuiButtonInspector : GuiEventReceiverInspector {
        SerializedProperty _visualsProperty;

        SerializedProperty _enableColorProperty;

        SerializedProperty _activeColorProperty;

        SerializedProperty _disableColorProperty;

        SerializedProperty _scaleOnPressProperty;

        SerializedProperty _tweenTimeProperty;

        protected override void OnEnable () {
            base.OnEnable ();
            _visualsProperty = serializedObject.FindProperty ("Visuals");
            _enableColorProperty = serializedObject.FindProperty ("EnableColor");
            _activeColorProperty = serializedObject.FindProperty ("ActiveColor");
            _disableColorProperty = serializedObject.FindProperty ("DisableColor");
            _scaleOnPressProperty = serializedObject.FindProperty ("ScaleOnPress");
            _tweenTimeProperty = serializedObject.FindProperty ("TweenTime");
        }

        public override void OnInspectorGUI () {
            serializedObject.Update ();

            EditorGUILayout.PropertyField (_visualsProperty, true);
            EditorGUILayout.PropertyField (_enableColorProperty);
            EditorGUILayout.PropertyField (_activeColorProperty);
            EditorGUILayout.PropertyField (_disableColorProperty);
            EditorGUILayout.PropertyField (_scaleOnPressProperty);
            EditorGUILayout.PropertyField (_tweenTimeProperty);

            serializedObject.ApplyModifiedProperties ();

            base.OnInspectorGUI ();
        }
    }
}