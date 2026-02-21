namespace SF.PhysicsLowLevel
{
    /// <summary>
    /// Keeps track of data and settings for custom SF Low Level Physics 2D
    /// API, classes, objects, and structs.
    /// </summary>
    public static class SFPhysicsManager
    {
        /// <summary>
        /// Should the SF Low Level Physics API spit out debug information for the consoles. 
        /// </summary>
        public static bool UsingDebugMode = true;

        /// <summary>
        /// Should the SF Low Level Physics use the <see cref=""/>. 
        /// </summary>
        public static bool UsingDebugRendering = true;

#region PhysicsMask Layers
        public const int PlatformsLayer = 0;
        public const int OneWayPlatformsLayer = 1;
        public const int MovingPlatformsLayer = 2;

        public const int IgnoreRaycastLayer = 6;
        public const int VolumesLayer = 7;
        public const int UILayer = 8;
        
        public const int PlayerLayer = 10;
        public const int EnemiesLayer = 11;
        public const int FriendliesLayer = 12;
        
        public const int WaterLayer = 14;
        public const int InteractableLayer = 17;
        public const int ProjectilesLayer = 18;
        public const int HitboxesLayer = 19;
#endregion
      
    }
}
