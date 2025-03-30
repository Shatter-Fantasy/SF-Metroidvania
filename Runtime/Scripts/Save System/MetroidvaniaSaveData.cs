using SF.InventoryModule;
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
        public float TestFloat;
    }
}
