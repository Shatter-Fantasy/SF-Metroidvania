using System.Collections.Generic;
using SF.Managers;
using UnityEngine;

namespace SF.ItemModule
{
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
