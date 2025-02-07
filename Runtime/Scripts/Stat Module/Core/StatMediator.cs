using System.Collections.Generic;
using UnityEngine;

using SF.Inventory.StatModule;

namespace SF.StatModule
{
    public enum StatValueRoundingType
    {
        None,
        RoundDown,
        RoundUp,
        Truncate // This removes the decimal spots. This makes positive values round down and negative values round up.
    }

    /// <summary>
    /// The source of truth for a stat on any object, character, and ect.
    /// This contains the modifiers values, modifier types (add, subtract, multiply, ect.), and the base value
    /// for a StatData.
    /// </summary>
    [System.Serializable]
    public class StatMediator<TStatData> where TStatData : StatData
    {
        public string Name = "Stat";
        public string StatDescription = "This is a stat.";
        public TStatData Stat;
        public StatValueRoundingType StatValueRounding;
        
        // Need a blank constructor so it won't have a compile error saying we need to pass an argument in.
        public StatMediator()
        {

        }

        public StatMediator(ElementalType elementalType, string statName)
        {
            if(Stat is ElementalAttributeStat elementalStat )
                elementalStat.ElementalType = elementalType;
            Name = statName;
        }

        public float CalculatedStatValue
        {
            get 
            {
                // If the Percent value is zero don't use it for calculations or you the stat will just be
                // the FlatModifiersCalculatedValue due to first half of the formula being times by zero.
                if(PercentModifiersCalculatedValue != 0)
                {
                    return (Stat.BaseValue + BaseModifiersCalculatedValue) * PercentModifiersCalculatedValue + FlatModifiersCalculatedValue;
                }
                else
                    return Stat.BaseValue + BaseModifiersCalculatedValue + FlatModifiersCalculatedValue;
            }
        }
        public float BaseStatValue => Stat.BaseValue;
        public float BaseStatModifiedValue => Stat.BaseValue + BaseModifiersCalculatedValue;

        public List<StatModifier> BaseModifiers = new();
        /// <summary>
        /// The calculated final value of all the base modifier values to be used in the final stat calculation formula.
        /// </summary>
        public float BaseModifiersCalculatedValue
        {
            get 
            {
                float modifierValue = 0;

                if(BaseModifiers != null && BaseModifiers.Count > 0)
                {
                    for(int i = 0; i < BaseModifiers.Count; i++)
                    {
                        modifierValue += BaseModifiers[i].ModifierAmount;
                    }
                }

                return modifierValue;
            }
        }

        public List<StatModifier> PercentModifiers = new();
        /// <summary>
        /// The calculated final value of all the percent modifier values to be used in the final stat calculation formula.
        /// </summary>
        public float PercentModifiersCalculatedValue
        {
            get
            {
                float modifierValue = 0;

                if(PercentModifiers != null && PercentModifiers.Count > 0)
                {
                    for(int i = 0; i < PercentModifiers.Count; i++)
                    {
                        modifierValue += PercentModifiers[i].ModifierAmount;
                    }
                }

                return modifierValue;
            }
        }

        public List<StatModifier> FlatModifiers = new();
        /// <summary>
        /// The calculated final value of all the percent modifier values to be used in the final stat calculation formula.
        /// </summary>
        public float FlatModifiersCalculatedValue
        {
            get
            {
                float modifierValue = 0;

                if(FlatModifiers != null && FlatModifiers.Count > 0)
                {
                    for(int i = 0; i < FlatModifiers.Count; i++)
                    {
                        modifierValue += FlatModifiers[i].ModifierAmount;
                    }
                }
                
                return StatRounding(modifierValue);
            }
        }

        private float StatRounding(float statValue) => StatValueRounding switch
        {
            StatValueRoundingType.None => statValue,
            StatValueRoundingType.RoundDown => Mathf.Floor(statValue),
            StatValueRoundingType.RoundUp => Mathf.Ceil(statValue),
            // Notes for Truncating: In C# casting a float/Double to an int just drops all zeros.
            // Due note this is not a common practice for most coding languages. In Java for example they actually just floor doubles when going to int.
            StatValueRoundingType.Truncate => (int)statValue,
            _ => statValue
        };
    }

    /// <summary>
    /// RPG Attributes are special stat types that can also act like a stat modifier. 
    /// See the Remarks tag in the class file for examples.
    /// </summary>
    /// <remarks>
    /// 
    /// Example 1. Attributes effecting other RPGStats and Spells/Skill Actions
    /// You have a RPGAttributeStat called strength. Your strength attribute increases your physical attack stat.
    /// The strength stat can also be used for chances of applying debuffs when using skills. 
    /// Let's say you have a skill that has a base 10% to make the enemy bleed, 
    /// but ever 10 strength increases the chnace by 7% past a base amount of strength of 50.
    /// So your strength stat is 60 which is ten over the base amount of 50 strength adding another 7% chance to bleed.
    /// 
    /// Example 2. Attributes that can effect stats that are not of the C# type StatData
    /// You could have a Charisma RPGAttributeStat that changes the stat modifiers for NPC reputations, shop prices, chances to have dialogue different option outcomes.
    /// Taking this farther you can have it be like Fallout New Vegas where certain attributes change your chances of failing/succeeding different actions.
    /// Intelligence for hacking computers, medical related stuff, and knowledge checks.
    /// Perception can be used as a modifer for noticing invisible enemies or someone sneaking from behind.
    /// </remarks>
    [System.Serializable]
    public class RPGAttributeStat : StatData
    {
      
    }
}
