using System.Collections.Generic;

using SF.Characters.Data;

using UnityEngine;

namespace SF.LootModule
{
    [CreateAssetMenu(fileName = "LootTableDatabase", menuName = "SF/Loot/Loot Table Database")]
    public class LootTableDatabase : SFDatabase<LootTableData>
    {
    }
}
