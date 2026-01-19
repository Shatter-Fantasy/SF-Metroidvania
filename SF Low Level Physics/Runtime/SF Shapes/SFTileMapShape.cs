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
            if (!isActiveAndEnabled || Application.isPlaying)
                return;
            
            if (_tilemap == null)
                _tilemap = GetComponent<Tilemap>();

            BodyDefinition.type = PhysicsBody.BodyType.Static;
        }

        protected override void PreEnabled()
        {
            if(TryGetComponent(out _tilemap))
            {
                _tilemap.CompressBounds();
            }
            
            Tilemap.tilemapTileChanged += TilemapTilesChanged;
        }

        protected override void PreDisable()
        {
            Tilemap.tilemapTileChanged -= TilemapTilesChanged;
        }

        /// <summary>
        /// Updates physics shape when tiles on the linked tilemap has been updated.
        /// </summary>
        /// <param name="tilemap"></param>
        /// <param name="syncTiles"></param>
        private void TilemapTilesChanged(Tilemap tilemap, Tilemap.SyncTile[] syncTiles)
        {
            if(_tilemap != null && _tilemap == tilemap)
                CreateShape();
        }

        protected override void CreateShape()
        {
            if (_tilemap == null)
            {
                DestroyBody();
                DestroyShape();
                return;
            }

            base.CreateShape();
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
            
            Profiler.BeginSample("SFTileMapShape Geometry",this);
            if (_tilesInBlock.Count == 0)
                return;
            
            Sprite tileSprite; 
            for (int i = 0; i < _tilesInBlock.Count; i++)
            {
                if(_tilesInBlock[i].sprite == null)
                    continue;

                tileSprite = _tilesInBlock[i].sprite;
                
                var physicsShapeCount = tileSprite.GetPhysicsShapeCount();
                if (physicsShapeCount == 0)
                    return;
                
                // Add all physic shape paths.
                for (var j = 0; j < physicsShapeCount; ++j)
                {
                    // Get the physics shape.
                    if (tileSprite.GetPhysicsShape(j, _physicsShapeVertex) > 0)
                    {
                        // TODO: The below really should be moved into a IJob struct for calculating the vertex path position to speed up.
                        //  We should calculate the final position of the vertexes in a job because when loading a bigger tilemap room we could have hundreds of tiles
                        //  to calculate the rotated + grid position + tile anchor position offset.
                        
                        // Add to something we can use.
                        for (int v = 0; v <  _physicsShapeVertex.Count; v++)
                        {
                            // The (Vector2)_tilesInBlock[i].transform.MultiplyPoint3x4 below matches the tiles rotation or scale value of the placed tile data.
#if UNITY_6000_4_OR_NEWER
                        vertexPath.Add((Vector2)_tilesInBlock[i].transform.MultiplyPoint3x4(_physicsShapeVertex[v]) + tilePosition[i].ToVector2Int() + (Vector2)_tilemap.tileAnchor);
#else
                        vertexPath.Add((Vector2)_tilesInBlock[i].transform.MultiplyPoint3x4(_physicsShapeVertex[v])+ positions[i].ToVector2Int() + (Vector2)_tilemap.tileAnchor);
#endif
                        }
                        
                        PhysicsTransform tileTransform = PhysicsTransform.identity;
                        
                        // Add the layer to the composer.
                        // I use PhysicsTransform.identity to get the relative position of tiles away from the grid origin.
                        composer.AddLayer(vertexPath.AsArray(), PhysicsTransform.identity);
                    }

                    vertexPath.Clear();
                }
            }
            
            using var polygons = composer.CreatePolygonGeometry(vertexScale: transform.lossyScale, Allocator.Temp);
            
            composer.Destroy();
            
            if (polygons.Length == 0)
                return;
            
            using var shapes = Body.CreateShapeBatch(polygons, ShapeDefinition);
            
            if (ShapesInComposite.IsCreated)
            {
                ShapesInComposite.AddRange(shapes);
                // For now we just set the default shape of the SFShapeComponent to be the first shape added to the ShapesInComposite list.
                // This is a bad solution and needs more updates for composite shapes.
                _shape = ShapesInComposite[0];
            }
            Profiler.EndSample();
        }
    }
}
