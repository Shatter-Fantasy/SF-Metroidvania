using UnityEngine;

using SF.Characters.Data;


namespace SF.LootModule
{
    [CreateAssetMenu(fileName = "LootTableDatabase", menuName = "SF/Loot/Loot Table Database")]
    public class LootTableDatabase : SFDatabase<LootTableData>
    {
    }
}
