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
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Meta.XR.MRUtilityKit
{
    public static class Data
    {

        [Serializable]
        public struct TransformData : IEqualityComparer<TransformData>, ICloneable
        {
            [JsonProperty("Translation")] public Vector3 Translation;
            [JsonProperty("Rotation")] public Vector3 Rotation;
            [JsonProperty("Scale")] public Vector3 Scale;

            public override string ToString()
            {
                return $"Translation: {Translation}, Rotation: {Rotation}, Scale: {Scale}";
            }

            public object Clone()
            {
                return new TransformData()
                {
                    Translation = Translation,
                    Rotation = Rotation,
                    Scale = Scale
                };
            }

            public bool Equals(TransformData x, TransformData y)
            {
                return x.Translation.Equals(y.Translation)
                       && x.Rotation.Equals(y.Rotation)
                       && x.Scale.Equals(y.Scale);
            }

            public int GetHashCode(TransformData obj)
            {
                return HashCode.Combine(obj.Translation, obj.Rotation, obj.Scale);
            }
        }


        [Serializable]
        public struct PlaneBoundsData : IEqualityComparer<PlaneBoundsData>, ICloneable
        {
            [JsonProperty("Min")] public Vector2 Min;
            [JsonProperty("Max")] public Vector2 Max;

            public override string ToString()
            {
                return $"Min: {Min}, Max: {Max}";
            }

            public object Clone()
            {
                return new PlaneBoundsData()
                {
                    Min = Min,
                    Max = Max
                };
            }

            public bool Equals(PlaneBoundsData x, PlaneBoundsData y)
            {
                return x.Min.Equals(y.Min)
                       && x.Max.Equals(y.Max);
            }

            public int GetHashCode(PlaneBoundsData obj)
            {
                return HashCode.Combine(obj.Min, obj.Max);
            }
        }


        [Serializable]
        public struct VolumeBoundsData : IEqualityComparer<VolumeBoundsData>,ICloneable
        {
            [JsonProperty("Min")] public Vector3 Min;
            [JsonProperty("Max")] public Vector3 Max;

            public override string ToString()
            {
                return $"Min: {Min}, Max: {Max}";
            }

            public object Clone()
            {
                return new VolumeBoundsData()
                {
                    Min = Min,
                    Max = Max
                };
            }

            public bool Equals(VolumeBoundsData x, VolumeBoundsData y)
            {
                return x.Min.Equals(y.Min)
                       && x.Max.Equals(y.Max);
            }

            public int GetHashCode(VolumeBoundsData obj)
            {
                return HashCode.Combine(obj.Min, obj.Max);
            }
        }

        [Serializable]
        public struct AnchorData : IEqualityComparer<AnchorData>, ICloneable
        {
            // Anchor here is not serialized/deserialized to/from JSON, it is used when loading from device
            // we need to store this data here temporarily in order to pass it/compare to the MRUKAnchor class
            [JsonIgnore] public OVRAnchor Anchor;
            [JsonProperty("UUID")] public string UUID;
            [JsonProperty("SemanticClassifications")] public List<String> SemanticClassifications;
            [JsonProperty("Transform")] public TransformData Transform;
            [JsonProperty("PlaneBounds")] public PlaneBoundsData? PlaneBounds;
            [JsonProperty("VolumeBounds")] public VolumeBoundsData? VolumeBounds;
            [JsonProperty("PlaneBoundary2D")] public List<Vector2> PlaneBoundary2D;
            [JsonProperty("GlobalMesh")] public GlobalMeshData? GlobalMesh;

            public override string ToString()
            {
                return $"UUID: {UUID}, " +
                       $"Transform: {Transform}, " +
                       $"PlaneBounds: {(PlaneBounds != null ? PlaneBounds : "null")}, " +
                       $"VolumeBounds: {(VolumeBounds != null? VolumeBounds : "null")}, " +
                       $"GlobalMesh: {(GlobalMesh != null ? GlobalMesh : "null")}, " +
                       $"PlaneBoundary2D: [{string.Join(", ", PlaneBoundary2D ?? new List<Vector2>())}], " +
                       $"SemanticClassifications: [{string.Join(", ", SemanticClassifications ?? new List<string>())}]";
            }

            public object Clone()
            {
                return new AnchorData()
                {
                    UUID = UUID,
                    Transform = Transform,
                    PlaneBounds = PlaneBounds,
                    VolumeBounds = VolumeBounds,
                    GlobalMesh = GlobalMesh,
                    PlaneBoundary2D = PlaneBoundary2D,
                    SemanticClassifications = SemanticClassifications
                };
            }

            public override bool Equals(object obj)
            {
                return obj is AnchorData other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(UUID, SemanticClassifications, Transform, PlaneBounds, VolumeBounds, PlaneBoundary2D, GlobalMesh);
            }

            public bool Equals(AnchorData x, AnchorData y)
            {
                return
                 x.UUID == y.UUID
                    && x.SemanticClassifications.SequenceEqual(y.SemanticClassifications)
                    && x.Transform.Equals(y.Transform)
                    && Nullable.Equals(x.PlaneBounds, y.PlaneBounds)
                    && Nullable.Equals(x.VolumeBounds, y.VolumeBounds)
                    && (x.PlaneBoundary2D ?? new List<Vector2>()).SequenceEqual(y.PlaneBoundary2D ?? new List<Vector2>())
                    && Nullable.Equals(x.GlobalMesh, y.GlobalMesh);
            }

            public bool Equals(AnchorData x)
            {
                return Equals(this, x);
            }
            public int GetHashCode(AnchorData obj)
            {
                return HashCode.Combine(obj.UUID, obj.SemanticClassifications, obj.Transform, obj.PlaneBounds, obj.VolumeBounds, obj.PlaneBoundary2D, obj.GlobalMesh);
            }

            public static bool operator ==(AnchorData left, AnchorData right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(AnchorData left, AnchorData right)
            {
                return !(left == right);
            }
        }

        [Serializable]
        public struct GlobalMeshData : ICloneable
        {
            [JsonProperty("Positions")] public Vector3[] Positions;
            [JsonProperty("Indices")] public int[] Indices;

            public override string ToString()
            {
                return $"Positions: [{string.Join(", ", Positions)}], Indices: [{string.Join(", ", Indices)}]";
            }

            public object Clone()
            {
                return new GlobalMeshData()
                {
                    Positions = Positions,
                    Indices = Indices
                };
            }
        }

        [Serializable]
        public struct RoomLayoutData : IEqualityComparer<RoomLayoutData> , ICloneable
        {
            [JsonProperty("FloorUuid")] public string FloorUuid;
            [JsonProperty("CeilingUuid")] public string CeilingUuid;
            [JsonProperty("GlobalMeshUuid")] public string GlobalMeshUuid;

            public List<string> WallsUUid
            {
                get { return _wallsUUid ??= new(); }
                set => _wallsUUid = value;
            }
            private List<string> _wallsUUid;

            public override string ToString()
            {
                return $"FloorUuid: {FloorUuid}, " +
                       $"CeilingUuid: {CeilingUuid}, " +
                       $"GlobalMeshUuid: {GlobalMeshUuid}, " +
                       $"WallsUUid: [{string.Join(", ", WallsUUid)}]";
            }

            public object Clone()
            {
                return new RoomLayoutData()
                {
                    FloorUuid = FloorUuid,
                    CeilingUuid = CeilingUuid,
                    GlobalMeshUuid = GlobalMeshUuid,
                    WallsUUid = WallsUUid
                };
            }

            public override bool Equals(object obj)
            {
                return obj is RoomLayoutData other && Equals(other);
            }

            public bool Equals(RoomLayoutData other)
            {
                return FloorUuid == other.FloorUuid
                       && CeilingUuid == other.CeilingUuid
                       && GlobalMeshUuid == other.GlobalMeshUuid
                       && WallsUUid.SequenceEqual(other.WallsUUid);
            }
            public bool Equals(RoomLayoutData left, RoomLayoutData right)
            {
                return left.FloorUuid == right.FloorUuid
                       && left.CeilingUuid == right.CeilingUuid
                       && left.GlobalMeshUuid == right.GlobalMeshUuid
                       && left.WallsUUid.SequenceEqual(right.WallsUUid);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(FloorUuid, CeilingUuid, GlobalMeshUuid, WallsUUid);
            }

            public int GetHashCode(RoomLayoutData obj)
            {
                return HashCode.Combine(obj.FloorUuid, obj.CeilingUuid, obj.GlobalMeshUuid, obj.WallsUUid);
            }
        }

        [Serializable]
        public struct RoomData : IEqualityComparer<RoomData>, ICloneable
        {
            // Anchor here is not serialized/deserialized to/from JSON, it is used when loading from device
            // we need to store this data here temporarily in order to pass it/compare to the MRUKRoom class
            [JsonIgnore] public OVRAnchor Anchor;
            [JsonProperty("UUID")] public string UUID;
            [JsonProperty("RoomLayout")] public RoomLayoutData RoomLayout;
            [JsonProperty("Anchors")] public List<AnchorData> Anchors;

            public override string ToString()
            {
                return $"UUID: {UUID}, " +
                       $"RoomLayout: {RoomLayout}, " +
                       $"Anchors: [{Utilities.GetAnchorDatasString(Anchors)}]";
            }

            public object Clone()
            {
                return new RoomData()
                {
                    UUID = UUID,
                    RoomLayout = RoomLayout,
                    Anchors = Anchors
                };
            }

            public override bool Equals(object obj)
            {
                return obj is RoomData other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(UUID, RoomLayout,Anchors);
            }

            public bool Equals(RoomData x, RoomData y)
            {
                return x.UUID == y.UUID && x.RoomLayout.Equals(y.RoomLayout) && x.Anchors.SequenceEqual(y.Anchors);
            }

            public bool Equals(RoomData x)
            {
                return Equals(this, x);
            }

            public int GetHashCode(RoomData obj)
            {
                return HashCode.Combine(obj.UUID, obj.RoomLayout, obj.Anchors);
            }

            public static bool operator ==(RoomData left, RoomData right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(RoomData left, RoomData right)
            {
                return !(left == right);
            }
        }

        [Serializable]
        public struct SceneData : IEqualityComparer<SceneData> , ICloneable
        {

            [JsonProperty("CoordinateSystem")] public SerializationHelpers.CoordinateSystem CoordinateSystem;
            [JsonProperty("Rooms")] public List<RoomData> Rooms;

            private bool Equals(SceneData other)
            {
                return CoordinateSystem == other.CoordinateSystem && Equals(Rooms, other.Rooms);
            }

            public override bool Equals(object obj)
            {
                return obj is SceneData other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine((int)CoordinateSystem, Rooms);
            }

            public object Clone()
            {
                return new SceneData()
                {
                    CoordinateSystem = CoordinateSystem,
                    Rooms = Rooms
                };
            }

            public static bool operator ==(SceneData left, SceneData right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(SceneData left, SceneData right)
            {
                return !(left == right);
            }

            public bool Equals(SceneData x, SceneData y)
            {
                return x.CoordinateSystem == y.CoordinateSystem
                       && x.Rooms.SequenceEqual(y.Rooms);
            }

            public int GetHashCode(SceneData obj)
            {
                return HashCode.Combine((int)obj.CoordinateSystem, obj.Rooms);
            }
        }

    }
}
