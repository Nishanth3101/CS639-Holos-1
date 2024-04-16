/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * This source code is licensed under the license found in the
 * LICENSE file in the root directory of this source tree.
 */

namespace Meta.WitAi.Composer.Interfaces
{
    public interface IComposerSpeechHandler
    {
        /// <summary>
        /// Speaks the specified phrase
        /// </summary>
        /// <param name="sessionData">Specified composer, context data and response data</param>
        void SpeakPhrase(ComposerSessionData sessionData, string final);

        /// <summary>
        /// Whether the specific session data response is still being spoken
        /// </summary>
        /// <param name="sessionData">Specified composer, context data and response data</param>
        bool IsSpeaking(ComposerSessionData sessionData);

        /// <summary>
        /// Called when a partial string is received
        /// </summary>
        /// <param name="sessionData">The current session's data at the time of the partial string event</param>
        /// <param name="partial">The partial string received in the partial response</param>
        void SpeakPartial(ComposerSessionData sessionData, string partial);
    }
}
