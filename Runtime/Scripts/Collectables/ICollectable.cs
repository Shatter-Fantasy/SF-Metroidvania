namespace SF.CollectableModule
{
    public interface ICollectable<in T>
    {
        public void Collect(T tValue);
    }
    public interface ICollectable
    {
        public void Collect();
    }
}
