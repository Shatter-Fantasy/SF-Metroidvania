using System;
using System.Collections.Generic;
using SF.CameraModule;
using Unity.Cinemachine;
using UnityEngine;

namespace SF.RoomModule
{
    public class RoomController : MonoBehaviour
    {
        /// <summary>
        /// The id for the room's spawned instance the RoomController is controlling.
        /// </summary>
        public int RoomID;
        public List<int> RoomIdsToLoadOnEnter = new();
        public BoxCollider2D RoomConfiner;
        public CinemachineCamera RoomCamera;
        
        private void Awake()
        {
            if (RoomConfiner == null)
                RoomConfiner = GetComponent<BoxCollider2D>();
        }
        
        public void LoadRoom()
        {
            // Can happen from CinemachineTriggerAction when exiting playmode.
            // If the collider is deloaded first it triggers an onexit callback while deloading the runtime.
            if (!RoomSystem.IsRoomLoaded(RoomID))
                return;
            
            RoomSystem.OnRoomEntered(RoomID);

            if (RoomCamera != null)
            {
                CameraController.SetPlayerCMCamera(RoomCamera);
            }

            foreach (var roomID in RoomIdsToLoadOnEnter)
            {
                RoomSystem.LoadConnectedRoom(roomID);
            }
        }
        
        private void OnDestroy()
        {
            RoomSystem.CleanUpRoom(RoomID);
        }
    }
}
