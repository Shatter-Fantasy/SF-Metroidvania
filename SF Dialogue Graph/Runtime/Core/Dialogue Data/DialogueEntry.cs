using System;
using SF.DialogueModule.Nodes;
using UnityEngine;

namespace SF.DialogueModule
{
    /// <summary>
    /// The individual parts of a full dialogue conversations including events to activate once the dialogue happens, starts, or ends.
    /// </summary>
    [Serializable]
    public class DialogueEntry
    {
        public string Text;
        /// <summary>
        /// The name of the current speaker.
        /// </summary>
        public string SpeakerName;
        /// <summary>
        /// An action that can be invoked when the Dialogue Entry starts.
        /// </summary>
        public Action StartAction;
        /// <summary>
        /// An action that can be invoked when the Dialogue Entry ends.
        /// This finishes running before the next DialogueEntries StartAction is invoked.
        /// </summary>
        public Action EndAction;

        public DialogueEntry() { }

        public DialogueEntry(string text, string speakerName = "")
        {
            Text = text;
            SpeakerName = speakerName;
        }
        
        public DialogueEntry(in ConversationEntryRuntimeNode entryNode)
        {
            Text = entryNode.Text;
            SpeakerName = entryNode.SpeakerName;
        }
        // Used for comparisons to see if a DialogueEntry is null.
        public static readonly DialogueEntry EmptyEntry = new()
        {
            Text = "",
            SpeakerName = ""
        };
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
