namespace SF.ItemModule
{
    /// <summary>
    /// The category type of the item.
    /// </summary>
    public enum ItemSubType
    {
        Consumable,
        Key,
        Equipment,
        None // This is used for when filtering items in different places.
    }
    
    /// <summary>
    /// IMPORTANT: This will be removed. Do not use this from now on.
    ///  Use <see cref="ItemSubType"/> from now on.
    /// </summary>
    [System.Obsolete]
    public enum ItemType
    {
        Consumable,
        Weapon,
        Armor,
        Key
    }
}
