/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * This source code is licensed under the license found in the
 * LICENSE file in the root directory of this source tree.
 */

using Meta.WitAi.Attributes;
using Meta.WitAi.Composer.Integrations;
using Meta.WitAi.Composer.Interfaces;
using Meta.WitAi.Events;
using Meta.WitAi.Json;
using Meta.WitAi.Utilities;
using UnityEngine;

namespace Meta.WitAi.Composer.Handlers
{
    /// <summary>
    /// Provides UnityEvents that can be subscribed to when Composer triggers speech based events.
    /// </summary>
    public class ComposerSpeechUnityEvents : MonoBehaviour, IComposerSpeechHandler
    {
        [TooltipBox("Events for receipt of partial transcriptions")]
        [SerializeField] private StringEvent onPartialText;
        [SerializeField] private WitObjectEvent onPartialTextResponse;

        [TooltipBox("Events for receipt of full transcriptions")]
        [SerializeField] private StringEvent onFullText;
        [SerializeField] private WitObjectEvent onFullTextResponse;

        /// <summary>
        /// Speaks the specified phrase
        /// </summary>
        /// <param name="sessionData">Specified composer, context data and response data</param>
        public void SpeakPhrase(ComposerSessionData sessionData, string final)
        {
            onFullTextResponse?.Invoke(HandleResponse(sessionData, final));
            onFullText.Invoke(final);
        }

        /// <summary>
        /// Whether the specific session data response is still being spoken
        /// </summary>
        /// <param name="sessionData">Specified composer, context data and response data</param>
        public bool IsSpeaking(ComposerSessionData sessionData)
        {
            return false;
        }

        /// <summary>
        /// Called when a partial string is received
        /// </summary>
        /// <param name="sessionData">The current session's data at the time of the partial string event</param>
        /// <param name="partial">The partial string received in the partial response</param>
        public void SpeakPartial(ComposerSessionData sessionData, string partial)
        {

            onPartialTextResponse?.Invoke(HandleResponse(sessionData, partial));
            onPartialText.Invoke(partial);
        }

        private WitResponseClass HandleResponse(ComposerSessionData sessionData, string text)
        {
            var speech = sessionData.GetTextResponse();
            if (null == speech)
            {
                speech = new WitResponseClass();
                speech[WitComposerConstants.RESPONSE_TEXT] = text;
            }

            return speech;
        }
    }
}
