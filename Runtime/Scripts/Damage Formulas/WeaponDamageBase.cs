using SF.StatModule;

namespace SF.DamageModule
{
    /// <summary>
    /// Used to calculate the damage a weapon should do to anything it is applying damage to.
    /// </summary>
    public class WeaponDamageBase : IDamageController
    {
        protected CharacterStats _characterStats;

        public int CalculateDamage(int damage)
        {
            return damage;
        }
    }
}
