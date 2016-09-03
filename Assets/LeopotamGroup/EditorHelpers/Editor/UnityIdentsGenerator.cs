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

        const string LayerMaskKey = "lg.editor.unity-idents-gen.layer";

        const string DefaultFileName = "Scripts/Common/UnityIdents.cs";

        const string DefaultNamespace = "Client.Common";

        const string DefaultLayerMask = "Layer{0}";

        const string CodeTemplate = "namespace {0} {{\n\tpublic static partial class {1} {{\n{2}\t}}\n}}";

        string _fileName;

        string _nsName;

        string _layerName;

        [MenuItem ("Window/LeopotamGroupLibrary/UnityIdents generator...")]
        static void InitGeneration () {
            GetWindow<UnityIdentsGenerator> ();
        }

        void OnEnable () {
            titleContent.text = Title;
            _fileName = ProjectPrefs.GetString (TargetFileKey, DefaultFileName);
            _nsName = ProjectPrefs.GetString (NamespaceKey, DefaultNamespace);
            _layerName = ProjectPrefs.GetString (LayerMaskKey, DefaultLayerMask);
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
            _layerName = EditorGUILayout.TextField ("Layer mask", _layerName).Trim ();
            if (string.IsNullOrEmpty (_layerName)) {
                _layerName = DefaultLayerMask;
            }

            if (GUILayout.Button ("Reset settings")) {
                ProjectPrefs.DeleteKey (TargetFileKey);
                ProjectPrefs.DeleteKey (NamespaceKey);
                ProjectPrefs.DeleteKey (LayerMaskKey);
                OnEnable ();
                Repaint ();
            }

            if (GUILayout.Button ("Save settings & generate")) {
                ProjectPrefs.SetString (TargetFileKey, _fileName);
                ProjectPrefs.SetString (NamespaceKey, _nsName);
                ProjectPrefs.SetString (LayerMaskKey, _layerName);
                Generate (_fileName, _nsName, _layerName);
            }
        }

        static void Generate (string assetName, string nsName, string layerName) {
            if (string.IsNullOrEmpty (assetName) || string.IsNullOrEmpty (nsName) || string.IsNullOrEmpty (layerName)) {
                return;
            }
            var fileName = Application.dataPath + "/" + assetName;
            var className = Path.GetFileNameWithoutExtension (fileName);

            try {
                var path = Path.GetDirectoryName (fileName);
                if (!Directory.Exists (path)) {
                    Directory.CreateDirectory (path);
                }
                var content = string.Format (CodeTemplate, nsName, className, GenerateFields (new string ('\t', 2)));
                File.WriteAllText (fileName, content.Replace ("\t", new string (' ', 4)));
                AssetDatabase.Refresh ();
            } catch (Exception ex) {
                LogError (ex.Message);
            }
        }

        static string GenerateFields (string indent) {
            var sb = new StringBuilder ();
            var layerMask = ProjectPrefs.GetString (LayerMaskKey);
            string fieldName;
            foreach (var layerName in InternalEditorUtility.layers) {
                fieldName = layerName.Replace (" ", string.Empty);
                fieldName = char.ToUpperInvariant (fieldName[0]) + fieldName.Substring (1);
                fieldName = string.Format (layerMask, fieldName);
                if (!string.IsNullOrEmpty (fieldName)) {
                    sb.AppendFormat ("{0}public const string {1} = \"{2}\";\n", indent, fieldName, layerName);
                }
            }
            return sb.ToString ();
        }

        static void LogError (string msg) {
            Debug.LogWarning ("UnityIdentsGenerator: " + msg);
        }
    }
}