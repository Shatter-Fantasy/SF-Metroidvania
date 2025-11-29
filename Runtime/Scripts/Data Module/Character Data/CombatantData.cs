using SF.LootModule;
using SF.Experience;
using UnityEngine;

namespace SF.Characters.Data
{
    public class CombatantData : CharacterData
    {
        public LootTableData EnemyLootTable;
        public RegionalLootTableData RegionalLootTable;
        public ExperienceValue Experience;
        
        
#if UNITY_EDITOR
        [SerializeField] private bool _debugSpawn;
        [SerializeField] private CharacterDTO _debugCharacterDTO;
        
        private void Start()
        {
            if (_debugCharacterDTO == null)
                return;
            
            SetData(_debugCharacterDTO);
        }
#endif
        
        public override void SetData(CharacterDTO dto)
        {
            base.SetData(dto);
            Experience = dto.Experience;
            EnemyLootTable = dto.EnemyLootTable;
            RegionalLootTable = dto.RegionalLootTable;
        }
    }
}