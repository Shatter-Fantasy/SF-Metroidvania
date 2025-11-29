using System.Collections.Generic;
using UnityEngine;

using SF.Inventory;
using SF.DataModule;
using SF.ItemModule;

namespace SF.LootModule
{
    [CreateAssetMenu(fileName = "Loot Table Data", menuName = "SF/Loot/Loot Table Data")]
    public class LootTableData : DTOAssetBase
    {
        public List<ItemDTO> LootTable = new List<ItemDTO>();
        
        /// <summary>
        /// The drop chance in percent.
        /// </summary>
        public float DropChance = 0.5f; 
        public void DropRandomLoot(Vector2 dropPosition)
        {
            if (LootTable.Count < 1 
                || Random.Range(0f,1f) < DropChance)
                return;

            int lootIndex = Random.Range(0, LootTable.Count);  
            
            var item = LootTable[lootIndex];

            if (item == null)
                return;
            
            // Default object that allows for not setting the prefab on all scriptable data assets. 
            if (item.Prefab == null)
            {
                GameObject spawnedItem = new GameObject(
                    item.Name,
                    typeof(SpriteRenderer),
                    typeof(PickupItem)
                )
                {
                    transform = { position = dropPosition }
                };

                spawnedItem.GetComponent<SpriteRenderer>().sprite = item.ItemIcon;
                spawnedItem.GetComponent<PickupItem>().Item = item;
                
                // We add the box collider after setting the sprite to make sure the size of the collider is set properly.
                var col = spawnedItem.AddComponent<BoxCollider2D>();
                col.isTrigger = true;
                
            }
            else
            {
                GameObject spawnedItem = Instantiate(
                    item.Prefab,
                    dropPosition,
                    Quaternion.identity
                );
                
                spawnedItem.name = item.Name;
                spawnedItem.GetComponent<PickupItem>().Item = item;
                spawnedItem.GetComponent<SpriteRenderer>().sprite = item.ItemIcon;
            }
        }
    }
}
