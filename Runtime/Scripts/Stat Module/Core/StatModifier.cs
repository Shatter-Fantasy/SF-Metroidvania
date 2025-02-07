namespace SF.StatModule
{
    /// <summary>
    /// This is the enum class for all modifier types. This is not just for RPG StatMediators.
    /// Think modifiers that can be used for stats, relationship bonuses, shop prices, and reputation.
    /// </summary>
    public enum StatModifierType
    {
        // Modifier formula if the Final Percent Value is not 0. When it is 0 don't use it or
        // you will get 0 as the final value due to anything times 0 is 0.
        // (BaseStatValue + Base Modifiers Final BaseValue) * Final Percent Value + Fina Flat BaseValue

        Base, // The base modifier types are calculated before the multiply and divide so they do a lot more.
        Flat, // A flat amount added or subtractice after the percent modifers are calculated.
        Percent // A percentage to multiply or subtract the base amount by before calculating the flat modifiers.
    }

    [System.Serializable]
    public class StatModifier
    {
        public float ModifierAmount;
        public StatModifierType ModifierType;
    }
}
