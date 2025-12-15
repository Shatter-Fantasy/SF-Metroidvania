using System.Collections.Generic;
using SF.Utilities;
using Unity.Collections;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;
using UnityEngine.Profiling;
using UnityEngine.Tilemaps;

namespace SF.PhysicsLowLevel
{
    public class SFTileMapShape : SFShapeComponent
    {
        [SerializeField] private Tilemap _tilemap;
        private List<TileData> _tilesInBlock = new ();

        private readonly List<Vector2> _physicsShapeVertex = new();

        private SFTileMapShape()
        {
            _useDelaunay     = true;
            IsCompositeShape = true;
        }
        
        private void Awake()
        {
            if (_tilemap == null)
                return;
            
            _tilemap.GetUsedTileData(out _tilesInBlock);
        }

        protected override void OnValidate()
        {
            if (_tilemap == null)
                _tilemap = GetComponent<Tilemap>();
            
            base.OnValidate();
        }
        
        /* TODO: We need to override the way we validate the Shape of the base class.
            Because this is a multiple Shape composited shape we will probably need 
            to use GeometryIslands for the base Shape parameter.0         */

        /* TODO: Implement the ConvexHull Implementation using PhysicsComposer.CreateConvexHulls
         *  This will be useful for Camera Confiners and the IPhysicsShapeContained list for calculating.
         */
        protected override void CreateShapeGeometry()
        {
            var composer = PhysicsComposer.Create();
            composer.useDelaunay = _useDelaunay;
            var vertexPath = new NativeList<Vector2>(Allocator.Temp);
            Profiler.BeginSample("Getting Tile Data");
            
            
#if UNITY_6000_4_OR_NEWER
            // Not yet implemented the updated method yet.
            _tilemap.GetUsedTileData(out _tilesInBlock);
#else
            _tilemap.GetUsedTileData(out _tilesInBlock);
#endif
            Profiler.EndSample();
            
            
            var positions = _tilemap.GetTileCellPositions();
            
            for (int i = 0; i < _tilesInBlock.Count; i++)
            {
                if(_tilesInBlock[i].sprite == null)
                    continue;
                
                var physicsShapeCount = _tilesInBlock[i].sprite.GetPhysicsShapeCount();
                if (physicsShapeCount == 0)
                    return;
                
                // Add all physic shape paths.
                for (var j = 0; j < physicsShapeCount; ++j)
                {
                    // Get the physics shape.
                    if (_tilesInBlock[i].sprite.GetPhysicsShape(j, _physicsShapeVertex) > 0)
                    {
                       
                        // Add to something we can use.
                        for (int v = 0; v <  _physicsShapeVertex.Count; v++)
                        {
                            vertexPath.Add(_physicsShapeVertex[v] + positions[i].ToVector2Int() + (Vector2)_tilemap.tileAnchor);
                        }
                       
                        // Add the layer to the composer.
                        // I use PhysicsTransform.identity to get the relative position of tiles away from the grid origin.
                        composer.AddLayer(vertexPath.AsArray(), PhysicsTransform.identity);
                    }

                    vertexPath.Clear();
                }
            }
            
            using var polygons = composer.CreatePolygonGeometry(vertexScale: transform.lossyScale, Allocator.Temp);
            
            vertexPath.Dispose();
            composer.Destroy();
            positions.Dispose();
            
            // Calculate the relative transform from the scene body to this scene shape.
            var relativeTransform = PhysicsMath.GetRelativeMatrix(transform, transform, Body.world.transformPlane, useScale: false);
            
            // Iterate the polygons.
            foreach (var geometry in polygons)
            {
                if (!geometry.isValid)
                    continue;

                var shapeGeometry = geometry.Transform(relativeTransform, false);
                if (!shapeGeometry.isValid)
                    continue;

                var shape = Body.CreateShape(shapeGeometry, ShapeDefinition);
                
                if (!shape.isValid)
                    continue;
                
                // Set the owner.
                ShapeOwnerKey = shape.SetOwner(this);

                // Add to owned shapes.
                _ownedShapes.Add(new OwnedShapes(shape, ShapeOwnerKey));
            }

            if (_ownedShapes is { IsCreated: true, Length: > 0 })
            {
                Shape = _ownedShapes[0].Shape;
            }
        }
    }
}
