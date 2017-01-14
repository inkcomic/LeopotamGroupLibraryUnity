
// -------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// -------------------------------------------------------

using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace LeopotamGroup.EditorHelpers.UnityEditors {
    /// <summary>
    /// Default inspector for all objects, add drag&drop ordering behaviour for arrays / lists.
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor (typeof (UnityEngine.Object), true, isFallback = true)]
    sealed class DefaultComponentInspector : Editor {
        const float ExpandButtonWidth = 22f;

        static Dictionary<string, ReorderableListProperty> _reorderableLists;

        void OnEnable () {
            if (_reorderableLists == null) {
                _reorderableLists = new Dictionary<string, ReorderableListProperty> (64);
            }
            _reorderableLists.Clear ();
        }

        void OnDisable () {
            if (_reorderableLists != null) {
                _reorderableLists.Clear ();
            }
        }

        public override void OnInspectorGUI () {
            serializedObject.Update ();
            var savedColor = GUI.color;
            var savedEnabled = GUI.enabled;
            var property = serializedObject.GetIterator ();
            var isDirty = false;
            var isValid = property.NextVisible (true);
            if (isValid) {
                do {
                    GUI.color = savedColor;
                    GUI.enabled = savedEnabled;
                    isDirty |= ProcessProperty (property);
                } while (property.NextVisible (false));
            }
            serializedObject.ApplyModifiedProperties ();

            if (isDirty) {
                EditorUtility.SetDirty (target);
                Repaint ();
            }
        }

        bool ProcessProperty (SerializedProperty property) {
            if (property.name.Equals ("m_Script") &&
                property.type.Equals ("PPtr<MonoScript>") &&
                property.propertyType == SerializedPropertyType.ObjectReference &&
                property.propertyPath.Equals ("m_Script")) {
                GUI.enabled = false;
            }
            if (property.isArray && property.propertyType != SerializedPropertyType.String) {
                return ProcessArray (property);
            }
            EditorGUILayout.PropertyField (property, true);
            return false;
        }

        bool ProcessArray (SerializedProperty property) {
            if (!property.isExpanded) {
                var isExpanding = false;
                EditorGUILayout.BeginHorizontal ();
                if (GUILayout.Button ("\u25bc", GUILayout.Width (ExpandButtonWidth))) {
                    property.isExpanded = !property.isExpanded;
                    isExpanding = true;
                }
                EditorGUILayout.LabelField (property.displayName, EditorStyles.boldLabel);
                EditorGUILayout.EndHorizontal ();
                return isExpanding;
            }
            GetReorderableList (property).List.DoLayoutList ();
            return !property.isExpanded;
        }

        ReorderableListProperty GetReorderableList (SerializedProperty property) {
            ReorderableListProperty retVal;
            if (_reorderableLists.TryGetValue (property.name, out retVal)) {
                retVal.Property = property;
                return retVal;
            }
            retVal = new ReorderableListProperty (property);
            _reorderableLists[property.name] = retVal;
            return retVal;
        }

        class ReorderableListProperty : IDisposable {
            public ReorderableList List { get; private set; }

            public SerializedProperty Property {
                get { return List.serializedProperty; }
                set { List.serializedProperty = value; }
            }

            public ReorderableListProperty (SerializedProperty property) {
                List = new ReorderableList (property.serializedObject, property, true, true, true, true);
                List.drawHeaderCallback += OnDrawHeader;
                List.onCanRemoveCallback += OnCanRemove;
                List.drawElementCallback += OnDrawElement;
                List.elementHeightCallback += OnElementHeight;
            }

            void OnDrawHeader (Rect rect) {
                var oldWidth = rect.width;
                rect.width = ExpandButtonWidth;
                rect.x -= 4;
                if (GUI.Button (rect, "\u25B2")) {
                    Property.isExpanded = !Property.isExpanded;
                }
                rect.width = oldWidth - ExpandButtonWidth - 4;
                rect.x += ExpandButtonWidth + 4;
                EditorGUI.LabelField (rect, Property.displayName, EditorStyles.boldLabel);
            }

            bool OnCanRemove (ReorderableList list) {
                return List.count > 0;
            }

            float OnElementHeight (int id) {
                return 4f + Mathf.Max (EditorGUIUtility.singleLineHeight,
                                       EditorGUI.GetPropertyHeight (Property.GetArrayElementAtIndex (id), GUIContent.none, true));
            }

            void OnDrawElement (Rect rect, int index, bool active, bool focused) {
                if (Property.GetArrayElementAtIndex (index).propertyType == SerializedPropertyType.Generic) {
                    EditorGUI.LabelField (rect, Property.GetArrayElementAtIndex (index).displayName);
                }

                rect.height = EditorGUI.GetPropertyHeight (Property.GetArrayElementAtIndex (index), GUIContent.none, true);
                EditorGUI.PropertyField (rect, Property.GetArrayElementAtIndex (index), GUIContent.none, true);
                List.elementHeight = rect.height + 4f;
            }

            public void Dispose () {
                List.drawHeaderCallback -= OnDrawHeader;
                List.onCanRemoveCallback -= OnCanRemove;
                List.drawElementCallback -= OnDrawElement;
                List.elementHeightCallback -= OnElementHeight;
                List = null;
            }
        }
    }
}