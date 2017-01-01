
// -------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// -------------------------------------------------------

using LeopotamGroup.Localization;
using LeopotamGroup.SystemUI.Atlases;
using UnityEngine.UI;
using UnityEngine;

namespace LeopotamGroup.SystemUI.Localization {
    /// <summary>
    /// Localization helper for System UI Sprite.
    /// </summary>
    [RequireComponent (typeof (Image))]
    public sealed class ImageLocalization : MonoBehaviour {
        [SerializeField]
        string _token;

        [SerializeField]
        SpriteAtlas _atlas;

        Image _image;

        void OnEnable () {
            OnLocalize ();
        }

        void OnLocalize () {
            if (!string.IsNullOrEmpty (_token) && (object) _atlas != null) {
                if ((object) _image == null) {
                    _image = GetComponent<Image> ();
                }
                _image.sprite = _atlas.Get (Localizer.Get (_token));
            }
        }
    }
}