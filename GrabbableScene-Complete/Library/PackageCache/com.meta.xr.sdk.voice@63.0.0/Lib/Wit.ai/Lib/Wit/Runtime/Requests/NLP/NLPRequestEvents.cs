/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * This source code is licensed under the license found in the
 * LICENSE file in the root directory of this source tree.
 */

using System;
using UnityEngine;
using UnityEngine.Events;

namespace Meta.Voice
{
    /// <summary>
    /// A unity event that returns a decoded nlp response data
    /// </summary>
    [Serializable]
    public class NLPRequestResponseEvent<TResponseData> : UnityEvent<TResponseData> {}

    [Serializable]
    public class NLPRequestEvents<TUnityEvent, TResponseData>
        : TranscriptionRequestEvents<TUnityEvent>
        where TUnityEvent : UnityEventBase
    {
        /// <summary>
        /// Called on request language processing raw text received
        /// </summary>
        public TranscriptionRequestEvent OnRawResponse => _onRawResponse;
        [Header("NLP Events")] [Tooltip("Called on every request response text.")]
        [SerializeField] private TranscriptionRequestEvent _onRawResponse = Activator.CreateInstance<TranscriptionRequestEvent>();

        /// <summary>
        /// Called on request language processing while audio is still being analyzed
        /// </summary>
        public NLPRequestResponseEvent<TResponseData> OnPartialResponse => _onPartialResponse;
        [Tooltip("Called for partially decoded request responses.")]
        [SerializeField] private NLPRequestResponseEvent<TResponseData> _onPartialResponse = Activator.CreateInstance<NLPRequestResponseEvent<TResponseData>>();

        /// <summary>
        /// Called on request language processing once completely analyzed
        /// </summary>
        public NLPRequestResponseEvent<TResponseData> OnFullResponse => _onFullResponse;
        [Tooltip("Called on request language processing once completely analyzed.")]
        [SerializeField] private NLPRequestResponseEvent<TResponseData> _onFullResponse = Activator.CreateInstance<NLPRequestResponseEvent<TResponseData>>();
    }
}
