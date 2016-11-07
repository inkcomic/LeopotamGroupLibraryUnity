//-------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2016 Leopotam <leopotam@gmail.com>
//-------------------------------------------------------

using LeopotamGroup.Gui.Layout;
using LeopotamGroup.Gui.UnityEditors;
using LeopotamGroup.Gui.Widgets;
using UnityEditor;
using UnityEngine;

namespace LeopotamGroup.Gui.Common.UnityEditors {
    [CanEditMultipleObjects]
    [CustomEditor (typeof (Transform))]
    sealed class GuiTransformInspector : Editor {
        SerializedProperty _positionProperty;

        SerializedProperty _rotationProperty;

        SerializedProperty _scaleProperty;

        void OnEnable () {
            _positionProperty = serializedObject.FindProperty ("m_LocalPosition");
            _rotationProperty = serializedObject.FindProperty ("m_LocalRotation");
            _scaleProperty = serializedObject.FindProperty ("m_LocalScale");
        }

        public override void OnInspectorGUI () {
            bool isFound = false;
            Transform tr;
            foreach (var item in targets) {
                tr = item as Transform;
                if (tr.GetComponent <GuiWidget> () || tr.GetComponent <GuiPanel> () || tr.GetComponent<GuiEventReceiver> ()) {
                    isFound = true;
                    break;
                }
            }

            serializedObject.Update ();
            EditorIntegration.SetLabelWidth (15f);
            DrawPosition (isFound);
            DrawRotation (isFound);
            DrawScale (isFound);

            serializedObject.ApplyModifiedProperties ();
        }

        void DrawPosition (bool isFound) {
            GUILayout.BeginHorizontal ();
            var isReset = GUILayout.Button ("P", GUILayout.Width (20f));
            EditorGUILayout.PropertyField (_positionProperty.FindPropertyRelative ("x"));
            EditorGUILayout.PropertyField (_positionProperty.FindPropertyRelative ("y"));
            GUI.enabled = !isFound;
            EditorGUILayout.PropertyField (_positionProperty.FindPropertyRelative ("z"));
            GUI.enabled = true;
            GUILayout.EndHorizontal ();
            if (isReset) {
                _positionProperty.FindPropertyRelative ("x").floatValue = 0f;
                _positionProperty.FindPropertyRelative ("y").floatValue = 0f;
                _positionProperty.FindPropertyRelative ("z").floatValue = 0f;
                if (isFound) {
                    foreach (var item in targets) {
                        var state = (item as Transform).gameObject.activeSelf;
                        if (state) {
                            (item as Transform).gameObject.SetActive (false);
                            (item as Transform).gameObject.SetActive (true);
                        }
                    }
                }
            }
        }

        void DrawRotation (bool isFound) {
            GUILayout.BeginHorizontal ();
            var isReset = GUILayout.Button ("R", GUILayout.Width (20f));
            var rot = (serializedObject.targetObject as Transform).localRotation;
            var angles = rot.eulerAngles;

            foreach (var item in targets) {
                if (!SameRotation (rot, ((Transform) item).localRotation)) {
                    EditorGUI.showMixedValue = true;
                    break;
                }
            }

            EditorGUI.BeginChangeCheck ();
            GUI.enabled = !isFound;
            var newX = EditorGUILayout.FloatField ("X", angles.x);
            var newY = EditorGUILayout.FloatField ("Y", angles.y);
            GUI.enabled = true;
            var newZ = EditorGUILayout.FloatField ("Z", angles.z);

            if (EditorGUI.EndChangeCheck ()) {
                Undo.RecordObjects (serializedObject.targetObjects, "leopotamgroup.gui.transform-rotate");
                angles = new Vector3 (newX, newY, newZ);
                foreach (var item in targets) {
                    (item as Transform).localEulerAngles = angles;
                }
                _rotationProperty.serializedObject.SetIsDifferentCacheDirty ();
            }

            GUILayout.EndHorizontal ();
            if (isReset) {
                var rotIdent = Quaternion.identity;
                _rotationProperty.FindPropertyRelative ("x").floatValue = rotIdent.x;
                _rotationProperty.FindPropertyRelative ("y").floatValue = rotIdent.y;
                _rotationProperty.FindPropertyRelative ("z").floatValue = rotIdent.z;
                _rotationProperty.FindPropertyRelative ("w").floatValue = rotIdent.w;
            }
        }

        bool SameRotation (Quaternion a, Quaternion b) {
            return a.x == b.x && a.y == b.y && a.z == b.z && a.w == b.w;
        }

        void DrawScale (bool isFound) {
            var fieldX = _scaleProperty.FindPropertyRelative ("x");
            var fieldY = _scaleProperty.FindPropertyRelative ("y");
            var fieldZ = _scaleProperty.FindPropertyRelative ("z");

            // fix invalid z scale.
            if (isFound && fieldZ.floatValue != 1f) {
                fieldZ.floatValue = 1f;
            }

            GUILayout.BeginHorizontal ();
            var isReset = GUILayout.Button ("S", GUILayout.Width (20f));
            EditorGUILayout.PropertyField (fieldX);
            EditorGUILayout.PropertyField (fieldY);

            GUI.enabled = !isFound;
            EditorGUILayout.PropertyField (fieldZ);
            GUI.enabled = true;
            GUILayout.EndHorizontal ();
            if (isReset) {
                fieldX.floatValue = 1f;
                fieldY.floatValue = 1f;
                fieldZ.floatValue = 1f;
            }
        }
    }
}