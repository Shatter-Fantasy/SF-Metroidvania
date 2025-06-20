using SF.Events;
using SF.InputModule;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace SF.DialogueModule
{
    /// <summary>
    /// The in scene manager that controls the way DialogueConversations are run and set up.
    /// </summary>
    public class DialogueManager : MonoBehaviour
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

        [field: SerializeField] public DialogueDatabase DialogueDB { get; private set; }
        private DialogueEntry _currentEntry;
        public static DialogueConversation RecentConversation
        {
            get => _instance._dialogueConversation;
            private set => _instance._dialogueConversation = value;
        }

        public static bool InConversation => RecentConversation != null;
        
        [SerializeField] private UIDocument _dialogueOverlayUXML;
        private VisualElement _dialogueView;
        private Label _dialogueLabel;
        private Label _speakerLabel;

        private void Awake()
        {
            if (_instance != null)
                Destroy(this);

            Instance = this;
        }

        private void Start()
        {
            _dialogueView = _dialogueOverlayUXML.rootVisualElement.Q<VisualElement>(name: "dialogue__view");
            _dialogueLabel = _dialogueOverlayUXML.rootVisualElement.Q<Label>(name: "overlay-dialogue__label");
            _speakerLabel = _dialogueOverlayUXML.rootVisualElement.Q<Label>(name: "dialogue-speaker__label");
        }
        
        public static void TriggerConversation(int guid)
        {
            if (_instance == null)
            {
                Debug.Log("There was no instance of the DialogueManager being set.");
                return;
            }

            // We were already in a conversation and are continuing it.
            if ( _instance._dialogueConversation?.GUID == guid)
            {

                RecentConversation.NextDialogueEntry(out _instance._currentEntry);
                
                _instance._dialogueLabel.text = _instance._currentEntry.Text;
                _instance._speakerLabel.text = _instance._currentEntry.SpeakerName;
                
                if (string.IsNullOrEmpty(_instance._dialogueLabel.text))
                {
                   StopConversation();
                }

                return;
            }
            
            // Just starting a new conversation.
            if(_instance.DialogueDB.GetConversation(guid, out DialogueConversation conversation))
            {
                _instance._dialogueConversation = conversation;
                RecentConversation.NextDialogueEntry(out _instance._currentEntry);
                OnDialogueOpen();
                DialogueEvent.Trigger(DialogueEventTypes.DialogueOpen, _instance._currentEntry.Text);
            }
            else
            {
                OnDialogueOpen();
                DialogueEvent.Trigger(DialogueEventTypes.DialogueOpen, $"No conversation for the Guid: {guid} was found. If you see this in game please report the guid value to the devs.");
            }
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
        
        private static void OnDialogueOpen()
        {
            _instance._dialogueView.style.visibility = Visibility.Visible;
            _instance._dialogueLabel.text = _instance._currentEntry.Text;
            _instance._speakerLabel.text = _instance._currentEntry.SpeakerName;
        }
        
        private void OnDialogueClose()
        {
            _dialogueView.style.visibility = Visibility.Hidden;
            _dialogueLabel.text = "";
            _speakerLabel.text = "";
            DialogueEvent.Trigger(DialogueEventTypes.DialogueClose,"");
        }
        
        private void OnEnable()
        {
            if(InputManager.Controls != null)
            {
                InputManager.Controls.UI.Talk.performed += OnTalk;
            }
        }

        private void OnDisable()
        {
            // Reset the static dialogue conversation value between play sessions.
            _instance._dialogueConversation = null;
            _instance._currentEntry = null;
            
            if(InputManager.Controls != null)
            {
                InputManager.Controls.UI.Talk.performed -= OnTalk;
            }
        }
        
        
        private void OnTalk(InputAction.CallbackContext ctx)
        {
            TriggerConversation(RecentConversation.GUID);
        }
    }
}
