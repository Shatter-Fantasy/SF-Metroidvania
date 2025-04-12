using SF.Characters.Controllers;
using SF.InventoryModule;
using SF.Managers;
using SF.SpawnModule;
using UnityEngine;

namespace SF.DataManagement
{
    public class MetroidvaniaSaveManager : SaveSystem
    {
        public static PlayerInventory PlayerInventory;
        public static MetroidvaniaSaveData CurrentMetroidvaniaSaveData = new();
        
        public static void SaveGame()
        {
            CurrentSaveFileData.SaveDatas.Clear();
            // Trigger save event first just in case something lsitening to the event updates data that would be put in the save file.
            SaveLoadEvent.Trigger(SaveLoadEventTypes.Saving);
            CurrentSaveFileData.TryAddOrSetDataBlock(CurrentMetroidvaniaSaveData);
            SaveDataFile();
        }

        public static void LoadGame()
        {
            LoadDataFile();
            
            // Out Metroidvania code here.
            var data = CurrentSaveFileData.GetSaveDataBlock<MetroidvaniaSaveData>();

            Debug.Log(data.PlayerInventory);
            
            if(PlayerInventory != null)
                PlayerInventory = data.PlayerInventory;
            
            CheckPointEvent.Trigger(CheckPointEventTypes.ChangeCheckPoint, CurrentSaveFileData.CurrentSaveStation as CheckPoint);
        }
    }
}
