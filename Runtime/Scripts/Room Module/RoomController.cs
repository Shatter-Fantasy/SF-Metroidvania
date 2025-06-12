using System;
using System.Collections.Generic;
using SF.CameraModule;
using SF.Characters.Controllers;
using SF.Managers;
using Unity.Cinemachine;
using UnityEngine;

namespace SF.RoomModule
{
    public class RoomController : MonoBehaviour
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
        public CinemachineCamera RoomCamera;

        /// <summary>
        /// These are optional transition ids for when room controller needs to keep track of fast travel points or using <see cref="TransitionTypes.Local"/>.
        /// </summary>
        public List<RoomTransition> RoomTransitions = new List<RoomTransition>();

        public Action OnRoomEnteredHandler;
        public Action OnRoomExitHandler;
        private void Awake()
        {
            // This is the ignore ray cast physics layer.
            gameObject.layer = 2;
            RoomIdsToLoadOnEnter = RoomDB.Instance[RoomID].ConnectedRoomsIDs;
        }

        private void Start()
        {
            if (!RoomSystem.IsRoomLoaded(RoomID))
            {
                RoomSystem.AddLoadedRoomManually(RoomID);
                RoomSystem.RoomDB[RoomID].SpawnedInstance = this.gameObject;
            }
        }
        
        
        /// <summary>
        /// Changes the current room and invokes all the required CameraSystem, RoomSystem, and GameManagers calls. 
        /// </summary>
        public void MakeCurrentRoom()
        {
            // Can happen from CinemachineTriggerAction when exiting playmode.
            // If the collider is deloaded first it triggers an onexit callback while deloading the runtime.
            if (!RoomSystem.IsRoomLoaded(RoomID))
            {
                return;
            }
            
            RoomSystem.OnRoomEntered(RoomID);
            OnRoomEnteredHandler?.Invoke();
            
            if (RoomCamera != null)
            {
                // This sets the priority of the virtual cameras for the old and new rooms while setting the new RoomConfiners.
                CameraController.SwitchPlayerCMCamera(RoomCamera);
            }

            foreach (var roomID in RoomIdsToLoadOnEnter)
            {
                RoomSystem.LoadConnectedRoom(roomID);
            }

            RoomSystem.SetCurrentRoom(RoomID);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out PlayerController controller) 
                && controller.CollisionController.IsActive)
            {
                MakeCurrentRoom();
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            OnRoomExitHandler?.Invoke();
        }

        private void OnDestroy()
        {
            RoomSystem.CleanUpRoom(RoomID);
        }
    }
}