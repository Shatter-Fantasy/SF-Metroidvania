using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace SF.Utilities
{
    /// <summary>
    /// This contains functions for converting Vectors to other vector types, pulling out Vector values, and making arrays of Vectors.
    /// </summary>
    public static class VectorExtensions
    {

        #region Vector 2 Extensions
        public static Vector3[] ToVector3Array(this Vector2[] vectors) =>
            vectors.Cast<Vector3>().ToArray();
        
        public static List<Vector3> ToVector3List(this Vector2[] vectors) =>
             vectors.Cast<Vector3>().ToList();

        /// <summary>
        /// Returns a Vector three with the x and y of the Vector2 passed in with another number to set the VectorZvalue.
        /// </summary>
        /// <param name="vector2"></param>
        /// <param name="zVector"></param>
        /// <returns></returns>
        public static Vector3 ToVector3(this Vector2 vector2, int zVector)
        {
            return new Vector3(vector2.x, vector2.y, zVector);
        }

        public static Vector2Int ToVector2Int(this Vector2 vector2)
        {
            return new Vector2Int((int)vector2.x, (int)vector2.y);
        }
        
        public static Vector2Int ToVector2Int(this Vector3Int vector3)
        {
            return new Vector2Int(vector3.x, vector3.y);
        }
        /// <summary>
        /// Allows choosing if the Vector2 is rounded down or rounded up  when converted to a Vector2Int.
        /// </summary>
        /// <param name="vector2"></param>
        /// <param name="roundedDown"></param>
        /// <returns></returns>
        public static Vector2Int ToVector2IntRounded(this Vector2 vector2, bool roundedDown = true)
        {
            if(roundedDown)
                return new Vector2Int(Mathf.FloorToInt(vector2.x), Mathf.FloorToInt(vector2.y));
            else
                return new Vector2Int(Mathf.CeilToInt(vector2.x), Mathf.CeilToInt(vector2.y));
        }
        #endregion

        #region Vector 3 Extensions
        public static Vector2[] ToVector2Array(this Vector3[] vectors)
        {
            return vectors.Cast<Vector2>().ToArray();
        }
        public static List<Vector2> ToVector2List(this Vector3[] vectors)
        {
            return vectors.Cast<Vector2>().ToList();
        }
        #endregion

        #region Vector Conversions
        public static (int x, int y, int z) SpreadToInts(this Vector3 vector3)
        {
            return ((int)vector3.x, (int)vector3.y, (int)vector3.z);
        }
        #endregion
    }
}
