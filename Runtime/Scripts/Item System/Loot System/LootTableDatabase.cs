using UnityEngine;

namespace SF.LootModule
{
    using DataModule;
    
    [CreateAssetMenu(fileName = "LootTableDatabase", menuName = "SF/Loot/Loot Table Database")]
    public class LootTableDatabase : SFDatabase<LootTableData>
    {
    }
}
