// -------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// -------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using LeopotamGroup.Common;
using LeopotamGroup.Serialization;
using UnityEditor;
using UnityEngine;

namespace LeopotamGroup.EditorHelpers.UnityEditors {
    /// <summary>
    /// Downloader from google docs sheets.
    /// </summary>
    sealed class GoogleDocsCsvDownloader : EditorWindow {
        const string Title = "CSV downloader";

        const string ProjectPrefsKey = "lg.external-csv.update";

        const string UrlDefault = "http://localhost";

        const string ResDefault = "NewCsv.csv";

        static readonly Regex CsvMultilineRegex = new Regex ("\"([^\"]|\"\"|\\n)*\"");

        Dictionary<string, string> _paths;

        Vector2 _scrollPos;

        string _newUrl;

        string _newRes;

        [MenuItem ("Window/LeopotamGroupLibrary/Download external CSV-data...")]
        static void OpenEditorWindow () {
            GetWindow<GoogleDocsCsvDownloader> (true);
        }

        void OnEnable () {
            titleContent.text = Title;
            _scrollPos = Vector2.zero;
        }

        void Load () {
            try {
                _paths = Singleton.Get<JsonSerialization> ()
                        .Deserialize<Dictionary<string, string>> (
                            ProjectPrefs.GetString (ProjectPrefsKey, string.Empty));
                if (_paths == null) {
                    throw new Exception ();
                }
            } catch {
                _paths = new Dictionary<string, string> ();
            }
        }

        void Save () {
            if (_paths != null && _paths.Count > 0) {
                ProjectPrefs.SetString (ProjectPrefsKey, Singleton.Get<JsonSerialization> ().Serialize (_paths));
            } else {
                ProjectPrefs.DeleteKey (ProjectPrefsKey);
            }
        }

        void OnDisable () {
            Save ();
        }

        // ReSharper disable once InconsistentNaming
        void OnGUI () {
            if (_paths == null) {
                Load ();
            }

            if (string.IsNullOrEmpty (_newUrl)) {
                _newUrl = UrlDefault;
            }
            if (string.IsNullOrEmpty (_newRes)) {
                _newRes = ResDefault;
            }

            if (_paths.Count > 0) {
                EditorGUILayout.LabelField ("List of csv resources", EditorStyles.boldLabel);
                GUILayout.BeginScrollView (_scrollPos, false, true);
                foreach (var key in new List<string> (_paths.Keys)) {
                    GUILayout.BeginHorizontal (GUI.skin.textArea);
                    GUILayout.BeginVertical ();
                    EditorGUILayout.LabelField ("Url:", key);
                    _paths[key] = EditorGUILayout.TextField ("Resources file:", _paths[key]).Trim ();
                    GUILayout.EndVertical ();
                    if (GUILayout.Button ("Remove", GUILayout.Width (80f), GUILayout.Height (32f))) {
                        _paths.Remove (key);
                    }
                    GUILayout.EndHorizontal ();
                }
                GUILayout.EndScrollView ();
            }

            GUILayout.Space (4f);
            GUILayout.BeginHorizontal (GUI.skin.box);
            GUILayout.BeginVertical ();
            EditorGUILayout.LabelField ("New external csv", EditorStyles.boldLabel);
            _newUrl = EditorGUILayout.TextField ("Url path:", _newUrl).Trim ();
            _newRes = EditorGUILayout.TextField ("Res path:", _newRes).Trim ();
            GUILayout.EndVertical ();
            GUI.enabled = !_paths.ContainsKey (_newUrl);
            if (GUILayout.Button ("Add", GUILayout.Width (80f), GUILayout.Height (52f))) {
                _paths.Add (_newUrl, string.Empty);
                _newUrl = UrlDefault;
                _newRes = ResDefault;
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal ();
            EditorGUILayout.EndFadeGroup ();
            GUILayout.Space (4f);

            GUI.enabled = _paths.Count > 0;
            if (GUILayout.Button ("Update data from external urls", GUILayout.Height (30f))) {
                var res = Process (_paths);
                EditorUtility.DisplayDialog (titleContent.text, res ?? "Success", "Close");
            }
            GUI.enabled = true;
        }

        public static string Process (Dictionary<string, string> paths) {
            if (paths == null || paths.Count == 0) {
                return "No data";
            }
            try {
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                using (var www = new WebClient ()) {
                    www.Encoding = Encoding.UTF8;
                    string data;
                    foreach (var pair in paths) {
                        if (!string.IsNullOrEmpty (pair.Value)) {
                            // Dirty hack for url, because standard "publish to web" has huge lag up to 30 minutes.
                            data = www.DownloadString (pair.Key.Replace ("?", string.Empty).Replace ("/edit", "/export?format=csv&"));
                            var path = string.Format ("{0}/{1}", Application.dataPath, pair.Value);
                            var folder = Path.GetDirectoryName (path);
                            if (!Directory.Exists (folder)) {
                                Directory.CreateDirectory (folder);
                            }

                            // Fix for multiline string.
                            data = CsvMultilineRegex.Replace (data, m => m.Value.Replace ("\n", "\\n"));

                            File.WriteAllText (path, data, Encoding.UTF8);
                        }
                    }
                }
                AssetDatabase.Refresh ();
                return null;
            } catch (Exception ex) {
                AssetDatabase.Refresh ();
                return ex.ToString ();
            } finally {
                ServicePointManager.ServerCertificateValidationCallback = null;
            }
        }
    }
}