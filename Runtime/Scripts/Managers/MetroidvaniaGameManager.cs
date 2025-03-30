using System.Collections.Generic;
using SF.Characters.Controllers;
using SF.DataManagement;
using UnityEngine;

namespace SF.Managers
{
    public class MetroidvaniaGameManager : GameManager
    {
        [SerializeReference]
        public List<SaveDataBlock> SaveDataBlocks = new List<SaveDataBlock> ();
        protected override void Start()
        {
            PlayerSceneObject = FindAnyObjectByType<PlayerController>().gameObject;
            MetroidvaniaSaveManager.LoadGame();
            SaveDataBlocks = SaveSystem.CurrentSaveDataBlocks();
        }
    }
}
