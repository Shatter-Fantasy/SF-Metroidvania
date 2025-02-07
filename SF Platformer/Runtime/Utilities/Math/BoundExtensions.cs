using UnityEngine;

namespace SF.Platformer.Utilities
{
#if !SF_Utilities
    /// <summary>
    /// A set of helper functions and extensions methods for Unity's Bounds structs of different types. Bounds and BoundsInt.
    /// </summary>
    public static class BoundExtensions
    {
        #region Bounds
        public static Vector2 TopRight(this Bounds bounds) => new(bounds.max.x, bounds.max.y);
        public static Vector2 TopCenter(this Bounds bounds) => new(bounds.center.x, bounds.max.y);
        public static Vector2 TopLeft(this Bounds bounds) => new(bounds.min.x, bounds.max.y);

        public static Vector2 BottomRight(this Bounds bounds) => new(bounds.max.x, bounds.min.y);
        public static Vector2 BottomCenter(this Bounds bounds) => new(bounds.center.x, bounds.min.y);
        public static Vector2 BottomLeft(this Bounds bounds) => new(bounds.min.x, bounds.min.y);

        public static Vector2 MiddleRight(this Bounds bounds) => new(bounds.max.x, bounds.center.y);
        public static Vector2 MiddleCenter(this Bounds bounds) => new(bounds.center.x, bounds.center.y);
        public static Vector2 MiddleLeft(this Bounds bounds) => new(bounds.min.x, bounds.center.y);
        #endregion

        #region BoundsInt
        public static Vector2 TopRight(this BoundsInt bounds) => new(bounds.max.x, bounds.max.y);
        public static Vector2 TopCenter(this BoundsInt bounds) => new(bounds.center.x, bounds.max.y);
        public static Vector2 TopLeft(this BoundsInt bounds) => new(bounds.min.x, bounds.max.y);

        public static Vector2 BottomRight(this BoundsInt bounds) => new(bounds.max.x, bounds.min.y);
        public static Vector2 BottomCenter(this BoundsInt bounds) => new(bounds.center.x, bounds.min.y);
        public static Vector2 BottomLeft(this BoundsInt bounds) => new(bounds.min.x, bounds.min.y);

        public static Vector2 MiddleRight(this BoundsInt bounds) => new(bounds.max.x, bounds.center.y);
        public static Vector2 MiddleCenter(this BoundsInt bounds) => new(bounds.center.x, bounds.center.y);
        public static Vector2 MiddleLeft(this BoundsInt bounds) => new(bounds.min.x, bounds.center.y);
        #endregion

        #region Bounds Conversion
        /// <summary>
        /// Converts a BoundInt to just a normal Bounds.
        /// </summary>
        /// <param name="boundsInt"></param>
        /// <returns></returns>
        public static Bounds ToBounds(this BoundsInt boundsInt) =>
            new(boundsInt.size, boundsInt.center);
        /// <summary>
        /// Converts a Bounds  to a BoundsInt
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public static BoundsInt ToBoundsInt(this Bounds bounds) =>
            new(bounds.GetBoundsMinInt(), bounds.GetBoundsSizeInt());
        /// <summary>
        /// Gets the min of a Bounds than converts the values to a intgear from a floating point to be put into a returned Vector3Int. 
        /// This is heavily used in things using Rect, Sprite Borders, and TileMaps with the grid functions.
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public static Vector3Int GetBoundsMinInt(this Bounds bounds) =>
            new((int)bounds.min.x, (int)bounds.min.y, (int)bounds.min.z);
        /// <summary>
        /// Gets the size of a normal Bounds while converting the sizes to be integears. Than returns a Vector3Int with the converted size values.
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public static Vector3Int GetBoundsSizeInt(this Bounds bounds) =>
            new((int)bounds.size.x, (int)bounds.size.y, (int)bounds.size.z);
        /// <summary>
        /// A decontructor that allows users to out the three values of a Vector3Int into three seperate int values.
        /// </summary>
        /// <param name="vector3Int"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public static void Deconstruct(this Vector3Int vector3Int, out int x, out int y, out int z)
        {
            x = vector3Int.x;
            y = vector3Int.y;
            z = vector3Int.z;
        }
        #endregion
    }
#endif
}
