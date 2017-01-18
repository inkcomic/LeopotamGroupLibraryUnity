// -------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// -------------------------------------------------------

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
        [Flags]
        public enum Options {
            Layers = 1,

            Tags = 2,

            Scenes = 4,

            Animators = 8,

            Axes = 16,

            Shaders = 32
        }

        const string Title = "Unity idents generator";

        const string TargetFileKey = "lg.unity-idents-gen.path";

        const string NamespaceKey = "lg.unity-idents-gen.ns";

        const string OptionsKey = "lg.unity-idents-gen.options";

        const string DefaultFileName = "Client/Scripts/Common/UnityIdents.cs";

        const string DefaultNamespace = "Client.Common";

        const Options DefaultOptions = (Options) (-1);

        const string CodeTemplate =
                "// Auto generated code, dont change it manually!\n\n" +
                "using UnityEngine;\n\nnamespace {0} {{\n\tpublic static partial class {1} {{\n{2}\n\t}}\n}}";

        const string LayerName = "{0}public static readonly int Layer{1} = LayerMask.NameToLayer (\"{2}\");";

        const string LayerMask = "{0}public static readonly int MaskLayer{1} = 1 << Layer{1};";

        const string TagName = "{0}public const string Tag{1} = \"{2}\";";

        const string SceneName = "{0}public const string Scene{1} = \"{2}\";";

        const string AnimatorName = "{0}public static readonly int Animator{1} = Animator.StringToHash (\"{2}\");";

        const string AxisName = "{0}public const string Axis{1} = \"{2}\";";

        const string ShaderName = "{0}public static readonly int Shader{1} = Shader.PropertyToID (\"{2}\");";

        string _fileName;

        string _nsName;

        Options _options;

        string[] _optionNames;

        [MenuItem ("Window/LeopotamGroupLibrary/UnityIdents generator...")]
        static void InitGeneration () {
            GetWindow<UnityIdentsGenerator> (true);
        }

        void OnEnable () {
            titleContent.text = Title;
            _fileName = ProjectPrefs.GetString (TargetFileKey, DefaultFileName);
            _nsName = ProjectPrefs.GetString (NamespaceKey, DefaultNamespace);
            _options = (Options) ProjectPrefs.GetInt (OptionsKey, (int) DefaultOptions);
        }

        // ReSharper disable once InconsistentNaming
        void OnGUI () {
            if (_optionNames == null) {
                _optionNames = Enum.GetNames (typeof (Options));
            }

            _fileName = EditorGUILayout.TextField ("Target file", _fileName).Trim ();
            if (string.IsNullOrEmpty (_fileName)) {
                _fileName = DefaultFileName;
            }
            _nsName = EditorGUILayout.TextField ("Namespace", _nsName).Trim ();
            if (string.IsNullOrEmpty (_nsName)) {
                _nsName = DefaultNamespace;
            }
            _options = (Options) EditorGUILayout.MaskField ("Options", (int) _options, _optionNames);

            if (GUILayout.Button ("Reset settings")) {
                ProjectPrefs.DeleteKey (TargetFileKey);
                ProjectPrefs.DeleteKey (NamespaceKey);
                ProjectPrefs.DeleteKey (OptionsKey);
                OnEnable ();
                Repaint ();
            }
            if (GUILayout.Button ("Save settings & generate")) {
                ProjectPrefs.SetString (TargetFileKey, _fileName);
                ProjectPrefs.SetString (NamespaceKey, _nsName);
                ProjectPrefs.SetInt (OptionsKey, (int) _options);
                var res = Generate (_fileName, _nsName, _options);
                EditorUtility.DisplayDialog (titleContent.text, res ?? "Success", "Close");
            }
        }

        static string GenerateFields (string indent, Options options) {
            var lines = new List<string> (128);
            var uniquesList = new HashSet<string> ();

            // layers, layer masks
            if ((int) (options & Options.Layers) != 0) {
                foreach (var layerName in InternalEditorUtility.layers) {
                    lines.Add (string.Format (LayerName, indent, CleanupName (layerName), CleanupValue (layerName)));
                    lines.Add (string.Format (LayerMask, indent, CleanupName (layerName)));
                }
            }

            // tags
            if ((int) (options & Options.Tags) != 0) {
                foreach (var tagName in InternalEditorUtility.tags) {
                    lines.Add (string.Format (TagName, indent, CleanupName (tagName), CleanupValue (tagName)));
                }
            }

            // scenes
            if ((int) (options & Options.Scenes) != 0) {
                foreach (var scene in EditorBuildSettings.scenes) {
                    var sceneName = Path.GetFileNameWithoutExtension (scene.path);
                    lines.Add (string.Format (SceneName, indent, CleanupName (sceneName), CleanupValue (sceneName)));
                }
            }

            // animators
            if ((int) (options & Options.Animators) != 0) {
                foreach (var guid in AssetDatabase.FindAssets ("t:animatorcontroller")) {
                    var assetPath = AssetDatabase.GUIDToAssetPath (guid);
                    var ac = AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController> (assetPath);
                    for (int i = 0, iMax = ac.parameters.Length; i < iMax; i++) {
                        var name = ac.parameters[i].name;
                        if (!uniquesList.Contains (name)) {
                            lines.Add (string.Format (AnimatorName, indent, CleanupName (name), CleanupValue (name)));
                            uniquesList.Add (name);
                        }
                    }
                }
                uniquesList.Clear ();
            }

            // axes
            if ((int) (options & Options.Axes) != 0) {
                var inputManager = AssetDatabase.LoadAllAssetsAtPath ("ProjectSettings/InputManager.asset")[0];
                var axes = new SerializedObject (inputManager).FindProperty ("m_Axes");
                for (int i = 0, iMax = axes.arraySize; i < iMax; i++) {
                    var axis = axes.GetArrayElementAtIndex (i).FindPropertyRelative ("m_Name").stringValue;
                    if (!uniquesList.Contains (axis)) {
                        lines.Add (string.Format (AxisName, indent, CleanupName (axis), CleanupValue (axis)));
                        uniquesList.Add (axis);
                    }
                }
                uniquesList.Clear ();
            }

            // shaders
            if ((int) (options & Options.Shaders) != 0) {
                foreach (var guid in AssetDatabase.FindAssets ("t:shader")) {
                    var assetPath = AssetDatabase.GUIDToAssetPath (guid);
                    var shader = AssetDatabase.LoadAssetAtPath<Shader> (assetPath);
                    if (shader.name.IndexOf ("Hidden", StringComparison.Ordinal) != 0) {
                        for (int i = 0, iMax = ShaderUtil.GetPropertyCount (shader); i < iMax; i++) {
                            if (!ShaderUtil.IsShaderPropertyHidden (shader, i)) {
                                var name = ShaderUtil.GetPropertyName (shader, i);
                                if (!uniquesList.Contains (name)) {
                                    lines.Add (string.Format (ShaderName, indent, CleanupName (name), CleanupValue (name)));
                                    uniquesList.Add (name);
                                }
                            }
                        }
                    }
                }
                uniquesList.Clear ();
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
        /// <param name="options">Options for generation.</param>
        public static string Generate (string fileName, string nsName, Options options) {
            if (string.IsNullOrEmpty (fileName) || string.IsNullOrEmpty (nsName)) {
                return "invalid parameters";
            }
            if ((int) options == 0) {
                return string.Empty;
            }
            var fullFileName = Path.Combine (Application.dataPath, fileName);
            var className = Path.GetFileNameWithoutExtension (fullFileName);
            try {
                var path = Path.GetDirectoryName (fullFileName);
                if (!Directory.Exists (path)) {
                    Directory.CreateDirectory (path);
                }
                var fields = GenerateFields (new string ('\t', 2), options);
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