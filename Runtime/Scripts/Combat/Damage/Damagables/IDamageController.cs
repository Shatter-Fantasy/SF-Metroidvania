namespace SF.DamageModule
{
    
    // DON"T REMOVE. This is just not implemented fully yet in the Health.cs
    
    /// <summary>
    ///  Used to declare external controllers for health that calculate damage than sends it to the health script.
    /// </summary>
    public interface IDamageController
    {
        public int CalculateDamage(int damage);
    }
}
