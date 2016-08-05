//-------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2016 Leopotam <leopotam@gmail.com>
//-------------------------------------------------------
#if UNITY_EDITOR

using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace LeopotamGroup.EditorHelpers {
    public enum GameObjectLabelIconType {
        Gray = 0,
        Blue,
        Teal,
        Green,
        Yellow,
        Orange,
        Red,
        Purple
    }

    public enum GameObjectImageIconType {
        CircleGray = 0,
        CircleBlue,
        CircleTeal,
        CircleGreen,
        CircleYellow,
        CircleOrange,
        CircleRed,
        CirclePurple,
        DiamondGray,
        DiamondBlue,
        DiamondTeal,
        DiamondGreen,
        DiamondYellow,
        DiamondOrange,
        DiamondRed,
        DiamondPurple
    }

    public static class GameObjectIcon {
        static List<Texture2D> _icons;

        const int IconTypeCount = 3;

        const string SetIconMethodName = "SetIconForObject";

        const string LabelIconMask = "sv_label_";

        const string ImageIconMask = "sv_icon_dot";

        const string ImageIconSmallSuffix = "_sml";

        const string ImageIconLargeSuffix = "_pix16_gizmo";

        readonly static MethodInfo _setIcon;

        readonly static object[] _setIconArgs = new object[2];

        static GameObjectIcon () {
            _setIcon = typeof (EditorGUIUtility).GetMethod (SetIconMethodName,
                BindingFlags.Static | BindingFlags.NonPublic);
            _icons = new List<Texture2D> ();
            FillIcons (_icons, LabelIconMask, string.Empty, 0, 8);
            FillIcons (_icons, ImageIconMask, ImageIconSmallSuffix, 0, 16);
            FillIcons (_icons, ImageIconMask, ImageIconLargeSuffix, 0, 16);
        }

        static void FillIcons (IList<Texture2D> dict, string baseName, string postFix, int start, int count) {
            GUIContent content;
            for (var i = 0; i < count; i++) {
                content = EditorGUIUtility.IconContent (baseName + (start + i) + postFix);
                _icons.Add (content != null ? content.image as Texture2D : null);
            }
        }

        static Texture2D GetIcon (int iconOffset, int iconId) {
            iconOffset = Mathf.Clamp (iconOffset + iconId, 0, _icons.Count - 1);
            return _icons[iconOffset];
        }

        public static void SetLabelIcon (this GameObject go, GameObjectLabelIconType iconType) {
            SetIcon (go, GetIcon (0, (int) iconType));
        }

        public static void SetSmallImageIcon (this GameObject go, GameObjectImageIconType iconType) {
            SetIcon (go, GetIcon (0 + 8, (int) iconType));
        }

        public static void SetLargeImageIcon (this GameObject go, GameObjectImageIconType iconType) {
            SetIcon (go, GetIcon (0 + 8 + 16, (int) iconType));
        }

        static void SetIcon (GameObject go, Texture2D icon) {
            _setIconArgs[0] = go;
            _setIconArgs[1] = icon;
            _setIcon.Invoke (null, _setIconArgs);
        }
    }
}

#endif