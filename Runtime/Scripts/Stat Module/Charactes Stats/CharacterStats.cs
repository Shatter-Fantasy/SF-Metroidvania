using UnityEngine;
using Unity.Properties;

namespace SF.StatModule
{
    using DamageModule;
    using SpawnModule;

    /// <summary>
    /// Links the character stats from the database to an actual gameobject. Also controls stat events for external components relying on character data. Example linking HP stat to a health controller script.
    /// </summary>
    public class CharacterStats : MonoBehaviour, IDamageController
    {
        public StatList CharacterStatList;
        /// <summary>
        /// The external character health component to link to.
        /// </summary>
        [CreateProperty] public CharacterHealth CharacterHealth;

        public int CalculateDamage(int preDamage)
        {
            return preDamage;
        }

        protected void Awake()
        {
            CharacterHealth  =  CharacterHealth != null 
                ? CharacterHealth 
                : GetComponent<CharacterHealth>();

            // Make sure a component was find just in case the above was null.
            // IUf it is not null tell it is being externally controlled.
            if(CharacterHealth != null)
                CharacterHealth.DamageController = this;
        }
    }
}
