/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * This source code is licensed under the license found in the
 * LICENSE file in the root directory of this source tree.
 */

using System;
using System.Text;
using Meta.WitAi.Json;

namespace Meta.WitAi.Composer.Data
{
    [Serializable]
    public class ComposerResponseData
    {
        public const string FIELD_EXPECTS_INPUT = "expects_input";
        public const string FIELD_ACTION = "action";
        public const string FIELD_RESPONSE = "response";
        public const string FIELD_TEXT = "text";
        
        /// <summary>
        /// Whether this response expects additional user input
        /// </summary>
        public bool expectsInput;

        /// <summary>
        /// The action id to be called automatically if desired
        /// </summary>
        public string actionID;

        /// <summary>
        /// Response phrase returned from the composer
        /// </summary>
        public string responsePhrase;

        /// <summary>
        /// Response for any errors
        /// </summary>
        public string error;

        public WitResponseNode witResponse;

        // Error constructor
        public ComposerResponseData(string newError)
        {
            error = newError;
        }

        // Json constructor
        public ComposerResponseData(WitResponseNode response, StringBuilder errorBuilder)
        {
            try
            {
                witResponse = response;
                expectsInput = witResponse[FIELD_EXPECTS_INPUT].AsBool;
                actionID = witResponse[FIELD_ACTION];
                WitResponseClass phrase = witResponse[FIELD_RESPONSE].AsObject;
                if (phrase != null)
                {
                    responsePhrase = phrase[FIELD_TEXT];
                }
            }
            catch (Exception e)
            {
                errorBuilder.AppendLine($"Response Parse Failed\n{e.ToString()}");
            }
        }
    }
}
