namespace SF.Characters
{
    /// <summary>
    /// Declares which type of combatant a character is.
    /// </summary>
    public enum CharacterCombatantTypes
    {
        Player = 0,
        Enemy = 1,
        Ally = 2, // used for summoning, tag along characters, and ect.
        NonCombatant = 3, // Think things like quest NPCs, random talking ones, and so forth.
    }

    /// <summary>
    /// What kind of quest type is an NPC apart of. 
    /// </summary>
    public enum NPCQuestTypes
    {
        None = 0,
        MainQuestNPC = 1,
        SideQuestNPC = 2,
    }
}
