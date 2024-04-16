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
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;

namespace Meta.XR.MRUtilityKit
{

    /// <summary>
    /// This class contains convenience functions that allow you to
    /// query your scene.
    ///
    /// Use together with <seealso cref="MRUKLoader"/> to
    /// load data via link, fake data, or on-device data.
    /// </summary>
    public class MRUK : MonoBehaviour
    {
        // when interacting specifically with tops of volumes, this can be used to
        // specify where the return position should be aligned on the surface
        // e.g. some apps might want a position right in the center of the table (chess)
        // for others, the edge may be more important (piano or pong)
        public enum PositioningMethod
        {
            DEFAULT,
            CENTER,
            EDGE
        }

        /// <summary>
        /// Specify the source of the scene data.
        /// </summary>
        public enum SceneDataSource
        {
            /// <summary>
            /// Load scene data from the device.
            /// </summary>
            Device,
            /// <summary>
            /// Load scene data from prefabs.
            /// </summary>
            Prefab,
            /// <summary>
            /// First try to load data from the device and if none can be found
            /// fall back to loading from a prefab.
            /// </summary>
            DeviceWithPrefabFallback,
            /// <summary>
            /// Load Scene from Json String
            /// </summary>
            Json,
        }



        [Flags]
        public enum SurfaceType
        {
            FACING_UP = 1 << 0,
            FACING_DOWN = 1 << 1,
            VERTICAL = 1 << 2,
        };

        [Flags]
        private enum AnchorRepresentation
        {
            PLANE = 1 << 0,
            VOLUME = 1 << 1,
        }

        public bool IsInitialized { get; private set; } = false;

        /// <summary>
        /// Event that is triggered when the scene is loaded.
        /// </summary>
        public UnityEvent SceneLoadedEvent = new();

        /// <summary>
        /// Event that is triggered when a room is created.
        /// </summary>
        public UnityEvent<MRUKRoom> RoomCreatedEvent = new();

        /// <summary>
        /// Event that is triggered when a room is updated.
        /// </summary>
        public UnityEvent<MRUKRoom> RoomUpdatedEvent = new();

        /// <summary>
        /// Event that is triggered when a room is removed.
        /// </summary>
        public UnityEvent<MRUKRoom> RoomRemovedEvent = new();
        /// <summary>
        /// When world locking is enabled the position of the camera rig will be adjusted each frame to ensure
        /// the room anchors are where they should be relative to the camera position.This is necessary to
        /// ensure the position of the virtual objects in the world do not get out of sync with the real world.
        /// </summary>
        public bool EnableWorldLock
        {
            get { return _enableWorldLock; }
            set
            {
                if (!value && _enableWorldLock)
                {
                    _cameraRig.trackingSpace.localPosition = Vector3.zero;
                    _cameraRig.trackingSpace.localRotation = Quaternion.identity;
                }
                _enableWorldLock = value;
            }
        }

        private OVRCameraRig _cameraRig;
        private bool _enableWorldLock = true;
        private bool _loadSceneCalled = false;

        /// <summary>
        /// This is the final event that tells developer code that Scene API and MR Utility Kit have been initialized, and that the room can be queried.
        /// </summary>
        void InitializeScene()
        {
            try
            {
                SceneLoadedEvent.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            IsInitialized = true;
        }

        /// <summary>
        /// Register to receive a callback when the scene is loaded. If the scene is already loaded
        /// at the time this is called, the callback will be invoked immediatly.
        /// </summary>
        /// <param name="callback"></param>
        public void RegisterSceneLoadedCallback(UnityAction callback)
        {
            SceneLoadedEvent.AddListener(callback);
            if (IsInitialized)
            {
                callback();
            }
        }

        /// <summary>
        /// Register to receive a callback when a new room has been created from scene capture.
        /// </summary>
        /// <param name="callback">
        /// - `MRUKRoom` The created room object.
        /// </param>
        public void RegisterRoomCreatedCallback(UnityAction<MRUKRoom> callback)
        {
            RoomCreatedEvent.AddListener(callback);
        }

        /// <summary>
        /// Register to receive a callback when a room has been updated from scene capture.
        /// </summary>
        /// <param name="callback">
        /// - `MRUKRoom` The updated room object.
        /// </param>
        public void RegisterRoomUpdatedCallback(UnityAction<MRUKRoom> callback)
        {
            RoomUpdatedEvent.AddListener(callback);
        }
        /// <summary>
        /// Registers a callback function to be called before the room is removed.
        /// </summary>
        /// <param name="callback">The function to be called when the room is removed. It takes one parameter:
        /// - `MRUKRoom` The removed room object.
        ///</param>
        public void RegisterRoomRemovedCallback(UnityAction<MRUKRoom> callback)
        {
            RoomRemovedEvent.AddListener(callback);
        }

        /// <summary>
        /// Get a list of all the rooms in the scene.
        /// </summary>
        public List<MRUKRoom> GetRooms() => _sceneRooms;

        /// <summary>
        /// Get a flat list of all Anchors in the scene
        /// </summary>
        public List<MRUKAnchor> GetAnchors() => GetCurrentRoom().GetRoomAnchors();


        /// <summary>
        /// Returns the current room the headset is in. If the headset is not in any given room
        /// then it will return the room the headset was last in when this function was called.
        /// If the headset hasn't been in a valid room yet then return the first room in the list.
        /// If no rooms have been loaded yet then return null.
        /// </summary>
        public MRUKRoom GetCurrentRoom()
        {
            // This is a rather expensive operation, we should only do it at most once per frame.
            if (_cachedCurrentRoomFrame != Time.frameCount)
            {
                if (_cameraRig?.centerEyeAnchor.position is Vector3 eyePos)
                {
                    foreach (var room in _sceneRooms)
                    {
                        if (room.IsPositionInRoom(eyePos, false))
                        {
                            _cachedCurrentRoom = room;
                            _cachedCurrentRoomFrame = Time.frameCount;
                            return room;
                        }
                    }
                }
            }

            if (_cachedCurrentRoom != null)
            {
                return _cachedCurrentRoom;
            }

            if (_sceneRooms.Count > 0)
            {
                return _sceneRooms[0];
            }

            return null;
        }

        /// <summary>
        /// Checks whether any anchors can be loaded.
        /// </summary>
        /// <returns>Returns a task-based bool, which is true if
        /// there are any scene anchors in the system, and false
        /// otherwise. If false is returned, then either
        /// the scene permission needs to be set, or the user
        /// has to run Scene Capture.</returns>
        public static async Task<bool> HasSceneModel()
        {
            var rooms = new List<OVRAnchor>();
            if (!await OVRAnchor.FetchAnchorsAsync<OVRRoomLayout>(rooms))
                return false;
            return rooms.Count > 0;
        }

        [Serializable]
        public class MRUKSettings
        {
            [SerializeField, Tooltip("Where to load the data from.")]
            public SceneDataSource DataSource = SceneDataSource.Device;
            [SerializeField, Tooltip("Which room to use; -1 is random.")]
            public int RoomIndex = -1;
            [SerializeField, Tooltip("The list of prefab rooms to use.")]
            public GameObject[] RoomPrefabs;
            [SerializeField, Tooltip("Trigger a scene load on startup.")]
            public bool LoadSceneOnStartup = true;
            [SerializeField, Tooltip("The width of a seat. Use to calculate seat positions")]
            public float SeatWidth = 0.6f;
            [SerializeField, Tooltip("The Scene Json to use")]
            public string SceneJson;
        }

        [Tooltip("Contains all the information regarding data loading.")]
        public MRUKSettings SceneSettings;

        MRUKRoom _cachedCurrentRoom = null;
        int _cachedCurrentRoomFrame = 0;
        List<MRUKRoom> _sceneRooms = new();

        public static MRUK Instance { get; private set; }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.Assert(false, "There should be only one instance of MRUK!");
                Destroy(this);
            }
            else
            {
                Instance = this;
            }

            if (SceneSettings == null) return;

            if (SceneSettings.LoadSceneOnStartup)
            {
                // We can't await for the LoadScene result because Awake is not async, silence the warning
#pragma warning disable CS4014
#if !UNITY_EDITOR && UNITY_ANDROID
                // If we are going to load from device we need to ensure we have permissions first
                if ((SceneSettings.DataSource == SceneDataSource.Device || SceneSettings.DataSource == SceneDataSource.DeviceWithPrefabFallback) &&
                    !Permission.HasUserAuthorizedPermission(OVRPermissionsRequester.ScenePermission))
                {
                    var callbacks = new PermissionCallbacks();
                    callbacks.PermissionDenied += permissionId =>
                    {
                        Debug.LogWarning("User denied permissions to use scene data");
                        // Permissions denied, if data source is using prefab fallback let's load the prefab scene instead
                        if (SceneSettings.DataSource == SceneDataSource.DeviceWithPrefabFallback)
                        {
                            LoadScene(SceneDataSource.Prefab);
                        }
                    };
                    callbacks.PermissionGranted += permissionId =>
                    {
                        // Permissions are now granted and it is safe to try load the scene now
                        LoadScene(SceneSettings.DataSource);
                    };
                    // Note: If the permission request dialog is already active then this call will silently fail
                    // and we won't receive the callbacks. So as a work-around there is a code in Update() to mitigate
                    // this problem.
                    Permission.RequestUserPermission(OVRPermissionsRequester.ScenePermission, callbacks);
                }
                else
#endif
                {
                    LoadScene(SceneSettings.DataSource);
                }
#pragma warning restore CS4014
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void Start()
        {
            if (!_cameraRig)
            {
                _cameraRig = FindObjectOfType<OVRCameraRig>();
            }
        }

        private void Update()
        {
            if (SceneSettings.LoadSceneOnStartup && !_loadSceneCalled)
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                // This is to cope with the case where the permissions dialog was already opened before we called
                // Permission.RequestUserPermission in Awake() and we don't get the PermissionGranted callback
                if (Permission.HasUserAuthorizedPermission(OVRPermissionsRequester.ScenePermission))
                {
                    // We can't await for the LoadScene result because Awake is not async, silence the warning
#pragma warning disable CS4014
                    LoadScene(SceneSettings.DataSource);
#pragma warning restore CS4014

                }
#endif
            }
            if (_enableWorldLock && _cameraRig)
            {
                var room = GetCurrentRoom();
                if (room)
                {
                    room.UpdateWorldLock(_cameraRig);
                }
            }
        }

        /// <summary>
        /// Load the scene asynchronously from the specified data source
        /// </summary>
        async Task LoadScene(SceneDataSource dataSource)
        {
            _loadSceneCalled = true;
            try
            {
                if (dataSource == SceneDataSource.Device ||
                    dataSource == SceneDataSource.DeviceWithPrefabFallback)
                {
                    await LoadSceneFromDevice();
                }
                if (dataSource == SceneDataSource.Prefab ||
                    (dataSource == SceneDataSource.DeviceWithPrefabFallback && _sceneRooms.Count == 0))
                {
                    if (SceneSettings.RoomPrefabs.Length == 0)
                    {
                        Debug.LogWarning($"Failed to load room from prefab because prefabs list is empty");
                        return;
                    }

                    // Clone the roomPrefab, but essentially replace all its content
                    // if -1 or out of range, use a random one
                    var roomIndex = SceneSettings.RoomIndex;
                    if (roomIndex == -1)
                        roomIndex = UnityEngine.Random.Range(0, SceneSettings.RoomPrefabs.Length);

                    Debug.Log($"Loading prefab room {roomIndex}");

                    GameObject roomPrefab = SceneSettings.RoomPrefabs[roomIndex];
                    LoadSceneFromPrefab(roomPrefab);
                }

                if (dataSource == SceneDataSource.Json)
                {
                    if (SceneSettings.SceneJson == "")
                    {
                        Debug.LogWarning($"Empty SceneJson string provided");
                    }
                    LoadSceneFromJsonString(SceneSettings.SceneJson);
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                throw;
            }
        }

        /// <summary>
        /// Called when the room is destroyed
        /// </summary>
        /// <remarks>
        /// This is used to keep the list of active rooms up to date.
        /// So there should never be any null entries in the list.
        /// </remarks>
        /// <param name="room"></param>
        internal void OnRoomDestroyed(MRUKRoom room)
        {
            _sceneRooms.Remove(room);
            if (_cachedCurrentRoom == room)
            {
                _cachedCurrentRoom = null;
            }
        }

        /// <summary>
        /// Destroys the rooms and all children
        /// </summary>
        public void ClearScene()
        {
            foreach (var room in _sceneRooms)
            {
                foreach (Transform child in room.transform)
                    Destroy(child.gameObject);
                Destroy(room.gameObject);
            }
            _sceneRooms.Clear();
            _cachedCurrentRoom = null;
        }

        /// <summary>
        /// Loads the scene from the data stored on the device.
        /// </summary>
        public async Task LoadSceneFromDevice(bool clearSceneFirst = true)
        {
            if (clearSceneFirst)
            {
                ClearScene();
            }

            var newSceneData = await CreateSceneDataFromDevice();

            if (newSceneData.Rooms.Count == 0)
            {
                Debug.Log("MRUKLoader couldn't load any scene data. Ensure " +
                          "that Scene Capture has been run and that the runtime " +
                          "permission for Scene Data has been granted.");
                return;
            }

            UpdateScene(newSceneData);

            InitializeScene();
        }

        /// <summary>
        /// Attempts to create scene data from the device.
        /// </summary>
        /// <returns>A tuple containing a boolean indicating whether the operation was successful, the created scene data, and a list of OVRAnchors.</returns>
        private async Task<Data.SceneData> CreateSceneDataFromDevice()
        {
            var sceneData = new Data.SceneData()
            {
                CoordinateSystem = SerializationHelpers.CoordinateSystem.Unity,
                Rooms = new List<Data.RoomData>()
            };

            var rooms = new List<OVRAnchor>();
            await OVRAnchor.FetchAnchorsAsync<OVRRoomLayout>(rooms);

#if UNITY_EDITOR
            OVRTelemetry.Start(TelemetryConstants.MarkerId.LoadSceneFromDevice)
                .AddAnnotation(TelemetryConstants.AnnotationType.NumRooms, rooms.Count.ToString())
                .SetResult(rooms.Count == 0 ? OVRPlugin.Qpl.ResultType.Fail : OVRPlugin.Qpl.ResultType.Success)
                .Send();
#endif

            foreach (var roomAnchor in rooms)
            {
                var room = roomAnchor.GetComponent<OVRAnchorContainer>();
                var childAnchors = new List<OVRAnchor>();
                await room.FetchChildrenAsync(childAnchors);

                var roomData = new Data.RoomData()
                {
                    Anchor = roomAnchor,
                    UUID = roomAnchor.Uuid.ToString(),
                    Anchors = new List<Data.AnchorData>(),
                    RoomLayout = new Data.RoomLayoutData()
                };

                // Enable locatable on all the child anchors, we do this before accessing all the positions
                // so that when we actually go to access them they are already enabled and we can get all
                // the position on the same frame. Or as close as possible to the same frame.
                var tasks = new List<OVRTask<bool>>();
                foreach (var child in childAnchors)
                {
                    if (!child.TryGetComponent<OVRLocatable>(out var locatable))
                        continue;

                    tasks.Add(locatable.SetEnabledSafeAsync(true));
                }
                await OVRTask.WhenAll(tasks);

                foreach (var child in childAnchors)
                {
                    var anchorData = new Data.AnchorData
                    {
                        Anchor = child,
                        UUID = child.Uuid.ToString("N").ToUpper()
                    };
                    var splitLabels = new List<string>();
                    if (child.TryGetComponent(out OVRSemanticLabels labels) && labels.IsEnabled)
                    {
                        splitLabels.AddRange(labels.Labels.Split(','));
                    }
                    anchorData.SemanticClassifications = splitLabels;
                    if (child.TryGetComponent(out OVRBounded2D bounds2) && bounds2.IsEnabled)
                    {
                        anchorData.PlaneBounds = new Data.PlaneBoundsData()
                        {
                            Min = bounds2.BoundingBox.min,
                            Max = bounds2.BoundingBox.max,
                        };

                        if (bounds2.TryGetBoundaryPointsCount(out var counts))
                        {
                            using var boundary = new NativeArray<Vector2>(counts, Allocator.Temp);
                            if (bounds2.TryGetBoundaryPoints(boundary))
                            {
                                anchorData.PlaneBoundary2D = new List<Vector2>();
                                for (int i = 0; i < counts; i++)
                                {
                                    anchorData.PlaneBoundary2D.Add(boundary[i]);
                                }
                            }
                        }
                    }
                    if (child.TryGetComponent(out OVRBounded3D bounds3) && bounds3.IsEnabled)
                    {
                        anchorData.VolumeBounds = new Data.VolumeBoundsData()
                        {
                            Min = bounds3.BoundingBox.min,
                            Max = bounds3.BoundingBox.max,
                        };
                    }

                    anchorData.Transform.Scale = Vector3.one;
                    bool gotLocation = false;
                    if (child.TryGetComponent(out OVRLocatable locatable) && locatable.IsEnabled)
                    {
                        if (locatable.TryGetSceneAnchorPose(out var pose))
                        {
                            var position = pose.ComputeWorldPosition(Camera.main);
                            var rotation = pose.ComputeWorldRotation(Camera.main);
                            if (rotation.HasValue && rotation.HasValue)
                            {
                                anchorData.Transform.Translation = position.Value;
                                anchorData.Transform.Rotation = rotation.Value.eulerAngles;
                                gotLocation = true;
                            }
                        }
                    }
                    if (!gotLocation)
                    {
                        Debug.LogWarning($"Failed to get location of anchor with UUID: {anchorData.UUID}");
                    }

                    roomData.Anchors.Add(anchorData);

                    if (anchorData.SemanticClassifications.Contains(OVRSceneManager.Classification.WallFace))
                    {
                        roomData.RoomLayout.WallsUUid.Add(anchorData.UUID);
                    }
                    else if (anchorData.SemanticClassifications.Contains(OVRSceneManager.Classification.Floor))
                    {
                        roomData.RoomLayout.FloorUuid = anchorData.UUID;
                    }
                    else if (anchorData.SemanticClassifications.Contains(OVRSceneManager.Classification.Ceiling))
                    {
                        roomData.RoomLayout.CeilingUuid = anchorData.UUID;
                    }
                    else if (anchorData.SemanticClassifications.Contains(OVRSceneManager.Classification.GlobalMesh))
                    {
                        roomData.RoomLayout.GlobalMeshUuid = anchorData.UUID;
                    }
                }
                sceneData.Rooms.Add(roomData);
            }
            return sceneData;
        }

        private void FindAllObjects(GameObject roomPrefab, out List<GameObject> walls, out List<GameObject> volumes,
            out List<GameObject> planes)
        {
            walls = new List<GameObject>();
            volumes = new List<GameObject>();
            planes = new List<GameObject>();
            FindObjects(MRUKAnchor.SceneLabels.WALL_FACE.ToString(), roomPrefab.transform, ref walls);
            FindObjects(MRUKAnchor.SceneLabels.OTHER.ToString(), roomPrefab.transform, ref volumes);
            FindObjects(MRUKAnchor.SceneLabels.TABLE.ToString(), roomPrefab.transform, ref volumes);
            FindObjects(MRUKAnchor.SceneLabels.COUCH.ToString(), roomPrefab.transform, ref volumes);
            FindObjects(MRUKAnchor.SceneLabels.WINDOW_FRAME.ToString(), roomPrefab.transform, ref planes);
            FindObjects(MRUKAnchor.SceneLabels.DOOR_FRAME.ToString(), roomPrefab.transform, ref planes);
            FindObjects(MRUKAnchor.SceneLabels.WALL_ART.ToString(), roomPrefab.transform, ref planes);
            FindObjects(MRUKAnchor.SceneLabels.PLANT.ToString(), roomPrefab.transform, ref volumes);
            FindObjects(MRUKAnchor.SceneLabels.SCREEN.ToString(), roomPrefab.transform, ref volumes);
            FindObjects(MRUKAnchor.SceneLabels.BED.ToString(), roomPrefab.transform, ref volumes);
            FindObjects(MRUKAnchor.SceneLabels.LAMP.ToString(), roomPrefab.transform, ref volumes);
            FindObjects(MRUKAnchor.SceneLabels.STORAGE.ToString(), roomPrefab.transform, ref volumes);
        }

        /// <summary>
        /// This simulates the creation of a scene in the Editor, using transforms and names from our prefab rooms.
        /// </summary>
        public void LoadSceneFromPrefab(GameObject roomPrefab, bool clearSceneFirst = true)
        {
#if UNITY_EDITOR
            OVRTelemetry.Start(TelemetryConstants.MarkerId.LoadSceneFromPrefab)
                .AddAnnotation(TelemetryConstants.AnnotationType.RoomName, roomPrefab.name)
                .Send();
#endif

            if (clearSceneFirst)
            {
                ClearScene();
            }

            FindAllObjects(roomPrefab, out var walls, out var volumes, out var planes);

            GameObject sceneRoom = new GameObject(roomPrefab.name);
            MRUKRoom roomInfo = sceneRoom.AddComponent<MRUKRoom>();

            // walls ordered sequentially, CW when viewed top-down
            List<MRUKAnchor> orderedWalls = new List<MRUKAnchor>();

            List<MRUKAnchor> unorderedWalls = new List<MRUKAnchor>();
            List<Vector3> floorCorners = new List<Vector3>();

            float wallHeight = 0.0f;

            for (int i = 0; i < walls.Count; i++)
            {
                if (i == 0)
                {
                    wallHeight = walls[i].transform.localScale.y;
                }

                MRUKAnchor objData = CreateAnchorFromRoomObject(walls[i].transform, walls[i].transform.localScale, AnchorRepresentation.PLANE);

                objData.transform.parent = sceneRoom.transform;
                objData.transform.Rotate(0, 180, 0);

                unorderedWalls.Add(objData);
            }

            // There may be imprecision between the prefab walls (misaligned edges)
            // so, we shift them so the edges perfectly match up:
            // bottom left corner of wall is fixed, right corner matches left corner of wall to the right
            int seedId = 0;
            for (int i = 0; i < unorderedWalls.Count; i++)
            {
                MRUKAnchor wall = GetAdjacentWall(ref seedId, unorderedWalls);
                orderedWalls.Add(wall);

                Rect wallRect = wall.PlaneRect.Value;
                Vector3 leftCorner = wall.transform.TransformPoint(new Vector3(wallRect.max.x, wallRect.min.y, 0.0f));
                floorCorners.Add(leftCorner);
            }
            for (int i = 0; i < orderedWalls.Count; i++)
            {
                Rect planeRect = orderedWalls[i].PlaneRect.Value;
                Vector3 corner1 = floorCorners[i];
                int nextID = (i == orderedWalls.Count - 1) ? 0 : i + 1;
                Vector3 corner2 = floorCorners[nextID];

                Vector3 wallRight = (corner1 - corner2);
                wallRight.y = 0.0f;
                float wallWidth = wallRight.magnitude;
                wallRight /= wallWidth;
                Vector3 wallUp = Vector3.up;
                Vector3 wallFwd = Vector3.Cross(wallRight, wallUp);
                Vector3 newPosition = (corner1 + corner2) * 0.5f + Vector3.up * (planeRect.height * 0.5f);
                Quaternion newRotation = Quaternion.LookRotation(wallFwd, wallUp);
                Rect newRect = new Rect(-0.5f * wallWidth, planeRect.y, wallWidth, planeRect.height);

                orderedWalls[i].transform.position = newPosition;
                orderedWalls[i].transform.rotation = newRotation;
                orderedWalls[i].PlaneRect = newRect;
                orderedWalls[i].PlaneBoundary2D = new List<Vector2>
                {
                    new Vector2(newRect.xMin, newRect.yMin),
                    new Vector2(newRect.xMax, newRect.yMin),
                    new Vector2(newRect.xMax, newRect.yMax),
                    new Vector2(newRect.xMin, newRect.yMax),
                };
            }

            for (int i = 0; i < volumes.Count; i++)
            {
                Vector3 cubeScale = new Vector3(volumes[i].transform.localScale.x, volumes[i].transform.localScale.z, volumes[i].transform.localScale.y);
                var representation = AnchorRepresentation.VOLUME;
                // Table and couch are special. They also have a plane attached to them.
                if (volumes[i].transform.name == MRUKAnchor.SceneLabels.TABLE.ToString() ||
                    volumes[i].transform.name == MRUKAnchor.SceneLabels.COUCH.ToString())
                {
                    representation |= AnchorRepresentation.PLANE;
                }
                MRUKAnchor objData = CreateAnchorFromRoomObject(volumes[i].transform, cubeScale, representation);
                objData.transform.parent = sceneRoom.transform;

                // in the prefab rooms, the cubes are more Unity-like and default: Y is up, pivot is centered
                // this needs to be converted to Scene format, in which the pivot is on top of the cube and Z is up
                objData.transform.position += cubeScale.z * 0.5f * Vector3.up;
                objData.transform.Rotate(new Vector3(-90, 0, 0), Space.Self);
            }
            for (int i = 0; i < planes.Count; i++)
            {
                MRUKAnchor objData = CreateAnchorFromRoomObject(planes[i].transform, planes[i].transform.localScale, AnchorRepresentation.PLANE);
                objData.transform.parent = sceneRoom.transform;

                // Unity quads have a surface normal facing the opposite direction
                // Rather than have "backwards" walls in the room prefab, we just rotate them here
                objData.transform.Rotate(0, 180, 0);
            }

            // mimic OVRSceneManager: floor/ceiling anchor aligns with longest wall, scaled to room size
            MRUKAnchor longestWall = null;
            float longestWidth = 0.0f;
            foreach (var wall in orderedWalls)
            {
                float wallWidth = wall.PlaneRect.Value.size.x;
                if (wallWidth > longestWidth)
                {
                    longestWidth = wallWidth;
                    longestWall = wall;
                }
            }

            // calculate the room bounds, relative to the longest wall
            float zMin = 0.0f;
            float zMax = 0.0f;
            float xMin = 0.0f;
            float xMax = 0.0f;
            for (int i = 0; i < floorCorners.Count; i++)
            {
                Vector3 localPos = longestWall.transform.InverseTransformPoint(floorCorners[i]);

                zMin = i == 0 ? localPos.z : Mathf.Min(zMin, localPos.z);
                zMax = i == 0 ? localPos.z : Mathf.Max(zMax, localPos.z);
                xMin = i == 0 ? localPos.x : Mathf.Min(xMin, localPos.x);
                xMax = i == 0 ? localPos.x : Mathf.Max(xMax, localPos.x);
            }
            Vector3 localRoomCenter = new Vector3((xMin + xMax) * 0.5f, 0, (zMin + zMax) * 0.5f);
            Vector3 roomCenter = longestWall.transform.TransformPoint(localRoomCenter);
            roomCenter -= Vector3.up * wallHeight * 0.5f;
            Vector3 floorScale = new Vector3(zMax - zMin, xMax - xMin, 1);

            for (int i = 0; i < 2; i++)
            {
                string anchorName = (i == 0 ? "FLOOR" : "CEILING");

                var position = roomCenter + Vector3.up * wallHeight * i;
                float anchorFlip = i == 0 ? 1 : -1;
                var rotation = Quaternion.LookRotation(longestWall.transform.up * anchorFlip, longestWall.transform.right);
                MRUKAnchor objData = CreateAnchor(anchorName, position, rotation, floorScale, AnchorRepresentation.PLANE);
                objData.transform.parent = sceneRoom.transform;

                objData.PlaneBoundary2D = new(floorCorners.Count);
                foreach (var corner in floorCorners)
                {
                    var localCorner = objData.transform.InverseTransformPoint(corner);
                    objData.PlaneBoundary2D.Add(new Vector2(localCorner.x, localCorner.y));
                }

                if (i == 1)
                {
                    objData.PlaneBoundary2D.Reverse();
                }
            }

            // after everything, we need to let the room computation run
            roomInfo.ComputeRoomInfo();
            _sceneRooms.Add(roomInfo);

            InitializeScene();
        }

        /// <summary>
        /// Serializes the current scene into a JSON string using the specified coordinate system for serialization.
        /// </summary>
        /// <param name="coordinateSystem">The coordinate system to be used for serialization (Unity/Unreal).</param>
        /// <returns>A JSON string representing the serialized scene data.</returns>
        public string SaveSceneToJsonString(SerializationHelpers.CoordinateSystem coordinateSystem)
        {
            return SerializationHelpers.Serialize(coordinateSystem);
        }

        /// <summary>
        /// Loads the scene from a JSON string representing the scene data.
        /// </summary>
        /// <param name="jsonString">The JSON string containing the serialized scene data.</param>
        /// <param name="clearSceneFirst">If set to true, the current scene will be cleared before loading the new data. Defaults to true.</param>
        public void LoadSceneFromJsonString(string jsonString, bool clearSceneFirst = true)
        {
            if (clearSceneFirst)
            {
                ClearScene();
            }

            var newSceneData = SerializationHelpers.Deserialize(jsonString);

            UpdateScene(newSceneData);

#if UNITY_EDITOR
            OVRTelemetry.Start(TelemetryConstants.MarkerId.LoadSceneFromJson)
                .AddAnnotation(TelemetryConstants.AnnotationType.NumRooms, _sceneRooms.Count.ToString())
                .Send();
#endif

            InitializeScene();
        }

        private MRUKAnchor CreateAnchorFromRoomObject(Transform refObject, Vector3 objScale, AnchorRepresentation representation)
        {
            return CreateAnchor(refObject.name, refObject.position, refObject.rotation, objScale, representation);
        }

        /// <summary>
        /// Creates an anchor with the specified properties.
        /// </summary>
        /// <param name="name">The name of the anchor.</param>
        /// <param name="position">The position of the anchor.</param>
        /// <param name="rotation">The rotation of the anchor.</param>
        /// <param name="objScale">The scale of the anchor.</param>
        /// <param name="representation">The representation of the anchor (plane or volume).</param>
        /// <returns>The created anchor.</returns>
        private MRUKAnchor CreateAnchor(string name, Vector3 position, Quaternion rotation, Vector3 objScale, AnchorRepresentation representation)
        {
            var realAnchor = new GameObject(name);
            realAnchor.transform.position = position;
            realAnchor.transform.rotation = rotation;
            MRUKAnchor objData = realAnchor.AddComponent<MRUKAnchor>();
            objData.AnchorLabels.Add(realAnchor.name);
            if ((representation & AnchorRepresentation.PLANE) != 0)
            {
                var size2d = new Vector2(objScale.x, objScale.y);
                var rect = new Rect(-0.5f * size2d, size2d);
                objData.PlaneRect = rect;
                objData.PlaneBoundary2D = new List<Vector2>
                {
                    new Vector2(rect.xMin, rect.yMin),
                    new Vector2(rect.xMax, rect.yMin),
                    new Vector2(rect.xMax, rect.yMax),
                    new Vector2(rect.xMin, rect.yMax),
                };
            }
            if ((representation & AnchorRepresentation.VOLUME) != 0)
            {
                Vector3 offsetCenter = new Vector3(0, 0, -objScale.z * 0.5f);
                objData.VolumeBounds = new Bounds(offsetCenter, objScale);
            }
            return objData;
        }

        void FindObjects(string objName, Transform rootTransform, ref List<GameObject> objList)
        {
            if (rootTransform.name.Equals(objName))
            {
                objList.Add(rootTransform.gameObject);
            }

            foreach (Transform child in rootTransform)
            {
                FindObjects(objName, child, ref objList);
            }
        }

        MRUKAnchor GetAdjacentWall(ref int thisID, List<MRUKAnchor> randomWalls)
        {
            Vector2 thisWallScale = randomWalls[thisID].PlaneRect.Value.size;

            Vector3 halfScale = thisWallScale * 0.5f;
            Vector3 bottomRight = randomWalls[thisID].transform.position - randomWalls[thisID].transform.up * halfScale.y - randomWalls[thisID].transform.right * halfScale.x;
            float closestCornerDistance = Mathf.Infinity;
            // When searching for a matching corner, the correct one should match positions. If they don't, assume there's a crack in the room.
            // This should be an impossible scenario and likely means broken data from Room Setup.
            int rightWallID = 0;
            for (int i = 0; i < randomWalls.Count; i++)
            {
                // compare to bottom left point of other walls
                if (i != thisID)
                {
                    Vector2 testWallHalfScale = randomWalls[i].PlaneRect.Value.size * 0.5f;
                    Vector3 bottomLeft = randomWalls[i].transform.position - randomWalls[i].transform.up * testWallHalfScale.y + randomWalls[i].transform.right * testWallHalfScale.x;
                    float thisCornerDistance = Vector3.Distance(bottomLeft, bottomRight);
                    if (thisCornerDistance < closestCornerDistance)
                    {
                        closestCornerDistance = thisCornerDistance;
                        rightWallID = i;
                    }
                }
            }
            thisID = rightWallID;
            return randomWalls[thisID];
        }

        /// <summary>
        /// Manages the scene by creating, updating, or deleting rooms and anchors based on new scene data.
        /// </summary>
        /// <param name="newSceneData">The new scene data.</param>
        /// <returns>A list of managed MRUKRoom objects.</returns>
        private void UpdateScene(Data.SceneData newSceneData)
        {
            List<Data.RoomData> newRoomsToCreate = new();
            List<MRUKRoom> rooms = new();

            //the existing rooms will get removed from this list and updated seperatly
            newRoomsToCreate.AddRange(newSceneData.Rooms);

            List<MRUKRoom> roomsToRemove = new();

            //check old rooms to see if a new received room match and then perform update on room,
            //update,delete or create on anchors.
            for (int i = 0; i < _sceneRooms.Count; i++)
            {
                var oldRoom = _sceneRooms[i];

                foreach (var newRoomData in newSceneData.Rooms)
                {
                    if (oldRoom.Equals(newRoomData))
                    {
                        newRoomsToCreate.Remove(newRoomData);
                        rooms.Add(oldRoom);

                        //we may added the room before as candidate to remove
                        roomsToRemove.RemoveAll(room => room == oldRoom);
                        break;
                    }
                    //check for similar room

                    bool containsExistingAnchor = false;
                    foreach (var oldAnchor in oldRoom.GetRoomAnchors())
                    {
                        foreach (var newAnchorData in newRoomData.Anchors)
                        {
                            if (oldAnchor.Equals(newAnchorData)) //if a new anchor matches an old, we are in the same room
                            {
                                containsExistingAnchor = true;
                                break;
                            }
                        }
                        if (containsExistingAnchor) break;
                    }

                    if (containsExistingAnchor)
                    {
                        oldRoom.Anchor = newRoomData.Anchor;
                        oldRoom.name = $"Room - {newRoomData.UUID}";

                        List<Tuple<MRUKAnchor, Data.AnchorData>> anchorsToUpdate = new();
                        Dictionary<Data.AnchorData, bool> newAnchorsExisting = new();
                        Dictionary<MRUKAnchor, bool> oldAnchorsExisting = new();
                        foreach (var anchorData in newRoomData.Anchors)
                        {
                            newAnchorsExisting[anchorData] = false;
                        }

                        foreach (var oldAnchor in oldRoom.GetRoomAnchors())
                        {
                            foreach (var anchorData in newRoomData.Anchors)
                            {
                                if (oldAnchor.Equals(anchorData))
                                {
                                    newAnchorsExisting[anchorData] = true;
                                    oldAnchorsExisting[oldAnchor] = true;
                                    break;
                                }

                                if (oldAnchor.Anchor == anchorData.Anchor)
                                {
                                    anchorsToUpdate.Add(
                                        new Tuple<MRUKAnchor, Data.AnchorData>(oldAnchor, anchorData));

                                    //because we do not need a create, we have the update
                                    newAnchorsExisting[anchorData] = true;
                                    oldAnchorsExisting[oldAnchor] = true;
                                }
                            }

                            oldAnchorsExisting.TryAdd(oldAnchor, false);
                        }

                        foreach (var kv in newAnchorsExisting)
                        {
                            if (kv.Value) continue; //existing,  not create a new one

                            var anchor = oldRoom.CreateAnchor(kv.Key);
                            oldRoom.GetRoomAnchors().Add(anchor);
                        }

                        foreach (var kv in oldAnchorsExisting)
                        {
                            if (kv.Value) continue; //if existing, we do not need to remove
                            oldRoom.GetRoomAnchors().Remove(kv.Key);
                            oldRoom.AnchorRemovedEvent?.Invoke(kv.Key);
                            Utilities.DestroyGameObjectAndChildren(kv.Key.gameObject);
                        }

                        foreach (var anchorToUpdate in anchorsToUpdate)
                        {
                            var oldAnchor = anchorToUpdate.Item1;
                            var newAnchorData = anchorToUpdate.Item2;
                            oldAnchor.UpdateAnchor(newAnchorData);
                            oldRoom.AnchorUpdatedEvent?.Invoke(oldAnchor);
                        }

                        rooms.Add(oldRoom);

                        RoomUpdatedEvent?.Invoke(oldRoom);

                        //we may added the room before as candidate to remove
                        roomsToRemove.RemoveAll(room => room == oldRoom);
                        newRoomsToCreate.Remove(newRoomData);
                        break;
                    }


                    //we did not break so far, so we assume we may need to delete it if it doesnt show up
                    if (!roomsToRemove.Contains(oldRoom)) roomsToRemove.Add(oldRoom);
                }
            }

            foreach (var oldRoom in roomsToRemove)
            {
                for (int j = oldRoom.GetRoomAnchors().Count - 1; j >= 0; j--)
                {
                    var anchor = oldRoom.GetRoomAnchors()[j];
                    oldRoom.GetRoomAnchors().Remove(anchor);
                    oldRoom.AnchorRemovedEvent?.Invoke(anchor);
                    Utilities.DestroyGameObjectAndChildren(anchor.gameObject);
                }

                _sceneRooms.Remove(oldRoom);
                RoomRemovedEvent?.Invoke(oldRoom);

                Utilities.DestroyGameObjectAndChildren(oldRoom.gameObject);

            }

            foreach (var newRoomData in newRoomsToCreate)
            {
                //create room and throw events for room and anchors
                var room = CreateRoom(newRoomData);
                rooms.Add(room);
                RoomCreatedEvent?.Invoke(room);
            }

            _sceneRooms = rooms;

            foreach (var room in _sceneRooms)
            {
                // after everything, we need to let the room computation run
                room.ComputeRoomInfo();
            }
        }

        /// <summary>
        /// Creates a new room with the specified parameters.
        /// </summary>
        /// <param name="roomData">The data for the new room.</param>
        /// <returns>The created MRUKRoom object.</returns>
        private MRUKRoom CreateRoom(Data.RoomData roomData)
        {
            GameObject sceneRoom = new GameObject($"Room - {roomData.UUID}");
            MRUKRoom roomInfo = sceneRoom.AddComponent<MRUKRoom>();

            roomInfo.Anchor = roomData.Anchor;

            foreach (Data.AnchorData anchorData in roomData.Anchors)
            {
                roomInfo.CreateAnchor(anchorData);
            }

            return roomInfo;
        }
    }
}
