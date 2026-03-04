using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

namespace SF.RoomModule
{
    using CameraModule;
    using Characters.Controllers;
    using Managers;
    using PhysicsLowLevel;

    
    public class RoomController : MonoBehaviour, 
        ITriggerShapeCallback
    {
        
        /* TODO List:
            Room Auto Align: Make a method that allows taking in two transforms.
            each transform is the floor of two connected rooms. 
            We can round the x/y values of the transform to make sure they align perfect.
            We might have to make one room round using ceiling and one round using floor depending on the values.         */
        [SerializeField] private Bounds _roomCameraBounds;
        
        /// <summary>
        /// The id for the room's spawned instance the RoomController is controlling.
        /// </summary>
        public int RoomID;
        [NonSerialized] public List<int> RoomIdsToLoadOnEnter = new();
        
        public Action OnRoomEnteredHandler;
        public Action OnRoomExitHandler;

        #region Room Extensions
        /// <summary>
        /// Unfiltered list of all <see cref="IRoomExtension"/> that are connected to this room. 
        /// </summary>
        private readonly List<IRoomExtension> _roomExtensions = new();

        private ReadOnlyCollection<IRoomExtension> _roomEnteredExtensions;
        private readonly List<IRoomExtension> _roomExitedExtensions = new();
        #endregion
        
        [SerializeReference] private SFShapeComponent _physicsShapeComponent;
        private void Awake()
        {
            if (TryGetComponent(out _physicsShapeComponent))
            {
                _physicsShapeComponent.BodyDefinition.type      = PhysicsBody.BodyType.Static;
            }
            
            // Non-allocating version when used with read only List<T>
            gameObject.GetComponents(_roomExtensions);
            
            var rooms  = _roomExtensions.Where((room => room.RoomExtensionType == RoomExtensionType.OnRoomEntered));
            _roomEnteredExtensions = new ReadOnlyCollection<IRoomExtension>(rooms.ToList());
        }

        private void Start()
        {
            if(_physicsShapeComponent != null)
                _physicsShapeComponent.AddTriggerCallbackTarget(this);
            
            if (RoomSystem.RoomDB == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"There is no database set in the {nameof(RoomSystem)}");
                return;
#endif
            }
            if (RoomSystem.RoomDB[RoomID] == null)
            {
                Debug.LogWarning($"A room with the RoomID of {RoomID} was not found in the RoomDatabase. Check if there was a room with the id of {RoomID} set inside the RoomDatabase");
                return;
            }
            RoomIdsToLoadOnEnter = RoomSystem.RoomDB[RoomID].ConnectedRoomsIDs;
        
            RoomSystem.LoadRoomManually(RoomID, gameObject);
        }

        private void OnDestroy()
        {
            RoomSystem.CleanUpRoom(RoomID);
        }
        
        /// <summary>
        /// Changes the current room and invokes all the required CameraSystem, RoomSystem, and GameManagers calls. 
        /// </summary>
        public void MakeCurrentRoom()
        {
            if (!RoomSystem.IsRoomLoaded(RoomID))
            {
                return;
            }

            // Probably should put this if and for loop in the RoomSystem itself.
            if (RoomSystem.DynamicRoomLoading)
            {
                foreach (var roomID in RoomIdsToLoadOnEnter)
                {
                    RoomSystem.LoadRoom(roomID);
                }
            }

            RoomSystem.SetCurrentRoom(RoomID);
        }
        
        public void OnTriggerBegin2D(PhysicsEvents.TriggerBeginEvent beginEvent, SFShapeComponent callingShapeComponent)
        {
            if (GameManager.Instance.ControlState == GameControlState.Cutscenes)
                return;
            
            // Grab the body data.
            var objectData = beginEvent.visitorShape.body.userData.objectValue;

            // SFShapeComponents default set the GameObject they are attached to as the objectValue in userData
            if (objectData is not GameObject visitingGameobject)
                return;
            
            if (!visitingGameobject.TryGetComponent(out PlayerController body2D))
                return;
            
            if (!body2D.CollisionInfo.CollisionActivated)
                return;
            
            OnRoomEnteredHandler?.Invoke();
            for (int i = 0; i < _roomEnteredExtensions.Count; i++)
            {
                _roomEnteredExtensions[i].Process();
            }

            PhysicsAABB aabb   = callingShapeComponent.Body.GetAABB();
            _roomCameraBounds = new Bounds(aabb.center,aabb.extents * 2);
            CameraController.UpdateRectangleConfiner(_roomCameraBounds);
            MakeCurrentRoom();
        }

        public void OnTriggerEnd2D(PhysicsEvents.TriggerEndEvent endEvent, SFShapeComponent callingShapeComponent)
        {
            OnRoomExitHandler?.Invoke();
        }
    }
}