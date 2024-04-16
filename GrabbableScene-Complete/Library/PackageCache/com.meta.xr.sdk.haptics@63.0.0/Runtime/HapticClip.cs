/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

// @lint-ignore-every LICENSELINT

using UnityEngine;

namespace Oculus.Haptics
{
    /// <summary>
    /// Represents an imported haptic clip asset.
    /// </summary>
    ///
    /// <c>HapticClip</c> contains the data of a haptic clip asset imported from a <c>.haptic</c> file,
    /// in a format suitable for playing it back at runtime.
    /// A <c>HapticClip</c> is created by <c>HapticImporter</c> when importing a haptic clip asset
    /// in the Unity editor.
    public class HapticClip : ScriptableObject
    {
        /// <summary>
        /// The JSON representation of the haptic clip, stored as string encoded in UTF-8.
        /// </summary>
        [SerializeField]
        public string json;
    }
}
