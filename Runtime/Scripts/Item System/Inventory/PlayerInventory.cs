using System;
using SF.DataManagement;

namespace SF.InventoryModule
{
    [Serializable]
    public class PlayerInventory : ItemContainer
    {
        private void Start()
        {
            MetroidvaniaSaveManager.PlayerInventory = this;
        }
    }
}
