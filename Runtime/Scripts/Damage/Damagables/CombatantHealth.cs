using SF.Characters.Data;
using SF.Experience;
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
                //  Grant the player his experience from the enemy kill.
                
                PlayerLevelStats.GainExperience(_combatantData.Experience.BaseExperience);
                
                if (_combatantData.EnemyLootTable is not null)
                {
                    _combatantData.EnemyLootTable.DropRandomLoot(transform.position);
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
        
        protected override void OnEnable()
        {
            _deathTimer = new Timer(_deathTimer.Duration,Despawn);
        }
    }
}
