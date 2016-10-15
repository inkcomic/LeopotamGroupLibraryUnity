//-------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2016 Leopotam <leopotam@gmail.com>
//-------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LeopotamGroup.EditorHelpers.UnityEditors {
    /// <summary>
    /// Project folders generator.
    /// </summary>
    class ProjectFoldersGenerator : EditorWindow {
        [Flags]
        public enum Options {
            Animations = 1,
            Fonts = 2,
            Models = 4,
            Prefabs = 8,
            Resources = 16,
            Scenes = 32,
            Scripts = 64,
            Shaders = 128,
            Sounds = 256,
            Textures = 512,
        }

        static readonly Dictionary<int, List<string>> _paths = new Dictionary<int, List<string>>
        {
            { (int) Options.Textures, new List<string>{ "AppIcon", "UI" } }
        };

        const string Title = "Project folders generator";

        const Options DefaultOptions = (Options) (-1);

        const string DefaultRootProjectFolder = "Client";

        const string DefaultCvsFileName = ".keep";

        string _projectRootFolder;

        Options _options;

        bool _cvsSupport;

        string _cvsFileName;

        string[] _optionNames;

        [MenuItem ("Window/LeopotamGroupLibrary/Project folders generator...")]
        static void InitGeneration () {
            var win = GetWindow<ProjectFoldersGenerator> ();
            win.Reset ();
        }

        void OnEnable () {
            titleContent.text = Title;
        }

        void Reset () {
            _projectRootFolder = DefaultRootProjectFolder;
            _options = DefaultOptions;
            _cvsSupport = true;
            _cvsFileName = DefaultCvsFileName;
        }

        void OnGUI () {
            if (_optionNames == null) {
                _optionNames = Enum.GetNames (typeof (Options));
            }

            _projectRootFolder = EditorGUILayout.TextField ("Project root folder", _projectRootFolder).Trim ();

            _options = (Options) EditorGUILayout.MaskField ("Options", (int) _options, _optionNames);

            var cvsSupport = EditorGUILayout.Toggle ("Cvs support", _cvsSupport);
            if (cvsSupport != _cvsSupport) {
                _cvsSupport = cvsSupport;
                _cvsFileName = DefaultCvsFileName;
            }

            if (_cvsSupport) {
                _cvsFileName = EditorGUILayout.TextField ("Cvs filename", _cvsFileName).Trim ();
            }

            if (GUILayout.Button ("Reset settings")) {
                Reset ();
                Repaint ();
            }
            if (GUILayout.Button ("Generate")) {
                if (string.IsNullOrEmpty (_cvsFileName)) {
                    _cvsSupport = false;
                }
                var res = Generate (_projectRootFolder, _options, _cvsSupport ? _cvsFileName : null);
                EditorUtility.DisplayDialog (titleContent.text, res ?? "Success", "Close");
            }
        }

        static void GenerateCvsSupport (string path, string cvsFileName) {
            if (!string.IsNullOrEmpty (cvsFileName)) {
                Debug.Log (Path.Combine (path, cvsFileName));
                path = Path.Combine (path, cvsFileName);
                if (!File.Exists (path)) {
                    File.WriteAllText (path, string.Empty);
                }
            }
        }

        static void GenerateItem (string rootFolder, int item, string cvsFileName) {
            var fullPath = Path.Combine (Application.dataPath, rootFolder);

            fullPath = Path.Combine (fullPath, ((Options) item).ToString ());
            if (!Directory.Exists (fullPath)) {
                Directory.CreateDirectory (fullPath);
            }

            if (_paths.ContainsKey (item)) {
                string path;
                foreach (var subFolder in _paths[item]) {
                    path = Path.Combine (fullPath, subFolder);
                    if (!Directory.Exists (path)) {
                        Directory.CreateDirectory (path);
                    }
                    GenerateCvsSupport (path, cvsFileName);
                }
            } else {
                GenerateCvsSupport (fullPath, cvsFileName);
            }
        }

        /// <summary>
        /// Generate class with idents at specified filename and with specified namespace.
        /// </summary>
        /// <returns>Error message or null on success.</returns>
        /// <param name="rootFolder">Root folder path.</param>
        /// <param name="options">Options</param>
        /// <param name="cvsFileName">Cvs filename for keep empty folders or null for disable.</param>
        public static string Generate (string rootFolder, Options options, string cvsFileName) {
            if ((int) options == 0) {
                return string.Empty;
            }
            try {
                foreach (Options item in Enum.GetValues (typeof (Options))) {
                    if ((int) (options & item) != 0) {
                        GenerateItem (rootFolder, (int) item, cvsFileName);
                    }
                } 
                AssetDatabase.Refresh ();
                return null;
            } catch (Exception ex) {
                AssetDatabase.Refresh ();
                return ex.Message;
            }
        }
    }
}