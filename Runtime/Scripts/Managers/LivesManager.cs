using System;
using System.Collections.ObjectModel;
using SF.Events;
using UnityEngine;

namespace SF
{
    [System.Serializable]
    public class LivesManager : MonoBehaviour, EventListener<LivesEvent>
    { 
        public static int CurrentLives = 3;
        private int _currentLives 
        {
            get { return CurrentLives;  }
            set { CurrentLives = value; } 
        }
        public int MaxLives = 99;

        public void OnEvent(LivesEvent livesEvent)
        {
            switch(livesEvent.EventType)
            {
                case LivesEventTypes.IncreaseLives:
                    ChangeLives(livesEvent.Lives);
                    break;
                case LivesEventTypes.DecreaseLives:
                    ChangeLives(-livesEvent.Lives);
                    break;
            }
        }

        private void ChangeLives(int lives)
        {
            _currentLives = Mathf.Clamp(_currentLives + lives , 0, MaxLives);
            
            if(_currentLives == 0)
            {
                // Do a game over event.
            }
            LivesEvent.Trigger(LivesEventTypes.ChangedLives);
        }

        private void OnEnable()
        {
            this.EventStartListening<LivesEvent>();
        }

        public void OnDisable()
        {
            this.EventStopListening<LivesEvent>();
        }
    }
}
