
// -------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// -------------------------------------------------------

using LeopotamGroup.Common;
using System.Collections;
using UnityEngine;

namespace LeopotamGroup.FX {
    /// <summary>
    /// Setup FX parameters on start.
    /// </summary>
    public sealed class SoundOnStart : MonoBehaviour {
        [SerializeField]
        AudioClip _sound;

        [SerializeField]
        SoundFXChannel _channel = SoundFXChannel.First;

        /// <summary>
        /// Should new FX force interrupts FX at channel or not.
        /// </summary>
        public bool IsInterrupt = false;

        IEnumerator Start () {
            yield return null;
            Singleton.Get<SoundManager> ().PlayFX (_sound, _channel, IsInterrupt);
        }
    }
}