using System.Collections.Generic;
using UnityEngine;

namespace SF.DialogueModule
{
    using SF.DataModule;
    [CreateAssetMenu(fileName = "Dialogue Database", menuName = "SF/Dialogue/Dialogue Database")]
    public class DialogueDatabase : SFAssetDatabase<DialogueConversation>
    {
        public bool GetConversation(int guid, out DialogueConversation conversation)
        {
            conversation = DataEntries.Find(x => x.GUID == guid);

            return conversation != null;
        }

        public override void AddData(DialogueConversation dataEntry)
        {
            base.AddData(dataEntry);
        }

        public override void OnRegisterDatabase()
        {   
            // Not implemented Yet
        }

        public override void OnDeregisterDatabase()
        {
            // Not implemented Yet
        }
    }
}
