
// -------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// -------------------------------------------------------

using LeopotamGroup.Gui.Widgets;
using LeopotamGroup.Localization;
using UnityEngine;

namespace LeopotamGroup.Gui.Localize {
    /// <summary>
    /// Localization helper for GuiLabel.
    /// </summary>
    [RequireComponent (typeof (GuiLabel))]
    public sealed class GuiLocalizeLabel : MonoBehaviour {
        [SerializeField]
        string _token;

        GuiLabel _label;

        void OnEnable () {
            OnLocalize ();
        }

        void OnLocalize () {
            if (_label == null) {
                _label = GetComponent<GuiLabel> ();
            }
            if (!string.IsNullOrEmpty (_token)) {
                _label.Text = Localizer.Get (_token);
            }
        }
    }
}