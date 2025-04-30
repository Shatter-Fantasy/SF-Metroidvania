using SF.Characters.Controllers;
using SF.InventoryModule;
using SF.Managers;
using SF.RoomModule;
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
            
            if(PlayerInventory != null)
                PlayerInventory = data.PlayerInventory;
            
            // We set the spawned instance of the save room first before spawning the player.
            // This makes the player spawning trigger the RoomSystem.OnRoomEnter properly. 
            RoomSystem.SetInitialRoom(data.SavedRoomID);
            
            CheckPointEvent.Trigger(CheckPointEventTypes.ChangeCheckPoint, CurrentSaveFileData.CurrentSaveStation as CheckPoint);
        }
    }
}
