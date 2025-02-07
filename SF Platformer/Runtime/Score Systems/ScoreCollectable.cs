using SF.Events;
using SF.SpawnModule;

using UnityEngine;
using UnityEngine.Audio;

namespace SF.CollectableModule
{
    public class ScoreCollectable : MonoBehaviour, ICollectable, EventListener<RespawnEvent>, EventListener<CheckPointEvent>
    {
        public int Score;
		[SerializeField] private bool ShouldRespawn = true;

		[SerializeField] private AudioClip _collectSFX;

		public void Collect()
        {
			if(_collectSFX != null)
				AudioManager.Instance.PlayOneShot(_collectSFX);

            ScoreEvent.Trigger(ScoreEventTypes.ScoreIncrease,Score);
			gameObject.SetActive(false);
        }

		public void OnEvent(RespawnEvent respawnEvent)
		{
			switch(respawnEvent.EventType)
			{
				case RespawnEventTypes.GameObjectRespawn:
					Respawn();
					break;
			}
		}

		public void OnEvent(CheckPointEvent checkPointEvent)
		{
			switch(checkPointEvent.EventType)
			{
				case CheckPointEventTypes.ChangeCheckPoint:
					ShouldRespawn = false;
					break;
			}
		}

		private void Respawn()
		{
			if(ShouldRespawn)
				gameObject.SetActive(true);
		}

		private void OnEnable()
		{
			this.EventStartListening<RespawnEvent>();
			this.EventStartListening<CheckPointEvent>();
		}
		private void OnDestroy()
		{
			this.EventStopListening<RespawnEvent>();
			this.EventStopListening<CheckPointEvent>();
		}
	}
}
