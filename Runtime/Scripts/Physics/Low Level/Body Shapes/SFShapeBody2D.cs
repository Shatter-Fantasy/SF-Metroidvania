using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;
using UnityEngine.U2D.Physics.LowLevelExtras;

namespace SF.PhysicsLowLevel
{
    /// <summary>
    /// Base class for all SF Shape Body components that act as a low level equivalent to the high level Unity Collider2D.
    /// </summary>
    [ExecuteAlways]
    [DefaultExecutionOrder(PhysicsLowLevelExtrasExecutionOrder.SceneShape)]
    public class SFShapeBody2D : MonoBehaviour, IWorldSceneDrawable, IWorldSceneTransformChanged
    {
        public PhysicsShapeDefinition ShapeDefinition;
        public PhysicsShape Shape;
        
        public PhysicsBodyDefinition BodyDefinition = PhysicsBodyDefinition.defaultDefinition;

        /// <summary>
        /// The <see cref="PhysicsBody"/> for the <see cref="Shape"/>.
        /// </summary>
        public PhysicsBody Body;

        /// <summary>
        /// Should we use the default <see cref="SceneWorld"/> located in the low level physics settings asset that is assigned in the project settings. 
        /// </summary>
        public bool UseDefaultWorld = true;

        /// <summary>
        /// The cached <see cref="SceneWorld"/> that this object will be simulated in.
        /// </summary>
        public SceneWorld SceneWorld;

        /// <summary>
        /// The id that keeps track of what object owns this <see cref="SFShapeBody2D"/>.
        /// </summary>
        protected int _ownerKey;

        /// <summary>
        /// The target of <see cref="PhysicsEvents"/> callbacks.
        /// </summary>
        public object CallbackTarget;

        /// <summary>
        /// A list of objects that are currently contained inside of <see cref="Shape"/>
        /// </summary>
        public List<IPhysicsShapeContained> ContainedPhysicsShapes = new();
        

        protected NativeList<OwnedShapes> _ownedShapes = new NativeList<OwnedShapes>(Allocator.Persistent);


        protected void CreateBody()
        {
            // Destroy any existing body.
            DestroyBody();

            var world = UseDefaultWorld || SceneWorld == null ? PhysicsWorld.defaultWorld : SceneWorld.World;

            // Fetch the transform plane.
            var transformPlane = world.transformPlane;

            // Create the body at the transform position.
            BodyDefinition.position = PhysicsMath.ToPosition2D(transform.position, transformPlane);
            BodyDefinition.rotation = new PhysicsRotate(PhysicsMath.ToRotation2D(transform.rotation, transformPlane));

            Body = PhysicsBody.Create(world: world, definition: BodyDefinition);
            if (Body.isValid)
            {
                // Set the transform object.
                Body.transformObject = transform;

                // Set the callback target.
                Body.callbackTarget = CallbackTarget;

                // Set Owner.
                _ownerKey = Body.SetOwner(this);
            }
        }

        protected void DestroyBody()
        {
            // Destroy the body.
            if (Body.isValid)
            {
                if(Body.isOwned)
                    Body.Destroy(this._ownerKey);
                
                Body = default;
                _ownerKey = 0;
            }
        }

        protected virtual void CreateShape()
        {

        }

        protected virtual void DestroyShape()
        {
            if (!_ownedShapes.IsCreated)
                return;

            foreach (var ownedShape in _ownedShapes)
            {
                if (ownedShape.Shape.isValid)
                    ownedShape.Shape.Destroy(updateBodyMass: false, ownerKey: ownedShape.OwnerKey);
            }

            _ownedShapes.Clear();
        }

        /// <summary>
        /// Called from editor tools to update the Shape after making changes using editor tools.
        /// Also can be used to force a shape update.
        /// </summary>
        public void UpdateShape() => CreateShape();

        protected virtual void Reset()
        {
        }

        protected virtual void OnEnable()
        {
            Reset();

            // Register to body recreation.
            if (SceneWorld != null)
            {
                SceneWorld.CreateWorldEvent += OnCreateWorld;
                SceneWorld.DestroyWorldEvent += OnDestroyWorld;
            }

            _ownedShapes = new NativeList<OwnedShapes>(Allocator.Persistent);

            // Create the body.
            CreateShape();

#if UNITY_EDITOR
            WorldSceneTransformMonitor.AddMonitor(this);
#endif
        }

        protected virtual void OnDisable()
        {
            DestroyShape();

            if (_ownedShapes.IsCreated)
                _ownedShapes.Dispose();

            // Register to body recreation.
            if (SceneWorld != null)
            {
                SceneWorld.CreateWorldEvent -= OnCreateWorld;
                SceneWorld.DestroyWorldEvent -= OnDestroyWorld;
            }

#if UNITY_EDITOR
            WorldSceneTransformMonitor.RemoveMonitor(this);
#endif
        }

        private void OnCreateWorld(SceneWorld sceneWorld)
        {
            if (!UseDefaultWorld)
                CreateBody();

            CreateShape();
        }

        private void OnDestroyWorld(SceneWorld sceneWorld)
        {
            if (!UseDefaultWorld)
                DestroyBody();

            DestroyShape();
        }

        protected virtual void OnValidate()
        {
            if (!isActiveAndEnabled)
                return;

            CreateShape();
            DestroyShape();
        }

        #region WorldScene Interfaces

        void IWorldSceneTransformChanged.TransformChanged()
        {
            if (Body.isValid)
                CreateShape();
        }

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
        #endregion
        
        
        public override string ToString() => Body.ToString();
        
    }
}