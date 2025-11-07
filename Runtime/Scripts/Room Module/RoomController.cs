using System;
using System.Collections.Generic;
using SF.CameraModule;
using SF.Managers;
using SF.PhysicsLowLevel;
using SF.SpawnModule;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;
using UnityEngine.U2D.Physics.LowLevelExtras;

namespace SF.RoomModule
{
    public class RoomController : MonoBehaviour, PhysicsCallbacks.ITriggerCallback
    {
        /* TODO List:
            Room Auto Align: Make a method that allows taking in two transforms.
            each transform is the floor of two connected rooms. 
            We can round the x/y values of the transform to make sure they align perfect.
            We might have to make one room round using ceiling and one round using floor depending on the values.         */
        
        
        /// <summary>
        /// The id for the room's spawned instance the RoomController is controlling.
        /// </summary>
        public int RoomID;
        [NonSerialized] public List<int> RoomIdsToLoadOnEnter = new();
        /// <summary>
        /// The camera confined to the room.
        /// </summary>
        public CinemachineCamera RoomCamera;

        /// <summary>
        /// These are optional transition ids for when room controller needs to keep track of fast travel points or using <see cref="TransitionTypes.Local"/>.
        /// </summary>
        public List<RoomTransition> RoomTransitions = new List<RoomTransition>();

        public Action OnRoomEnteredHandler;
        public Action OnRoomExitHandler;
        
        
        private SceneShape _sceneShape;
        private void Awake()
        {
            if (TryGetComponent(out _sceneShape))
            {
                _sceneShape.CallbackTarget = this;
            }
             
            // This is the ignore ray cast physics layer.
            gameObject.layer = 2;
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
            
            OnRoomEnteredHandler?.Invoke();
            
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

        private void InitializeRoom()
        {
            if (RoomSystem.RoomDB[RoomID] == null)
            {
                Debug.LogWarning($"A room with the RoomID of {RoomID} was not found in the RoomDatabase. Check if there was a room with the id of {RoomID} set inside the RoomDatabase");
                return;
            }
            RoomIdsToLoadOnEnter = RoomSystem.RoomDB[RoomID].ConnectedRoomsIDs;
            
            RoomSystem.LoadRoomManually(RoomID, gameObject);

            if (RoomCamera == null)
                return;
            
            CameraController.SetCameraFollow(RoomCamera, SpawnSystem.SpawnedPlayer.transform);
        }
        
        private void OnEnable()
        {
            GameLoader.LevelReadyHandler += InitializeRoom;
        }

        private void OnDisable()
        {
            GameLoader.LevelReadyHandler -= InitializeRoom;
        }

        private void OnDestroy()
        {
            RoomSystem.CleanUpRoom(RoomID);
        }

        public void OnTriggerBegin2D(PhysicsEvents.TriggerBeginEvent beginEvent)
        {
            if (GameManager.Instance.ControlState == GameControlState.Cutscenes)
                return;
            
            if (((GameObject)beginEvent.visitorShape.callbackTarget).TryGetComponent(out ControllerBody2D controller)
                && controller.CollisionInfo.CollisionActivated)
            {
                MakeCurrentRoom();
            }
        }

        public void OnTriggerEnd2D(PhysicsEvents.TriggerEndEvent endEvent)
        {
            OnRoomExitHandler?.Invoke();
        }
    }
}