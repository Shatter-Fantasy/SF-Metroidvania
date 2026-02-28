using System.Collections.Generic;
using UnityEngine;

namespace SF.DialogueModule
{
    [CreateAssetMenu(fileName = "Dialogue Database", menuName = "SF/Dialogue/Dialogue Database")]
    public class DialogueDatabase : ScriptableObject
    {
        public List<DialogueConversation> Conversations = new();
        
        public bool GetConversation(int guid, out DialogueConversation conversation)
        {
            conversation = Conversations.Find(x => x.GUID == guid);

            return conversation != null;
        }
    }
}
