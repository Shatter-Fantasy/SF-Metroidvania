using SF.Characters.Data;
using SF.RoomModule;
using SF.StatModule;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace SF.SpawnModule
{
    [System.Serializable]
    public struct SpawnSet
    {
        public int SpawnCharacterID;
        public Vector2 SpawnPosition;
        public GameObject SpawnedCharacter;
        public CharacterHealth SpawnedHealth;
    }
    
    /// <summary>
    /// Controls the spawn characters to keep spawned or despawn based on the current room that the player is in. 
    /// </summary>
    public class RoomCharacterSpawner : MonoBehaviour
    {
        public CharacterDatabase CharacterDB;
        public SpawnSet[] SpawnSets;

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

            if (SpawnSets.Length < 1)
                return;
            
            
            for (int i = 0; i < SpawnSets.Length; i++)
            {
                var spawnedCharacterData = CharacterDB.GetDataByID(SpawnSets[i].SpawnCharacterID);
                SpawnSets[i].SpawnedCharacter = Instantiate(spawnedCharacterData.Prefab,
                SpawnSets[i].SpawnPosition,
                Quaternion.identity);

                if(!SpawnSets[i].SpawnedCharacter.TryGetComponent(out CharacterStats stats))
                {
                    stats = SpawnSets[i].SpawnedCharacter.AddComponent<CharacterStats>();
                }

                stats.CharacterStatList = spawnedCharacterData.Stats;

                if(!SpawnSets[i].SpawnedCharacter.TryGetComponent(out CharacterData characterData))
                {
                    characterData = SpawnSets[i].SpawnedCharacter.AddComponent<CharacterData>();
                }
                    
                if(characterData is CombatantData cData)
                {
                    SpawnSets[i].SpawnedHealth = stats.CharacterHealth;
                    SpawnSets[i].SpawnedHealth.Respawn();
                    
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
            for (int i = 0; i < SpawnSets.Length; i++)
            {
                if(SpawnSets[i].SpawnedCharacter == null)
                    continue;
                
                SpawnSets[i].SpawnedHealth?.Despawn();
            }
        }

        /// <summary>
        /// Called if the characters were already loaded, but than respawned.
        /// Happens when first loading a room and it's characters than exiting the room despawning them.
        /// </summary>
        private void RespawnCharacters()
        {
            for (int i = 0; i < SpawnSets.Length; i++)
            {
                if(SpawnSets[i].SpawnedCharacter == null)
                    continue;
                
                SpawnSets[i].SpawnedHealth.Respawn();
                SpawnSets[i].SpawnedCharacter.transform.position = SpawnSets[i].SpawnPosition;
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(RoomCharacterSpawner))]
    public class RoomCharacterSpawnerEditor : Editor
    {
        private Vector3 _lastFramePosition;
        private RoomCharacterSpawner _target;
        private void Awake()
        {
            _target = target as RoomCharacterSpawner;
            
            if(_target != null)
                _lastFramePosition = _target.transform.position;
        }

        public void OnSceneGUI()
        {
            RoomCharacterSpawner t = target as RoomCharacterSpawner;
            
            if (t == null || t.SpawnSets?.Length < 1)
                return;

            if (_lastFramePosition != _target.transform.position)
            {
                Vector2 deltaPosition = _target.transform.position - _lastFramePosition;
                for (int i = 0; i < t.SpawnSets?.Length; i++)
                {
                    t.SpawnSets[i].SpawnPosition += deltaPosition;
                }

                _lastFramePosition = _target.transform.position;
            }
            
            var color = new Color(1, 0.8f, 0.4f, 1);
            Handles.color = color;

            for (int i = 0; i < t.SpawnSets?.Length; i++)
            {
                EditorGUI.BeginChangeCheck();
                Vector3 newTargetPosition = Handles.PositionHandle(t.SpawnSets[i].SpawnPosition, Quaternion.identity);

                if (t.CharacterDB != null &&
                    t.CharacterDB.GetDataByID(t.SpawnSets[i].SpawnCharacterID, out CharacterDTO characterDTO))
                {
                    Handles.Label(newTargetPosition, $"{characterDTO.name}");
                    if (characterDTO.Prefab?.GetComponent<SpriteRenderer>() != null)
                    {
                        
                    }
                }

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(t, "Change Look At Target Position");
                    t.SpawnSets[i].SpawnPosition = newTargetPosition;
                }
            }
        }
    }
#endif
}
