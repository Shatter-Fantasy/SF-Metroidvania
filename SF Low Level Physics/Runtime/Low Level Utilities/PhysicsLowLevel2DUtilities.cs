using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

namespace SF.PhysicsLowLevel
{
    public static class PhysicsLowLevel2DUtilities
    {

#region PhysicsShape
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
       
     
        public static T GetCallbackComponent<T>(this PhysicsShape shape, bool checkShapeValidation = false) where T : Component
        {
            // Optional check for only using Component set as a callbackTarget for valid shapes.
            if (checkShapeValidation && !shape.isValid)
                return null;

            if (shape.callbackTarget is Component component)
                return component as T;

            return null;
        }

        /// <summary>
        /// Gets the <see cref="PhysicsEvents.TriggerBeginEvent"/> visiting <see cref="PhysicsShape.callbackTarget"/>
        /// as a <see cref="Component"/> if casting is possible.
        /// Returns null if the set callbackTarget is not a component.
        /// </summary>
        /// <param name="beginEvent"></param>
        /// <param name="checkValidation"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetCallbackComponentOnVisitor<T>(this PhysicsEvents.TriggerBeginEvent beginEvent, bool checkValidation = false) where T : Component
        {
            // Optional check for only using Component set as a callbackTarget for valid shapes.
            if (checkValidation && !beginEvent.visitorShape.isValid)
                return null;

            if (beginEvent.visitorShape.callbackTarget is Component component)
                return component as T;

            return null;
        }
        
        /// <summary>
        /// Gets the <see cref="PhysicsEvents.TriggerBeginEvent"/> triggerShape <see cref="PhysicsShape.callbackTarget"/>
        /// as a <see cref="Component"/> if casting is possible.
        /// Returns null if the set callbackTarget is not a component.
        /// </summary>
        /// <param name="beginEvent"></param>
        /// <param name="checkValidation"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetCallbackComponentOnTrigger<T>(this PhysicsEvents.TriggerBeginEvent beginEvent, bool checkValidation = false) where T : Component
        {
            // Optional check for only using Component set as a callbackTarget for valid shapes.
            if (checkValidation && !beginEvent.visitorShape.isValid)
                return null;

            if (beginEvent.triggerShape.callbackTarget is Component component)
                return component as T;

            return null;
        }

        
        /// <summary>
        /// Attempts to get a <see cref="SFShapeComponent"/> that is set as a callback target to a <see cref="PhysicsShape"/>
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="checkShapeValidation"></param>
        /// <returns></returns>
        public static SFShapeComponent GetCallbackShapeComponent(this PhysicsShape shape, bool checkShapeValidation = false)
        {
            // Optional check for only using SFShapeComponents set as a callbackTarget for valid shapes.
            if (checkShapeValidation && !shape.isValid)
                return null;

            if (shape.callbackTarget is SFShapeComponent shapeComponent)
                return shapeComponent;

            return null;
        }

#region  PhysicsBody 
        /// <summary>
        /// Attempts to get a <see cref="SFShapeComponent"/> that is set as a callback target to a <see cref="PhysicsBody"/>
        /// </summary>
        /// <param name="body"></param>
        /// <param name="checkShapeValidation"></param>
        /// <returns></returns>
        public static T GetCallbackComponent<T>(this PhysicsBody body, bool checkShapeValidation = false) where T : Component
        {
            // Optional check for only using Component set as a callbackTarget for valid shapes.
            if (checkShapeValidation && !body.isValid)
                return null;

            if (body.callbackTarget is Component component)
                return component as T;

            return null;
        }
        
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
#endregion
    }
}
