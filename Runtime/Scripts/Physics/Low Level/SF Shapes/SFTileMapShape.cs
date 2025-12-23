using System.Collections.Generic;
using SF.Utilities;
using Unity.Collections;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;
using UnityEngine.Profiling;
using UnityEngine.Tilemaps;

namespace SF.PhysicsLowLevel
{
    [RequireComponent(typeof(Tilemap))]
    public class SFTileMapShape : SFShapeComponent
    {
        [SerializeField, HideInInspector] private Tilemap _tilemap;
        private List<TileData> _tilesInBlock = new ();

        private readonly List<Vector2> _physicsShapeVertex = new();

        private SFTileMapShape()
        {
            _useDelaunay     = true;
            IsCompositeShape = true;
        }

        protected override void OnValidate()
        {
            if (!isActiveAndEnabled)
                return;
            
            if (_tilemap == null)
                _tilemap = GetComponent<Tilemap>();

            BodyDefinition.type = PhysicsBody.BodyType.Static;
            base.OnValidate();
        }

        protected override void CreateBody()
        {
            BodyDefinition.type = PhysicsBody.BodyType.Static;
            BodyDefinition.constraints = PhysicsBody.BodyConstraints.All;
            base.CreateBody();
        }

        protected override void DebugPhysicsExtra()
        {
            if (_tilemap == null)
            {
                Debug.LogWarning($"The {GetType().Name} component on game object named: {gameObject.name} didn't have a TileMap assigned, so no shape can be created.", gameObject);
            }
        }

        /* TODO: Implement the ConvexHull Implementation using PhysicsComposer.CreateConvexHulls
         *  This will be useful for Camera Confiners and the IPhysicsShapeContained list for calculating.
         */
        protected override void CreateBodyShapeGeometry()
        {
            var composer = PhysicsComposer.Create();
            composer.useDelaunay = _useDelaunay;
            
            using var   vertexPath        = new NativeList<Vector2>(Allocator.Temp);
            
            Profiler.BeginSample("Getting Tile Data");
#if UNITY_6000_4_OR_NEWER
            Tilemap.PositionArray tilePosition = new Tilemap.PositionArray();
            _tilemap.GetUsedTileData(out _tilesInBlock, out tilePosition);
#else
            _tilemap.GetUsedTileData(out _tilesInBlock);
            using var positions = _tilemap.GetTileCellPositions();
#endif
            Profiler.EndSample();
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
#if UNITY_6000_4_OR_NEWER
                        vertexPath.Add(_physicsShapeVertex[v] + tilePosition[i].ToVector2Int() + (Vector2)_tilemap.tileAnchor);
#else
                        vertexPath.Add(_physicsShapeVertex[v] + positions[i].ToVector2Int() + (Vector2)_tilemap.tileAnchor);
#endif
                            
                        }
                       
                        // Add the layer to the composer.
                        // I use PhysicsTransform.identity to get the relative position of tiles away from the grid origin.
                        composer.AddLayer(vertexPath.AsArray(), PhysicsTransform.identity);
                    }

                    vertexPath.Clear();
                }
            }
            
            using var polygons = composer.CreatePolygonGeometry(vertexScale: transform.lossyScale, Allocator.Temp);
            
            composer.Destroy();
            
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

                // Add to owned shapes.
                if(ShapesInComposite.IsCreated)
                    ShapesInComposite.Add(shape);
            }

            if (ShapesInComposite is { IsCreated: true, Length: > 0 })
            {
                // For now we just set the default shape of the SFShapeComponent to be the first shape added to the ShapesInComposite list.
                // This is a bad solution and needs more updates for composite shapes.
                _shape = ShapesInComposite[0];
            }
        }
    }
}
