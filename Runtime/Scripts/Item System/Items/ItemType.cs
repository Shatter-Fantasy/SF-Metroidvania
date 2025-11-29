namespace SF.ItemModule
{
    /// <summary>
    /// The category type of the item.
    /// </summary>
    public enum ItemSubType : int
    {
        Consumable = 0,
        Key = 1,
        Equipment = 2,
        Material = 4, // Used in crafting and enchanting mainly.
        SideQuest = 8, // Used in side quests only.
        MainQuest = 16,
    }
}
