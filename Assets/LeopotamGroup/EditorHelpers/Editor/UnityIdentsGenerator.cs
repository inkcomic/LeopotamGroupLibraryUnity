//-------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2016 Leopotam <leopotam@gmail.com>
//-------------------------------------------------------

using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace LeopotamGroup.EditorHelpers.UnityEditors {
    class UnityIdentsGenerator : EditorWindow {
        const string Title = "IdentsGen";

        const string TargetFileKey = "lg.editor.unity-idents-gen.path";

        const string NamespaceKey = "lg.editor.unity-idents-gen.ns";

        const string DefaultFileName = "Scripts/Common/UnityIdents.cs";

        const string DefaultNamespace = "Client.Common";

        const string CodeTemplate = "using UnityEngine;\nnamespace {0} {{\n\tpublic static partial class {1} {{\n{2}\t}}\n}}";

        const string LayerName = "{0}public static readonly int Layer{1} = LayerMask.NameToLayer (\"{2}\");\n";

        const string LayerMask = "{0}public static readonly int LayerMask{1} = 1 << Layer{1};\n";

        const string TagName = "{0}public const string Tag{1} = \"{2}\";\n";

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
                Generate (_fileName, _nsName);
            }
        }

        static string GenerateFields (string indent) {
            var sb = new StringBuilder ();
            // layers
            foreach (var layerName in InternalEditorUtility.layers) {
                sb.AppendFormat (LayerName, indent, CleanupName (layerName), layerName);
            }
            // layer masks
            foreach (var layerName in InternalEditorUtility.layers) {
                sb.AppendFormat (LayerMask, indent, CleanupName (layerName));
            }
            // tags
            foreach (var tagName in InternalEditorUtility.tags) {
                sb.AppendFormat (TagName, indent, CleanupName (tagName), tagName);
            }
            return sb.ToString ();
        }

        static string CleanupName (string dirtyName) {
            dirtyName = dirtyName.Replace (" ", string.Empty);
            dirtyName = dirtyName.Replace ("-", "_");
            return char.ToUpperInvariant (dirtyName[0]) + dirtyName.Substring (1);
        }

        static void LogError (string msg) {
            Debug.LogWarning ("UnityIdentsGenerator: " + msg);
        }

        public static void Generate (string assetName, string nsName) {
            if (string.IsNullOrEmpty (assetName) || string.IsNullOrEmpty (nsName)) {
                return;
            }

            var fileName = Application.dataPath + "/" + assetName;
            var className = Path.GetFileNameWithoutExtension (fileName);

            try {
                var path = Path.GetDirectoryName (fileName);
                if (!Directory.Exists (path)) {
                    Directory.CreateDirectory (path);
                }
                var fields = GenerateFields (new string ('\t', 2));
                var content = string.Format (CodeTemplate, nsName, className, fields);
                File.WriteAllText (fileName, content.Replace ("\t", new string (' ', 4)));
                AssetDatabase.Refresh ();
            } catch (Exception ex) {
                LogError (ex.Message);
            }
        }
    }
}