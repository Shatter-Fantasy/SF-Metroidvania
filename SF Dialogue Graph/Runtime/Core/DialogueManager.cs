using System;
using System.Collections.Generic;
using System.Linq;
using SF.DialogueModule.Nodes;
using UnityEngine;
using UnityEngine.InputSystem;

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
        
        private static DialogueEntry  _currentEntry;
        public static DialogueConversation RecentConversation
        {
            get => _instance._dialogueConversation;
            private set => _instance._dialogueConversation = value;
        }

        public static bool InConversation => RecentConversation != null;
        
        /// <summary>
        /// This is the runtime graph for conversations nodes created from the Dialogue Graph Importer.
        /// </summary>
        public DialogueRuntimeGraph RuntimeGraph;
        
        [NonSerialized] public List<DialogueExtensionBase> Extensions = new(); 
        
        #region Dialogue Events
        public static event Action DialogueStartedHandler;  
        public static event Action DialogueEndedHandler;
        /// <summary>
        /// Invoked when the dialogue changes the current <see cref="DialogueEntry"/> to the next conversation node.
        /// </summary>
        public static event Action<DialogueEntry> DialogueTextChangedHandler;  
        #endregion
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;

            Extensions = GetComponents<DialogueExtensionBase>().ToList();
        }

        public static void TriggerConversation(DialogueConversation conversation, Component callingComponent = null)
        {
            if(conversation != null)
                TriggerConversation(conversation.GUID,callingComponent);
        }
        
        public static void TriggerConversation(int guid, Component callingComponent = null)
        {
            if (_instance == null)
            {
                Debug.Log("There was no instance of the DialogueManager being set.");
                return;
            }
            if (_instance.DialogueDB == null)
            {
#if  UNITY_EDITOR
            Debug.LogWarning("There was no Dialogue Database assigned in the DialogueManager.");
#endif           
                return;
            }

            // Are we in a conversation?
            if (_instance._dialogueConversation?.GUID != guid)
            {
                // If not check if the passed in conversation id is valid.
                // Than start a new conversation and open the dialogue panel if it.
                if(_instance.DialogueDB.GetConversation(guid, out _instance._dialogueConversation))
                {
                    _instance.StartConversation();
                    // Let other objects know a dialogue is starting - DialogueUIManager uses this to know to open the dialogue UI.
                    DialogueStartedHandler?.Invoke();
                }
                else
                {
                    Debug.LogError($"The {_instance.DialogueDB} that is set doesn't have a conversation matching the passed in ID of {guid}");
                    return;
                }
            }

            // This works perfectly as intended.
            _instance.RuntimeGraph = new DialogueRuntimeGraph(_instance._dialogueConversation);
            
            _instance.RuntimeGraph?.ProcessExtensions(callingComponent);
            _instance.RuntimeGraph?.GetCurrentBranch();
            _ = _instance.RuntimeGraph?.ProcessNodeBranch();
        }

        public static void UpdateDialogueText(DialogueEntry dialogueEntry)
        {
            _currentEntry = dialogueEntry;
            // Update the Dialogue Text and invoke the event to let objects know it has been updated.
            DialogueTextChangedHandler?.Invoke(_currentEntry);
        }
        
        private void AdvanceConversation(InputAction.CallbackContext ctx)
        {
            if (!InConversation)
                return;

            // We want to make sure we are working on the Runtime graph of the instanced cloned conversation.
            _instance.RuntimeGraph.IsPaused = false;
            
            //if (string.IsNullOrEmpty(_currentEntry.Text))
            //{
            //    StopConversation();
            //}
        }

        public virtual void StartConversation()
        {

        }
        public static void StopConversation()
        {
            // If there is no conversation going on, no need to try and stop any.
            if (_instance == null 
                || RecentConversation == null)
                return;

            DialogueEndedHandler?.Invoke();
            RecentConversation = null;
        } 
        
        
        private void OnDestroy()
        {
            // Reset the static dialogue conversation value between play sessions.
            _instance._dialogueConversation = null;
            _currentEntry = null;
        }
    }
}
