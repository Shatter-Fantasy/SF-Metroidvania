using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;
using Unity.U2D.Physics.Extras;
namespace SF.PhysicsLowLevel
{
    /// <summary>
    /// Base class for the <see cref="MonoBehaviour"/> component based
    /// <see cref="PhysicsShape"/> classes used in the SF Metroidvania package.
    /// </summary>
    /// <remarks>
    /// Inherit from this class and implement your own custom Geometry shape or declare the vertexes for your own shape.
    /// For examples of geometry see Unity's built in types <see cref="CapsuleGeometry"/>, <see cref="PolygonGeometry"/>, and <see cref="CircleGeometry"/>.
    /// Create a custom implicit casting to a <see cref="PhysicsShape.ShapeProxy"/> that calls one of the constructors for geometry.
    /// </remarks>
    [ExecuteAlways]
    [Icon("Packages/shatterfantasy.sf-metroidvania/Editor/Icons/SceneBody.png")]
    
    public abstract class SFShapeComponent : MonoBehaviour, IWorldSceneDrawable, IWorldSceneTransformChanged
    {
        public PhysicsShape _shape;
        /// <summary>
        /// The completed physics shape data struct for the <see cref="SFShapeComponent"/>.
        /// </summary>
        /// <remarks>
        /// Keyword here is completed because you can use <see cref="PhysicsComposer"/> to merge shapes and vertexes into a single shape.
        /// If a <see cref="SFShapeComponent"/> is made from multiple individual shapes and a single shape is created this is the completed merged shape.
        /// <see cref="SF.PhysicsLowLevel.SFTileMapShape"/> for an example of this.
        /// </remarks>
        
        public PhysicsShape Shape
        {
            get
            {
                // For Composite Shapes we just return the very first shape in the ShapesInComposite NativeList
                if (IsCompositeShape)
                {
                    if (ShapesInComposite is { IsCreated: true, Length: > 0 })
                        return ShapesInComposite[0];
                }

                // For composite shapes reaching this point the _shape value will be null or the default.
                return _shape;
            }
            set
            {
                if (IsCompositeShape)
                {
#if UNITY_EDITOR
                    Debug.LogWarning($"The physics shape of the {GetType().Name} on gameobject: {gameObject.name} is set as a composite shape. " +
                              $"composite shapes can not have the Shape value directly set. Instead the Shape is set at the same time as ShapesInComposite is set.", this);
#endif
                    return;
                }
                
                _shape = value; 
            }
        }

        public virtual void SetShape<TGeometryType>(TGeometryType geometryType) where  TGeometryType : struct
        {
            if (!_shape.isValid)
                return;

            switch (geometryType)
            {
                case CircleGeometry circleGeometry:
                {
                    Debug.Log(circleGeometry.radius);
                    _shape.circleGeometry = circleGeometry;
                    break;
                }
                case PolygonGeometry polygonGeometry:
                {
                    _shape.polygonGeometry = polygonGeometry;
                    break;
                }
                case CapsuleGeometry capsuleGeometry:
                {
                    _shape.capsuleGeometry = capsuleGeometry;
                    break;
                }
                case SegmentGeometry segmentGeometry:
                {
                    _shape.segmentGeometry = segmentGeometry;
                    break;
                }
                default:
                {
#if UNITY_EDITOR
                    Debug.LogWarning($"When trying to set the shape geometry of the {GetType().Name} on the game object : {gameObject.name}, an unsupported geometry type was passed in ", gameObject);
                    break;
#endif
                }
            }
        }

        public NativeList<PhysicsShape> ShapesInComposite;
        
        /// <summary>
        /// The definition for the <see cref="Shape"/> for the <see cref="SFShapeComponent"/>.
        /// </summary>
        /// <remarks>
        /// When adding this component to a GameObject it default's to the <see cref="PhysicsShapeDefinition.defaultDefinition"/>
        /// set in the LowLevelPhysics settings asset in the project settings.
        /// </remarks>
        public PhysicsShapeDefinition ShapeDefinition = PhysicsShapeDefinition.defaultDefinition;
        
        /// <summary>
        /// The generic proxy data container for the <see cref="Shape"/>
        /// Used to allow support for all types of shapes and geometry when
        /// doing queries and casting. 
        ///
        /// <see cref="PhysicsWorld.CastShapeProxy"/> for an example of use  cases.
        /// </summary>
        public PhysicsShape.ShapeProxy ShapeProxy
        {
            get
            {
                if (Shape.isValid)
                    return Shape.CreateShapeProxy();
#if UNITY_EDITOR
                Debug.Log($"The physics shape of the {GetType().Name} on gameobject: {gameObject.name}  wasn't valid when trying to get it's ShapeProxy", this);
#endif
                return new PhysicsShape.ShapeProxy();
            }
        }
        
        /// <summary>
        /// A list of objects that are currently contained inside of <see cref="Shape"/>
        /// </summary>
        public List<IPhysicsShapeContained> ContainedPhysicsShapes = new();

        public PhysicsBody Body;
        public PhysicsBodyDefinition BodyDefinition = PhysicsBodyDefinition.defaultDefinition;

        public PhysicsWorld PhysicsWorld;

        /// <summary>
        /// Is the <see cref="Shape"/> created by multiple separate <see cref="PhysicsShape"/>?
        /// </summary>
        [HideInInspector] public bool IsCompositeShape;
        
        /// <summary>
        /// Should the <see cref="Shape"/> size be scaled wiht the game objects transform.
        /// </summary>
        public bool ScaleSize = true;
        /// <summary>
        /// Should the Delaunay algorithm be used for creating meshes using <see cref="PhysicsComposer"/>.
        /// </summary>
        protected bool _useDelaunay;
        
        public object CallbackTarget;
        
        protected void OnEnable()
        {
            PreEnabled();
            CreateShape();

#if UNITY_EDITOR
            Unity.U2D.Physics.Extras.WorldSceneTransformMonitor.AddMonitor(this);
#endif
            DebugPhysics();
        }

        /// <summary>
        /// Override to set up required components when first adding a SFShapeComponent class to a gameobject.
        /// Example <see cref="SFTileMapShape"/> requires a TileMap component to be set before generating the <see cref="Shape"/>. 
        /// </summary>
        protected virtual void PreEnabled() { }
        
        protected void OnDisable()
        {
            DestroyBody();
            DestroyShape();
            
#if UNITY_EDITOR
            Unity.U2D.Physics.Extras.WorldSceneTransformMonitor.RemoveMonitor(this);
#endif
        }
        
        protected virtual void OnValidate()
        {
            if (!isActiveAndEnabled)
                return;
            CreateShape();
            DebugPhysics();
        }
        
        /// <summary>
        /// Called from editor tools to update the Shape after making changes using editor tools.
        /// Also can be used to force a shape update.
        /// </summary>
        public void UpdateShape() => CreateShape();

        protected virtual void CreateShape()
        {
            // Clean up any already created Shape data.
            DestroyShape();

            if (IsCompositeShape)
            {
                if (ShapesInComposite.IsCreated)
                {
                    ShapesInComposite.Dispose();
                }
                ShapesInComposite = new NativeList<PhysicsShape>(Allocator.Persistent);
            }

            // Create the physics body from the physics body definition that the Shape will use.
            CreateBody();
            
            // Make sure the Physics Body is valid before moving to shape creation.
            if (!Body.isValid)
                return;

            // Called from classes inheriting from the abstract class SFShapeComponent.
            // The CreateShapeGeometry is overridden to set up custom shape components for game objects. 
            CreateBodyShapeGeometry();
            
            // Make sure the shape is valid.
            if (!Shape.isValid)
                return;

            // If there was no custom callback target set use this component's gameobject as the callback target.
            _shape.callbackTarget = CallbackTarget ?? gameObject;
            Body.callbackTarget  = CallbackTarget ?? gameObject;
        }

        /// <summary>
        /// Creates the <see cref="Shape"/> geometry to use when calling the <see cref="Body.CreateShape()"/> method. 
        /// </summary>
        protected virtual void CreateBodyShapeGeometry()
        {
            // For the abstract method just creating a example shape for people to see how to do.
            Shape = Body.CreateShape(PolygonGeometry.CreateBox(Vector2.one));
        }

        protected virtual void CreateBody()
        {
            // Destroy any existing body.
            DestroyBody();
            
            // Check for a valid physics world.
            if (!PhysicsWorld.isValid)
            {
                PhysicsWorld = PhysicsWorld.defaultWorld;
            }
            // Sync the shape position with the component's transform position.
            BodyDefinition.position = PhysicsMath.ToPosition2D(transform.position, PhysicsWorld.transformPlane);
            BodyDefinition.rotation = new PhysicsRotate(PhysicsMath.ToRotation2D(transform.rotation, PhysicsWorld.transformPlane));
            
            // Create the physics body to inject into the shape when creating it.
            Body = PhysicsBody.Create(world:PhysicsWorld, definition: BodyDefinition);
            if (Body.isValid)
            {
                // Set the transform object.
                Body.transformObject = transform;

                // Set the callback target.
                Body.callbackTarget = CallbackTarget;
            }
        }
        protected virtual void DestroyShape()
        {
            if (IsCompositeShape 
                && ShapesInComposite.IsCreated)
            {
                if (ShapesInComposite.Length > 0)
                {
                    for (int i = 0; i < ShapesInComposite.Length; i++)
                    {
                        if (ShapesInComposite[i].isValid)
                            ShapesInComposite[i].Destroy();
                    }
                }
                ShapesInComposite.Dispose();
            }

            if (!Shape.isValid)
                return;
            
            Shape.Destroy();
            Shape     = default;
        }

        protected virtual void DestroyBody()
        {
            // Destroy the body.
            if (Body.isValid)
            {
                Body.Destroy();
                Body         = default;
            }
        }
        
        /// <summary>
        /// If debugging is enabled in editor, a set of logs will be sent to console just in case something was not set right.
        /// </summary>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        protected virtual void DebugPhysics()
        {
            DebugPhysicsExtra();
            if (!Shape.isValid)
            {
                if (IsCompositeShape)
                {
                    if (!ShapesInComposite.IsCreated)
                    {
                        Debug.LogWarning(
                            $"The Shape was marked not valid for in {GetType().Name} component on game object named: {gameObject.name} " +
                            $"because it is a composite type shaped, but the ShapesInComposite NativeList hasn't been created yet." +
                            $"If this is not a custom component, but a built in SF Component please file a bug report at the GitHub repo.",
                            gameObject);
                    }
                    else if (ShapesInComposite.Length < 1)
                    {
                        Debug.LogWarning(
                            $"The Shape was marked not valid for in {GetType().Name} component on game object named: {gameObject.name} " +
                            $"because it is a composite type shaped, but the ShapesInComposite NativeList has zero elements in it so no default Shape was set making the Shape invalid." +
                            $"If this is a SFTileMapShape there was no valid tiles painted on the TileMap.",
                            gameObject);
                    }
                }
                else
                { 
                    // The default warning message if we haven't created a message for other valid checks.
                    Debug.LogWarning($"The Shape was not valid for the {GetType().Name} on the game object: {gameObject.name}",gameObject);
                }
            }
            else if (!Body.isValid)
            {
                if (Body.type == PhysicsBody.BodyType.Dynamic && Shape.definition.density <= 0)
                    Debug.LogWarning(
                        $"The PhysicsShape's density value was set to be a zero or negative value while the PhysicsBody is RigidbodyType2D is set to Dynamic. This means gravity will not be applied to the PhysicsBody.",
                        gameObject);
                else
                {
                    // The default warning message for body validation if we haven't created a message for the specific valid check that failed.
                    Debug.LogWarning($"The Body was marked not valid for {GetType().Name} component on game object named: {gameObject.name}", gameObject);
                }
            }
        }
        
        /// <summary>
        /// Override this to add custom debug log checking on top of the normal checks in DebugPhysics.
        /// </summary>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        protected virtual void DebugPhysicsExtra(){}
        /// <summary>
        /// Draws a debug render to the game and scene view to allow for visual debugging.
        /// </summary>
        void Unity.U2D.Physics.Extras.IWorldSceneDrawable.Draw()
        {
            if (Shape.world.drawOptions.HasFlag(PhysicsWorld.DrawOptions.SelectedShapes))
            {
                Shape.Draw();
            }
            
            // Finish if we've nothing to draw.
            if (IsCompositeShape
                && ShapesInComposite is { IsCreated: true, Length: > 0 })
            {
                // Finish if we're not drawing selections.
                if (!ShapesInComposite[0].world.drawOptions.HasFlag(PhysicsWorld.DrawOptions.SelectedShapes))
                    return;
            
                // Draw selections.
                foreach (var shape in ShapesInComposite)
                    shape.Draw();
            }
        }

        /// <summary>
        /// Updates the Physics shape when transform changes in the game scene.
        /// <remarks>
        /// This only runs if EditorApplication.isPlaying returns false.
        /// </remarks>
        /// </summary>
        void IWorldSceneTransformChanged.TransformChanged()
        {
            
            if (Body.isValid)
                CreateShape();
        }
    }
}