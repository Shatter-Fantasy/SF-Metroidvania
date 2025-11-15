using System;
using SF.PhysicsLowLevel;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SF.SpawnModule
{
    /// <summary>
    /// The system that controls the logic for spawning the player. 
    /// </summary>
    public class SpawnSystem : MonoBehaviour
    {
        /// <summary>
        /// The spawned root gameobject of the player.
        /// </summary>
        public static GameObject SpawnedPlayer;
        
        /// <summary>
        /// The <see cref="ControllerBody2D"/> of the <see cref="SpawnedPlayer"/>.
        /// </summary>
        public static ControllerBody2D SpawnedPlayerController;
        
        public static event Action<GameObject> InitialPlayerSpawnHandler;

        /// <summary>
        /// Tell the game to start the initial spawning of the player when loading up a save file.
        /// </summary>
        public static ControllerBody2D OnInitialPlayerSpawn(GameObject playerPrefab)
        {
            if (playerPrefab == null)
                return null;

            SpawnedPlayer = Instantiate(playerPrefab);

            if (SpawnedPlayer == null)
                return null;
            
            // Instead of the RoomSystem current room we should use the active spawn point.
            //SpawnedPlayer.transform.position = RoomSystem.CurrentRoom.SpawnedInstance.transform.position;
            SpawnedPlayer.TryGetComponent(out SpawnedPlayerController);
            
            InitialPlayerSpawnHandler?.Invoke(SpawnedPlayer);
            
            return SpawnedPlayer.GetComponent<ControllerBody2D>();
        }
    }
}
