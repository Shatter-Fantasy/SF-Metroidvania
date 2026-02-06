using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SF.SpawnModule
{
    using Characters.Data;
    using StatModule;
    
    public class CharacterSpawner : MonoBehaviour
    {
        [FormerlySerializedAs("_characterDB")] public CharacterDatabase CharacterDB;
        [FormerlySerializedAs("_spawnSets")] public SpawnSet[] SpawnSets;

        [SerializeField] protected int _spawnLimit = 1;
        protected int _spawnedCount;
        public virtual void SpawnCharacters()
        {
            if (SpawnSets.Length < 1)
                return;

            if (_spawnedCount >= _spawnLimit)
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
                    SpawnSets[i].SpawnedHealth                      =  stats.CharacterHealth;
                    SpawnSets[i].SpawnedHealth.CharacterDiedHandler += OnCharacterDied;
                    SpawnSets[i].SpawnedHealth.Respawn();
                    
                    cData.SetData(spawnedCharacterData);
                }
                else
                {
                    characterData.SetData(spawnedCharacterData);
                }

            } // End of for loop.

            _spawnedCount++;
        }
        
        public virtual void DespawnCharacters()
        {
            for (int i = 0; i < SpawnSets.Length; i++)
            {
                if(SpawnSets[i].SpawnedCharacter == null)
                    continue;
                
                SpawnSets[i].SpawnedHealth?.Despawn();
            }

            _spawnedCount--;
        }

        protected virtual void OnCharacterDied()
        {
            _spawnedCount--;
        }
    }
    
    
#if UNITY_EDITOR
    [CustomEditor(typeof(CharacterSpawner),true)]
    public class CharacterSpawnerEditor : Editor
    {
        private Vector3 _lastFramePosition;
        private CharacterSpawner _target;
        private void Awake()
        {
            _target = target as CharacterSpawner;
            
            if(_target != null)
                _lastFramePosition = _target.transform.position;
        }

        public void OnSceneGUI()
        {
            CharacterSpawner t = target as CharacterSpawner;
            
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
