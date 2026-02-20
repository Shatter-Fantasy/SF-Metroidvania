using UnityEngine;
using Random = UnityEngine.Random;

namespace SF.SpawnModule
{
    public class TimedCharacterSpawner : CharacterSpawner
    {
        [SerializeField] private Timer _spawnTimer;
        [SerializeField] private int _chanceToSpawn = 100;

        private void Start()
        {
            _spawnTimer = new Timer(_spawnTimer.Duration, OnSpawnCheck);
            _           = _spawnTimer.StartTimerAsync();
        }

        private void OnSpawnCheck()
        {
            int randomChance = Random.Range(0, 100);
			
            if (randomChance > _chanceToSpawn)
            {
                SpawnCharacters();
            }

            if (_spawnedCount <= _spawnLimit)
            {
                _spawnTimer = new Timer(_spawnTimer.Duration, OnSpawnCheck);
                _           = _spawnTimer.StartTimerAsync();
            }
        }

        protected override void OnCharacterDied()
        {
            _spawnedCount--;

            if (!_spawnTimer.TimerStopped)
                return;
            
            _spawnTimer = new Timer(_spawnTimer.Duration, OnSpawnCheck);
            _           = _spawnTimer.StartTimerAsync();
        }
    }
}
