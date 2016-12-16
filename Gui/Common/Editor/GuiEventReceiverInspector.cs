
// -------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2016 Leopotam <leopotam@gmail.com>
// -------------------------------------------------------

using LeopotamGroup.Gui.UnityEditors;
using UnityEditor;
using UnityEngine;

namespace LeopotamGroup.Gui.Common.UnityEditors {
    [CustomEditor (typeof (GuiEventReceiver))]
    class GuiEventReceiverInspector : Editor {
        SerializedProperty _widthProperty;

        SerializedProperty _heightProperty;

        SerializedProperty _depthProperty;

        SerializedProperty _clipPanelProperty;

        SerializedProperty _onPressProperty;

        SerializedProperty _onClickProperty;

        SerializedProperty _onDragProperty;

        protected virtual void OnEnable () {
            _widthProperty = serializedObject.FindProperty ("Width");
            _heightProperty = serializedObject.FindProperty ("Height");
            _depthProperty = serializedObject.FindProperty ("Depth");
            _onPressProperty = serializedObject.FindProperty ("OnPress");
            _onClickProperty = serializedObject.FindProperty ("OnClick");
            _onDragProperty = serializedObject.FindProperty ("OnDrag");
        }

        public override void OnInspectorGUI () {
            serializedObject.Update ();

            EditorGUILayout.PropertyField (_widthProperty);
            EditorGUILayout.PropertyField (_heightProperty);
            EditorGUILayout.IntSlider (_depthProperty, -49, 49);
            EditorGUILayout.PropertyField (_onPressProperty);
            EditorGUILayout.PropertyField (_onClickProperty);
            EditorGUILayout.PropertyField (_onDragProperty);

            if (serializedObject.ApplyModifiedProperties () || EditorIntegration.IsUndo ()) {
                EditorIntegration.UpdateVisuals (target);
            }
        }

        [DrawGizmo (GizmoType.NonSelected | GizmoType.InSelectionHierarchy)]
        static void OnDrawRootGizmo (GuiEventReceiver receiver, GizmoType gizmoType) {
            if (receiver.enabled) {
                var tr = receiver.transform;
                var oldColor = Gizmos.color;
                Gizmos.color = (gizmoType & GizmoType.InSelectionHierarchy) != 0 ? Color.green : new Color (0f, 0.5f, 0f);
                var oldMat = Gizmos.matrix;
                Gizmos.matrix = Matrix4x4.TRS (tr.position, tr.rotation, tr.lossyScale);
                Gizmos.DrawWireCube (Vector3.zero, new Vector3 (receiver.Width, receiver.Height));
                Gizmos.matrix = oldMat;
                Gizmos.color = oldColor;
            }
        }
    }
}