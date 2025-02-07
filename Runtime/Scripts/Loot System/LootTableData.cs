using System.Collections.Generic;

using SF.Inventory;

using UnityEngine;

namespace SF.LootModule
{
    [CreateAssetMenu(fileName = "Loot Table Data", menuName = "SF/Loot/Loot Table Data")]
    public class LootTableData : DTOAssetBase
    {
        public List<ItemDTO> LootTable = new List<ItemDTO>();
    }

}
