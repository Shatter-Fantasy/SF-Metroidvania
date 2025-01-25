using UnityEngine;
using SF.StatModule;

namespace SF.Inventory.StatModule
{
    public enum ElementalType
    {
        Fire,
        Ice,
        Water,
        Lightning,
        Light,
        Dark,
        Wind,
        Earth
    }
    /// <summary>
    /// Elemntal types are used in elemntal resistances, damage modifiers, and elemental affinity bonuses.
    /// </summary>
    /// <remarks>
    /// Example 1. Elemental affinities.
    /// You can have a game where characters with certain elemental affinities will have different out comes compared to others.
    /// Imagine a volcano level where characters can try and make a pact for a new lava summoning.
    /// Characters with higher Fire/Earth affinities will have an easier chance to make the pact.
    /// </remarks>
    [System.Serializable]
    public class ElementalAttributeStat : StatData
    {
        public ElementalType ElementalType;

        public ElementalAttributeStat(ElementalType elementalType)
        {
            ElementalType = elementalType;
        }
    }
}
