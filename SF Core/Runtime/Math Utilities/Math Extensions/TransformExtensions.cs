using UnityEngine;
using UnityEngine.Internal;

namespace  SF.Utilities.MathUtilities
{
    public static class TransformExtensions
    {

       /// <summary>
       /// Rotates the Transform of the calling transform to look at the view targeted object and align its rotation up with it.
       /// Both passed in values should be in the same space (world/local). If one is world space then the other should be as well.
       /// </summary>
       /// <param name="transform"></param>
       /// <param name="viewTargetTransform"></param>
       /// <param name="upDirection"></param>
       /// <returns></returns>
        public static void RotateToTargetView(this Transform transform, Transform viewTargetTransform,[DefaultValue("Vector.up")] Vector3 upDirection)
        {
            Vector3 relativePos = viewTargetTransform.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(relativePos,upDirection);
            transform.rotation = rotation;
        }
       
        /// <summary>
        /// Rotates the Transform of the calling transform to look at the view targeted object and align its rotation up with it.
        /// Both passed in values should be in the same space (world/local). If one is world space then the other should be as well.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="viewTargetPosition"></param>
        /// <param name="upDirection"></param>
        /// <returns></returns>
        public static void RotateToTargetView(this Transform transform, Vector3 viewTargetPosition,[DefaultValue("Vector.up")] Vector3 upDirection)
        {
            Vector3 relativePos = viewTargetPosition - transform.position;
            Quaternion rotation = Quaternion.LookRotation(relativePos,upDirection);
            transform.rotation = rotation;
        }
    }
}