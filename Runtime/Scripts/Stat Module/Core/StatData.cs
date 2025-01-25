namespace SF.StatModule
{
    [System.Serializable]
    public class StatData
    {
        public float BaseValue = 10;

        public StatData(float baseValue = 0)
        {
            BaseValue = baseValue;
        }
    }
}
