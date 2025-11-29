using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;
using UnityEngine.Tilemaps;

namespace  SF.LowLevel.Physics
{
    /// <summary>
    /// The geometry that represents the outline of a Tilemap Renderer with tiles that have a physics shape.
    /// See LowLevelPhysics2D.PhysicsBody.CreateShape.
    ///</summary>
    /// <remarks>
    /// This heavily relies on Burst code for generating grabbing the vertices. For each shape needing to be read from a TilemapCollider
    /// a C# Job is made to set the vertices.
    /// This uses <see cref="PhysicsShapeGroup2D.GetShapeData"/> to grab a NativeArray of the Shapes and Vertices that makes of
    /// a normal <see cref="TilemapCollider2D"/> and than registers a listener to <see cref="Tilemap.tilemapTileChanged"/>
    /// to know when to update the shape of the collider.
    /// </remarks>
    [Serializable]
    public struct TilemapGeometry
    {
        /// <summary>
        /// The amount of non-connected shapes that make up the TilemapGeometry.
        /// </summary>
        public int ShapeCount;
        public int VertexCount;
        
        /// <summary>
        /// The NativeArray of <see cref="PolygonGeometry"/> that represents the different shapes of a TilemapGeometry.
        /// </summary>
        /// <remarks>
        /// This is used because you can have tilemaps with gaps making vertices not directly connected.
        /// Each section of a collider that is not connected has it own PolygonGeometry to make up the shape.
        /// </remarks>
        private NativeArray<PolygonGeometry> _tilemapPolygonGeometry;
        
        /// <summary>
        /// Takes in a NativeArray of PhysicsShape2D and their vertices to generate a Tilemap Geometry.
        /// </summary>
        /// <remarks>
        /// This is useful for taking in the collider of a <see cref="Tilemap"/> using a composite collider
        /// and taking the shape by the Composite Collider to create a low level representation of it.
        /// This is done using <see cref="PhysicsShapeGroup2D.GetShapeData(NativeArray PhysicsShape2D shapes, NativeArray Vector2 vertices)"/> on the CompositeCollider
        /// to get the NativeArrays to pass into this method.
        /// </remarks>
        /// <param name="shapesArray"></param>
        /// <param name="vertices"></param>
        /// <returns></returns>
        public static TilemapGeometry GenerateTilemapGeometry(NativeArray<PhysicsShape2D> shapesArray, NativeArray<Vector2> vertices)
        {
            TilemapGeometry geometry = new TilemapGeometry();
            return geometry;
        }
    }
}

