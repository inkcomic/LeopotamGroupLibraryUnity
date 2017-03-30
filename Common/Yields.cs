// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;

namespace LeopotamGroup.Common {
    /// <summary>
    /// Helper for use yield waiters without GC.
    /// </summary>
    public static class Yields {
        /// <summary>
        /// Get WaitForEndOfFrame yield instruction. Yes, its null. :)
        /// </summary>
        public static readonly WaitForEndOfFrame WaitForEndOfFrame = null;

        /// <summary>
        /// Get WaitForFixedUpdate yield instruction.
        /// </summary>
        public static readonly WaitForFixedUpdate WaitForFixedUpdate = new WaitForFixedUpdate ();

        static Dictionary<float, WaitForSeconds> _waitForSeconds = new Dictionary<float, WaitForSeconds> ();

        /// <summary>
        /// Get WaitForSeconds yield instruction.
        /// </summary>
        /// <param name="seconds">Required delay.</param>
        public static WaitForSeconds WaitForSeconds (float seconds) {
            WaitForSeconds retVal;
            if (!_waitForSeconds.TryGetValue (seconds, out retVal)) {
                _waitForSeconds[seconds] = new WaitForSeconds (seconds);
            }
            return retVal;
        }
    }
}