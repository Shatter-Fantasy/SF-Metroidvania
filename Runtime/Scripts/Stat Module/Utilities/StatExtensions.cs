using System.Collections.Generic;

using SF.Inventory.StatModule;

namespace SF.StatModule
{
    public static class StatExtensions
    {
        /// <summary>
        /// Adds a new modifier to a StatMediator than returns the StatMediator allowing for method chaining.
        /// </summary>
        /// <param name="mediator"></param>
        /// <param name="modifier"></param>
        /// <returns></returns>
        public static StatMediator<TStatData> AddStatModifier<TStatData>(this StatMediator<TStatData> mediator, StatModifier modifier)
            where TStatData : StatData
        {
            // This should never happen, but better to be safe than sorry.
            if(mediator == null)
                mediator = new StatMediator<TStatData>();

            if(modifier.ModifierAmount == 0)
                return mediator;

            if(modifier.ModifierType == StatModifierType.Base)
                mediator.BaseModifiers.Add(modifier);
            else if(modifier.ModifierType == StatModifierType.Percent)
                mediator.PercentModifiers.Add(modifier);
            else // modifier.ModifierType == StatModifierType.Flat
                mediator.FlatModifiers.Add(modifier);

            return mediator;
        }

        public static List<StatMediator<ElementalAttributeStat>> SetDefaultElementalStats(this List<StatMediator<ElementalAttributeStat>> elementalStats)
        {
            if(elementalStats == null)
                elementalStats = new();

            elementalStats.Clear();

            elementalStats.Add(new StatMediator<ElementalAttributeStat>(ElementalType.Fire, "Fire"));
            elementalStats.Add(new StatMediator<ElementalAttributeStat>(ElementalType.Ice, "Ice"));
            elementalStats.Add(new StatMediator<ElementalAttributeStat>(ElementalType.Lightning, "Lighting"));
            elementalStats.Add(new StatMediator<ElementalAttributeStat>(ElementalType.Water, "Water"));
            elementalStats.Add(new StatMediator<ElementalAttributeStat>(ElementalType.Wind, "Wind"));
            elementalStats.Add(new StatMediator<ElementalAttributeStat>(ElementalType.Earth, "Earth"));
            elementalStats.Add(new StatMediator<ElementalAttributeStat>(ElementalType.Light, "Light"));
            elementalStats.Add(new StatMediator<ElementalAttributeStat>(ElementalType.Dark, "Dark"));
            return elementalStats;
        }
    }
}
