// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Audio;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// Sample script for text to speech
    /// </summary>
    [RequireComponent(typeof(TextToSpeech))]
    public class TextToSpeechSample : MonoBehaviour
    {
        private TextToSpeech textToSpeech;

        private void Awake()
        {
            textToSpeech = GetComponent<TextToSpeech>();
        }

        /// <summary>
        /// Start playing Text To Speech generated voice
        /// </summary>
        public void Speak()
        {
            // If we have a text to speech manager on the target object, say something.
            // This voice will appear to emanate from the object.
            if (textToSpeech != null)
            {
                // Create message
                var msg = string.Format("This is the {0} voice. You hear me cause you clicked me. Welcome to the HoloLens experience! Here everyhting is possible!", textToSpeech.Voice.ToString());

                // Speak message
                textToSpeech.StartSpeaking(msg);
            }
        }
    }
}
