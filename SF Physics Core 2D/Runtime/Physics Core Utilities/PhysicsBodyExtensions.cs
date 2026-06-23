using Unity.Burst;
using UnityEngine;
using Unity.U2D.Physics;

namespace SF.U2D.Physics
{
    [BurstCompile]
    public static class PhysicsBodyExtensions
    {
        public static bool TryGetCallbackComponent<T>(in this PhysicsBody body, out T component, bool checkShapeValidation = false) where T : Component
        {
            component = null;
            
            // Optional check for only using Component set as a callbackTarget for valid PhysicsBody.
            if (checkShapeValidation && !body.isValid)
                return false;

            if (body.callbackTarget is not T callbackTarget) 
                return false;
            
            component = callbackTarget;
            return true;
        }
        
        public static bool TryGetCallbackShapeComponent<T>(in this PhysicsBody body,out T component, bool checkShapeValidation = false) where T : SFShapeComponent
        {
            component = null;
            
            // Optional check for only using Component set as a callbackTarget for valid PhysicsBody.
            if (checkShapeValidation && !body.isValid)
                return false;

            if (body.callbackTarget is not T callbackTarget) 
                return false;
            
            component = callbackTarget;
            return true;
        }


#region Direction Extensions
        [BurstCompile]
        public static void GetNormalizedDirectionTo(in this PhysicsBody fromBody, in PhysicsBody toBody, ref Vector2 direction)
        {
            if (!fromBody.isValid || !toBody.isValid)
                return;
            direction = (fromBody.position - toBody.position).normalized;
        }
        
        /// <summary>
        /// Get the normalzied direction between using the <see cref="fromBody"/> as the point of reference and the <see cref="toShape"/>
        /// position in world space as the target heading to calculate to. 
        /// </summary>
        /// <param name="fromBody"></param>
        /// <param name="toShape"></param>
        /// <param name="direction"></param>
        [BurstCompile]
        public static void GetNormalizedDirectionTo(in this PhysicsBody fromBody, in PhysicsShape toShape, ref Vector2 direction)
        {
            if (!fromBody.isValid || !toShape.isValid || !toShape.body.isValid)
                return;
            direction = (fromBody.position - toShape.transform.position).normalized;
        }
        
        [BurstCompile]
        public static void GetNormalizedDirectionTo(in Vector2 fromPosition, in Vector2 toPosition, ref Vector2 direction)
        {
            direction = (fromPosition - toPosition).normalized;
        }
#endregion
       
    }
}
