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

using System;
using UnityEngine;

namespace Oculus.Interaction
{
    public enum PointerEventType
    {
        Hover,
        Unhover,
        Select,
        Unselect,
        Move,
        Cancel
    }

    public struct PointerEvent: IEvent
    {
        public int Identifier { get; }
        public PointerEventType Type { get; }
        public Pose Pose { get; }
        public object Data { get; }

        public PointerEvent(int identifier, PointerEventType type, Pose pose, object data = null)
        {
            Identifier = identifier;
            Type = type;
            Pose = pose;
            Data = data;
        }
    }

    /// <summary>
    /// Broadcasts a set of <cref="PointerEvent" />s every time it changes state.
    /// </summary>
    public interface IPointable
    {
        event Action<PointerEvent> WhenPointerEventRaised;
    }

    public interface IPointableElement : IPointable
    {
        void ProcessPointerEvent(PointerEvent evt);
    }
}
