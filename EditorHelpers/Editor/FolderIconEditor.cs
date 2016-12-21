
// -------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2016 Leopotam <leopotam@gmail.com>
// -------------------------------------------------------

using LeopotamGroup.Common;
using LeopotamGroup.Serialization;
using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEngine;

namespace LeopotamGroup.EditorHelpers.UnityEditors {
    sealed class FolderIconEditor : EditorWindow {
        class FolderIconDesc {
            Color32? _validColor;

            [JsonIgnore]
            public Color32 ValidColor {
                get {
                    if (!_validColor.HasValue) {
                        _validColor = HexColor.ToColor24 ();
                    }
                    return _validColor.Value;
                }
                set {
                    _validColor = value;
                    HexColor = ((Color) value).ToHexString24 ();
                }
            }

            [JsonName ("c")]
            public string HexColor = "ffffff";

            [JsonName ("i")]
            public string OverlayIcon;
        }

        const string Title = "Folder Icon Editor";

        const int IconSize = 64;

        const string FolderIconName = "Folder Icon";

        const string StorageKey = "lg.folder-icon.storage";

        static Texture2D _folderIconBack;

        static Dictionary<string, FolderIconDesc> _allDescs;

        string _folderPath;

        string _folderGuid;

        FolderIconDesc _folderDesc;

        [InitializeOnLoadMethod]
        static void InitFolderIconEditor () {
            EditorApplication.projectWindowItemOnGUI += OnDrawProjectWindowItem;
            if ((object) _folderIconBack == null) {
                _folderIconBack = EditorGUIUtility.Load (FolderIconName) as Texture2D;
            }
            LoadInfo ();
            SaveInfo ();
        }

        static void LoadInfo () {
            try {
                _allDescs = Singleton.Get<JsonSerialization> ().Deserialize<Dictionary<string, FolderIconDesc>> (
                    ProjectPrefs.GetString (StorageKey, "{}"));
                if (_allDescs == null) {
                    throw new Exception ();
                }
            } catch {
                _allDescs = new Dictionary<string, FolderIconDesc> ();
            }
        }

        static void SaveInfo () {
            if (_allDescs.Count > 0) {
                try {
                    ProjectPrefs.SetString (StorageKey, Singleton.Get<JsonSerialization> ().Serialize (_allDescs));
                } catch (Exception ex) {
                    Debug.LogWarning ("FolderIconEditor.SaveInfo: " + ex.Message);
                }
            } else {
                ProjectPrefs.DeleteKey (StorageKey);
            }
        }

        static void OnDrawProjectWindowItem (string guid, Rect rect) {
            var path = AssetDatabase.GUIDToAssetPath (guid);
            if (AssetDatabase.IsValidFolder (path)) {
                var icon = GetCustomIcon (guid);
                if (icon != null) {
                    if (rect.width > rect.height) {
                        // dirty fix for unity 5.5.
                        rect.x += 3;
                        rect.width = rect.height;
                    } else {
                        rect.height = rect.width;
                    }
                    if (rect.width > IconSize) {
                        var offset = (rect.width - IconSize) * 0.5f;
                        rect.Set (rect.x + offset, rect.y + offset, IconSize, IconSize);
                    }
                    var savedColor = GUI.color;
                    GUI.color = icon.ValidColor;
                    GUI.DrawTexture (rect, _folderIconBack);
                    if (icon.OverlayIcon != null) {
                        GUI.color = savedColor;
                        var tex = EditorGUIUtility.Load (icon.OverlayIcon) as Texture2D;
                        if ((object) tex != null) {
                            rect.width *= 0.5f;
                            rect.height *= 0.5f;
                            rect.x += rect.width;
                            rect.y += rect.height;
                            GUI.DrawTexture (rect, tex);
                        }
                    } else {
                        GUI.color = savedColor;
                    }
                }
            }
        }

        static FolderIconDesc GetCustomIcon (string guid) {
            return _allDescs.ContainsKey (guid) ? _allDescs[guid] : null;
        }

        [MenuItem ("Assets/LeopotamGroup/FolderIcons/Open editor...", false, 1)]
        static void OpenUI () {
            var path = AssetDatabase.GetAssetPath (Selection.activeObject);
            if (!string.IsNullOrEmpty (path) && AssetDatabase.IsValidFolder (path)) {
                var win = GetWindow<FolderIconEditor> ();
                win.Init (path);
                win.ShowUtility ();
            } else {
                EditorUtility.DisplayDialog (Title, "Select valid folder first", "Close");
            }
        }

        [MenuItem ("Assets/LeopotamGroup/FolderIcons/Reset all settings", false, 100)]
        static void ClearAllUI () {
            _allDescs.Clear ();
            SaveInfo ();
            EditorUtility.DisplayDialog (Title, "Successfully reset", "Close");
        }

        void Init (string folderPath) {
            _folderPath = folderPath;
            _folderGuid = AssetDatabase.AssetPathToGUID (_folderPath);
            _folderDesc = GetCustomIcon (_folderGuid) ?? new FolderIconDesc ();
        }

        void OnGUI () {
            if (_folderDesc == null) {
                Close ();
                return;
            }
            var needToSave = false;
            var color = _folderDesc.ValidColor;
            var newR = (byte) EditorGUILayout.IntSlider ("Color R", color.r, 0, 255);
            var newG = (byte) EditorGUILayout.IntSlider ("Color G", color.g, 0, 255);
            var newB = (byte) EditorGUILayout.IntSlider ("Color B", color.b, 0, 255);
            needToSave |= newR != color.r || newG != color.g || newB != color.b;

            var iconName = _folderDesc.OverlayIcon ?? "";
            var newIconName = EditorGUILayout.TextField ("Folder Overlay icon", iconName);
            needToSave |= string.CompareOrdinal (newIconName, iconName) != 0;

            if (needToSave) {
                _folderDesc.ValidColor = new Color32 (newR, newG, newB, 255);
                _folderDesc.OverlayIcon = newIconName.Length > 0 ? newIconName : null;
                if (_folderDesc != GetCustomIcon (_folderGuid)) {
                    _allDescs[_folderGuid] = _folderDesc;
                }
                EditorApplication.RepaintProjectWindow ();
            }
            if (GUILayout.Button ("Reset for this folder")) {
                if (GetCustomIcon (_folderGuid) != null) {
                    _allDescs.Remove (_folderGuid);
                }
                OnLostFocus ();
            }
        }

        void OnLostFocus () {
            Close ();
        }

        void OnDisable () {
            SaveInfo ();
        }
    }
}