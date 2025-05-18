using SF.Characters.Data;
using SF.RoomModule;
using SF.StatModule;
using UnityEngine;

namespace SF.SpawnModule
{
    /// <summary>
    /// Controls the spawn characters to keep spawned or despawn based on the current room that the player is in. 
    /// </summary>
    public class RoomCharacterSpawner : MonoBehaviour
    {
        [SerializeField] private CharacterDatabase _characterDB;
        [SerializeField] private SpawnSet[] _spawnSets;

        private bool _alreadySpawned;
        private void Awake()
        {
            if (TryGetComponent(out RoomController roomController))
            {
                roomController.OnRoomEnteredHandler += SpawnCharacters;
                roomController.OnRoomExitHandler += DespawnCharacters;
            }
        }

        private void SpawnCharacters()
        {
            // Don't respawn the characters when they are already loaded in memory.
            if (_alreadySpawned)
            {
                RespawnCharacters();
                return;
            }

            if (_spawnSets.Length < 1)
                return;
            
            
            for (int i = 0; i < _spawnSets.Length; i++)
            {
                var spawnedCharacterData = _characterDB.GetDataByID(_spawnSets[i].SpawnCharacterID);
                _spawnSets[i].SpawnedCharacter = Instantiate(spawnedCharacterData.Prefab,
                _spawnSets[i].SpawnPosition,
                Quaternion.identity);

                if(!_spawnSets[i].SpawnedCharacter.TryGetComponent(out CharacterStats stats))
                {
                    stats = _spawnSets[i].SpawnedCharacter.AddComponent<CharacterStats>();
                }

                stats.CharacterStatList = spawnedCharacterData.Stats;

                if(!_spawnSets[i].SpawnedCharacter.TryGetComponent(out CharacterData characterData))
                {
                    characterData = _spawnSets[i].SpawnedCharacter.AddComponent<CharacterData>();
                }
                    
                if(characterData is CombatantData cData)
                {
                    _spawnSets[i].SpawnedHealth = stats.CharacterHealth;
                    _spawnSets[i].SpawnedHealth.Respawn();
                    
                    cData.SetData(spawnedCharacterData);
                }
                else
                {
                    characterData.SetData(spawnedCharacterData);
                }

            } // End of for loop.

            _alreadySpawned = true;
        }

        private void DespawnCharacters()
        {
            for (int i = 0; i < _spawnSets.Length; i++)
            {
                if(_spawnSets[i].SpawnedCharacter == null)
                    continue;
                
                _spawnSets[i].SpawnedHealth.Despawn();
            }
        }

        /// <summary>
        /// Called if the characters were already loaded, but than respawned.
        /// Happens when first loading a room and it's characters than exiting the room despawning them.
        /// </summary>
        private void RespawnCharacters()
        {
            for (int i = 0; i < _spawnSets.Length; i++)
            {
                if(_spawnSets[i].SpawnedCharacter == null)
                    continue;
                
                _spawnSets[i].SpawnedHealth.Respawn();
                _spawnSets[i].SpawnedCharacter.transform.position = _spawnSets[i].SpawnPosition;
            }
        }
    }
    [System.Serializable]
    public struct SpawnSet
    {
        public int SpawnCharacterID;
        public Vector2 SpawnPosition;
        public GameObject SpawnedCharacter;
        public CharacterHealth SpawnedHealth;
    }
}
