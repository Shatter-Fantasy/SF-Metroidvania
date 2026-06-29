using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.U2D.Physics;
using UnityEngine;

#if  UNITY_EDITOR
using UnityEditor;
#endif

namespace SF.U2D.Physics
{
    public interface IPreSolveShapeCallback
    {
        bool OnPreSolve2D(PhysicsEvents.PreSolveEvent preSolveEvent,SFShapeComponent callingShapeComponent);
    }
    
    public interface IBodyUpdate2DCallback
    {
        void OnBodyUpdate2D(PhysicsEvents.BodyUpdateEvent bodyUpdateEvent, SFShapeComponent callingComponent);
    }
    
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
    [BurstCompile]
    [Icon("Packages/shatterfantasy.sf-metroidvania/Editor/Icons/SceneBody.png")]
    public abstract class SFShapeComponent : MonoBehaviour,  PhysicsCallbacks.ITransformChangedCallback,
        ITriggerShapeCallback, PhysicsCallbacks.ITriggerCallback,
        IPreSolveShapeCallback, PhysicsCallbacks.IPreSolveCallback,
        IBodyUpdate2DCallback, PhysicsCallbacks.IBodyUpdateCallback,
        IContactShapeBegin2DCallback, IContactShapeEnd2DCallback,
        PhysicsCallbacks.IContactCallback
    {

        protected PhysicsShape _shape;
        /// <summary>
        /// The completed physics shape data struct for the <see cref="SFShapeComponent"/>.
        /// </summary>
        /// <remarks>
        /// Keyword here is completed because you can use <see cref="PhysicsComposer"/> to merge shapes and vertexes into a single shape.
        /// If a <see cref="SFShapeComponent"/> is made from multiple individual shapes and a single shape is created this is the completed merged shape.
        /// <see cref="SF.U2D.Physics.SFTileMapShape"/> for an example of this.
        /// </remarks>
        public ref PhysicsShape Shape => ref _shape;

        public PhysicsWorld World => Body.isValid ? Body.world : PhysicsWorld.defaultWorld;

        public virtual void SetShape<TGeometryType>(TGeometryType geometryType) where  TGeometryType : struct
        {
            if (!_shape.isValid)
                return;

            switch (geometryType)
            {
                case CircleGeometry circleGeometry:
                {
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
#endif
                    break;
                }
            }
        }

        [NonSerialized] public NativeList<PhysicsShape> ShapesInComposite;
        
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

        [NonSerialized] public PhysicsBody Body;
        public PhysicsBodyDefinition BodyDefinition = PhysicsBodyDefinition.defaultDefinition;

        [NonSerialized] public PhysicsWorld PhysicsWorld;

        /// <summary>
        /// Is the <see cref="Shape"/> created by multiple separate <see cref="PhysicsShape"/>?
        /// </summary>
        [HideInInspector] public bool IsCompositeShape;
        
        /// <summary>
        /// Should the <see cref="Shape"/> size be scaled with the game objects transform.
        /// </summary>
        public bool ScaleSize = true;
        
        public Vector2 Offset = Vector2.zero; 
        
        /// <summary>
        /// Should the Delaunay algorithm be used for creating meshes using <see cref="PhysicsComposer"/>.
        /// </summary>
        protected bool _useDelaunay;

        /// <summary>
        /// The callback targets has to implement the interfaces for the type of Physics Event you want to have sent to it.
        /// Because of this you can not set a GameObject for example as the callback target currently as of 6.3
        /// </summary>
        private readonly List<IBodyUpdate2DCallback> _bodyUpdateTargets = new();
        private readonly List<IContactShapeBegin2DCallback> _contactBegin2DTargets = new();
        private readonly List<IContactShapeEnd2DCallback> _contactEnd2DTargets = new();
        private readonly List<ITriggerShapeBegin2DCallback> _triggerBegin2DTargets = new();
        private readonly List<ITriggerShapeEnd2DCallback> _triggerEnd2DTargets = new();
        private readonly List<IPreSolveShapeCallback> _preSolveTargets = new();

        public Action ShapeCreatedHandler;
        public Action ShapeDestroyedHandler;
        
        protected void OnEnable()
        {
            PreEnabled();
            CreateShape();
            
            DebugPhysics();
        }

        /// <summary>
        /// Override to set up required components when first adding a SFShapeComponent class to a gameobject.
        /// Example <see cref="SFTileMapShape"/> requires a TileMap component to be set before generating the <see cref="Shape"/>. 
        /// </summary>
        protected virtual void PreEnabled() { }
        
        /// <summary>
        /// Used to clean up any resources before changing cleaning up the <see cref="Shape"/> and <see cref="Body"/>.
        /// </summary>
        protected virtual void PreDisable() { }
        
        protected void OnDisable()
        {
            PreDisable();
            DestroyBody();
            DestroyShape();
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
            
            _shape.callbackTarget = this;
            _shape.SetOwnerUserData(new PhysicsUserData()
            {
                objectValue = gameObject
            });
            ShapeCreatedHandler?.Invoke();
        }

        /// <summary>
        /// Creates the <see cref="Shape"/> geometry to use when calling the <see cref="PhysicsBody.CreateShape(PolygonGeometry)"/> method or other CreateShape method. 
        /// </summary>
        protected virtual void CreateBodyShapeGeometry()
        {
            // For the abstract method just creating a example shape for people to see how to do.
            _shape = Body.CreateShape(PolygonGeometry.CreateBox(Vector2.one));
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
            BodyDefinition.rotation = PhysicsRotate.FromDegrees(PhysicsMath.ToRotation2D(transform.rotation, 
                PhysicsWorld.transformPlane));
            
            // Create the physics body to inject into the shape when creating it.
            Body = PhysicsBody.Create(world:PhysicsWorld, definition: BodyDefinition);
            if (Body.isValid)
            {
                // Set the transform object.
                Body.transformObject      = transform;
                Body.callbackTarget       = this;
                
                Body.SetOwnerUserData(new PhysicsUserData()
                {
                    objectValue = gameObject
                });
                
                Body.userData = new()
                {
                    objectValue = gameObject
                };
                
                
            }
            if(PhysicsWorld.isValid)
                PhysicsWorld.RegisterTransformChange(transform,this);
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
            _shape     = default;
            ShapeDestroyedHandler?.Invoke();
        }

        protected virtual void DestroyBody()
        {
            // Destroy the body.
            if (Body.isValid)
            {
                Body.Destroy();
                Body         = default;
            }
            
            if(PhysicsWorld.isValid)
                PhysicsWorld.UnregisterTransformChange(transform,this);
        }

#region Physic Event Callbacks
    
#region BodyUpdate Callbacks
        
        
        public void AddBodyUpdateCallbackTarget<TBodyUpdateCallback>(TBodyUpdateCallback target) 
            where TBodyUpdateCallback : IBodyUpdate2DCallback
        {
            _bodyUpdateTargets?.Add(target);
        }
        
        public void RemoveBodyUpdateCallbackTarget<TBodyUpdateCallback>(TBodyUpdateCallback target) 
            where TBodyUpdateCallback : IBodyUpdate2DCallback
        {
            _bodyUpdateTargets?.Remove(target);
        }
        
        private void OnBodyUpdateCallbacks(PhysicsEvents.BodyUpdateEvent bodyUpdateEvent)
        {
            // The top if and foreach will be removed after setting up the new IContactCallback interfaces.
            if (_bodyUpdateTargets is { Count: < 1 }) 
                return;
            
            foreach (var target in _bodyUpdateTargets)
            {
                target.OnBodyUpdate2D(bodyUpdateEvent, this);
            }
        }
        
        public void OnBodyUpdate2D(PhysicsEvents.BodyUpdateEvent bodyUpdateEvent, SFShapeComponent callingComponent)
        {
            OnBodyUpdate2D(bodyUpdateEvent);
        }
       
        public void OnBodyUpdate2D(PhysicsEvents.BodyUpdateEvent bodyUpdateEvent)
        {
            OnBodyUpdateCallbacks(bodyUpdateEvent);
        }
        
#endregion
#region Trigger Callbacks
    
        public void AddTriggerCallbackTarget<TTriggerCallback>(TTriggerCallback target) 
            where TTriggerCallback : ITrigger2DCallbackBase
        { 
            // Objects can implement both ITriggerShapeBegin2DCallback and ITriggerShapeEnd2DCallback, so don't use else if.
            if(target is ITriggerShapeBegin2DCallback begin2DCallback)
                    _triggerBegin2DTargets.Add(begin2DCallback);
            if(target is ITriggerShapeEnd2DCallback end2DCallback)
               _triggerEnd2DTargets.Add(end2DCallback);
        }
        
        public void RemoveTriggerCallbackTarget<TTriggerCallback>(TTriggerCallback target) 
            where TTriggerCallback : ITrigger2DCallbackBase
        {
            // Objects can implement both ITriggerShapeBegin2DCallback and ITriggerShapeEnd2DCallback, so don't use else if.
            if(target is ITriggerShapeBegin2DCallback begin2DCallback)
                _triggerBegin2DTargets.Remove(begin2DCallback);
            if(target is ITriggerShapeEnd2DCallback end2DCallback)
                _triggerEnd2DTargets.Remove(end2DCallback);
        }
        
        private void OnTriggerBeginCallbacks(PhysicsEvents.TriggerBeginEvent beginEvent)
        {
            if(_triggerBegin2DTargets is { Count: < 1 })
                return;
            
            foreach (var target in _triggerBegin2DTargets)
            {
                target.OnTriggerBegin2D(beginEvent, this);
            }
        }
        
        private void OnTriggerEndCallbacks(PhysicsEvents.TriggerEndEvent endEvent)
        {
            if(_triggerEnd2DTargets is { Count: < 1 })
                return;

            foreach (var target in _triggerEnd2DTargets)
            {
                target.OnTriggerEnd2D(endEvent, this);
            }
        }
#endregion
    
        public void AddContactCallbackTarget<TContactCallback>(TContactCallback target) 
            where TContactCallback : IContact2DCallbackBase
        {
            // Objects can implement both IContactShapeBegin2DCallback and IContactShapeEnd2DCallback, so don't use else if.
            if (target is IContactShapeBegin2DCallback begin2DCallback)
                _contactBegin2DTargets.Add(begin2DCallback);
            if (target is IContactShapeEnd2DCallback end2DCallback)
                _contactEnd2DTargets.Add(end2DCallback);
        }
        
        public void RemoveContactCallbackTarget<TContactCallback>(TContactCallback target) 
            where TContactCallback : IContact2DCallbackBase
        {
            // Objects can implement both IContactShapeBegin2DCallback and IContactShapeEnd2DCallback, so don't use else if.
            if (target is IContactShapeBegin2DCallback begin2DCallback)
                _contactBegin2DTargets.Remove(begin2DCallback);
            if (target is IContactShapeEnd2DCallback end2DCallback)
                _contactEnd2DTargets.Remove(end2DCallback);
        }
        
        private void OnContactBeginCallbacks(PhysicsEvents.ContactBeginEvent beginEvent)
        {
            if(_contactBegin2DTargets is { Count: < 1 })
                return;

            foreach (var target in _contactBegin2DTargets)
            {
                target.OnContactBegin2D(beginEvent, this);
            }
        }
        
        private void OnContactEndCallbacks(PhysicsEvents.ContactEndEvent endEvent)
        {
            if(_contactEnd2DTargets is { Count: < 1 })
                return;
            
            foreach (var target in _contactEnd2DTargets)
            {
                target.OnContactEnd2D(endEvent, this);
            }
        }
        
        public void OnContactBegin2D(PhysicsEvents.ContactBeginEvent beginEvent)
        {
            OnContactBeginCallbacks(beginEvent);
        }

        public void OnContactEnd2D(PhysicsEvents.ContactEndEvent endEvent)
        {
            OnContactEndCallbacks(endEvent);
        }
        

        public void OnContactBegin2D(PhysicsEvents.ContactBeginEvent beginEvent, SFShapeComponent callingShapeComponent)
        {
            OnContactBegin2D(beginEvent);
        }

        public void OnContactEnd2D(PhysicsEvents.ContactEndEvent endEvent, SFShapeComponent callingShapeComponent)
        {
            OnContactEnd2D(endEvent);
        }
        
        
        public void AddPreSolveCallbackTarget(IPreSolveShapeCallback target)
        {
            _preSolveTargets.Add(target);
        }


        public virtual void OnTriggerBegin2D(PhysicsEvents.TriggerBeginEvent beginEvent)
        {
            OnTriggerBeginCallbacks(beginEvent);
        }

        public virtual void OnTriggerEnd2D(PhysicsEvents.TriggerEndEvent endEvent)
        {
            OnTriggerEndCallbacks(endEvent);
        }


        public void OnTriggerBegin2D(PhysicsEvents.TriggerBeginEvent beginEvent, SFShapeComponent callingShapeComponent)
        {
            OnTriggerBegin2D(beginEvent);
        }

        public void OnTriggerEnd2D(PhysicsEvents.TriggerEndEvent endEvent, SFShapeComponent callingShapeComponent)
        {
            OnTriggerEnd2D(endEvent);
        }

        
        public bool OnPreSolve2D(PhysicsEvents.PreSolveEvent preSolveEvent)
        {
            return OnPreSolve2D(preSolveEvent, this);
        }

        public bool OnPreSolve2D(PhysicsEvents.PreSolveEvent preSolveEvent, SFShapeComponent callingShapeComponent)
        {
            if(_preSolveTargets == null || _preSolveTargets.Count < 1)
                return true;

            foreach (var target in _preSolveTargets)
            {
                target.OnPreSolve2D(preSolveEvent, this);
            }

            return true;
        }
#endregion
#region Transform Callbacks
        
        public void OnTransformChanged(PhysicsEvents.TransformChangeEvent transformChangeEvent)
        {
            Body.transform  = new PhysicsTransform(transform.position, PhysicsRotate.identity);
        } 
#endregion
        
        /// <summary>
        /// If debugging is enabled in editor, a set of logs will be sent to console just in case something was not set right.
        /// </summary>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public virtual void DebugPhysics()
        {
            // We allow people to do their own custom debugging for custom components even when UsingDebugMode is disabled.
            DebugPhysicsExtra();
            
            if (!SFPhysicsManager.UsingDebugMode)
                return;
            
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
        /// <remarks>
        /// This runs even when <see cref="SFPhysicsManager.UsingDebugMode"/> is set to false
        /// and is only ran inside of the editor at the current moment. Runtime support coming soon.
        /// </remarks>
        /// </summary>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        protected virtual void DebugPhysicsExtra(){}

        public PhysicsAABB CalculateAABB()
        {
            return GetAABB(_shape);
        }
        
        
        public static PhysicsAABB GetAABB(in PhysicsShape physicsShape)
        {
            switch (physicsShape.shapeType)
            {
                case PhysicsShape.ShapeType.Circle :
                    return physicsShape.circleGeometry.CalculateAABB(physicsShape.transform);
                case PhysicsShape.ShapeType.Capsule:
                    return physicsShape.capsuleGeometry.CalculateAABB(physicsShape.transform);
                case PhysicsShape.ShapeType.Segment:
                    return physicsShape.segmentGeometry.CalculateAABB(physicsShape.transform);
                case PhysicsShape.ShapeType.Polygon:
                    return physicsShape.polygonGeometry.CalculateAABB(physicsShape.transform);
                case PhysicsShape.ShapeType.ChainSegment:
                    return physicsShape.chainSegmentGeometry.CalculateAABB(physicsShape.transform);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

#if  UNITY_EDITOR
        public void TransformChanged()
        {
            UpdateShape();
        }

        [ContextMenu("Check Selected Shape Filters")]
        public static void CheckShapeFilters()
        {
            var gameObjects = Selection.gameObjects;
            if (gameObjects.Length < 2)
                return;

            if (gameObjects[0].TryGetComponent(out SFShapeComponent shapeOne)
                && gameObjects[1].TryGetComponent(out SFShapeComponent shapeTwo))
            {
                if (!shapeOne._shape.isValid || !shapeTwo._shape.isValid)
                {
                    Debug.Log(" One of the shapes was not valid.");
                    return;
                }
            }
        }
#endif

  
    }
}