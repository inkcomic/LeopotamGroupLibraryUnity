// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2017 Mopsicus <immops@gmail.com>
// ----------------------------------------------------------------------------

using System.IO;
using UnityEditor;
using UnityEngine;

namespace LeopotamGroup.EditorHelpers.UnityEditors {

	/// <summary>
	/// Buildmanager for assets bundle
	/// </summary>
	sealed class CreateAssetsBundle : EditorWindow {

		private string _path = "AssetBundles";
		private BuildAssetBundleOptions _options = BuildAssetBundleOptions.UncompressedAssetBundle;
		private BuildTarget _platform = BuildTarget.iOS;

		[MenuItem ("Window/LeopotamGroupLibrary/Build assets bundle...")]
		public static void OpenEditorWindow () {
			var win = EditorWindow.GetWindow (typeof (CreateAssetsBundle));
			var pos = win.position;
			pos.width = 500f;
			pos.height = 250f;
			win.position = pos;
			win.titleContent.text = "Asset bundles";
		}

		void OnGUI () {
			GUILayout.Label ("AssetBundles Settings", EditorStyles.boldLabel);

			GUILayout.BeginHorizontal (GUI.skin.box);
			GUILayout.BeginVertical ();

			GUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Output Path:", EditorStyles.label, GUILayout.Width (EditorGUIUtility.labelWidth));
			EditorGUILayout.SelectableLabel (_path, EditorStyles.textField, GUILayout.Height (EditorGUIUtility.singleLineHeight));
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Options:", EditorStyles.label, GUILayout.Width (EditorGUIUtility.labelWidth));
			_options = (BuildAssetBundleOptions) EditorGUILayout.EnumPopup (_options);
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Platform:", EditorStyles.label, GUILayout.Width (EditorGUIUtility.labelWidth));
			_platform = (BuildTarget) EditorGUILayout.EnumPopup (_platform);
			GUILayout.EndHorizontal ();

			GUILayout.EndVertical ();
			GUILayout.EndHorizontal ();

			GUILayout.Space (10f);

			EditorGUILayout.HelpBox ("Note: if you will load bundle directly from disk – choose option \"UncompressedAssetBundle\", to pack bundle and download from remote source – choose \"None\".", MessageType.Info, true);

			GUILayout.Space (10f);

			EditorGUILayout.Separator ();
			if (GUILayout.Button ("Create AssetBundles", GUILayout.Height (50f))) {
				if (!Directory.Exists (_path))
					Directory.CreateDirectory (_path);
				BuildPipeline.BuildAssetBundles (_path, _options, _platform);
				EditorUtility.RevealInFinder (_path);
			}
		}
	}
}
