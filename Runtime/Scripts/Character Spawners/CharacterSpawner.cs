using SF.Characters.Data;
using SF.StatModule;

using UnityEngine;

namespace SF.SpawnModule
{
    public class CharacterSpawner : MonoBehaviour
    {
        /// <summary>
        /// This is the id of the character data inside of the database.
        /// </summary>
        public int SpawnedCharacterID = 0;

        public CharacterDatabase CharacterDB;

        public CharacterDTO SpawnedCharacterData;
        public void Start()
        {
            if(CharacterDB == null)
                return;

            SpawnedCharacterData = CharacterDB.GetDataByID(SpawnedCharacterID);

            if(SpawnedCharacterData != null)
                SpawnedCharacter();
        }

        private void SpawnedCharacter()
        {
            GameObject spawnedObject = Instantiate(SpawnedCharacterData.Prefab,
                transform.position,
                Quaternion.identity);

            if(!spawnedObject.TryGetComponent(out CharacterStats stats))
            {
               stats = spawnedObject.AddComponent<CharacterStats>();
            }

            stats.CharacterStatList = SpawnedCharacterData.Stats;

            if(!spawnedObject.TryGetComponent(out CharacterData characterData))
            {
                characterData = spawnedObject.AddComponent<CharacterData>();
            }
            
            if(characterData is CombatantData cData)
            {
                stats.CharacterHealth.Respawn();
                cData.SetData(SpawnedCharacterData);
                return;
            }
            
            characterData.SetData(SpawnedCharacterData);
        }
    }
}
