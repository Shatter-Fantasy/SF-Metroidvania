using System.Reflection;
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
        
        [SerializeField] private DialogueConversation _dialogueConversation;
        [SerializeField] private DialogueDatabase _dialogueDB;
        private static DialogueConversation _recentConversation => _instance._dialogueConversation;
        
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
            if(_instance._dialogueDB.GetConversation(guid, out DialogueConversation conversation))
            {
                _instance._dialogueConversation = conversation;
                DialogueEvent.Trigger(DialogueEventTypes.DialogueOpen, _recentConversation.NextDialogueEntry());
            }
            else
                DialogueEvent.Trigger(DialogueEventTypes.DialogueOpen,"No conversation for the Guid was found.");
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
            this.EventStopListening<DialogueEvent>();
        }
    }
}
