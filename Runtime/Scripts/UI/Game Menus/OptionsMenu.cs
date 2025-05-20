using UnityEngine;

using SF.Events;

namespace SF.UI
{
    public class OptionsMenu : MonoBehaviour, EventListener<GameMenuEvent>
    {

        private Canvas _optionsCanvas;

        private void Awake()
        {
            _optionsCanvas = GetComponent<Canvas>();
        }

        private void OnOpenOptionsMenu()
        {
            // For now just toggling these two
            _optionsCanvas.enabled = !_optionsCanvas.enabled;
            // _optionsCanvas.enabled = true;
        }

        private void OnCloseOptionsMenu()
        {
            _optionsCanvas.enabled = false;
        }

        public void OnEvent(GameMenuEvent eventType)
        {
            switch(eventType.EventType)
            {
                case GameMenuEventTypes.OpenGameMenu:
                    break;
                case GameMenuEventTypes.CloseGameMenu:
                    break;
                case GameMenuEventTypes.OpenOptionsMenu:

                    OnOpenOptionsMenu();
                    break;
                case GameMenuEventTypes.CloseOptionsMenu:
                    OnCloseOptionsMenu();
                    break;
                default:
                    break;
            }
        }

        private void OnEnable()
        {
            this.EventStartListening<GameMenuEvent>();
        }

        private void OnDisable()
        {
            this.EventStopListening<GameMenuEvent>();
        }
    }
}
