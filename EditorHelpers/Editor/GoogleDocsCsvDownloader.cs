// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

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

        List<RecordInfo> _items;

        Vector2 _scrollPos;

        string _newUrl;

        string _newRes;

        JsonMode _newJson;

        [MenuItem ("Window/LeopotamGroupLibrary/Download external CSV-data...")]
        static void OpenEditorWindow () {
            GetWindow<GoogleDocsCsvDownloader> (true);
        }

        void OnEnable () {
            titleContent.text = Title;
            _scrollPos = Vector2.zero;
            var pos = position;
            pos.width = 300f;
            pos.height = 200f;
            position = pos;
        }

        void Load () {
            try {
                _items = Singleton.Get<JsonSerialization> ()
                    .Deserialize<List<RecordInfo>> (ProjectPrefs.GetString (ProjectPrefsKey, "{}"));
                if (_items == null) {
                    throw new Exception ();
                }
            } catch {
                _items = new List<RecordInfo> ();
            }
        }

        void Save () {
            if (_items != null && _items.Count > 0) {
                ProjectPrefs.SetString (ProjectPrefsKey, Singleton.Get<JsonSerialization> ().Serialize (_items));
            } else {
                ProjectPrefs.DeleteKey (ProjectPrefsKey);
            }
        }

        void OnDisable () {
            Save ();
        }

        void OnGUI () {
            if (_items == null) {
                Load ();
            }

            if (string.IsNullOrEmpty (_newUrl)) {
                _newUrl = UrlDefault;
            }
            if (string.IsNullOrEmpty (_newRes)) {
                _newRes = ResDefault;
            }

            if (_items.Count > 0) {
                EditorGUILayout.LabelField ("List of csv resources", EditorStyles.boldLabel);
                GUILayout.BeginScrollView (_scrollPos, false, true);
                for (var i = 0; i < _items.Count; i++) {
                    var item = _items[i];
                    GUILayout.BeginHorizontal (GUI.skin.box);
                    GUI.enabled = false;
                    GUILayout.BeginVertical ();
                    EditorGUILayout.LabelField ("External Url path:", item.Url);
                    EditorGUILayout.TextField ("Resource file:", item.ResourceName);
                    EditorGUILayout.EnumPopup ("Convert to JSON:", item.JsonMode);
                    GUILayout.EndVertical ();
                    GUI.enabled = true;
                    if (GUILayout.Button ("Remove", GUILayout.Width (80f), GUILayout.Height (52f))) {
                        _items.Remove (item);
                    }
                    GUILayout.EndHorizontal ();
                }
                GUILayout.EndScrollView ();
            }

            GUILayout.Space (4f);
            EditorGUILayout.LabelField ("New external csv", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal (GUI.skin.box);
            GUILayout.BeginVertical ();
            _newUrl = EditorGUILayout.TextField ("External Url path:", _newUrl).Trim ();
            _newRes = EditorGUILayout.TextField ("Resource file:", _newRes).Trim ();
            _newJson = (JsonMode) EditorGUILayout.EnumPopup ("Convert to JSON:", _newJson);
            GUILayout.EndVertical ();
            if (GUILayout.Button ("Add", GUILayout.Width (80f), GUILayout.Height (52f))) {
                var newItem = new RecordInfo ();
                newItem.Url = _newUrl;
                newItem.ResourceName = _newRes;
                newItem.JsonMode = _newJson;
                _items.Add (newItem);
                _newUrl = UrlDefault;
                _newRes = ResDefault;
                _newJson = JsonMode.None;
            }
            GUILayout.EndHorizontal ();

            GUILayout.Space (4f);

            GUI.enabled = _items.Count > 0;
            if (GUILayout.Button ("Update data from external urls", GUILayout.Height (30f))) {
                var res = Process (_items);
                EditorUtility.DisplayDialog (titleContent.text, res ?? "Success", "Close");
            }
            GUI.enabled = true;
        }

        static string ConvertToJson (string data, JsonMode jsonMode) {
            var sb = new StringBuilder (data.Length * 2);
            var csvLines = Singleton.Get<CsvSerialization> ().Deserialize (data);
            if (csvLines.Count < 2) {
                throw new Exception ("Invalid header data: first line should contains field names, second line - pair of wrapping chars.");
            }
            sb.Append (jsonMode == JsonMode.Array ? "[" : "{");
            var it = csvLines.GetEnumerator ();

            // header.
            it.MoveNext ();
            var headerKey = it.Current.Key;
            var headerValue = it.Current.Value;

            // wrappers.
            it.MoveNext ();
            var wrapperKey = it.Current.Key;
            var wrapperValue = it.Current.Value;
            for (var i = 0; i < wrapperValue.Length; i++) {
                if (!(
                        wrapperValue[i] == string.Empty ||
                        wrapperValue[i] == "[]" ||
                        wrapperValue[i] == "{}" ||
                        wrapperValue[i] == "\"\"")) {
                    throw new Exception (string.Format ("Invalid wrapper data for \"{0}\" field.", headerValue[i]));
                }
            }
            if (jsonMode == JsonMode.Dictionary && wrapperKey != "\"\"") {
                throw new Exception ("Invalid wrapper data: key should be wrapped with \"\".");
            }

            var needComma = false;
            while (it.MoveNext ()) {
                if (jsonMode == JsonMode.Dictionary) {
                    sb.AppendFormat ("{0}\"{1}\":{{", needComma ? "," : string.Empty, it.Current.Key);
                } else {
                    sb.AppendFormat ("{0}{{", needComma ? "," : string.Empty);
                }
                for (var i = 0; i < headerValue.Length; i++) {
                    var wrapChars = wrapperValue[i];
                    var item = wrapChars.Length > 0 ?
                        string.Format ("{0}{1}{2}", wrapChars[0], it.Current.Value[i], wrapChars[1]) : it.Current.Value[i];
                    sb.AppendFormat ("{0}\"{1}\":{2}", i > 0 ? "," : string.Empty, headerValue[i], item);
                }
                sb.Append ("}");
                needComma = true;
            }

            sb.Append (jsonMode == JsonMode.Array ? "]" : "}");
            return sb.ToString ();
        }

        public static string Process (List<RecordInfo> items) {
            if (items == null || items.Count == 0) {
                return "No data";
            }
            try {
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                using (var www = new WebClient ()) {
                    www.Encoding = Encoding.UTF8;
                    string data;
                    foreach (var item in items) {
                        if (!string.IsNullOrEmpty (item.Url) && !string.IsNullOrEmpty (item.ResourceName)) {
                            // Dirty hack for url, because standard "publish to web" has huge lag up to 30 minutes.
                            try {
                                data = www.DownloadString (item.Url.Replace ("?", string.Empty).Replace ("/edit", "/export?format=csv&"));
                            } catch (Exception urlEx) {
                                throw new Exception (string.Format ("\"{0}\": {1}", item.Url, urlEx.Message));
                            }
                            var path = string.Format ("{0}/{1}", Application.dataPath, item.ResourceName);
                            var folder = Path.GetDirectoryName (path);
                            if (!Directory.Exists (folder)) {
                                Directory.CreateDirectory (folder);
                            }

                            // Fix for multiline string.
                            data = CsvMultilineRegex.Replace (data, m => m.Value.Replace ("\n", "\\n"));

                            // json generation.
                            if (item.JsonMode != JsonMode.None) {
                                data = ConvertToJson (data, item.JsonMode);
                            }

                            File.WriteAllText (path, data, Encoding.UTF8);
                        }
                    }
                }
                AssetDatabase.Refresh ();
                return null;
            } catch (Exception ex) {
                AssetDatabase.Refresh ();
                return ex.Message;
            } finally {
                ServicePointManager.ServerCertificateValidationCallback = null;
            }
        }

        public enum JsonMode {
            None = 0,
            Array = 1,
            Dictionary = 2
        }

        public sealed class RecordInfo {
            [JsonName ("u")]
            public string Url = string.Empty;

            [JsonName ("r")]
            public string ResourceName = string.Empty;

            [JsonName ("j")]
            public JsonMode JsonMode = JsonMode.None;
        }
    }
}