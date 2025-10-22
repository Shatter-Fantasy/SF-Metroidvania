using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.LowLevelPhysics2D;

namespace SF.PhysicsLowLevel
{
    /*  Special thanks to MelvMay, the creator of Unity's low level 2D physics system. He personally created these method during the Unity 6.3 beta to help give examples.
        Check out the link below for the public repo where he helped show examples on how to do this.
        https://github.com/Unity-Technologies/PhysicsExamples2D/tree/master/LowLevel/Projects/Snippets/Assets/Examples/ContactFilteringExtensions    
         */
    
    public enum FilterFunctionMode
    {
        NormalAngle,
        NormalImpulse,
        NormalAngleAndImpulse
    }
    
    public enum FilterMathOperator
    {
        GreaterThan = 0,
        LessThan = 1,
        Equal = 2,
        GreaterThanOrEqual = 4,
        LessThanOrEqual = 8
    }
    
    /// <summary>
    /// Used to choose which value or values of a Vector2 should a filter operation be done on. 
    /// </summary>
    public enum FilterVector2Operator
    {
        BothXY = 0, // Both have to pass the filter predicate check.
        JustX = 1,
        JustY = 2,
        EitherXY = 4, // Only one needs to pass the filter predicate check.
    }

    public static class ContactFiltering
    {
        
        public static bool NormalImpulseFilter(ref PhysicsShape.Contact contact, PhysicsShape shapeContext)
        {
            foreach (var point in contact.manifold)
            {
                if (point.totalNormalImpulse > 4.0f)
                    return true;
            }

            return false;
        }
        
        
        public static bool NormalAngleFilter(ref PhysicsShape.Contact contact, PhysicsShape shapeContext, float lowRange, float highRange)
        {
            // Fetch the normal.
            // NOTE: Normal is always in the direction of shape A to shape B so always ensure we're referring to it in context.
            var manifold = contact.manifold;
            var normal = shapeContext == contact.shapeB ? manifold.normal : -manifold.normal;
        
            // Filter the normal.
            var normalAngle = PhysicsMath.ToDegrees(new PhysicsRotate(normal).angle);
            return normalAngle > lowRange && normalAngle < highRange;
        }
        
        
        public static bool NormalXFilter(ref PhysicsShape.Contact contact, 
            PhysicsShape shapeContext, 
            float normalizedXValue = 0f, 
            FilterMathOperator filterMathOperator = FilterMathOperator.Equal)
        {
            // Fetch the normal.
            // NOTE: Normal is always in the direction of shape A to shape B so always ensure we're referring to it in context.
            var manifold = contact.manifold;
            var normal = shapeContext == contact.shapeB ? manifold.normal : -manifold.normal;

            bool passFilter = filterMathOperator switch
            {
                FilterMathOperator.GreaterThan => normal.x > normalizedXValue,
                FilterMathOperator.LessThan => normal.x > normalizedXValue,
                FilterMathOperator.Equal => normal.x > normalizedXValue,
                _ => false
            };

            return passFilter;
        }
        
        public static bool NormalYFilter(ref PhysicsShape.Contact contact, 
            PhysicsShape shapeContext, 
            float normalizedYValue = 0f, 
            FilterMathOperator filterMathOperator = FilterMathOperator.Equal)
        {
            // Fetch the normal.
            // NOTE: Normal is always in the direction of shape A to shape B so always ensure we're referring to it in context.
            var manifold = contact.manifold;
            var normal = shapeContext == contact.shapeB ? manifold.normal : -manifold.normal;

            bool passFilter = filterMathOperator switch
            {
                FilterMathOperator.GreaterThan => normal.y > normalizedYValue,
                FilterMathOperator.LessThan => normal.y > normalizedYValue,
                FilterMathOperator.Equal => normal.y > normalizedYValue,
                _ => false
            };

            return passFilter;
        }
    }
}
