/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * This source code is licensed under the license found in the
 * LICENSE file in the root directory of this source tree.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Meta.Voice;
using Meta.WitAi.Composer.Attributes;
using UnityEngine;
using Meta.WitAi.Composer.Data;
using Meta.WitAi.Composer.Integrations;
using Meta.WitAi.Composer.Interfaces;
using Meta.WitAi.Json;
using Meta.WitAi.Requests;
using UnityEngine.Serialization;

namespace Meta.WitAi.Composer
{
    public abstract class ComposerService : MonoBehaviour
    {
        #region VARIABLES

        /// <summary>
        /// Current session id to be used with composer service
        /// </summary>
        public string SessionID { get; private set; }

        /// <summary>
        /// Start of the current session
        /// </summary>
        public DateTime SessionStart { get; private set; }

        /// <summary>
        /// Current elapsed time of session
        /// </summary>
        public TimeSpan SessionElapsed => (SessionStart - DateTime.UtcNow);

        [SerializeField] private ComposerContextMap _currentContextMap;

        /// <summary>
        /// The current context map being used with the composer service
        /// </summary>
        public ComposerContextMap CurrentContextMap
        {
            get
            {
                if (null == _currentContextMap)
                {
                    SetContextMap(new ComposerContextMap());
                }

                return _currentContextMap;
            }
            private set => _currentContextMap = value;
        }

        /// <summary>
        /// The voice service this composer will use for activation
        /// </summary>
        [Header("Voice Settings")] [SerializeField]
        private VoiceService _voiceService;

        [SerializeField] private bool handleTts = true;

        public VoiceService VoiceService
        {
            get => _voiceService;
#if UNITY_EDITOR
            set => _voiceService = value;
#endif
        }

        /// <summary>
        /// Whether the composer service will be used for voice activation
        /// </summary>
        public bool RouteVoiceServiceToComposer
        {
            get => _routeVoiceServiceToComposer;
            set
            {
                _routeVoiceServiceToComposer = value;
                Events.OnComposerActiveChange?.Invoke(this, value);
            }
        }

        [FormerlySerializedAs("RouteVoiceServiceToComposer")] [SerializeField]
        private bool _routeVoiceServiceToComposer = true;

        /// <summary>
        /// Whether composer is currently active for the current voice request
        /// </summary>
        public bool IsComposerActive => _requests.Count > 0;

        private List<CurrentComposerRequest> _requests = new List<CurrentComposerRequest>();

        /// <summary>
        /// Delay from action completion and response to listen or graph continuation
        /// activation
        /// </summary>
        [Header("Composer Settings")] public float continueDelay = 0f;

        /// <summary>
        /// The context_map flag name used when to identify an event vs a text/voice input.
        /// </summary>
        [Tooltip(
            "A configurable flag for use in the Composer graph to differentiate activations to the server without" +
            " text/voice input, such as a context map update. In such cases, this will be set to true. \n" +
            "For voice and text activations, this will be set to false.")]
        [SerializeField]
        public string contextMapEventKey = "state_event";

        /// <summary>
        /// Whether this service should automatically handle input
        /// activation
        /// </summary>
        public bool expectInputAutoActivation = true;

        /// <summary>
        /// Whether this service should automatically end the session
        /// on graph completion or not
        /// </summary>
        public bool endSessionOnCompletion = false;

        /// <summary>
        /// Whether this service should automatically clear the
        /// context map on graph completion or not
        /// </summary>
        public bool clearContextMapOnCompletion = false;

        /// <summary>
        /// The which deployed version to use (defaults to current when empty)
        /// </summary>
        [Tooltip("Which deployed version to use (defaults to current when empty)")]
        [VersionTagDropdown]
        [SerializeField]
        public string editorVersionTag;

        /// <summary>
        /// The which deployed version to use (defaults to current when empty)
        /// </summary>
        [Tooltip("Which deployed version to use in a build (defaults to current when empty)")]
        [FormerlySerializedAs("versionTag")]
        [VersionTagDropdown]
        [SerializeField]
        public string buildVersionTag;

        /// <summary>
        /// All event callbacks for Composer specific responses
        /// </summary>
        [Tooltip("Events that will fire before, during and after an activation")] [SerializeField]
        private ComposerEvents _events = new ComposerEvents();

        public ComposerEvents Events => _events;

        /// <summary>
        /// Implement in order to override responses with localized text
        /// </summary>
        public Func<string, string> GetLocalizedResponseText;

        /// <summary>
        /// Handles activation overide & response callback
        /// </summary>
        protected abstract IComposerRequestHandler GetRequestHandler();

        /// <summary>
        /// Handles response message read/playback
        /// </summary>
        [SerializeField] protected IComposerSpeechHandler[] _speechHandlers;

        /// <summary>
        /// Handles response message action calls
        /// </summary>
        [SerializeField] protected IComposerActionHandler _actionHandler;

        // Context map coroutine
        private Coroutine _mapCoroutine;
        private CurrentComposerRequest _activeRequest;

        #endregion

        #region LIFECYCLE

        // Initial setup
        protected virtual void Awake()
        {
            // If voice service is not found, grab from this or child game object
            if (_voiceService == null)
            {
                _voiceService = gameObject.GetComponentInChildren<VoiceService>();

                // Warn without voice service
                if (_voiceService == null)
                {
                    Log("No Voice Service found", true);
                }
            }

            // If speech handler is not found, grab from this or child game object
            if (_speechHandlers == null)
            {
                _speechHandlers = gameObject.GetComponentsInChildren<IComposerSpeechHandler>();
            }

            // If action handler is not found, grab from this or child game object
            if (_actionHandler == null)
            {
                _actionHandler = gameObject.GetComponentInChildren<IComposerActionHandler>();
            }
        }

        // Add delegates
        protected virtual void OnEnable()
        {
            if (_voiceService != null)
            {
                _voiceService.VoiceEvents.OnRequestInitialized.AddListener(OnVoiceServiceActivation);
            }
        }

        // Remove delegates
        protected virtual void OnDisable()
        {
            if (_voiceService != null)
            {
                _voiceService.VoiceEvents.OnRequestInitialized.RemoveListener(OnVoiceServiceActivation);
            }
        }

        // Handle breakdown
        protected virtual void OnDestroy()
        {

        }

        // Log while editing
        protected void Log(string comment, bool error = false)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(comment);
            sb.AppendLine($"Composer GO: {gameObject.name}");
            string scriptName = System.IO.Path.GetExtension(this.GetType().ToString()).Substring(1);
            sb.AppendLine($"Composer Script: {scriptName})");
            if (!string.IsNullOrEmpty(SessionID))
            {
                sb.AppendLine($"Session ID: {SessionID}");
            }

            if (CurrentContextMap != null && CurrentContextMap.Data != null &&
                CurrentContextMap.Data.ChildNodeNames.Length > 0)
            {
                sb.AppendLine("Context Map");
                foreach (var key in CurrentContextMap.Data.ChildNodeNames)
                {
                    sb.AppendLine($"\t{key}: {CurrentContextMap.GetData<string>(key, "-")}");
                }
            }

            // Log Error
            if (error)
            {
                VLog.W(sb.ToString());
            }
            // Log
            else
            {
                VLog.D(sb.ToString());
            }
        }

        #endregion

        #region SESSION

        /// <summary>
        /// Session start
        /// </summary>
        public void StartSession(string newSessionID = null)
        {
            // Get default session id
            if (string.IsNullOrEmpty(newSessionID))
            {
                newSessionID = GetDefaultSessionID();
            }

            // Apply session id
            SessionID = newSessionID;
            SessionStart = DateTime.UtcNow;
            Log("Start Composer Session");

            // Session start event
            Events.OnComposerSessionBegin?.Invoke(GetDefaultSessionData());
        }

        /// <summary>
        /// Get a default session id using a randomly generated + current timestamp
        /// </summary>
        /// <returns>session id</returns>
        public string GetDefaultSessionID()
        {
            string timestamp = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds.ToString("0");
            string guid = System.Guid.NewGuid().ToString();
            return $"{guid}-{timestamp}";
        }

        /// <summary>
        /// End the current session
        /// </summary>
        public void EndSession()
        {
            // Ignore if already over
            if (string.IsNullOrEmpty(SessionID))
            {
                return;
            }

            // Store for callback
            ComposerSessionData oldSessionData = GetDefaultSessionData();
            Log($"End Composer Session\nElapsed: {SessionElapsed.TotalSeconds:0.00}");

            // Remove
            SessionID = null;

            // Session end event
            Events.OnComposerSessionEnd?.Invoke(oldSessionData);
        }

        /// <summary>
        /// Get default session data
        /// </summary>
        /// <returns></returns>
        protected virtual ComposerSessionData GetDefaultSessionData()
        {
            ComposerSessionData sessionData = new ComposerSessionData();
            sessionData.sessionID = SessionID;
            sessionData.composer = this;
            sessionData.contextMap = CurrentContextMap;
            sessionData.responseData = null;
            #if UNITY_EDITOR
            sessionData.versionTag = editorVersionTag;
            #else
            sessionData.versionTag = buildVersionTag;
            #endif
            return sessionData;
        }

        #endregion

        #region CONTEXT MAP

        /// <summary>
        /// Erases the current map and entirely replaces it the given one.
        /// NB: This is potentially dangerous as there may be data we
        /// want to persist from the previous map, if there was one. 
        /// </summary>
        /// <param name="newContext">the new map object to use</param>
        private void SetContextMap(ComposerContextMap newContext)
        {
            // Ignore if same somehow
            if (CurrentContextMap == newContext)
            {
                return;
            }

            // Apply context data
            CurrentContextMap = newContext;
            Log("Context Map Set");

            // Map changed event
            Events.OnComposerContextMapChange?.Invoke(GetDefaultSessionData());
        }

        /// <summary>
        /// Updates the current context map with the data from the given map.
        /// </summary>
        /// <param name="newContext">the map with new data</param>
        public void UpdateContextMap(ComposerContextMap newContext)
        {
            // Ignore if same somehow
            if (CurrentContextMap == newContext)
            {
                return;
            }

            // Apply context data
            newContext.CopyPersistentData(CurrentContextMap);
            CurrentContextMap = newContext;
            Log("Context Map Set");

            // Map changed event
            Events.OnComposerContextMapChange?.Invoke(GetDefaultSessionData());
        }

        #endregion

        #region HELPERS

        // Activate message
        public void Activate(string message) => _voiceService?.Activate(message);

        // Activate speech via mic volume threshold
        public void Activate() => _voiceService?.Activate();

        // Activate speech via mic without waiting for volume threshold
        public void ActivateImmediately() => _voiceService?.ActivateImmediately();

        // Deactivate speech immediately
        public void Deactivate() => _voiceService?.Deactivate();

        // Deactivate speech and ignore cancel response from server
        public void DeactivateAndAbortRequest() => _voiceService?.DeactivateAndAbortRequest();

        // Only sends a context map
        public void SendContextMapEvent()
        {
            SendEvent(string.Empty);
        }

        // Send an event with a message
        public void SendEvent(string eventJson)
        {
            // Log if not in event json format
            if (!IsEventJson(eventJson))
            {
                Log("Sending event without properly formatted json assumes event is a message.", true);
            }

            // Perform activate
            _voiceService?.Activate(eventJson);
        }

        // Get required event parameters
        protected abstract string[] GetRequiredEventParams();

        // Whether or not in json event json format
        public bool IsEventJson(string json)
        {
            // Empty json is an event
            if (string.IsNullOrEmpty(json))
            {
                return true;
            }

            // Json if deserializes and has children
            WitResponseNode node = JsonConvert.DeserializeToken(json);
            if (node != null)
            {
                // Check if any required event parameters are missing
                bool missing = false;
                WitResponseClass nodeObj = node.AsObject;
                string[] requiredEventParams = GetRequiredEventParams();
                if (requiredEventParams != null)
                {
                    foreach (var eventParam in requiredEventParams)
                    {
                        if (!nodeObj.HasChild(eventParam))
                        {
                            missing = true;
                            break;
                        }
                    }
                }

                // Successful if none are missing
                if (!missing)
                {
                    return true;
                }
            }

            // Not event json
            return false;
        }

        // Get request index from list
        private int GetRequestIndex(VoiceServiceRequest findRequest)
        {
            if (_requests != null)
            {
                string findId = findRequest?.Options?.RequestId;
                return _requests.FindIndex((request) => string.Equals(findId, request.Request?.Options?.RequestId));
            }
            return -1;
        }

        // Get request index with session id
        private int GetSessionRequestIndex(string sessionId)
        {
            if (_requests != null)
            {
                return _requests.FindIndex((request) => string.Equals(sessionId, request.SessionId));
            }
            return -1;
        }

        /// <summary>
        /// Whether or not the provided session id is currently active
        /// </summary>
        /// <param name="sessionId">Unique session id</param>
        /// <returns>True if active due to a request being performed</returns>
        public bool IsSessionActive(string sessionId)
        {
            if (!gameObject.activeSelf)
            {
                return false;
            }
            int index = GetSessionRequestIndex(sessionId);
            return index != -1 && _requests[index].IsActive;
        }

    #endregion

        #region REQUEST
        // Request created, override with custom handling
        protected virtual void OnVoiceServiceActivation(VoiceServiceRequest request)
        {
            // If disabled, do not perform composer request
            if (!RouteVoiceServiceToComposer)
            {
                return;
            }

            // Ensure request is not being tracked
            int requestIndex = GetRequestIndex(request);
            if (requestIndex != -1)
            {
                return;
            }

            // Start session if empty
            if (string.IsNullOrEmpty(SessionID))
            {
                StartSession();
            }

            // Wait for session (Use utility to ensure it continues if disabled)
            CoroutineUtility.StartCoroutine(WaitForSession(SessionID, request));
        }

        // Hold up request until session is complete
        private IEnumerator WaitForSession(string sessionId, VoiceServiceRequest request)
        {
            // Hold while session is active
            request.OnHold = true;
            while (IsSessionActive(sessionId))
            {
                yield return null;
            }
            request.OnHold = false;

            // If not active, cancel request
            if (!gameObject.activeSelf)
            {
                request.Cancel("Composer disabled");
                yield break;
            }

            // Generate composer request & add to request list
            var sessionData = GetDefaultSessionData();
            var composerRequest = new CurrentComposerRequest(this, request, sessionData);
            _requests.Add(composerRequest);

            // Activation event
            Log($"Activation Begin\nId: {request?.Options?.RequestId}");
            Events.OnComposerActivation?.Invoke(sessionData);

            // Init complete
            OnVoiceRequestInit(sessionData, request);
        }

        // Handle sending of data
        protected virtual void OnVoiceRequestInit(ComposerSessionData sessionData, VoiceServiceRequest request)
        {
            // Request handler setup
            IComposerRequestHandler requestHandler = GetRequestHandler();
            if (requestHandler != null)
            {
                requestHandler.OnComposerRequestSetup(sessionData, request);
            }

            // Delegate
            Log($"Request Init\nId: {request?.Options?.RequestId}");
            Events.OnComposerRequestInit?.Invoke(sessionData);
        }

        // Handle sending of data
        protected virtual void OnVoiceRequestSend(ComposerSessionData sessionData, VoiceServiceRequest request)
        {
            Log($"Request Send\nId: {request?.Options?.RequestId}");
            Events.OnComposerRequestBegin?.Invoke(sessionData);
        }

        // Handle Partial Resposne
        protected virtual void OnPartialResponse(ComposerSessionData sessionData, WitResponseNode response)
        {
            sessionData.responseData.witResponse = response;
            var partial = response.GetTTS();
            // TODO: Potentially add new pre speech actions here and trigger them even if no text speech is present.
            // Reasoning: Any actions that happen from server via response will be tied to when they are "spoken"
            // If the NPC is still speaking from a previous response, we will want to let it finish before triggering
            // any actions here. Exception being interruptive.

            if (!string.IsNullOrEmpty(partial))
            {
                for (int i = 0; null != _speechHandlers && i < _speechHandlers.Length; i++)
                {
                    var speechHandler = _speechHandlers[i];
                    speechHandler.SpeakPartial(sessionData, partial);
                }
            }
        }

        // Handle completion
        protected virtual void OnVoiceRequestComplete(ComposerSessionData sessionData, VoiceServiceRequest request)
        {
            // Ignore if already off
            int requestIndex = GetRequestIndex(request);
            if (requestIndex == -1)
            {
                Log($"Request Complete Missing\nId: {request?.Options?.RequestId}", true);
                return;
            }

            // Cancelled
            if (request.State == VoiceRequestState.Canceled)
            {
                OnComposerCanceled(sessionData, request.Results.Message);
            }
            // Failed
            else if (request.State == VoiceRequestState.Failed)
            {
                OnComposerError(sessionData, request.Results.Message);
            }
            // Successful
            else if (request.State == VoiceRequestState.Successful)
            {
                OnComposerResponse(sessionData, request.ResponseData);
            }

            // Voice service/Composer is no longer active
            Log($"Request Complete\nId: {request?.Options?.RequestId}");
            _requests.RemoveAt(requestIndex);
        }
        #endregion

        #region RESPONSE
        // Composer request setup
        protected virtual void OnComposerCanceled(ComposerSessionData sessionData, string reason)
        {
            // Error response
            sessionData.responseData = new ComposerResponseData(reason);

            // Error callback
            Log($"Request Canceled\nReason: {sessionData.responseData.error}", true);
            Events.OnComposerCanceled?.Invoke(sessionData);
        }

        // Handle composer error
        protected virtual void OnComposerError(ComposerSessionData sessionData, string error)
        {
            // Error response
            sessionData.responseData = new ComposerResponseData(error);

            // Error callback
            Log($"Request Error\nError: {sessionData.responseData.error}", true);
            Events.OnComposerError?.Invoke(sessionData);
        }

        // Composer response returned via json
        protected virtual void OnComposerResponse(ComposerSessionData sessionData, WitResponseNode response)
        {
            // Get parse errors
            StringBuilder error = new StringBuilder();
            // Parse new context map
            sessionData.contextMap = new ComposerContextMap(response, error);
            // Parse response data
            sessionData.responseData = new ComposerResponseData(response, error);

            // Composer error
            if (!string.IsNullOrEmpty(error.ToString()))
            {
                OnComposerError(sessionData, error.ToString());
                return;
            }

            // Localize response phrase
            if (GetLocalizedResponseText != null && !string.IsNullOrEmpty(sessionData.responseData.responsePhrase))
            {
                sessionData.responseData.responsePhrase = GetLocalizedResponseText(sessionData.responseData.responsePhrase);
            }

            // Apply new context map
            UpdateContextMap(sessionData.contextMap);

            // Response event
            Log("Request Success");
            Events.OnComposerResponse?.Invoke(sessionData);

            // Check if composer should continue
            bool needsContinue = false;

            // Read phrase if possible
            if (handleTts && !string.IsNullOrEmpty(sessionData.responseData.responsePhrase))
            {
                needsContinue = true;
                OnComposerSpeakPhrase(sessionData);
            }
            // Perform action if possible
            if (!string.IsNullOrEmpty(sessionData.responseData.actionID))
            {
                needsContinue = true;

                OnComposerPerformAction(sessionData);
            }
            // Expect input once complete
            if (sessionData.responseData.expectsInput)
            {
                needsContinue = true;
            }

            // Wait to continue the composer
            if (needsContinue)
            {
                CoroutineUtility.StartCoroutine(WaitToContinue(sessionData));
            }
        }

        // Speak phrase callback & handle with speech handler
        protected virtual void OnComposerSpeakPhrase(ComposerSessionData sessionData)
        {
            // Speak phrase callback
            string final = sessionData.responseData.responsePhrase;
            Events.OnComposerSpeakPhrase?.Invoke(sessionData);

            // Handle phrase if possible
            for (int i = 0; null != _speechHandlers && i < _speechHandlers.Length; i++)
            {
                var speechHandler = _speechHandlers[i];
                speechHandler.SpeakPhrase(sessionData, final);
            }
        }

        // Perform action
        protected virtual void OnComposerPerformAction(ComposerSessionData sessionData)
        {
            // Perform action callback
            Log($"Perform Action\nAction: {sessionData.responseData.actionID}");

            Events.OnComposerPerformAction?.Invoke(sessionData);

            // Handle action if possible
            if (_actionHandler != null)
            {
                _actionHandler.PerformAction(sessionData);
            }
        }

        // Perform expect input
        protected virtual void OnComposerExpectsInput(ComposerSessionData sessionData)
        {
            // Perform action callback
            Log($"Expects Input");
            Events.OnComposerExpectsInput?.Invoke(sessionData);

            // Activate voice service
            if (expectInputAutoActivation && _voiceService != null)
            {
                _voiceService.Activate();
            }
        }

        // Composer graph completed
        protected virtual void OnComposerComplete(ComposerSessionData sessionData)
        {
            Log($"Graph Complete");
            Events.OnComposerComplete?.Invoke(sessionData);

            // End session on completion
            if (endSessionOnCompletion)
            {
                EndSession();
            }
            // Clear context map on completion
            if (clearContextMapOnCompletion)
            {
                CurrentContextMap.ClearAllNonReservedData();
            }
        }
        #endregion

        #region AUTO ACTIVATION
        // Perform coroutine to wait for completion & then auto activate
        private IEnumerator WaitToContinue(ComposerSessionData sessionData)
        {
            // Wait for everything to continue
            Log($"Wait to Continue - Begin");
            yield return null; // Needs an initial wait to ensure data was returned
            yield return new WaitUntil(() => IsContinueAllowed(sessionData));
            yield return new WaitForSeconds(continueDelay);
            Log($"Wait to Continue - Complete");

            // Call expects input
            if (sessionData.responseData.expectsInput)
            {
                OnComposerExpectsInput(sessionData);
            }
            // Nowhere to go, complete session
            else
            {
                OnComposerComplete(sessionData);
            }
        }

        // Whether continue should be allowed
        protected virtual bool IsContinueAllowed(ComposerSessionData sessionData)
        {
            // Wait for service to stop being active
            if (_voiceService.IsRequestActive)
            {
                return false;
            }

            for (int i = 0; null != _speechHandlers && i < _speechHandlers.Length; i++)
            {
                var speechHandler = _speechHandlers[i];
                // Wait for speech handler completion if applicable
                if (speechHandler.IsSpeaking(sessionData))
                {
                    return false;
                }
            }

            // Wait for action handler completion if applicable
            if (_actionHandler != null && _actionHandler.IsPerformingAction(sessionData))
            {
                return false;
            }
            // Input allowed
            return true;
        }
        #endregion

        /// <summary>
        /// Handles subscribing/unsubscribing to events for an active composer session request.
        /// </summary>
        private class CurrentComposerRequest
        {

            private ComposerService _service;
            public VoiceServiceRequest Request { get; private set; }
            private readonly ComposerSessionData _sessionData;
            public string SessionId => _sessionData?.sessionID;
            public bool IsActive => Request == null ? false : Request.IsActive;

            public CurrentComposerRequest(ComposerService service, VoiceServiceRequest request, ComposerSessionData sessionData)
            {
                _service = service;
                _sessionData = sessionData;
                Request = request;
                request.Events.OnSend.AddListener(OnSend);
                request.Events.OnPartialResponse.AddListener(OnPartial);
                request.Events.OnComplete.AddListener(OnComplete);
                request.Events.OnComplete.AddListener(OnCleanup);

            }

            private void OnSend(VoiceServiceRequest r)
            {
                UpdateResponseData(r.ResponseData);
                _service.OnVoiceRequestSend(_sessionData, r);
            }

            private void OnPartial(WitResponseNode r)
            {
                UpdateResponseData(r);
                _service.OnPartialResponse(_sessionData, r);
            }

            private void OnComplete(VoiceServiceRequest r)
            {
                UpdateResponseData(r.ResponseData);
                _service.OnVoiceRequestComplete(_sessionData, r);
            }

            private void OnCleanup(VoiceServiceRequest r)
            {
                Request.Events.OnSend.RemoveListener(OnSend);
                Request.Events.OnPartialResponse.RemoveListener(OnPartial);
                Request.Events.OnComplete.RemoveListener(OnComplete);
                Request.Events.OnComplete.RemoveListener(OnCleanup);
            }

            private void UpdateResponseData(WitResponseNode r)
            {
                if (null == _sessionData.responseData)
                {
                    _sessionData.responseData = new ComposerResponseData(r, new StringBuilder());
                }
                if (_sessionData.responseData.witResponse == null)
                {
                    _sessionData.responseData.witResponse = r;
                }
            }
        }
    }
}
