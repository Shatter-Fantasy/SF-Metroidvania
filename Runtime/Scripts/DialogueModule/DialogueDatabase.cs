using System;
using System.Collections.Generic;
using UnityEngine;

namespace SF.DialogueModule
{
    [CreateAssetMenu(fileName = "Dialogue Database", menuName = "SF/Dialogue/Dialogue Database")]
    public class DialogueDatabase : ScriptableObject
    {
        public List<DialogueConversation> Conversations = new();
    }

    /// <summary>
    /// Data container for an entire conversation of a dialogue sequence.
    /// Including dialogue entries, actor data, and event details related to the dialogue.
    /// </summary>
    [Serializable]
    public class DialogueConversation
    {
        /// <summary>
        /// The unique identifier for a <see cref="DialogueConversation"/>.
        /// </summary>
        // Shh you don't see this stupid way to generate a unique int value.
        public int GUID = Guid.NewGuid().GetHashCode();
        public List<DialogueEntry> DialogueEntries = new();

        /// <summary>
        /// Index of the current active instance of the conversation. -1 if not the active conversation.
        /// </summary>
        private int _index = -1;
        
        /// <summary>
        /// Is this conversation currently going on.
        /// </summary>
        public bool InConversation => (_index) <= DialogueEntries.Count - 1 
                                      && _index > -1;
        
        /// <summary>
        /// Goes to the next step of the conversation and returns the next DialogueEntry's Text value.
        /// </summary>
        /// <returns></returns>
        public string NextDialogueEntry()
        {
            _index++;

            if (!InConversation)
            {
                _index = -1;
                return "";
            }

            // If this is past the first entry call the previous entries on end action before starting the next dialogue entry.
            if (_index > 0)
                DialogueEntries[_index - 1].EndAction?.Invoke();
            
            DialogueEntries[_index].StartAction?.Invoke();
            
            return DialogueEntries[_index].Text;
        }
    }

    /// <summary>
    /// The individual parts of a full dialogue conversations including events to activate once the dialogue happens, starts, or ends.
    /// </summary>
    [Serializable]
    public class DialogueEntry
    {
        /// <summary>
        /// The unique identifier for this one piece of Dialogue Entry.
        /// </summary>
        public int GUID = Guid.NewGuid().GetHashCode();
        public string Text;
        /// <summary>
        /// An action that can be invoked when the Dialogue Entry starts.
        /// </summary>
        public Action StartAction;
        /// <summary>
        /// An action that can be invoked when the Dialogue Entry ends.
        /// This finishes running before the next DialogueEntries StartAction is invoked.
        /// </summary>
        public Action EndAction;
    }

    /// <summary>
    /// A class to allow DialogueEntries can have an action that can be called if
    /// the user stays on the dialogue for a set amount of time.
    /// </summary>
    /// <remarks>
    /// This allows for having special dialogue that calls events if the player hesitates to take an action or make a choice.
    /// </remarks>
    public class TimedDialogueEntry : DialogueEntry
    {
        /// <summary>
        /// How long before the delayed action is activated. 
        /// </summary>
        public float DelayTimer = 1;

        /// <summary>
        /// The action to invoke when the <see cref="DelayTimer"/> has reached 0. 
        /// </summary>
        public Action DelayedAction;
    }
}
