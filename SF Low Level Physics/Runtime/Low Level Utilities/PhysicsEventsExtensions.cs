using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

namespace SF.PhysicsLowLevel
{
    public static class PhysicsEventsExtensions
    {
#region PhysicsEvents.TriggerBeginEvent 
        /// <summary>
        /// Tries to get the <see cref="PhysicsEvents.TriggerBeginEvent.visitorShape"/> <see cref="PhysicsShape.callbackTarget"/>
        /// as a certain type of component and returns true or false if that type of component was set for the callback target.
        /// </summary>
        /// <param name="beginEvent"></param>
        /// <param name="component"></param>
        /// <param name="checkValidation"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool TryGetCallbackComponentOnVisitor<T>(this PhysicsEvents.TriggerBeginEvent beginEvent, out T component, bool checkValidation = false) where T : Component
        {
            component = null;
            // Optional check for only using Component set as a callbackTarget for valid shapes.
            if (checkValidation && !beginEvent.visitorShape.isValid)
                return false;

            if (beginEvent.visitorShape.callbackTarget is not T callbackTarget)
                return false;

            component = callbackTarget;
            return true;
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
        /// Tries to get the <see cref="PhysicsEvents.TriggerBeginEvent.triggerShape"/> <see cref="PhysicsShape.callbackTarget"/>
        /// as a certain type of component and returns true or false if that type of component was set for the callback target.
        /// </summary>
        /// <param name="beginEvent"></param>
        /// <param name="component"></param>
        /// <param name="checkValidation"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool TryGetCallbackComponentOnTrigger<T>(this PhysicsEvents.TriggerBeginEvent beginEvent, out T component, bool checkValidation = false) where T : Component
        {
            component = null;
            // Optional check for only using Component set as a callbackTarget for valid shapes.
            if (checkValidation && !beginEvent.triggerShape.isValid)
                return false;

            if (beginEvent.triggerShape.callbackTarget is not T callbackTarget)
                return false;

            component = callbackTarget;
            return true;
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
#endregion

#region PhysicsEvents.ContactBeginEvent  
        public static bool TryGetCallbackComponentOnShapeA<T>(this PhysicsEvents.ContactBeginEvent beginEvent,out T component, bool checkValidation = false)
        {
            component = default(T);
            // Optional check for only using Component set as a callbackTarget for valid shapes.
            if (checkValidation && !beginEvent.shapeA.isValid)
                return false;

            if (beginEvent.shapeA.callbackTarget is not T callbackTarget)
                return false;

            component = callbackTarget;

            return true;
        }
        
        public static bool TryGetCallbackComponentOnShapeB<T>(this PhysicsEvents.ContactBeginEvent beginEvent,out T component, bool checkValidation = false)
        {
            component = default(T);
            // Optional check for only using Component set as a callbackTarget for valid shapes.
            if (checkValidation && !beginEvent.shapeB.isValid)
                return false;

            if (beginEvent.shapeB.callbackTarget is not T callbackTarget)
                return false;

            component = callbackTarget;

            return true;
        }
#endregion
    }
}
