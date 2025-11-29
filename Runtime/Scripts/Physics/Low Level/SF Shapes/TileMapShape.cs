using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using SF.Utilities;
using UnityEngine.LowLevelPhysics2D;
using UnityEngine.Profiling;
using UnityEngine.U2D.Physics.LowLevelExtras;

namespace SF.PhysicsLowLevel   
{   
    [ExecuteAlways]
    [DefaultExecutionOrder(PhysicsLowLevelExtrasExecutionOrder.SceneShape)]
    [AddComponentMenu("Physics 2D/LowLevel/TileMap Shape", 22)]
    
    public class TileMapShape : MonoBehaviour, IWorldSceneDrawable, IWorldSceneTransformChanged
    {
        [SerializeField] private Tilemap _tilemap;
        [SerializeField] private List<TileData> _tilesInBlock = new ();

        [Header("Physics Definition")] 
        public SceneBody SceneBody;
        public PhysicsShapeDefinition ShapeDefinition = PhysicsShapeDefinition.defaultDefinition;
        [SerializeField] private  bool _useDelaunay;
        private PhysicsShape _physicShape;

        
        private readonly List<Vector2> _physicsShapeVertex = new();
        
        private struct OwnedShapes
        {
            public PhysicsShape Shape;
            public int OwnerKey;
        }
        
        private NativeList<OwnedShapes> _ownedShapes = new NativeList<OwnedShapes>(Allocator.Persistent);
        
        private void Awake()
        {
            if (_tilemap == null)
                return;
            
            _tilemap.GetUsedTileData(out _tilesInBlock);
        }

        private void CreateShapes()
        {
            if (!SceneBody)
                return;

            var body = SceneBody.Body;
            
            if (!body.isValid)
                return;

            var composer = PhysicsComposer.Create();
            composer.useDelaunay = _useDelaunay;
            var vertexPath = new NativeList<Vector2>(Allocator.Temp);
            Profiler.BeginSample("Getting Tile Data");
            // Code to measure...
         
#if UNITY_6000_4_OR_NEWER
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
            var relativeTransform = PhysicsMath.GetRelativeMatrix(SceneBody.transform, transform, SceneBody.Body.world.transformPlane, useScale: false);
            
            // Iterate the polygons.
            foreach (var geometry in polygons)
            {
                if (!geometry.isValid)
                    continue;

                var shapeGeometry = geometry.Transform(relativeTransform, false);
                if (!shapeGeometry.isValid)
                    continue;

                var shape = body.CreateShape(shapeGeometry, ShapeDefinition);
                if (!shape.isValid)
                    continue;

                
                // Set the owner.
                var ownerKey = shape.SetOwner(this);

                // Add to owned shapes.
                _ownedShapes.Add(new OwnedShapes
                {
                    Shape = shape,
                    OwnerKey = ownerKey
                });
            }
        }
        
        private void DestroyShapes()
        {
            if (!_ownedShapes.IsCreated)
                return;

            foreach (var ownedShape in _ownedShapes)
            {
                if (ownedShape.Shape.isValid)
                    ownedShape.Shape.Destroy(updateBodyMass: false, ownerKey: ownedShape.OwnerKey);
            }

            _ownedShapes.Clear();

            if (SceneBody != null && SceneBody.Body.isValid)
                SceneBody.Body.ApplyMassFromShapes();
        }

        private void OnCreateBody(SceneBody sceneBody)
        {
            CreateShapes();
        }

        private void OnDestroyBody(SceneBody sceneBody)
        {
            DestroyShapes();
        }
        
        private void Reset()
        {
            if (SceneBody == null)
                SceneBody = SceneBody.FindSceneBody(gameObject);
            
            if (_tilemap == null)
                _tilemap = GetComponent<Tilemap>();
        }

        void IWorldSceneTransformChanged.TransformChanged() => CreateShapes();

        void IWorldSceneDrawable.Draw()
        {
            // Finish if we've nothing to draw.
            if (!_ownedShapes.IsCreated || _ownedShapes.Length == 0)
                return;

            // Finish if we're not drawing selections.
            if (!_ownedShapes[0].Shape.world.drawOptions.HasFlag(PhysicsWorld.DrawOptions.SelectedShapes))
                return;

            // Draw selections.
            foreach (var ownedShape in _ownedShapes)
                ownedShape.Shape.Draw();
        }
        
        private void OnEnable()
        {
            Reset();

            if (SceneBody != null)
            {
                SceneBody.CreateBodyEvent += OnCreateBody;
                SceneBody.DestroyBodyEvent += OnDestroyBody;
            }

            _ownedShapes = new NativeList<OwnedShapes>(Allocator.Persistent);

            CreateShapes();

#if UNITY_EDITOR
            WorldSceneTransformMonitor.AddMonitor(this);
#endif
        }
        
        private void OnDisable()
        {
            DestroyShapes();

            if (_ownedShapes.IsCreated)
                _ownedShapes.Dispose();

            if (SceneBody != null)
            {
                SceneBody.CreateBodyEvent -= OnCreateBody;
                SceneBody.DestroyBodyEvent -= OnDestroyBody;
            }

#if UNITY_EDITOR
            WorldSceneTransformMonitor.RemoveMonitor(this);
#endif
        }
        
        private void OnValidate()
        {
            if (!isActiveAndEnabled)
                return;

            DestroyShapes();
            CreateShapes();
        }
    }
}
