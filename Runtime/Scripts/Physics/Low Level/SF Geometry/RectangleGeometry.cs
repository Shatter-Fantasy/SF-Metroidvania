using System;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

namespace SF.PhysicsLowLevel
{
    /// <summary>
    /// <para>
    /// The geometry of a closed rectangle. When this is used in the low level phyiscs 2D API calls it is converted to a polygon geometry
    /// This allows it to use the same exact API already built in.
    /// See LowLevelPhysics2D.PhysicsBody.CreateShape.
    /// </para>
    /// </summary>
    [Serializable]
    public struct RectangleGeometry
    {
#region Rectangle Properties
        [Header("Rectangle Properties")]
        [Min(0.000005f)]
        public Vector2 Size;
        /// <summary>
        /// The radius of the <see cref="SFRectangleShape"/> box corner radius.
        /// </summary>
        [Min(0.0f)]
        public float CornerRadius;
        
        /// <summary>
        /// When creating the physics shape should the corner radius be inscribed during the
        /// <see cref="PolygonGeometry.CreateBox"/> method call.
        /// </summary>
        public bool InscribeRadius;

        /// <summary>
        /// The backing <see cref="UnityEngine.LowLevelPhysics2D.PolygonGeometry"/> that supports the RectangleGeometry.
        /// </summary>
        [NonSerialized] public PolygonGeometry Polygon;
#endregion
        
        /// <summary>
        ///        <para>
        /// The geometry vertices stored in a LowLevelPhysics2D.PhysicsShape.ShapeArray.
        /// </para>
        ///      </summary>
        public PhysicsShape.ShapeArray Vertices;
        /// <summary>
        ///        <para>
        /// The geometry normal stored in a LowLevelPhysics2D.PhysicsShape.ShapeArray.
        /// </para>
        ///      </summary>
        public PhysicsShape.ShapeArray Normals;
        
        [SerializeField]
        public Vector2 Centroid;
        
#region Constructors
        /// <summary>
        /// <para>
        /// Get the default Rectangle.
        /// </para>
        /// </summary>
        public static readonly RectangleGeometry DefaultGeometry = new RectangleGeometry(size:Vector2.one)
        {
            CornerRadius = 0.5f,
        };
        
        public RectangleGeometry(Vector2 size, float cornerRadius = 0.5f, bool inscribeRadius = false)
        {
            if(size == Vector2.zero)
                size = Vector2.one;
            
            Size           = size;
            CornerRadius   = cornerRadius;
            InscribeRadius = inscribeRadius;
            Polygon        = PolygonGeometry.CreateBox(Size, CornerRadius, InscribeRadius);
            Centroid       = new Vector2(0, 0);
            Vertices       = default;
            Normals    = default;
        }
        
#endregion
        public readonly bool IsValid => Polygon.isValid;
        
        /// <summary>
        /// Deconstructs the properties of the <see cref="RectangleGeometry"/>
        /// and passes them out of the method call.
        /// </summary>
        /// <param name="size"></param>
        /// <param name="cornerRadius"></param>
        public void Deconstruct(out Vector2 size, out float cornerRadius)
        {
            size         = Size;
            cornerRadius = CornerRadius;
        }

        public static implicit operator PolygonGeometry(RectangleGeometry rectangleGeometry)
        {
            rectangleGeometry.Polygon = PolygonGeometry.CreateBox(
                    rectangleGeometry.Size,
                    rectangleGeometry.CornerRadius,
                    rectangleGeometry.InscribeRadius
                );

            return rectangleGeometry.Polygon;
        }
    }
}