using SF.ItemModule;
using SF.SpawnModule;
using SF.StatModule;

namespace SF.DataManagement
{
    [System.Serializable]
    public class MetroidvaniaSaveData : SaveDataBlock
    {
        public PlayerHealth PlayerHealth;
        public PlayerStats PlayerStats;
        public PlayerInventory PlayerInventory;
        /// <summary>
        /// The id of the room the save station that was used is in.
        /// </summary>
        public int SavedRoomID;
    }
}
