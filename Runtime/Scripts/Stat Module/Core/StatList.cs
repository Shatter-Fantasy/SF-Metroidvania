using System.Collections.Generic;

using SF.Inventory.StatModule;

namespace SF.StatModule
{

    [System.Serializable]
    public class StatList
    {
        public AttributesStats AttributesStats;

        /// <summary>
        /// This is the default stats for characters used in combat.
        /// Think physical damage, magical defence, and so forth.
        /// </summary>
        public CombatStats CombatStats;
        public EquipmentStats EquipmentStats;

        public ElementalStatList ElementalPower;
        public ElementalStatList ElementalResistances;
        public ElementalStatList ElementalAffinities;
    }

    public abstract class StatDataList<T> where T : StatData
    {
        public List<StatMediator<T>> StatMediators = new List<StatMediator<T>>();
    }

    [System.Serializable]
    public class AttributesStats: StatDataList<RPGAttributeStat>
    {

    }

    [System.Serializable]
    public class CombatStats : StatDataList<StatData>
    {

    }

    // For now StatDataList<StatData> works, but we might want a StatDataList<EquipmentData> type sooner or later.
    [System.Serializable]
    public class EquipmentStats : StatDataList<StatData>
    {

    }

    [System.Serializable]
    public class ElementalStatList : StatDataList<ElementalAttributeStat>
    {

    }
}
