using TMPro;
using UnityEngine;

using SF.Events;

namespace SF
{
    public class PlayerHUD : MonoBehaviour, EventListener<LivesEvent>
    {
        public TMP_Text LivesText;

        public void OnEvent(LivesEvent livesEvent)
        {
            if (LivesText == null || livesEvent.EventType != LivesEventTypes.ChangedLives)
                return;

            LivesText.text = $"Lives: {LivesManager.CurrentLives}";
        }

        private void OnEnable()
        {
            this.EventStartListening<LivesEvent>();
        }

        private void OnDisable()
        {
            this.EventStopListening<LivesEvent>();
        }

        
    }
}
