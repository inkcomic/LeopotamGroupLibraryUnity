
// -------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2016 Leopotam <leopotam@gmail.com>
// -------------------------------------------------------

using UnityEditor;
using UnityEngine;

namespace LeopotamGroup.EditorHelpers.UnityEditors {
    /// <summary>
    /// Helper for align selection to grid with specified steps.
    /// </summary>
    sealed class SelectionAligner : EditorWindow {
        Vector3 _lastPosition;

        float _alignFactorX = 0.1f;

        float _alignFactorY = 0.1f;

        float _alignFactorZ = 0.1f;

        bool _isAlignX = true;

        bool _isAlignY = true;

        bool _isAlignZ = true;

        static readonly float[] Presets = new[]
        {
            0.1f, 0.5f, 1f, 5f, 10f
        };

        public const string Title = "Align selection";

        [MenuItem ("Window/LeopotamGroupLibrary/Align selection...")]
        [MenuItem ("CONTEXT/Transform/Align selection...")]
        public static void ShowWindow () {
            var window = GetWindow<SelectionAligner> ();
            window.ShowUtility ();
            window.minSize = new Vector2 (200, 200);
            window.maxSize = new Vector2 (200, 201);
            window.titleContent.text = Title;
        }

        void OnGUI () {
            _isAlignX = EditorGUILayout.BeginToggleGroup ("Align to X", _isAlignX);
            _alignFactorX = Mathf.Clamp (PresetFilter (EditorGUILayout.FloatField ("Grid size", _alignFactorX)), 0.1f, 100f);
            EditorGUILayout.EndToggleGroup ();

            EditorGUILayout.Separator ();

            _isAlignY = EditorGUILayout.BeginToggleGroup ("Align to Y", _isAlignY);
            _alignFactorY = Mathf.Clamp (PresetFilter (EditorGUILayout.FloatField ("Grid size", _alignFactorY)), 0.1f, 100f);
            EditorGUILayout.EndToggleGroup ();

            EditorGUILayout.Separator ();

            _isAlignZ = EditorGUILayout.BeginToggleGroup ("Align to Z", _isAlignZ);
            _alignFactorZ = Mathf.Clamp (PresetFilter (EditorGUILayout.FloatField ("Grid size", _alignFactorZ)), 0.1f, 100f);
            EditorGUILayout.EndToggleGroup ();
        }

        float PresetFilter (float v) {
            GUILayout.BeginHorizontal ();
            for (int i = 0, iMax = Presets.Length; i < iMax; i++) {
                if (GUILayout.Button (Presets[i] + "m")) {
                    v = Presets[i];
                    break;
                }
            }
            GUILayout.EndHorizontal ();

            return v;
        }

        void Update () {
            if (!EditorApplication.isPlaying && (_isAlignX || _isAlignY || _isAlignZ)) {
                AlignSelection ();
            }
        }

        void AlignSelection () {
            if (Selection.transforms.Length > 0 && Selection.transforms[0].position != _lastPosition) {
                Undo.RecordObjects (Selection.transforms, Title);
                foreach (var item in Selection.transforms) {
                    item.position = AlignPosition (item.position);
                }
                _lastPosition = Selection.transforms[0].position;
            }
        }

        Vector3 AlignPosition (Vector3 v) {
            if (_isAlignX) {
                v.x = _alignFactorX * Mathf.Round (v.x / _alignFactorX);
            }
            if (_isAlignY) {
                v.y = _alignFactorY * Mathf.Round (v.y / _alignFactorY);
            }
            if (_isAlignZ) {
                v.z = _alignFactorZ * Mathf.Round (v.z / _alignFactorZ);
            }
            return v;
        }
    }
}