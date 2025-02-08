using UnityEngine;

using SF.Characters;
using SF.ItemModule;

namespace SF.SpawnModule
{
    public class CombatantHealth : CharacterHealth
    {
        private CombatantData _combatantData;

        protected override void Awake()
        {
            base.Awake();
            _combatantData = GetComponent<CombatantData>();
        }

        protected override void Kill()
        {
            if (_combatantData != null 
                && _combatantData.EnemyLootTable != null 
                && _combatantData.EnemyLootTable.LootTable.Count > 0)
            {
                GameObject spawnedItem = Instantiate(
                    _combatantData.EnemyLootTable.LootTable[0].Prefab,
                    transform.position,
                    Quaternion.identity
                );

                spawnedItem.GetComponent<PickupItem>().Item = _combatantData.EnemyLootTable.LootTable[0];
            }

            base.Kill();
        }
    }
}
