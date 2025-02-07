namespace SF.Physics
{
    /// <summary>
    /// Used to allow choosing when certain event or callbacks are invoked.
    /// <see cref="ZoneKill"/> for an example of it's implementation.
    /// </summary>
    public enum CollisionEventTypes
    {
        OnTriggerEnter2D,
        OnTriggerExit2D,
    }
}
