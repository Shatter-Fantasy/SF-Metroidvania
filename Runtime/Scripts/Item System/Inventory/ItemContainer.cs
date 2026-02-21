using System.Collections.Generic;
using UnityEngine;

namespace SF.ItemModule
{
    using Managers;
    public class ItemContainer : MonoBehaviour
    {
        [SerializeReference]
        public List<ItemData> Items = new List<ItemData>();

        public virtual void AddItem(int itemID)
        {
            var item = GameLoader.Instance?.ItemDatabase[itemID];
            
            if (item != null)
            {
                Items.Add(item);
            }
        }
    }
}
