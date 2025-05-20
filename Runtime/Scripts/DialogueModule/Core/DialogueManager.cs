using SF.Events;
using UnityEngine;
using UnityEngine.UIElements;

namespace SF.DialogueModule
{
    /// <summary>
    /// The in scene manager that controls the way DialogueConversations are run and set up.
    /// </summary>
    public class DialogueManager : MonoBehaviour, EventListener<DialogueEvent>
    {
        private static DialogueManager _instance;
        public static DialogueManager Instance
        {
            get => _instance;
            set
            {
                if (value == null)
                    return;
                
                _instance = value;
            }
        }
        
        /// <summary>
        /// The current conversation going on.
        /// </summary>
        [SerializeField] private DialogueConversation _dialogueConversation;
        [SerializeField] private DialogueDatabase _dialogueDB;

        public static DialogueConversation RecentConversation
        {
            get => _instance._dialogueConversation;
            private set => _instance._dialogueConversation = value;
        }

        public static bool InConversation => RecentConversation != null;
        
        [SerializeField] private UIDocument _dialogueOverlayUXML;
        private VisualElement _dialogueContainer;
        private Label _dialogueLabel;

        private void Awake()
        {
            if (_instance != null)
                Destroy(this);

            Instance = this;
        }

        private void Start()
        {
            _dialogueContainer = _dialogueOverlayUXML.rootVisualElement.Q<VisualElement>(name: "overlay-dialogue__container");
            _dialogueLabel = _dialogueOverlayUXML.rootVisualElement.Q<Label>(name: "overlay-dialogue__label");
        }
        
        public static void TriggerConversation(int guid)
        {
            if (_instance == null)
            {
                Debug.Log("There was no instance of the DialogueManager being set.");
                return;
            }

            if ( _instance._dialogueConversation?.GUID == guid)
            {
                
                // Bad little man need to improve this so we don't have a shit ton of string garbage collection.
                _instance._dialogueLabel.text = RecentConversation.NextDialogueEntry();
                if (string.IsNullOrEmpty(_instance._dialogueLabel.text))
                {
                   StopConversation();
                }

                return;
            }
            
            if(_instance._dialogueDB.GetConversation(guid, out DialogueConversation conversation))
            {
                _instance._dialogueConversation = conversation;
                DialogueEvent.Trigger(DialogueEventTypes.DialogueOpen, RecentConversation.NextDialogueEntry());
            }
            else
                DialogueEvent.Trigger(DialogueEventTypes.DialogueOpen,"No conversation for the Guid was found.");
        }

        public static void StopConversation()
        {
            if (_instance == null)
                return;
            
            // If there is no conversation going on, no need to try and stop any.
            if (RecentConversation == null)
                return;

            RecentConversation = null;
            _instance.OnDialogueClose();
        }
        
        private void OnDialogueOpen(string dialogue)
        {
            _dialogueContainer.style.visibility = Visibility.Visible;
            _dialogueLabel.text = dialogue;
        }
        
        private void OnDialogueClose()
        {
            _dialogueContainer.style.visibility = Visibility.Hidden;
            _dialogueLabel.text = "";
            DialogueEvent.Trigger(DialogueEventTypes.DialogueClose,"");
        }
        
        public void OnEvent(DialogueEvent dialogueEvent)
        {
            switch (dialogueEvent.EventType)
            {
                case DialogueEventTypes.DialogueOpen:
                {
                    if (string.IsNullOrEmpty(dialogueEvent.Dialogue))
                    {
                        OnDialogueClose();
                        break;
                    }

                    OnDialogueOpen(dialogueEvent.Dialogue);
                    break;
                }
            }
        }
        
        private void OnEnable()
        {
            this.EventStartListening<DialogueEvent>();
        }
        private void OnDisable()
        {
            // Reset the static dialogue conversation value between play sessions.
            _instance._dialogueConversation = null;
            this.EventStopListening<DialogueEvent>();
        }
    }
}
