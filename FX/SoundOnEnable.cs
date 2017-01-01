
// -------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// -------------------------------------------------------

using LeopotamGroup.Common;
using UnityEngine;

namespace LeopotamGroup.FX {
    /// <summary>
    /// Setup FX parameters on enable.
    /// </summary>
    public sealed class SoundOnEnable : MonoBehaviour {
        [SerializeField]
        AudioClip _sound;

        [SerializeField]
        SoundFXChannel _channel = SoundFXChannel.First;

        /// <summary>
        /// Should new FX force interrupts FX at channel or not.
        /// </summary>
        [SerializeField]
        bool _isInterrupt;

        void OnEnable () {
            Singleton.Get<SoundManager> ().PlayFX (_sound, _channel, _isInterrupt);
        }
    }
}