using System;
using UnityEngine;

namespace SF.SpawnModule
{
    using Characters.Controllers;
    using PhysicsLowLevel;
    using RoomModule;
    /// <summary>
    /// The system that controls the logic for spawning the player. 
    /// </summary>
    public class SpawnSystem : MonoBehaviour
    {
        public GameObject Controller;
        private void Start()
        {
            if (Controller != null)
                OnInitialPlayerSpawn(Controller);
        }

        private void OnDestroy()
        {
            CurrentSpawnPosition = null;
            SpawnedPlayer = null;
            SpawnedPlayerController = null;
        }

        public static Transform CurrentSpawnPosition;
        
        /// <summary>
        /// The spawned root gameobject of the player.
        /// </summary>
        public static GameObject SpawnedPlayer;
        
        /// <summary>
        /// The <see cref="ControllerBody2D"/> of the <see cref="SpawnedPlayer"/>.
        /// </summary>
        public static ControllerBody2D SpawnedPlayerController;
        
        public static event Action<GameObject> InitialPlayerSpawnHandler;
        public static event Action PlayerRespawnHandler;

        /// <summary>
        /// Tell the game to start the initial spawning of the player when loading up a save file.
        /// </summary>
        public static ControllerBody2D OnInitialPlayerSpawn(GameObject playerPrefab)
        {
            if (playerPrefab == null)
                return null;
            
            SpawnedPlayer = GameObject.Instantiate(playerPrefab,RoomSystem.CurrentRoomPosition,Quaternion.identity);
            if (SpawnedPlayer == null)
                return null;
            
            // Instead of the RoomSystem current room we should use the active spawn point.
            //SpawnedPlayer.transform.position = RoomSystem.CurrentRoom.SpawnedInstance.transform.position;
            SpawnedPlayer.TryGetComponent(out SpawnedPlayerController);
            
            InitialPlayerSpawnHandler?.Invoke(SpawnedPlayer);
            
            return SpawnedPlayer.GetComponent<PlayerController>();
        }

        /// <summary>
        /// Respawns the player and invokes the <see cref="PlayerRespawnHandler"/> event.
        /// </summary>
        public static void RespawnPlayer()
        {
            PlayerRespawnHandler?.Invoke();
        }
    }
}
