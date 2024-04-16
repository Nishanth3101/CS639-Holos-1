/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * This source code is licensed under the license found in the
 * LICENSE file in the root directory of this source tree.
 */

using UnityEngine;
using Meta.WitAi.Json;
using Meta.WitAi.Data.Entities;

namespace Meta.WitAi.Composer.Samples
{
    public class SimpleColorChanger : MonoBehaviour
    {
        [SerializeField] private Renderer _renderer;
        [SerializeField] private string entityID = "color";


        /// <summary>
        /// Directly processes a command result getting the slots with WitResult utilities
        /// </summary>
        /// <param name="commandResult">Result data from Wit.ai activation to be processed</param>
        public void SetColorActionNonComposer(WitResponseNode commandResult)
        {
            WitEntityData colorEntity = commandResult.GetFirstWitEntity("color:color");
            if (colorEntity == null)
            {
                return;
            }
            var color = GetColor(colorEntity);
            SetColor(color);
        }

        /// <summary>
        /// Set color action
        /// </summary>
        /// <param name="sessionData">Composer data, persisted throughout the session.</param>
        public void SetColorAction(ComposerSessionData sessionData)
        {
            // Check context map
            if (sessionData.contextMap == null || sessionData.contextMap.Data == null)
            {
                VLog.E("Set Color Action Failed - No Context Map");
                return;
            }
            if (!sessionData.contextMap.Data.HasChild(entityID))
            {
                VLog.E($"Set Color Action Failed - Context map does not contain {entityID}");
                return;
            }

            // Get color name from context map
            WitResponseArray colorArray = sessionData.contextMap.Data[entityID].AsArray;
            WitEntityData colorEntity = colorArray?.Count > 0 ? colorArray[0].AsWitEntity() : null;

            var color = GetColor(colorEntity);
            SetColor(color);
        }

        /// <summary>
        /// Retrieves the first color in the provided entity
        /// </summary>
        /// <param name="colorEntity">entity data, regardless of original parsing source</param>
        /// <returns>The color in the data, or the original material color if none found</returns>
        private Color GetColor(WitEntityData colorEntity)
        {
            var defaultColor = _renderer.material.color;
            string colorName = colorEntity?.value;
            if (string.IsNullOrEmpty(colorName))
            {
                VLog.E($"Set Color Action Failed - No '{entityID}' value found");
                return defaultColor;
            }

            // Decode name into color
            if (!ColorUtility.TryParseHtmlString(colorName, out var color))
            {
                VLog.E($"Set Color Action Failed - Could not parse color\nName: {colorName}");
                return defaultColor;
            }

            return color;
        }

        /// <summary>
        /// Set the specified color
        /// </summary>
        public void SetColor(Color newColor)
        {
            _renderer.material.color = newColor;
        }
    }
}
