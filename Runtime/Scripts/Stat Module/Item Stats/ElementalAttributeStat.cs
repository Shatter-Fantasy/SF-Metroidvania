using System;
using UnityEngine;
using SF.StatModule;

namespace SF.Inventory.StatModule
{
    [Flags]
    public enum ElementalType : uint
    {
        None = 0,
        Fire = 1,
        Ice = 2,
        Water = 4,
        Lightning = 8,
        Light = 16,
        Dark = 32,
        Wind = 64,
        Earth = 128,
        // We use combination values to set the fusion of elemental types.
        Plasma = 9, // Fire + Lightning
        Lava = 129, // Earth + Fire.
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
