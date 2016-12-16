
// -------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2016 Leopotam <leopotam@gmail.com>
// -------------------------------------------------------

using LeopotamGroup.Gui.Common;
using LeopotamGroup.Gui.UnityEditors;
using System;
using UnityEditor;
using UnityEngine;

namespace LeopotamGroup.Gui.Widgets.UnityEditors {
    [CanEditMultipleObjects]
    [CustomEditor (typeof (GuiSprite))]
    sealed class GuiSpriteInspector : Editor {
        SerializedProperty _atlasProperty;

        SerializedProperty _nameProperty;

        SerializedProperty _typeProperty;

        SerializedProperty _fillCenterProperty;

        SerializedProperty _flipHorProperty;

        SerializedProperty _flipVerProperty;

        SerializedProperty _widthProperty;

        SerializedProperty _heightProperty;

        SerializedProperty _depthProperty;

        SerializedProperty _colorProperty;

        SerializedProperty _fillValueProperty;

        static GUIContent _typeGuiContent;

        static GUIContent _fillCenterGuiContent;

        static GUIContent _flipHorGuiContent;

        static GUIContent _flipVerGuiContent;

        static GUIStyle _textStyle;

        void OnEnable () {
            if (_typeGuiContent == null) {
                _typeGuiContent = new GUIContent ("Type");
                _fillCenterGuiContent = new GUIContent ("Fill center");
                _flipHorGuiContent = new GUIContent ("Flip horizontal");
                _flipVerGuiContent = new GUIContent ("Flip vertical");
                _textStyle = new GUIStyle {
                    alignment = TextAnchor.LowerCenter,
                    fontSize = 16,
                    normal = new GUIStyleState { textColor = Color.white }
                };
            }

            _atlasProperty = serializedObject.FindProperty ("_spriteAtlas");
            _nameProperty = serializedObject.FindProperty ("_spriteName");
            _typeProperty = serializedObject.FindProperty ("_spriteType");
            _fillCenterProperty = serializedObject.FindProperty ("_isSpriteCenterFilled");
            _flipHorProperty = serializedObject.FindProperty ("_isSpriteFlippedHorizontal");
            _flipVerProperty = serializedObject.FindProperty ("_isSpriteFlippedVertical");
            _widthProperty = serializedObject.FindProperty ("_width");
            _heightProperty = serializedObject.FindProperty ("_height");
            _depthProperty = serializedObject.FindProperty ("_depth");
            _colorProperty = serializedObject.FindProperty ("_color");
            _fillValueProperty = serializedObject.FindProperty ("_fillValue");
        }

        public override void OnInspectorGUI () {
            serializedObject.Update ();
            var sprite = target as GuiSprite;

            var atlasName = string.Format ("Atlas: <{0}>", sprite.SpriteAtlas != null ? sprite.SpriteAtlas.name : "Empty");
            if (GUILayout.Button (atlasName)) {
                SearchWindow.Open<GuiAtlas> ("Select atlas", "t:prefab", sprite.SpriteAtlas, assetPath => {
                    // If not canceled.
                    if (assetPath != null) {
                        // None.
                        if (assetPath == string.Empty) {
                            sprite.SpriteAtlas = null;
                            _nameProperty.stringValue = null;
                        } else {
                            sprite.SpriteAtlas = AssetDatabase.LoadAssetAtPath<GuiAtlas> (assetPath);
                        }
                        _atlasProperty.objectReferenceValue = sprite.SpriteAtlas;
                    }
                });
            }

            if (sprite.SpriteAtlas != null) {
                var spriteList = sprite.SpriteAtlas.GetSpriteNames ();
                var id = Array.IndexOf (spriteList, sprite.SpriteName);
                id = EditorGUILayout.Popup ("Sprite", id, spriteList);
                if (id >= 0 && id < spriteList.Length) {
                    _nameProperty.stringValue = spriteList[id];
                }
            }

            EditorGUILayout.PropertyField (_typeProperty, _typeGuiContent);

            var type = (GuiSpriteType) _typeProperty.enumValueIndex;
            if (type != GuiSpriteType.Simple && type != GuiSpriteType.RoundFilled) {
                EditorGUILayout.PropertyField (_fillCenterProperty, _fillCenterGuiContent);
            }

            if (type == GuiSpriteType.RoundFilled) {
                EditorGUILayout.Slider (_fillValueProperty, 0f, 1f);
            }

            EditorGUILayout.Separator ();

            EditorGUILayout.PropertyField (_flipHorProperty, _flipHorGuiContent);
            EditorGUILayout.PropertyField (_flipVerProperty, _flipVerGuiContent);

            EditorGUILayout.Separator ();

            EditorGUILayout.PropertyField (_widthProperty);
            if (_widthProperty.intValue < 0) {
                _widthProperty.intValue = 0;
            }

            EditorGUILayout.PropertyField (_heightProperty);
            if (_heightProperty.intValue < 0) {
                _heightProperty.intValue = 0;
            }

            bool needUpdate = false;
            if (GUILayout.Button ("Reset size to original")) {
                Undo.RecordObject (sprite, "leopotamgroup.gui.sprite.set-original-size");
                sprite.ResetSize ();
                needUpdate = true;
            }
            if (GUILayout.Button ("Align tiled size to original")) {
                Undo.RecordObject (sprite, "leopotamgroup.gui.sprite.align-original-size");
                sprite.AlignTiledSizeToOriginal ();
                needUpdate = true;
            }

            if (GUILayout.Button ("Bake scale to widget size")) {
                Undo.RecordObject (sprite, "leopotamgroup.gui.sprite.bake-scale-size");
                GuiWidget s;
                foreach (var item in targets) {
                    s = item as GuiSprite;
                    if (s != null) {
                        s.BakeScale ();
                    }
                }
                needUpdate = true;
                SceneView.RepaintAll ();
            }

            EditorGUILayout.Separator ();

            EditorGUILayout.IntSlider (_depthProperty, -GuiWidget.DepthLimit, GuiWidget.DepthLimit);
            EditorGUILayout.PropertyField (_colorProperty);

            if (serializedObject.ApplyModifiedProperties () || needUpdate || EditorIntegration.IsUndo ()) {
                EditorIntegration.UpdateVisuals (target);
            }
        }

        public override bool HasPreviewGUI () {
            return true;
        }

        public override void OnPreviewGUI (Rect r, GUIStyle background) {
            base.OnPreviewGUI (r, background);
            var sprite = target as GuiSprite;
            if (sprite.SpriteAtlas != null) {
                var sprData = sprite.SpriteAtlas.GetSpriteData (sprite.SpriteName);
                if (sprData != null) {
                    var c = r.center;
                    var size = Mathf.Min (r.width, r.height);
                    r.Set (c.x - size * 0.5f, c.y - size * 0.5f, size, size);
                    var uvRect = new Rect (sprData.CornerX, sprData.CornerY, sprData.CornerW, sprData.CornerH);
                    GUI.DrawTextureWithTexCoords (r, sprite.SpriteAtlas.ColorTexture, uvRect);

                    var caption = sprite.GetOriginalSize ().ToString ();
                    GUI.color = Color.black;
                    GUI.Label (r, caption, _textStyle);
                    var r2 = r;
                    r2.position -= Vector2.one;
                    GUI.color = Color.yellow;
                    GUI.Label (r2, caption, _textStyle);

                    Handles.color = Color.yellow;
                    var offset = r.x + r.width * sprData.BorderL / sprData.CornerW;
                    Handles.DrawAAPolyLine (4f, new Vector3 (offset, r.y), new Vector3 (offset, r.yMax));
                    offset = r.xMax - r.width * sprData.BorderR / sprData.CornerW;
                    Handles.DrawAAPolyLine (4f, new Vector3 (offset, r.y), new Vector3 (offset, r.yMax));

                    offset = r.y + r.height * sprData.BorderT / sprData.CornerH;
                    Handles.DrawAAPolyLine (4f, new Vector3 (r.x, offset), new Vector3 (r.xMax, offset));
                    offset = r.yMax - r.height * sprData.BorderB / sprData.CornerH;
                    Handles.DrawAAPolyLine (4f, new Vector3 (r.x, offset), new Vector3 (r.xMax, offset));
                }
            }
        }
    }
}