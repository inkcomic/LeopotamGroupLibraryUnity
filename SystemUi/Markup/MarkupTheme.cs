// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using UnityEngine;

namespace LeopotamGroup.SystemUi.Markup {
    public sealed class MarkupTheme : ScriptableObject {
        [SerializeField]
        string _name = "NewMarkupTheme";

        [SerializeField]
        Sprite _buttonNormalSprite;

        [SerializeField]
        Sprite _buttonPressedSprite;

        [SerializeField]
        Sprite _buttonHighlightedSprite;

        [SerializeField]
        Sprite _buttonDisabledSprite;

        [SerializeField]
        Color _buttonNormalColor = Color.white;

        [SerializeField]
        Color _buttonPressedColor = Color.white;

        [SerializeField]
        Color _buttonHighlightedColor = Color.white;

        [SerializeField]
        Color _buttonDisabledColor = Color.gray;

        [SerializeField]
        Sprite _sliderBackgroundSprite;

        [SerializeField]
        Sprite _sliderForegroundSprite;

        [SerializeField]
        Sprite _sliderHandleSprite;

        [SerializeField]
        Color _sliderBackgroundColor = Color.gray;

        [SerializeField]
        Color _sliderForegroundColor = Color.white;

        [SerializeField]
        Color _sliderHandleColor = Color.white;

        [SerializeField]
        Sprite _toggleBackgroundSprite;

        [SerializeField]
        Sprite _toggleForegroundSprite;

        [SerializeField]
        Color _toggleBackgroundColor = Color.white;

        [SerializeField]
        Color _toggleForegroundColor = Color.white;

        [SerializeField]
        Vector2 _toggleBackgroundSize = Vector2.one * 24f;

        [SerializeField]
        Vector2 _toggleForegroundSize = Vector2.one * 24f;

        public enum ButtonState {
            Normal,
            Pressed,
            Highlighted,
            Disabled
        }

        public enum SliderState {
            Background,
            Foreground,
            Handle
        }

        public enum ToggleState {
            Background,
            Foreground
        }

        public string GetName () {
            return _name;
        }

        public Sprite GetButtonSprite (ButtonState state) {
            switch (state) {
                case ButtonState.Normal:
                    return _buttonNormalSprite;
                case ButtonState.Pressed:
                    return _buttonPressedSprite;
                case ButtonState.Highlighted:
                    return _buttonHighlightedSprite;
                case ButtonState.Disabled:
                    return _buttonDisabledSprite;
                default:
                    return null;
            }
        }

        public Color GetButtonColor (ButtonState state) {
            switch (state) {
                case ButtonState.Normal:
                    return _buttonNormalColor;
                case ButtonState.Pressed:
                    return _buttonPressedColor;
                case ButtonState.Highlighted:
                    return _buttonHighlightedColor;
                case ButtonState.Disabled:
                    return _buttonDisabledColor;
                default:
                    return Color.black;
            }
        }

        public Sprite GetSliderSprite (SliderState state) {
            switch (state) {
                case SliderState.Background:
                    return _sliderBackgroundSprite;
                case SliderState.Foreground:
                    return _sliderForegroundSprite;
                case SliderState.Handle:
                    return _sliderHandleSprite;
                default:
                    return null;
            }
        }

        public Color GetSliderColor (SliderState state) {
            switch (state) {
                case SliderState.Background:
                    return _sliderBackgroundColor;
                case SliderState.Foreground:
                    return _sliderForegroundColor;
                case SliderState.Handle:
                    return _sliderHandleColor;
                default:
                    return Color.black;
            }
        }

        public Sprite GetToggleSprite (ToggleState state) {
            switch (state) {
                case ToggleState.Background:
                    return _toggleBackgroundSprite;
                case ToggleState.Foreground:
                    return _toggleForegroundSprite;
                default:
                    return null;
            }
        }

        public Color GetToggleColor (ToggleState state) {
            switch (state) {
                case ToggleState.Background:
                    return _toggleBackgroundColor;
                case ToggleState.Foreground:
                    return _toggleForegroundColor;
                default:
                    return Color.black;
            }
        }

        public Vector2 GetToggleSize (ToggleState state) {
            switch (state) {
                case ToggleState.Background:
                    return _toggleBackgroundSize;
                case ToggleState.Foreground:
                    return _toggleForegroundSize;
                default:
                    return Vector2.zero;
            }
        }
    }
}