namespace SF.StatModule
{
    [System.Serializable]
    public class StatData
    {
        public float BaseValue;

        public StatData(float baseValue = 0)
        {
            BaseValue = baseValue;
        }
    }
}
