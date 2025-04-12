using SF.Events;

namespace SF.InventoryModule
{
    public enum ItemEventTypes
    {
        PickUp,
        GiveItem // This can be called when giving NPC or item crafting stations an item to lose from the inventory.
    }
    public struct ItemEvent
    {
        public ItemEventTypes EventType;
        public int ItemId;
        public ItemEvent(ItemEventTypes eventType)
        {
            EventType = eventType;
            ItemId = 0;
        }
        static ItemEvent itemEvent;

        public static void Trigger(ItemEventTypes eventType, int itemID)
        {
            itemEvent.EventType = eventType;
            itemEvent.ItemId = itemID;
            EventManager.TriggerEvent(itemEvent);
        }
    }
}
