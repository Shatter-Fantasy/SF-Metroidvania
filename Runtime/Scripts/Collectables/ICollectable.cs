namespace SF.CollectableModule
{
    public interface ICollectable<T>
    {
        public void Collect(T tValue);
    }
    public interface ICollectable
    {
        public void Collect();
    }
}
