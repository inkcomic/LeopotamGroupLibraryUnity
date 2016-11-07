//-------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2016 Leopotam <leopotam@gmail.com>
//-------------------------------------------------------

using LeopotamGroup.Gui.Widgets;
using LeopotamGroup.Localization;
using UnityEngine;

namespace LeopotamGroup.Gui.Localize {
    /// <summary>
    /// Localization helper for GuiSprite.
    /// </summary>
    [RequireComponent (typeof (GuiSprite))]
    public sealed class GuiLocalizeSprite : MonoBehaviour {
        [SerializeField]
        string _token;

        GuiSprite _sprite;

        void OnEnable () {
            OnLocalize ();
        }

        void OnLocalize () {
            if (_sprite == null) {
                _sprite = GetComponent <GuiSprite> ();
            }
            if (!string.IsNullOrEmpty (_token)) {
                _sprite.SpriteName = Localizer.Get (_token);
            }
        }
    }
}