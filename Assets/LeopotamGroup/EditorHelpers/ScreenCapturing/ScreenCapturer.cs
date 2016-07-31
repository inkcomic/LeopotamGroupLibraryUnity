//-------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2016 Leopotam <leopotam@gmail.com>
//-------------------------------------------------------

#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using LeopotamGroup.Math;
using UnityEditor;
using UnityEngine;

namespace LeopotamGroup.EditorHelpers.ScreenCapturing {
    [ExecuteInEditMode]
    sealed class ScreenCapturer : MonoBehaviour {
        readonly static List<Vector2i> _resList = new List<Vector2i>
        {
            new Vector2i (800, 480),
            new Vector2i (960, 640),
            new Vector2i (1024, 768),
            new Vector2i (1136, 640),
            new Vector2i (1280, 800),
            new Vector2i (1920, 1080),
            new Vector2i (2048, 1536),
        };

        const string FileNameMask = "Screenshot_{0}_{1}x{2}.png";

        [MenuItem ("Window/LeopotamGroupLibrary/Capture screenshots...")]
        public static void Capture () {
            if (!Application.isPlaying) {
                EditorUtility.DisplayDialog ("ScreenCapturer", "You can capture shots only at play more!", "Close");
                return;
            }
            var go = new GameObject ("_CAPTURER");
            go.hideFlags = HideFlags.HideAndDontSave;
            go.AddComponent<ScreenCapturer> ();
        }

        void Awake () {
            if (!Application.isPlaying) {
                FinishCapturing ("ScreenCapturer can works only at play mode!");
            }
        }

        IEnumerator Start () {
            var path = EditorUtility.OpenFolderPanel ("Select target folder to screenshots", string.Empty, string.Empty);
            if (string.IsNullOrEmpty (path) || !Directory.Exists (path)) {
                FinishCapturing ("Invalid path");
                yield break;
            }

            var platform = EditorUserBuildSettings.activeBuildTarget.ToString ();

            var oldRes = Screen.currentResolution;
            var oldResFS = Screen.fullScreen;
            var waiter = new WaitForEndOfFrame ();
            var savedPreset = GameViewResolution.GetPreset ();
            string err = null;
            int tempPreset;
            foreach (var res in _resList) {
                tempPreset = GameViewResolution.AddCustomPreset (res.x, res.y, "_capturer");
                GameViewResolution.SetPreset (tempPreset);
                yield return waiter;
                try {
                    Application.CaptureScreenshot (
                        Path.Combine (path, string.Format (FileNameMask, platform, res.x, res.y)));
                } catch (Exception ex) {
                    err = ex.ToString ();
                    break;
                }
                GameViewResolution.SetPreset (savedPreset);
                GameViewResolution.RemoveCustomPreset (tempPreset);
            }

            FinishCapturing (err);
        }

        void FinishCapturing (string result) {
            var msg = string.Format ("[ScreenCapturer] {0}", result ?? "Success");
            if (result != null) {
                Debug.LogWarning (msg);
            } else {
                Debug.Log (msg);
            }
            DestroyImmediate (gameObject);
        }

        static class GameViewResolution {
            static readonly EditorWindow _gvWindow;

            static readonly PropertyInfo _gvSelectedPreset;

            static readonly PropertyInfo _gvsCurrentGroup;

            static readonly MethodInfo _gvRepaintAll;

            static readonly MethodInfo _gvgGetTotalCount;

            static readonly MethodInfo _gvgAddCustomSize;

            static readonly MethodInfo _gvgRemoveCustomSize;

            static readonly object _gvsInstance;

            static readonly ConstructorInfo _gvsCtor;

            static GameViewResolution () {
                var gvType = typeof (Editor).Assembly.GetType ("UnityEditor.GameView");
                _gvSelectedPreset = gvType.GetProperty ("selectedSizeIndex", BindingFlags.Instance | BindingFlags.NonPublic);
                _gvRepaintAll = gvType.GetMethod ("RepaintAll", BindingFlags.Static | BindingFlags.Public);
                _gvWindow = EditorWindow.GetWindow (gvType);

                var gvgType = typeof (Editor).Assembly.GetType ("UnityEditor.GameViewSizeGroup");
                _gvgGetTotalCount = gvgType.GetMethod ("GetTotalCount", BindingFlags.Instance | BindingFlags.Public);
                _gvgAddCustomSize = gvgType.GetMethod ("AddCustomSize", BindingFlags.Instance | BindingFlags.Public);
                _gvgRemoveCustomSize = gvgType.GetMethod ("RemoveCustomSize", BindingFlags.Instance | BindingFlags.Public);

                var sizesType = typeof (Editor).Assembly.GetType ("UnityEditor.GameViewSizes");
                _gvsCurrentGroup = sizesType.GetProperty ("currentGroup", BindingFlags.Instance | BindingFlags.Public);
                var singleType = typeof (ScriptableSingleton<>).MakeGenericType (sizesType);
                var instanceProp = singleType.GetProperty ("instance");
                _gvsInstance = instanceProp.GetValue (null, null);

                var gvsType = typeof (Editor).Assembly.GetType ("UnityEditor.GameViewSize");
                _gvsCtor = gvsType.GetConstructor (new [] { typeof (int), typeof (int), typeof (int), typeof (string) });
            }

            static object GetCurrentGroup () {
                return _gvsCurrentGroup.GetValue (_gvsInstance, null);
            }

            public static int GetPreset () {
                return (int) _gvSelectedPreset.GetValue (_gvWindow, null);
            }

            public static void SetPreset (int id) {
                _gvSelectedPreset.SetValue (_gvWindow, id, null);
                _gvRepaintAll.Invoke (null, null);
            }

            public static int AddCustomPreset (int width, int height, string text) {
                var grp = GetCurrentGroup ();
                var newSize = _gvsCtor.Invoke (new object[] { 1, width, height, text });
                _gvgAddCustomSize.Invoke (grp, new [] { newSize });
                return (int) _gvgGetTotalCount.Invoke (grp, null) - 1;
            }

            public static void RemoveCustomPreset (int id) {
                _gvgRemoveCustomSize.Invoke (GetCurrentGroup (), new object[] { id });
            }
        }
    }
}

#endif