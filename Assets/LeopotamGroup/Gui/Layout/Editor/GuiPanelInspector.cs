//-------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2016 Leopotam <leopotam@gmail.com>
//-------------------------------------------------------

using LeopotamGroup.Gui.Common;
using LeopotamGroup.Gui.UnityEditors;
using UnityEditor;
using UnityEngine;

namespace LeopotamGroup.Gui.Layout.UnityEditors {
    [CustomEditor (typeof (GuiPanel))]
    sealed class GuiPanelInspector : Editor {
        SerializedProperty _depthProperty;

        SerializedProperty _clipProperty;

        SerializedProperty _clipWidthProperty;

        SerializedProperty _clipHeightProperty;

        static GUIContent _clipTypeGuiContent = new GUIContent ("Clip type");

        void OnEnable () {
            _depthProperty = serializedObject.FindProperty ("_depth");
            _clipProperty = serializedObject.FindProperty ("_clipType");
            _clipWidthProperty = serializedObject.FindProperty ("_clipWidth");
            _clipHeightProperty = serializedObject.FindProperty ("_clipHeight");
        }

        public override void OnInspectorGUI () {
            serializedObject.Update ();

            EditorGUILayout.PropertyField (_clipProperty, _clipTypeGuiContent);
            var clipType = (GuiPanelClipType) _clipProperty.enumValueIndex;
            if (clipType == GuiPanelClipType.Range) {
                EditorGUILayout.PropertyField (_clipWidthProperty);
                if (_clipWidthProperty.intValue < 0) {
                    _clipWidthProperty.intValue = 0;
                }

                EditorGUILayout.PropertyField (_clipHeightProperty);
                if (_clipHeightProperty.intValue < 0) {
                    _clipHeightProperty.intValue = 0;
                }
            }

            EditorGUILayout.IntSlider (_depthProperty, -10, 10);

            if (serializedObject.ApplyModifiedProperties () || EditorIntegration.IsUndo ()) {
                EditorIntegration.UpdateVisuals (target);
            }
        }

        [DrawGizmo (GizmoType.NonSelected | GizmoType.InSelectionHierarchy)]
        static void OnDrawRootGizmo (GuiPanel panel, GizmoType gizmoType) {
            if (panel.enabled && panel.ClipType == GuiPanelClipType.Range) {
                Gizmos.color = (gizmoType & GizmoType.InSelectionHierarchy) != 0 ? Color.red : new Color (0.5f, 0f, 0f);
                Gizmos.DrawWireCube (panel.transform.position, new Vector3 (panel.ClipWidth, panel.ClipHeight, 0f));
            }
        }
    }
}