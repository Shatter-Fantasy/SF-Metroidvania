using UnityEngine;
using Unity.U2D.Physics;

namespace SF.U2D.Physics
{
    public static class PhysicsBodyExtensions
    {
        public static bool TryGetCallbackComponent<T>(this PhysicsBody body,out T component, bool checkShapeValidation = false) where T : Component
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
        
        public static bool TryGetCallbackShapeComponent<T>(this PhysicsBody body,out T component, bool checkShapeValidation = false) where T : SFShapeComponent
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
    }
}
