using SF.Experience;
using SF.LootModule;
using SF.StatModule;

using UnityEngine;

namespace SF.Characters.Data
{
    [CreateAssetMenu(fileName = "New Character Data", menuName = "SF/Data/Character Data")]
    public class CharacterDTO : DTOAssetBase
    {
        public GameObject Prefab;

        public CharacterCombatantTypes CombatantType = CharacterCombatantTypes.Enemy;
        
        public NPCQuestTypes NPCQuestType = NPCQuestTypes.None;

        public StatList Stats;

        public ExperienceValue Experience;
        public LootTableData EnemyLootTable;
        public RegionalLootTableData RegionalLootTable;
    }
}
