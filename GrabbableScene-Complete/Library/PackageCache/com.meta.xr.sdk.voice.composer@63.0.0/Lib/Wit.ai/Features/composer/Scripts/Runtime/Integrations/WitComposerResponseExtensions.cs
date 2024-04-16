/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * This source code is licensed under the license found in the
 * LICENSE file in the root directory of this source tree.
 */

using Meta.WitAi.Composer.Data;
using Meta.WitAi.Json;

namespace Meta.WitAi.Composer.Integrations
{
    public static class WitComposerResponseExtensions
    {
        /// <summary>
        /// Gets the speech blob from a response object.
        ///
        /// ex:
        /// {
        ///   "partial_response": {
        ///     "speech": {
        ///        "q": "some tts"
        ///     }
        /// }
        /// 
        /// </summary>
        /// <param name="response">The composer response data that might contain speech data from a response</param>
        /// <returns>The speech blob or null if none was found.</returns>
        public static WitResponseClass GetTextResponse(this WitResponseNode response)
        {
            return response.GetResponse();
        }

        /// <summary>
        /// Gets the speech blob from a response object in ComposerSessionData
        ///
        /// ex:
        /// {
        ///   "partial_response": {
        ///     "speech": {
        ///        "q": "some tts"
        ///     }
        /// }
        /// 
        /// </summary>
        /// <param name="responseData">The composer response data that might contain speech data from a response</param>
        /// <returns>The speech blob or null if none was found.</returns>
        public static WitResponseClass GetTextResponse(this ComposerResponseData responseData)
        {
            return responseData?.witResponse?.GetTextResponse();
        }

        /// <summary>
        /// Gets the speech blob from a response object in ComposerSessionData
        ///
        /// ex:
        /// {
        ///   "partial_response": {
        ///     "speech": {
        ///        "q": "some tts"
        ///     }
        /// }
        /// 
        /// </summary>
        /// <param name="sessionData">The composer session data to check for speech values</param>
        /// <returns>The speech blob or null if none was found.</returns>
        public static WitResponseClass GetTextResponse(this ComposerSessionData sessionData)
        {
            return sessionData?.responseData?.GetTextResponse();
        }

        /// <summary>
        /// Returns written text meant to be displayed for accessibility or visual purposes.
        ///
        /// Note: This text can be used as a fallback for TTS, but if you are passing data to a TTS system you will
        /// more likely want to use GetSpeechTTS
        /// </summary>
        /// <param name="response"></param>
        /// <returns>Returns the text or string.Empty if no text was found</returns>
        public static string GetText(this WitResponseNode response)
        {
            var responseObject = response.AsObject;
            if (responseObject?.HasChild(WitComposerConstants.RESPONSE_TEXT) ?? false)
            {
                return responseObject[WitComposerConstants.RESPONSE_TEXT];
            }

            responseObject = responseObject?.GetResponse();
            return responseObject?[WitComposerConstants.RESPONSE_TEXT].Value ?? string.Empty;
        }

        /// <summary>
        /// Returns written text meant to be displayed for accessibility or visual purposes.
        ///
        /// Note: This text can be used as a fallback for TTS, but if you are passing data to a TTS system you will
        /// more likely want to use GetSpeechTTS
        /// </summary>
        /// <param name="response"></param>
        /// <returns>Returns the text or string.Empty if no text was found</returns>
        public static string GetText(this ComposerResponseData responseData)
        {
            return responseData?.witResponse?.GetText();
        }

        /// <summary>
        /// Returns written text meant to be displayed for accessibility or visual purposes.
        ///
        /// Note: This text can be used as a fallback for TTS, but if you are passing data to a TTS system you will
        /// more likely want to use GetSpeechTTS
        /// </summary>
        /// <param name="response"></param>
        /// <returns>Returns the text or string.Empty if no text was found</returns>
        public static string GetText(this ComposerSessionData sessionData)
        {
            return sessionData?.responseData?.GetText();
        }

        /// <summary>
        /// Gets the speech node from a wit response.
        ///
        /// NOTE: this can either be a root response with a partial/final or the contents of that node.
        /// </summary>
        /// <param name="response"></param>
        /// <returns>A speech object {"q": "something", "voice": "some voice", ...} or null.</returns>
        public static WitResponseClass GetSpeech(this WitResponseNode response)
        {
            var node = response.AsObject;

            if (node.HasChild(WitComposerConstants.RESPONSE_NODE_PARTIAL))
            {
                node = node[WitComposerConstants.RESPONSE_NODE_PARTIAL].AsObject;
            } 
            else if (node.HasChild(WitComposerConstants.RESPONSE_NODE_FINAL))
            {
                node = node[WitComposerConstants.RESPONSE_NODE_FINAL].AsObject;
            }

            if (node.HasChild(WitComposerConstants.RESPONSE_NODE_SPEECH))
            {
                node = node[WitComposerConstants.RESPONSE_NODE_SPEECH].AsObject;
            }
            else
            {
                node = null;
            }

            return node;
        }

        /// <summary>
        /// Returns text that is optimized for TTS if present or the regular response text if no TTS optimized text is present.
        /// </summary>
        /// <param name="response"></param>
        /// <returns>Returns the text or string.Empty if no text was found</returns>
        public static string GetTTS(this WitResponseNode response)
        {
            // We'll first check to see if this is a speech blob of some kind and it has a TTS data node.
            var speechBlock = response.AsObject;

            if (speechBlock?.HasChild(WitComposerConstants.RESPONSE_NODE_SPEECH) ?? false)
            {
                speechBlock = speechBlock[WitComposerConstants.RESPONSE_NODE_SPEECH].AsObject;
            }
            
            if (speechBlock?.HasChild(WitComposerConstants.RESPONSE_NODE_Q) ?? false)
            {
                return speechBlock[WitComposerConstants.RESPONSE_NODE_Q].Value ?? string.Empty;
            }

            // If there wasn't a q node in any of the previous locations, we'll check for a response block and attempt
            // to get the q node from it.
            var responseBlock = response.GetTextResponse();
            if (responseBlock?.HasChild(WitComposerConstants.RESPONSE_NODE_SPEECH) ?? false)
            {
                speechBlock = responseBlock[WitComposerConstants.RESPONSE_NODE_SPEECH].AsObject;
                return speechBlock?[WitComposerConstants.RESPONSE_NODE_Q] ?? string.Empty;
            }

            return string.Empty;
        }

        /// <summary>
        /// Returns text that is optimized for TTS if present or the regular response text if no TTS optimized text is present.
        /// </summary>
        /// <param name="response"></param>
        /// <returns>Returns the text or string.Empty if no text was found</returns>
        public static string GetTTS(this ComposerResponseData responseData)
        {
            return responseData?.witResponse?.GetTTS();
        }

        /// <summary>
        /// Returns text that is optimized for TTS if present or the regular response text if no TTS optimized text is present.
        /// </summary>
        /// <param name="response"></param>
        /// <returns>Returns the text or string.Empty if no text was found</returns>
        public static string GetTTS(this ComposerSessionData sessionData)
        {
            return sessionData?.responseData?.GetTTS();
        }


        /// <summary>
        /// Finds the full TTS response object
        /// </summary>
        /// <param name="data">Data including voice preset data as well as TTS to convert to speech.</param>
        /// <returns>Returns null if no tts data was found in the hierarchy</returns>
        public static WitResponseClass GetTTSObject(this WitResponseClass data)
        {
            WitResponseClass tts = null;

            // Walk down the tree if needed until we find the 'q' node. The 'q' node is our real target here since it
            // contains the full tts data we want to send to /synthesize to get the proper decoding.
            if (data.HasChild(WitResultUtilities.WIT_PARTIAL_RESPONSE))
            {
                data = data[WitResultUtilities.WIT_PARTIAL_RESPONSE].AsObject;
            }
            
            if (data.HasChild(WitComposerConstants.RESPONSE_NODE_SPEECH))
            {
                data = data[WitComposerConstants.RESPONSE_NODE_SPEECH].AsObject;
            }

            if (data.HasChild(WitComposerConstants.RESPONSE_NODE_Q))
            {
                tts = data[WitComposerConstants.RESPONSE_NODE_Q].AsObject;
            }

            return tts;
        }
    }
}
