using Unity.Burst;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

namespace SF.PhysicsLowLevel
{
    [BurstCompile]
    public static class PhysicsShapeExtensions
    {
        
#region PhysicsShape Callbacks Targets
        public static bool TryGetCallbackComponent<T>(this PhysicsShape shape,out T component, bool checkShapeValidation = false) where T : Component
        {
            component = null;
            
            // Optional check for only using Component set as a callbackTarget for valid shapes.
            if (checkShapeValidation && !shape.isValid)
                return false;

            if (shape.callbackTarget is not T callbackTarget) 
                return false;
            
            component = callbackTarget;
            return true;
        }
        
#endregion

#region PhysicsShape.ContactFilter

        [BurstCompile]
        public static void SetCategoriesBit(this ref PhysicsShape.ContactFilter filter, int bitIndex)
        {
            if (bitIndex < 0 || bitIndex > 63 /*0x3F*/)
                return;
            
            PhysicsMask mask = filter.categories;
            mask.SetBit(bitIndex);
            filter.categories = mask;
        }
        
        [BurstCompile]
        public static void ResetCategoriesBit(this ref PhysicsShape.ContactFilter filter, int bitIndex)
        {
            if (bitIndex < 0 || bitIndex > 63 /*0x3F*/)
                return;
            
            PhysicsMask mask = filter.categories;
            mask.ResetBit(bitIndex);
            filter.categories = mask;
        }
        [BurstCompile]
        public static void SetContactsBit(this ref PhysicsShape.ContactFilter filter, int bitIndex)
        {
            if (bitIndex < 0 || bitIndex > 63 /*0x3F*/)
                return;
            
            PhysicsMask mask = filter.contacts;
            mask.SetBit(bitIndex);
            filter.contacts = mask;
        }
        
        [BurstCompile]
        public static void ResetContactsBit(this ref PhysicsShape.ContactFilter filter, int bitIndex)
        {
            if (bitIndex < 0 || bitIndex > 63 /*0x3F*/)
                return;
            
            PhysicsMask mask = filter.contacts;
            mask.ResetBit(bitIndex);
            filter.contacts = mask;
        }
        
#endregion

    }
}
