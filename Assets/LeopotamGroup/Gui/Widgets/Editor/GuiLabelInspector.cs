//-------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2016 Leopotam <leopotam@gmail.com>
//-------------------------------------------------------

using LeopotamGroup.Gui.Common;
using LeopotamGroup.Gui.UnityEditors;
using UnityEditor;
using UnityEngine;

namespace LeopotamGroup.Gui.Widgets.UnityEditors {
    [CustomEditor (typeof (GuiLabel))]
    sealed class GuiLabelInspector : Editor {
        SerializedProperty _fontProperty;

        SerializedProperty _fontSizeProperty;

        SerializedProperty _alignmentProperty;

        SerializedProperty _textProperty;

        SerializedProperty _widthProperty;

        SerializedProperty _heightProperty;

        SerializedProperty _depthProperty;

        SerializedProperty _colorProperty;

        SerializedProperty _lineHeightProperty;

        SerializedProperty _effectProperty;

        SerializedProperty _effectValueProperty;

        SerializedProperty _effectColorProperty;

        void OnEnable () {
            _fontProperty = serializedObject.FindProperty ("_font");
            _fontSizeProperty = serializedObject.FindProperty ("_fontSize");
            _alignmentProperty = serializedObject.FindProperty ("_alignment");
            _textProperty = serializedObject.FindProperty ("_text");
            _widthProperty = serializedObject.FindProperty ("_width");
            _heightProperty = serializedObject.FindProperty ("_height");
            _depthProperty = serializedObject.FindProperty ("_depth");
            _colorProperty = serializedObject.FindProperty ("_color");
            _lineHeightProperty = serializedObject.FindProperty ("_lineHeight");
            _effectProperty = serializedObject.FindProperty ("_effect");
            _effectValueProperty = serializedObject.FindProperty ("_effectValue");
            _effectColorProperty = serializedObject.FindProperty ("_effectColor");
        }

        public override void OnInspectorGUI () {
            serializedObject.Update ();
            EditorGUILayout.PropertyField (_fontProperty);
            EditorGUILayout.PropertyField (_fontSizeProperty);
            if (_fontSizeProperty.intValue < 4) {
                _fontSizeProperty.intValue = 4;
            }
            EditorGUILayout.PropertyField (_alignmentProperty);
            EditorGUILayout.PropertyField (_textProperty);
            EditorGUILayout.PropertyField (_widthProperty);
            EditorGUILayout.PropertyField (_heightProperty);
            EditorGUILayout.IntSlider (_depthProperty, -GuiWidget.DepthLimit, GuiWidget.DepthLimit);
            EditorGUILayout.PropertyField (_colorProperty);

            EditorGUILayout.PropertyField (_lineHeightProperty);
            if (_lineHeightProperty.floatValue <= 0.1f) {
                _lineHeightProperty.floatValue = 0.1f;
            }

            EditorGUILayout.PropertyField (_effectProperty);

            if ((GuiFontEffect) _effectProperty.enumValueIndex != GuiFontEffect.None) {
                _effectValueProperty.vector2Value = EditorGUILayout.Vector2Field ("Effect Value", _effectValueProperty.vector2Value);
                EditorGUILayout.PropertyField (_effectColorProperty);
            }
                
            if (GUILayout.Button ("Bake scale to widget size")) {
                Undo.RecordObject (target, "leopotamgroup.gui.label.bake-scale-size");
                GuiLabel l;
                foreach (var item in targets) {
                    l = item as GuiLabel;
                    if (l != null) {
                        l.BakeScale ();
                    }
                }
                SceneView.RepaintAll ();
            }

            EditorGUILayout.HelpBox ("Only strings with length <= 75 (except spaces) can be batched.\n\n" +
            "Only strings with length <= 37 and shadow effect (except spaces) can be batched.\n\n" +
            "Only strings with length <= 15 and outline effect (except spaces) can be batched.\n\n" +
            "Copy&paste labels for creating custom shading/glowing and keep batching.", MessageType.Warning);

            if (serializedObject.ApplyModifiedProperties () || EditorIntegration.IsUndo ()) {
                EditorIntegration.UpdateVisuals (target);
            }
        }

        [DrawGizmo (GizmoType.NonSelected | GizmoType.InSelectionHierarchy)]
        static void OnDrawRootGizmo (GuiLabel lbl, GizmoType gizmoType) {
            if (lbl.IsVisible) {
                var tr = lbl.transform;
                var oldColor = Gizmos.color;
                var oldMat = Gizmos.matrix;
                Gizmos.matrix = Matrix4x4.TRS (tr.position, tr.rotation, tr.lossyScale);
                Gizmos.color = (gizmoType & GizmoType.InSelectionHierarchy) != 0 ? Color.yellow : new Color (0.5f, 0.5f, 0f);
                Gizmos.DrawWireCube (Vector3.zero, new Vector3 (lbl.Width, lbl.Height, 0f));
                Gizmos.matrix = oldMat;
                Gizmos.color = oldColor;
            }
        }
    }
}