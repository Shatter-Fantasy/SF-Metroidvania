using SF.RoomModule.RegionModule;

namespace SF.DataManagement
{
    using ItemModule;
    using RoomModule;

    public class MetroidvaniaSaveManager : SaveSystem
    {
        public static SavePoint CurrentSavePoint;
        
        public static PlayerInventory PlayerInventory;
        public static MetroidvaniaSaveData CurrentMetroidvaniaSaveData = new();


        public static void NewGame()
        {
            RegionSystem.LoadRegionAsync(RegionSystem.RegionDatabase[0]);
        }
        
        public static void SaveGame()
        {
            CurrentSaveFileData.SaveDatas.Clear();
            // Trigger save event first just in case something listening to the event updates data that would be put in the save file.
            SaveDataHandler?.Invoke();
            CurrentSaveFileData.TryAddOrSetDataBlock(CurrentMetroidvaniaSaveData);
            SaveDataFile();
        }

        public static void LoadGame()
        {
            if (HasSaveFiles())
            {
                LoadDataFile();
                
                // Out Metroidvania code here.
                var data = CurrentSaveFileData.GetSaveDataBlock<MetroidvaniaSaveData>();
            
                if(PlayerInventory != null)
                    PlayerInventory = data.PlayerInventory;
            
                // We set the spawned instance of the save room first before spawning the player.
                // This makes the player spawning trigger the RoomSystem.OnRoomEnter properly. 
                RoomSystem.SetInitialRoom(data.SavedRoomID);
            }
            else // if there is no save file already made.
            {
                // Set the starting room to the default new game room.
                RoomSystem.SetInitialRoom(RoomSystem.StartingRoomId);
            }
            
            CurrentSavePoint = CurrentSaveFileData.CurrentSaveStation;
        }
    }
}
