/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * This source code is licensed under the license found in the
 * LICENSE file in the root directory of this source tree.
 */

using UnityEngine;
using UnityEngine.UI;

namespace Meta.WitAi.Composer.Samples
{
    public class SimpleActivationButton : MonoBehaviour
    {
        // Voice service
        [SerializeField] private VoiceService _voiceService;
        [SerializeField] private bool _abortWithDeactivation = false;
        // Activation button label
        [SerializeField] private Button _button;
        [SerializeField] private Text _label;
        [SerializeField] private string _activateText = "Activate";
        [SerializeField] private string _deactivateText = "Deactivate";

        // Whether currently activated
        private bool _isActivated = false;

        // Grab label
        private void Awake()
        {
            if (_button == null)
            {
                _button = gameObject.GetComponent<Button>();
            }
            if (_label == null)
            {
                _label = gameObject.GetComponentInChildren<Text>();
            }
            if (_voiceService == null)
            {
                VLog.W($"{GetType()} ({gameObject.name}) - Needs a Voice Service reference");
            }
        }
        // Reset
        private void OnEnable()
        {
            // Set prompt
            RefreshText();

            // Add delegates
            if (_voiceService != null)
            {
                _voiceService.VoiceEvents.OnStartListening.AddListener(OnRequestBegin);
                _voiceService.VoiceEvents.OnRequestCompleted.AddListener(OnRequestComplete);
                _voiceService.VoiceEvents.OnStoppedListeningDueToDeactivation.AddListener(OnRequestComplete);
                _voiceService.VoiceEvents.OnStoppedListeningDueToInactivity.AddListener(OnRequestComplete);
                _voiceService.VoiceEvents.OnStoppedListeningDueToTimeout.AddListener(OnRequestComplete);
            }
            if (_button != null)
            {
                _button.onClick.AddListener(ToggleActivation);
            }
        }
        // Remove delegates
        private void OnDisable()
        {
            if (_voiceService != null)
            {
                _voiceService.VoiceEvents.OnStartListening.RemoveListener(OnRequestBegin);
                _voiceService.VoiceEvents.OnRequestCompleted.RemoveListener(OnRequestComplete);
                _voiceService.VoiceEvents.OnStoppedListeningDueToDeactivation.RemoveListener(OnRequestComplete);
                _voiceService.VoiceEvents.OnStoppedListeningDueToInactivity.RemoveListener(OnRequestComplete);
                _voiceService.VoiceEvents.OnStoppedListeningDueToTimeout.RemoveListener(OnRequestComplete);
            }
            if (_button != null)
            {
                _button.onClick.AddListener(ToggleActivation);
            }
        }

        // Request begin
        private void OnRequestBegin()
        {
            _isActivated = true;
            RefreshText();
        }
        // Request over
        private void OnRequestComplete()
        {
            _isActivated = false;
            RefreshText();
        }
        // Set text
        private void RefreshText()
        {
            if (_label != null)
            {
                _label.text = _isActivated ? _deactivateText : _activateText;
            }
        }
        // Toggle activation
        private void ToggleActivation()
        {
            if (_isActivated)
            {
                if (_abortWithDeactivation)
                {
                    _voiceService.DeactivateAndAbortRequest();
                }
                else
                {
                    _voiceService.Deactivate();
                }
            }
            else
            {
                _voiceService.Activate();
            }
        }
    }
}
