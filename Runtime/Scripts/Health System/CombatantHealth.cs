using UnityEngine;

using SF.Characters.Data;
using SF.Experience;
using SF.ItemModule;
using SF.StateMachine.Core;


namespace SF.SpawnModule
{
    public class CombatantHealth : CharacterHealth
    {
        private CombatantData _combatantData;
        private StateMachineBrain _combatantStateBrain;
        
        protected override void Awake()
        {
            base.Awake();
            _combatantData = GetComponent<CombatantData>();
            _combatantStateBrain = GetComponentInChildren<StateMachineBrain>();
        }
        
        protected override void Kill()
        {
            if (_combatantData is not null)
            {
                // TODO: Will need checks later for allies and summonings to not grant experience.
                //  Grant the player his expierence from the enemy kill.
                
                ExperienceEvent.Trigger(ExperienceEventTypes.Gain,_combatantData.Experience.BaseExperience);

                // Loot Drop checks.
                if (_combatantData.EnemyLootTable is not null
                    && _combatantData.EnemyLootTable.LootTable.Count > 0)
                {
                    GameObject spawnedItem = Instantiate(
                        _combatantData.EnemyLootTable.LootTable[0].Prefab,
                        transform.position,
                        Quaternion.identity
                    );

                    spawnedItem.GetComponent<PickupItem>().Item = _combatantData.EnemyLootTable.LootTable[0];
                }
            }

            base.Kill();
        }

        public override void Respawn()
        {
            base.Respawn();
            
            if(_combatantStateBrain != null)
                _combatantStateBrain.InitStateBrain();
        }
    }
}
