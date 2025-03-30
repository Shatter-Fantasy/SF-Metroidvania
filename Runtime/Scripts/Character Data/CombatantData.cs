using SF.LootModule;
using SF.Experience;

namespace SF.Characters.Data
{
    public class CombatantData : CharacterData
    {
        public LootTableData EnemyLootTable;
        public RegionalLootTableData RegionalLootTable;
        public ExperienceValue Experience;


        public override void SetData(CharacterDTO dto)
        {
            base.SetData(dto);
            Experience = dto.Experience;
            EnemyLootTable = dto.EnemyLootTable;
            RegionalLootTable = dto.RegionalLootTable;
        }
    }
}