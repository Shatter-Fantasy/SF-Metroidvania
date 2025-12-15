using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;
using UnityEngine.U2D.Physics.LowLevelExtras;

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
    [DefaultExecutionOrder(PhysicsLowLevelExtrasExecutionOrder.SceneShape)]
    public abstract class SFShapeComponent : MonoBehaviour, IWorldSceneDrawable, IWorldSceneTransformChanged
    {
        /// <summary>
        /// The completed physics shape data struct for the <see cref="SFShapeComponent"/>.
        /// </summary>
        /// <remarks>
        /// Keyword here is completed because you can use <see cref="PhysicsComposer"/> to merge shapes and vertexes into a single shape.
        /// If a <see cref="SFShapeComponent"/> is made from multiple individual shapes and a single shape is created this is the completed merged shape.
        /// <see cref="TileMapShape"/> for an example of this.
        /// </remarks>
        public PhysicsShape Shape;
        
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
                Debug.Log($"The physics shape of the SFShapeComponent on gameobject: {gameObject.name}  wasn't valid when trying to get it's ShapeProxy", this);
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
        /// The owner key hash for the <see cref="Shape"/>
        /// </summary>
        [NonSerialized] public int ShapeOwnerKey;  
        /// <summary>
        /// The owner key hash for the <see cref="Body"/>
        /// </summary>
        [NonSerialized] public int BodyOwnerKey;

        /// <summary>
        /// The object that owns this <see cref="SFShapeComponent"/> <see cref="Shape"/> and the <see cref="Body"/>
        /// </summary>
        protected UnityEngine.Object _owner;
        /// <summary>
        /// The object that owns this <see cref="SFShapeComponent"/> <see cref="Shape"/> and the <see cref="Body"/>
        /// </summary>
        public UnityEngine.Object Owner
        {
            get
            {
                // Return the owner if the shape is valid.
                // If the Shape is valid we know the Body is valid because we use the body to create the Shape.
                // So only need to get the Shape Owner.
                if(Shape.isValid)
                    return Shape.GetOwner();
#if UNITY_EDITOR
                Debug.Log($"When trying to retrieve the Owner of the SFShapeComponent on gameobject: {gameObject.name}, the shape was not valid so it returned a null UnityEngine.Object", this);
#endif
                // Return null if the shape is not valid.
                return null;
            }

            set
            {
                // Don't allow setting a null owner.
                // If we want to erase an owner you call PhysicsBody.Destroy.
                // This will destroy the PhysicsBody and connected PhysicShapes while erasing the owner connection.
                if (value == null)
                    return;
                
                ShapeOwnerKey = Shape.SetOwner(value);
                BodyOwnerKey = Body.SetOwner(value);
            }
        }

        /// <summary>
        /// Keeps track of Shape and owner ids pairs if (<see cref="OwnedShapes"/>) if this <see cref="SFShapeComponent"/>
        /// has <see cref="IsCompositeShape"/> set to true and made from multiple smaller <see cref="PhysicsShape"/>
        /// </summary>
        /// <remarks>
        /// Most times don't ever make this Allocator.Persistent in the actually variable declaration as default value.
        /// This can cause memory leaks under certain cases when running in edit mode.
        /// It is okay to set this by default under some cases, just be careful.
        /// </remarks>
        protected NativeList<OwnedShapes> _ownedShapes;

        /// <summary>
        /// Is the <see cref="Shape"/> created by multiple seperate <see cref="PhysicsShape"/>?
        /// </summary>
        [HideInInspector] public bool IsCompositeShape;

        public object CallbackTarget;
        
        protected void OnEnable()
        {
            if(IsCompositeShape)
                _ownedShapes = new NativeList<OwnedShapes>(Allocator.Persistent);

            CreateShape();

#if UNITY_EDITOR
            WorldSceneTransformMonitor.AddMonitor(this);
#endif
        }
        
        protected void OnDisable()
        {
            DestroyShape();

            if (_ownedShapes.IsCreated)
                _ownedShapes.Dispose();

#if UNITY_EDITOR
            WorldSceneTransformMonitor.RemoveMonitor(this);
#endif
        }
        
        protected void OnValidate()
        {
            if (!isActiveAndEnabled)
                return;

            DestroyShape();
            CreateShape();
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

            // Create the physics body from the physics body definition that the Shape will use.
            CreateBody();
            
            // Make sure the Physics Body is valid before moving to shape creation.
            if (!Body.isValid)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"The Body was not valid for the SFShapeComponent: {name}",this);
#endif
                return;
            }

            // Called from classes inheriting from the abstract class SFShapeComponent.
            // The CreateShapeGeometry is overridden to set up custom shape components for game objects. 
            CreateShapeGeometry();
            
            // Make sure the shape is valid and set this component as it's owner.
            if (!Shape.isValid)
                return;
            
            Shape.SetOwner(this);
        }

        /// <summary>
        /// Creates the <see cref="Shape"/> geometry to use when calling the <see cref="Body.CreateShape()"/> method. 
        /// </summary>
        protected virtual void CreateShapeGeometry()
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

            if (IsCompositeShape && !_ownedShapes.IsCreated)
                return;
            
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

                // Set Owner.
                BodyOwnerKey = Body.SetOwner(this);
            }
        }
        protected virtual void DestroyShape()
        {
            if (_ownedShapes.IsCreated)
            {
                foreach (var ownedShape in _ownedShapes)
                {
                    if (ownedShape.Shape.isValid)
                        ownedShape.Shape.Destroy(updateBodyMass: false, ownerKey: ownedShape.OwnerKey);
                }

                _ownedShapes.Clear();
            }

            if (Body.isValid)
                Body.Destroy(BodyOwnerKey);
        }

        protected virtual void DestroyBody()
        {
            // Destroy the body.
            if (Body.isValid)
            {
                if(Body.isOwned)
                    Body.Destroy(BodyOwnerKey);
                
                Body         = default;
                BodyOwnerKey = 0;
            }
        }
        
        /// <summary>
        /// Draws a debug render to the game and scene view to allow for visual debugging.
        /// </summary>
        void IWorldSceneDrawable.Draw()
        {
            if (Shape.world.drawOptions.HasFlag(PhysicsWorld.DrawOptions.SelectedShapes))
            {
                Shape.Draw();
            }
            
            // Finish if we've nothing to draw.
            if (IsCompositeShape
                && _ownedShapes is { IsCreated: true, Length: > 0 })
            {
                // Finish if we're not drawing selections.
                if (!_ownedShapes[0].Shape.world.drawOptions.HasFlag(PhysicsWorld.DrawOptions.SelectedShapes))
                    return;
            
                // Draw selections.
                foreach (var ownedShape in _ownedShapes)
                    ownedShape.Shape.Draw();
            }
        }

        /// <summary>
        /// Updates the Physics shape when transform changes in the game scene. 
        /// </summary>
        void IWorldSceneTransformChanged.TransformChanged()
        {
            if (Body.isValid)
                CreateShape();
        }
    }
}