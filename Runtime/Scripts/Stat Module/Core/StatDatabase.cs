using System.Collections.Generic;
using UnityEngine;
using SF.Inventory.StatModule;

namespace SF.StatModule
{
    /// <summary>
    /// The default set stat for different type of objects that will be using any stat sets.
    /// </summary>
    /// <remarks>
    /// Each list of StatMediators can be used to show the default set of stats each object can 
    /// use with the stat names, default values, and stat descriptions.
    /// 
    /// Example 1. All characters have the same set of attributes.
    /// Strength, agility, vitility, and so forth.
    /// </remarks>
    [CreateAssetMenu(fileName = "Stat System Database", menuName = "SF/StatMediators/Stat Database")]
    public partial class StatSetDatabase : ScriptableObject
    {
        // Important Information. This class is partial to allow for people to add their own types of Stat Sets
        // It also let's them add their own type casting overloads properties.

        public StatList DefaultStatTypes;

        [Space(10)]
        public List<StatMediator<PricingStat>> PriceStats = new List<StatMediator<PricingStat>>();
    }

}
