namespace SF.PhysicsLowLevel
{
    /// <summary>
    /// Useful Execution orders for low level physics.
    /// <remarks>
    /// This mimics closely the Unity low level physics 2D extra package made by Melvyn May to
    /// line up with any logic for those using that package as well.
    /// This are just included for those who don't want to install an extra package.
    /// </remarks>
    /// </summary>
    public static class LowLevelPhysicsExecutionOrder
    {
        public const int PhysicsWorld = -1000;
        public const int PhysicsBody = PhysicsWorld + 1;
        public const int PhysicsShape = PhysicsBody + 1;
        public const int PhysicsJoint = PhysicsBody + 1;
    }
}
