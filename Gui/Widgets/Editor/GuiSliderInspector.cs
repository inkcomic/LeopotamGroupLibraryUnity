
// -------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// -------------------------------------------------------

using LeopotamGroup.Gui.UnityEditors;
using UnityEditor;

namespace LeopotamGroup.Gui.Widgets.UnityEditors {
    [CustomEditor (typeof (GuiSlider))]
    sealed class GuiSliderInspector : Editor {
        SerializedProperty _backgroundProperty;

        SerializedProperty _foregroundProperty;

        SerializedProperty _valueProperty;

        void OnEnable () {
            _backgroundProperty = serializedObject.FindProperty ("_background");
            _foregroundProperty = serializedObject.FindProperty ("_foreground");
            _valueProperty = serializedObject.FindProperty ("_value");
        }

        public override void OnInspectorGUI () {
            serializedObject.Update ();

            EditorGUILayout.PropertyField (_backgroundProperty);
            EditorGUILayout.PropertyField (_foregroundProperty);
            EditorGUILayout.Slider (_valueProperty, 0f, 1f);

            if (serializedObject.ApplyModifiedProperties () || EditorIntegration.IsUndo ()) {
                (target as GuiSlider).UpdateVisuals ();
                EditorIntegration.UpdateVisuals (target);
            }
        }
    }
}