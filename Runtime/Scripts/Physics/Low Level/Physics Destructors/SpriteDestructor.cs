using System;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

namespace SF.PhysicsLowLevel
{
    public class SpriteDestructor : MonoBehaviour
    {
        [NonSerialized] public SFShapeComponent ShapeComponent;
        private PhysicsDestructor.FragmentGeometry _fragmentGeometry;
        
        /// <summary>
        /// Fragment's the sprite into smaller geometry islands.
        /// </summary>
        public void Fragment()
        {
            if (ShapeComponent == null)
                return;
        }
    }
}
