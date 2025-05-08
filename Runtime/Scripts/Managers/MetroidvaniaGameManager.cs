using System.Collections.Generic;
using SF.Characters.Controllers;
using SF.DataManagement;
using SF.RoomModule;
using UnityEngine;

namespace SF.Managers
{
    public class MetroidvaniaGameManager : GameManager
    {
        [SerializeReference]
        public List<SaveDataBlock> SaveDataBlocks = new List<SaveDataBlock> ();
        
        #if UNITY_EDITOR
        /// <summary>
        /// When playing from editor you can set a default room to load into to bypass the starting spawn room.
        /// </summary>
        [Header("Editor only debug settings.")][SerializeField] private bool _loadDebugRoom = false;
        /// <summary>
        /// The <see cref="Room.RoomID"/> of a debug room to start in.
        /// </summary>
        [SerializeField] private int _debugRoomID;
        #endif
        
        protected override void Start()
        {
#if UNITY_EDITOR
            if (_shouldLoadData)
            {
#endif
                MetroidvaniaSaveManager.LoadGame();
                SaveDataBlocks = SaveSystem.CurrentSaveDataBlocks();
#if UNITY_EDITOR
            }

            if (_loadDebugRoom)
            {
                RoomSystem.SetInitialRoom(_debugRoomID);
            }
#endif
        }
        
    }
}
