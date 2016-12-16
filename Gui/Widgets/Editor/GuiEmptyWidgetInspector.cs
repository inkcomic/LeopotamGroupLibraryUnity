
// -------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2016 Leopotam <leopotam@gmail.com>
// -------------------------------------------------------

using LeopotamGroup.Gui.UnityEditors;
using UnityEditor;
using UnityEngine;

namespace LeopotamGroup.Gui.Widgets.UnityEditors {
    [CustomEditor (typeof (GuiEmptyWidget))]
    sealed class GuiEmptyWidgetInspector : Editor {
        SerializedProperty _widthProperty;

        SerializedProperty _heightProperty;

        void OnEnable () {
            _widthProperty = serializedObject.FindProperty ("_width");
            _heightProperty = serializedObject.FindProperty ("_height");
        }

        public override void OnInspectorGUI () {
            serializedObject.Update ();
            EditorGUILayout.PropertyField (_widthProperty);
            EditorGUILayout.PropertyField (_heightProperty);

            if (GUILayout.Button ("Bake scale to widget size")) {
                Undo.RecordObject (target, "leopotamgroup.gui.empty-widget.bake-scale-size");
                GuiWidget l;
                foreach (var item in targets) {
                    l = item as GuiWidget;
                    if (l != null) {
                        l.BakeScale ();
                    }
                }
                SceneView.RepaintAll ();
            }

            if (serializedObject.ApplyModifiedProperties () || EditorIntegration.IsUndo ()) {
                EditorIntegration.UpdateVisuals (target);
            }
        }

        [DrawGizmo (GizmoType.NonSelected | GizmoType.InSelectionHierarchy)]
        static void OnDrawRootGizmo (GuiEmptyWidget w, GizmoType gizmoType) {
            if (w.enabled) {
                var tr = w.transform;
                var oldColor = Gizmos.color;
                var oldMat = Gizmos.matrix;
                Gizmos.matrix = Matrix4x4.TRS (tr.position, tr.rotation, tr.lossyScale);
                Gizmos.color = (gizmoType & GizmoType.InSelectionHierarchy) != 0 ? Color.white : new Color (0.5f, 0.5f, 0f);
                Gizmos.DrawWireCube (Vector3.zero, new Vector3 (w.Width, w.Height, 0f));
                Gizmos.matrix = oldMat;
                Gizmos.color = oldColor;
            }
        }
    }
}