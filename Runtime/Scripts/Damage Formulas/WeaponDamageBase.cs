using SF.StatModule;

namespace SF.DamageModule
{
    public class WeaponDamageBase : IDamageController
    {
        protected CharacterStats _characterStats;

        public int CalculateDamage(int damage)
        {
            return damage;
        }
    }
}
