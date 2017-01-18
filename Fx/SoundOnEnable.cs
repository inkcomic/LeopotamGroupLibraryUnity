// -------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// -------------------------------------------------------

using LeopotamGroup.Common;
using UnityEngine;

#pragma warning disable 649

namespace LeopotamGroup.Fx {
    /// <summary>
    /// Setup FX parameters on enable.
    /// </summary>
    public sealed class SoundOnEnable : MonoBehaviour {
        [SerializeField]
        AudioClip _sound;

        [SerializeField]
        SoundFxChannel _channel = SoundFxChannel.First;

        /// <summary>
        /// Should new FX force interrupts FX at channel or not.
        /// </summary>
        [SerializeField]
        bool _isInterrupt;

        void OnEnable () {
            Singleton.Get<SoundManager> ().PlayFx (_sound, _channel, _isInterrupt);
        }
    }
}