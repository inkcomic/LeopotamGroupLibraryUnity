//-------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2016 Leopotam <leopotam@gmail.com>
//-------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace LeopotamGroup.EditorHelpers.UnityEditors {
    /// <summary>
    /// Unity idents generator.
    /// </summary>
    class UnityIdentsGenerator : EditorWindow {
        const string Title = "Unity idents generator";

        const string TargetFileKey = "lg.editor.unity-idents-gen.path";

        const string NamespaceKey = "lg.editor.unity-idents-gen.ns";

        const string DefaultFileName = "Scripts/Common/UnityIdents.cs";

        const string DefaultNamespace = "Client.Common";

        const string CodeTemplate =
            "// Auto generated code, dont change it manually!\n\n" +
            "using UnityEngine;\n\nnamespace {0} {{\n\tpublic static partial class {1} {{\n{2}\n\t}}\n}}";

        const string LayerName = "{0}public static readonly int Layer{1} = LayerMask.NameToLayer (\"{2}\");";

        const string LayerMask = "{0}public static readonly int LayerMask{1} = 1 << Layer{1};";

        const string TagName = "{0}public const string Tag{1} = \"{2}\";";

        const string SceneName = "{0}public const string Scene{1} = \"{2}\";";

        const string AnimatorName = "{0}public static readonly int Animator{1} = Animator.StringToHash (\"{2}\");";

        string _fileName;

        string _nsName;

        [MenuItem ("Window/LeopotamGroupLibrary/UnityIdents generator...")]
        static void InitGeneration () {
            GetWindow<UnityIdentsGenerator> ();
        }

        void OnEnable () {
            titleContent.text = Title;
            _fileName = ProjectPrefs.GetString (TargetFileKey, DefaultFileName);
            _nsName = ProjectPrefs.GetString (NamespaceKey, DefaultNamespace);
        }

        void OnGUI () {
            _fileName = EditorGUILayout.TextField ("Target file", _fileName).Trim ();
            if (string.IsNullOrEmpty (_fileName)) {
                _fileName = DefaultFileName;
            }
            _nsName = EditorGUILayout.TextField ("Namespace", _nsName).Trim ();
            if (string.IsNullOrEmpty (_nsName)) {
                _nsName = DefaultNamespace;
            }
            if (GUILayout.Button ("Reset settings")) {
                ProjectPrefs.DeleteKey (TargetFileKey);
                ProjectPrefs.DeleteKey (NamespaceKey);
                OnEnable ();
                Repaint ();
            }
            if (GUILayout.Button ("Save settings & generate")) {
                ProjectPrefs.SetString (TargetFileKey, _fileName);
                ProjectPrefs.SetString (NamespaceKey, _nsName);
                var res = Generate (_fileName, _nsName);
                EditorUtility.DisplayDialog (titleContent.text, res ?? "Success", "Close");
            }
        }

        static string GenerateFields (string indent) {
            var lines = new List<string> (128);

            // layers, layer masks
            foreach (var layerName in InternalEditorUtility.layers) {
                lines.Add (string.Format (LayerName, indent, CleanupName (layerName), CleanupValue (layerName)));
                lines.Add (string.Format (LayerMask, indent, CleanupName (layerName)));
            }

            // tags
            foreach (var tagName in InternalEditorUtility.tags) {
                lines.Add (string.Format (TagName, indent, CleanupName (tagName), CleanupValue (tagName)));
            }

            // scenes
            foreach (var scene in EditorBuildSettings.scenes) {
                var sceneName = Path.GetFileNameWithoutExtension (scene.path);
                lines.Add (string.Format (SceneName, indent, CleanupName (sceneName), CleanupValue (sceneName)));
            }

            // animators
            var animNames = new HashSet<string> ();
            foreach (var guid in AssetDatabase.FindAssets ("t:animatorcontroller")) {
                var assetPath = AssetDatabase.GUIDToAssetPath (guid);
                var ac = AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController> (assetPath);
                for (int i = 0, iMax = ac.parameters.Length; i < iMax; i++) {
                    var name = ac.parameters[i].name;
                    if (!animNames.Contains (name)) {
                        lines.Add (string.Format (AnimatorName, indent, CleanupName (name), CleanupValue (name)));
                        animNames.Add (name);
                    }
                }
            }

            lines.Sort ();
            return string.Join ("\n\n", lines.ToArray ());
        }

        static string CleanupName (string dirtyName) {
            // cant use "CultureInfo.InvariantCulture.TextInfo.ToTitleCase" due it will break already upcased chars.
            var sb = new StringBuilder ();
            var needUp = true;
            foreach (var c in dirtyName) {
                if (char.IsLetterOrDigit (c)) {
                    sb.Append (needUp ? char.ToUpperInvariant (c) : c);
                    needUp = false;
                } else {
                    needUp = true;
                }
            }
            return sb.ToString ();
        }

        static string CleanupValue (string dirtyValue) {
            return dirtyValue.Replace ("\"", "\\\"");
        }

        /// <summary>
        /// Generate class with idents at specified filename and with specified namespace.
        /// </summary>
        /// <returns>Error message or null on success.</returns>
        /// <param name="fileName">Filename.</param>
        /// <param name="nsName">Namespace.</param>
        public static string Generate (string fileName, string nsName) {
            if (string.IsNullOrEmpty (fileName) || string.IsNullOrEmpty (nsName)) {
                return "invalid parameters";
            }
            var fullFileName = Path.Combine (Application.dataPath, fileName);
            var className = Path.GetFileNameWithoutExtension (fullFileName);
            try {
                var path = Path.GetDirectoryName (fullFileName);
                if (!Directory.Exists (path)) {
                    Directory.CreateDirectory (path);
                }
                var fields = GenerateFields (new string ('\t', 2));
                var content = string.Format (CodeTemplate, nsName, className, fields);
                File.WriteAllText (fullFileName, content.Replace ("\t", new string (' ', 4)));
                AssetDatabase.Refresh ();
                return null;
            } catch (Exception ex) {
                return ex.Message;
            }
        }
    }
}