using UnityEngine;
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
        GreaterThan,
        LessThan,
        Equal
    }

    public static class ContactFiltering
    {
        // Example filter that checks if the normal angle is within a set range.
        private static bool NormalAngleFilter(ref PhysicsShape.Contact contact, PhysicsShape shapeContext)
        {
            // Fetch the normal.
            // NOTE: Normal is always in the direction of shape A to shape B so always ensure we're referring to it in context.
            var manifold = contact.manifold;
            var normal = shapeContext == contact.shapeB ? manifold.normal : -manifold.normal;
        
            // Filter the normal.
            var normalAngle = PhysicsMath.ToDegrees(new PhysicsRotate(normal).angle);
            return normalAngle is > 85f and < 95f;
        }
        
         
        // Example filter that checks if the normal angle is within a set range.
        private static bool NormalAngleFilter(ref PhysicsShape.Contact contact, PhysicsShape shapeContext, float lowRange, float highRange)
        {
            // Fetch the normal.
            // NOTE: Normal is always in the direction of shape A to shape B so always ensure we're referring to it in context.
            var manifold = contact.manifold;
            var normal = shapeContext == contact.shapeB ? manifold.normal : -manifold.normal;
        
            // Filter the normal.
            var normalAngle = PhysicsMath.ToDegrees(new PhysicsRotate(normal).angle);
            return normalAngle > lowRange && normalAngle < highRange;
        }
        
        // Example filter that checks if the normal impulse is above a threshold.
        private static bool NormalImpulseFilter(ref PhysicsShape.Contact contact, PhysicsShape shapeContext)
        {
            foreach (var point in contact.manifold)
            {
                if (point.totalNormalImpulse > 4.0f)
                    return true;
            }

            return false;
        }
        
        // Example filter that checks both the normal angle and impulse threshold.
        private static bool NormalAngleAndImpulseFilter(ref PhysicsShape.Contact contact, PhysicsShape shapeContext)
        {
            return NormalAngleFilter(ref contact, shapeContext) && NormalImpulseFilter(ref contact, shapeContext);
        }
        
        
        // Example filter that checks if the normal angle is within a set range.
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
                FilterMathOperator.LessThan => normal.y < normalizedYValue,
                FilterMathOperator.Equal => Mathf.Approximately(normal.y, normalizedYValue),
                _ => false
            };

            return passFilter;
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
                FilterMathOperator.LessThan => normal.x < normalizedXValue,
                FilterMathOperator.Equal => Mathf.Approximately(normal.x,normalizedXValue),
                _ => false
            };

            return passFilter;
        }
    }
}
