using System;
using System.Collections.Generic;
using SF.Characters;
using SF.Physics;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;
using UnityEngine.U2D.Physics.LowLevelExtras;

namespace SF.PhysicsLowLevel
{
    /// <summary>
    /// The base class for high performance physics controller that uses Unity's Low Level Physics2D system to allow for
    /// highly customizable physics that can even destroy, edit, and manipulate physics shapes in real time.
    /// </summary>
    /// <remarks>
    /// This can be used for Character Controllers, real time editable terrain (think Team 17 Worms games), and fast pace physics based 2D games that need high performance. 
    /// </remarks>
    [DisallowMultipleComponent]
    [ExecuteAlways]
    [DefaultExecutionOrder(PhysicsLowLevelExtrasExecutionOrder.SceneBody)]
    [Icon("Packages/com.unity.2d.physics.lowlevelextras/Editor/Icons/SceneBody.png")]
    public class ControllerBody2D : PhysicController2D, IWorldSceneTransformChanged, IWorldSceneDrawable
    {
        public CharacterState CharacterState;
        
        #region Low Level Physics Shape
        public PhysicsShapeDefinition PhysicsShapeDefinition;
        public CapsuleGeometry CapsuleGeometry;
        public PhysicsShape PhysicsShape;
        #endregion
        
        #region  Low Level Physics Settings.
        
        /// <summary>
        /// Should the low level physics 2d world settings assigned in the Unity project settings be used for this Controller2D.
        /// </summary>
        [Header(" Physics Settings")]
        [SerializeField] private bool _useDefaultWorld = true;
        
        /// <summary>
        /// The active <see cref="SceneWorld"/> that this ControllerBody2D is being simulated in.
        /// </summary>
        [SerializeField] private SceneWorld _sceneWorld;
        
        /// <summary>
        /// The <see cref="PhysicsBodyDefinition"/> that defines the physics properties of the controller's <see cref="ControllerBody"/>
        /// </summary>
        /// <remarks>
        /// This includes <see cref="RigidbodyType2D"/>, transform syncing for the controlling object, <see cref="RigidbodyConstraints2D"/>
        /// the calculated velocities, gravity scale, and rotational data of the <see cref="PhysicsBody"/> angular velocity. 
        /// </remarks>
        public PhysicsBodyDefinition PhysicsBodyDefinition;


        /// <summary>
        /// A list of MonoBehaviours that can have physics events be registered to.
        /// </summary>
        /// <remarks>
        /// This allows having components on different game objects still react to collision, ray hit,
        /// and contact events without needing to have a physics shape/body on them as well. 
        /// </remarks>
        public List<MonoBehaviour> CallbackTargets = new ();
        
        /// <remarks>
        /// <see cref="PhysicsBody"/> is the low level physics version of Rigidbody2D.
        /// It just has a lot more options and customization to it compared to the Rigidbody2D component.
        /// </remarks>
        [NonSerialized] public PhysicsBody ControllerBody;

        /// <summary>
        /// Owner key tells the physics backend what object is allowed to create or destroy a physics body.
        /// This allows for destroying a game object and keeping the physics body alive to do some interesting physics simulation tricks.
        /// </summary>
        private int _ownerKey;
        #endregion
        
        [Header("Movement Booleans")]
        public bool IsRunning;
        public bool IsSwimming;
        public bool IsJumping;
        public bool IsFalling;
        public bool IsGliding;
        public bool IsCrouching;
        
        public bool IsClimbing
        {
            get { return _isClimbing; }
            set
            {
                // Don't do any other checks if we are setting is climbing to the exact same value.
                if (_isClimbing == value)
                    return;
				
                _isClimbing = value; 

                if(!_isClimbing)
                {
                    CurrentPhysics.GravityScale = DefaultPhysics.GravityScale;
                    _character.CanTurnAround = true;
                }
                else
                {
                    CurrentPhysics.GravityScale = 0;
                    _character.CanTurnAround = false;
                }
            }
        }
        [SerializeField] private bool _isClimbing;

        public BodyCollisionInfo CollisionInfo = new BodyCollisionInfo();
        
        protected CharacterRenderer2D _character;
        
        #region Low Level SceneBody EventHandler
        public delegate void ControllerBodyCreateEventHandler(ControllerBody2D sceneBody);
        public delegate void ControllerBodyDestroyEventHandler(ControllerBody2D sceneBody);
        
        public event ControllerBodyCreateEventHandler CreateBodyEvent;
        public event ControllerBodyDestroyEventHandler DestroyBodyEvent;
        #endregion

        /// <summary>
        /// Should the debug rendering and log system be active.
        /// </summary>
        /// <remarks>
        /// Debug rendering works in builds and in editor, but requires a device that supports compute shaders.
        /// WebGL doesn't work, but WebGPU does work.
        /// </remarks>
        public bool DebugMode = false;
        
        #region IWorldSceneTransformChanged implementation
        /// <summary>
        /// Updates the PhysicsBody in the editor to allow updating the visual renderer position of the PhysicsShape without entering playmode.
        /// </summary>
        public void TransformChanged()
        {
            if (ControllerBody.isValid)
                CreatePhysicsBody();
        }
        #endregion

        #region IWorldSceneDrawable implementation
        /// <summary>
        /// Draws a visual renderer for the <see cref="PhysicsBody"/> used by this Controller2D in scene to allow visually debugging.
        /// </summary>
        void IWorldSceneDrawable.Draw()
        {
            if (!ControllerBody.isValid || !DebugMode)
                return;

            // Draw if we're drawing selections.
            if (ControllerBody.world.drawOptions.HasFlag(PhysicsWorld.DrawOptions.SelectedBodies))
                ControllerBody.Draw();
        }
        #endregion
                
        protected override void Move()
        {
            if (!ControllerBody.isValid)
                return;
            
            /* Slope Calculations
            if(_onSlope)
            {
                _calculatedVelocity *= _slopeMultiplier;
                // TODO: Make the ability to walk up slopes.
                //_calculatedVelocity = Vector3.ProjectOnPlane(_calculatedVelocity, _slopeNormal);
            } */

            if (IsFrozen && CollisionInfo.IsGrounded)
                _calculatedVelocity.x = 0;
            
            if(CharacterState.CharacterStatus == CharacterStatus.Dead)
                _calculatedVelocity = Vector2.zero;

            if(_externalVelocity != Vector2.zero)
            {
                _calculatedVelocity = _externalVelocity;
                _externalVelocity = Vector2.zero;
            }
            
            ControllerBody.linearVelocity = _calculatedVelocity;
        }

        protected override void CalculateHorizontal()
        {
            if(IsClimbing)
            {
                _calculatedVelocity.x = 0;
                return;
            }

            if(Direction.x != 0)
            {
                // We only have to do a single clamp because than Direction.x takes care of it being negative or not when being multiplied.
                ReferenceSpeed = Mathf.Clamp(ReferenceSpeed, 0, CurrentPhysics.GroundMaxSpeed);

                // TODO: When turning around erase previously directional velocity.
                // If it is kept the player could slide in the previous direction for a second before running the new direction on smaller ground acceleration values.
                _calculatedVelocity.x = Mathf.MoveTowards(_calculatedVelocity.x, ReferenceSpeed * Direction.x, CurrentPhysics.GroundAcceleration);
            }
            else
            {
                _calculatedVelocity.x = Mathf.MoveTowards(_calculatedVelocity.x, 0, CurrentPhysics.GroundDeacceleration);
            }
			
            // If we are moving left and not hitting a slope, but an obstacle, stop moving left.
            if (CollisionInfo.IsCollidingLeft && Direction.x < 0 && CollisionInfo.IsCollidingLeft)
            {
                _calculatedVelocity.x = 0;
            }
			
            // If we are moving Right and not hitting a slope, but an obstacle, stop moving Right.
            if (CollisionInfo.IsCollidingRight && Direction.x > 0 && CollisionInfo.IsCollidingRight)
            {
                _calculatedVelocity.x = 0;
            }
        }

        protected override void CalculateVertical()
        {
            
            if(IsClimbing)
            {
                _calculatedVelocity.y = Direction.y * CurrentPhysics.ClimbSpeed.y;
            }
            
            //if(!CollisionInfo.IsGrounded  && !IsClimbing && !_onSlope)
            if(!CollisionInfo.IsGrounded && !IsClimbing)
            {
                // This is related to the formula of: square root of ( -2 (gravity * height) )
                // https://en.wikipedia.org/wiki/Equations_for_a_falling_body#Example
                _calculatedVelocity.y -= Mathf.Sqrt( -2 * -CurrentPhysics.GravityScale * Time.deltaTime); 

                _calculatedVelocity.y = Mathf.Clamp(_calculatedVelocity.y,
                    -CurrentPhysics.TerminalVelocity,
                    CurrentPhysics.MaxUpForce);
            }
        }

        protected override void CalculateMovementState()
        {
            if(CharacterState.CharacterStatus == CharacterStatus.Dead)
                return;

            // TODO: There are some places that set the values outside of this function. Find a way to make it where this function is the only needed one. Example IsJump in the Jumping Ability.

            if(IsClimbing)
            {
                CharacterState.CurrentMovementState = _calculatedVelocity.y != 0 
                    ? MovementState.Climbing 
                    : MovementState.ClimbingIdle;
            }

            // If our velocity is negative we are either falling/gliding.
            //if(_calculatedVelocity.y < 0 && !IsClimbing && !_onSlope)
            if(_calculatedVelocity.y < 0 && !IsClimbing)
            {
                if(IsGliding)
                    CharacterState.CurrentMovementState = MovementState.Gliding;
                else
                {
                    // Need to remove the crouch check when I get the collider calculation more accurate on resizing.
                    if(!IsCrouching)
                        CharacterState.CurrentMovementState = MovementState.Falling;
                }
                IsFalling = true;
                IsJumping = false;
            }

            if(IsJumping)
                CharacterState.CurrentMovementState = MovementState.Jumping;

            if(CollisionInfo.IsGrounded)
            {
                IsFalling = false;

                if(IsCrouching)
                {
                    CharacterState.CurrentMovementState = MovementState.Crouching;
                    return;
                }
				
                CharacterState.CurrentMovementState = (Direction.x == 0) ? MovementState.Idle : MovementState.Walking;
            }
        }


        protected override void OnAwake()
        {
            CollisionInfo = new BodyCollisionInfo();
            CollisionInfo.ControllerBody2D = this;
        }

        protected override void FixedUpdate()
        {
            if (_direction.x != 0)
                _directionLastFrame.x = _direction.x;

            
            if (!PhysicsShape.isValid)
                return;
            
            OnPreFixedUpdate();
            
            // We now use the PhysicsEvent.PostSimulate callback for checking collisions post movement
            // We should make sure this is not skipping first frame of collision though.
            CollisionInfo.CheckCollisions();
            
            CalculateHorizontal();
            CalculateVertical();
            
            Move();
        }
        
        
        public void Reset()
        {
            if (_useDefaultWorld || _sceneWorld != null)
                return;

            // This is super slow, hopefully we don't need to do this.
            _sceneWorld = SceneWorld.FindSceneWorld(gameObject);
        }
        public void ResizeCollider(Vector2 newSize)
        {
            
        }
        public void ResetColliderSize()
        {
            
        }

        public virtual void UpdatePhysicsProperties(MovementProperties movementProperties, 
            PhysicsVolumeType volumeType = PhysicsVolumeType.None)
        {
            CurrentPhysics.GroundSpeed = movementProperties.GroundSpeed;
            CurrentPhysics.GroundRunningSpeed = movementProperties.GroundRunningSpeed;
            CurrentPhysics.GroundAcceleration = movementProperties.GroundAcceleration;
            CurrentPhysics.GroundMaxSpeed = movementProperties.GroundMaxSpeed;

            CurrentPhysics.GravityScale = movementProperties.GravityScale;
            CurrentPhysics.TerminalVelocity = movementProperties.TerminalVelocity;
            CurrentPhysics.MaxUpForce = movementProperties.MaxUpForce;

            PhysicsVolumeType = volumeType;

            ReferenceSpeed = CurrentPhysics.GroundSpeed;
        }
        
        public virtual void ResetPhysics(MovementProperties movementProperties)
        {
            CurrentPhysics = DefaultPhysics;
            PhysicsVolumeType = PhysicsVolumeType.None;

            ReferenceSpeed = CurrentPhysics.GroundSpeed;
        }
        
        protected void OnEnable()
        {
            Reset();
            
            // Register to body recreation.
            if (_sceneWorld != null)
            {
                _sceneWorld.CreateWorldEvent += OnCreateWorld;
                _sceneWorld.DestroyWorldEvent += OnDestroyWorld;
            }

            CreatePhysicsBody();
            
            CreatePhysicsShape();
            DebugPhysics();

            
#if UNITY_EDITOR
            // This allows the editor to keep track of the transform out of play mode and update the scene view for drawing the visuals. 
            WorldSceneTransformMonitor.AddMonitor(this);
#endif
        }



        protected void OnDisable()
        {
            DestroyPhysicsBody();
            DestroyPhysicsShape();
            
            if (_sceneWorld != null)
            {
                _sceneWorld.CreateWorldEvent -= OnCreateWorld;
                _sceneWorld.DestroyWorldEvent -= OnDestroyWorld;
            }

#if UNITY_EDITOR
            WorldSceneTransformMonitor.RemoveMonitor(this);
#endif
        }
        
        private void OnValidate()
        {
            if (!isActiveAndEnabled)
                return;

            // Create a new body.
            CreatePhysicsBody();
            CreatePhysicsShape();
            DebugPhysics();
        }

        /// <summary>
        /// Create the <see cref="PhysicsBody"/> that will be used by this ControllerBody2D and initialize it's properties
        /// from the <see cref="PhysicsBodyDefinition"/>
        /// </summary>
        private void CreatePhysicsBody()
        {
            // Destroy any already existing PhysicsBody for the Controller2D to prevent double physics callbacks.
            DestroyPhysicsBody();
            DestroyPhysicsShape();

            // If we are using the _useDefaultWorld grab its properties from the project settings and assign it locally for use.
            var world = _useDefaultWorld || _sceneWorld == null 
                ? PhysicsWorld.defaultWorld 
                : _sceneWorld.World;
            
            // Grab the transformPlane from the SceneWorld being used.
            var transformPlane = world.transformPlane;
            
            // Sync the starting transform of the PhysicsBody with the GameObjects transform in game.
            PhysicsBodyDefinition.position = PhysicsMath.ToPosition2D(transform.position, transformPlane);
            PhysicsBodyDefinition.rotation = new PhysicsRotate(PhysicsMath.ToRotation2D(transform.rotation, transformPlane));
            
            // Create the _controllerBody in the used SceneWorld with our defined PhysicsBodyDefinition
            ControllerBody = PhysicsBody.Create(world:world, definition: PhysicsBodyDefinition);
            
            // If all the required information has been set up for the _controllerBody we can now add it to the PhysicsWorld simulations.
            if (ControllerBody.isValid)
            {
                // Set the transform object.
                ControllerBody.transformObject = transform;

                /* Set the callback target as this game object to allow for doing a custom filtering in this component
                 that than send the callback events to the defined CallbackTargets list allowing other mono behaviors to also react
                 to callbacks from this ControllerBody2D. */
                ControllerBody.callbackTarget = this;

                // Set Owner.
                _ownerKey = ControllerBody.SetOwner(this);
                
                CreateBodyEvent?.Invoke(this);
            }
        }
        
        /// <summary>
        /// Destroys and cleans up the <see cref="PhysicsBody"/> used by this ControllerBody2D.
        /// </summary>
        private void DestroyPhysicsBody()
        {
            if (ControllerBody.isValid)
            {
                DestroyBodyEvent?.Invoke(this);

                // Because the PhysicsBody _controllerBody is a struct we set it to default and call the clean up Destroy method on it.
                ControllerBody.Destroy(_ownerKey);
                ControllerBody = default;
                _ownerKey = 0;
            }
        }
        
        private void CreatePhysicsShape()
        {
            if (!ControllerBody.isValid)
                return;
            
            PhysicsShape = ControllerBody.CreateShape(CapsuleGeometry, PhysicsShapeDefinition);

            if (PhysicsShape.isValid)
            {
                PhysicsShape.callbackTarget = gameObject;
            }
        }
        
        private void DestroyPhysicsShape()
        {
            if (PhysicsShape.isValid)
            {
                PhysicsShape.Destroy(true, _ownerKey);
                PhysicsShape = default;
                _ownerKey = 0;
            }
        }
        
        /// <summary>
        /// Invoked when the SceneWorld has been created.
        /// </summary>
        /// <param name="sceneWorld"></param>
        private void OnCreateWorld(SceneWorld sceneWorld)
        {
            // If we are not using the default SceneWorld create a new PhysicsBody for the Controller2DBody.
            if (!_useDefaultWorld)
            {
                CreatePhysicsBody();
                CreatePhysicsShape();
                DebugPhysics();
            }
        }
        
        private void OnDestroyWorld(SceneWorld sceneWorld)
        {
            // If we are not using the default SceneWorld destroy the PhysicsBody used by this Controller2DBody and clean it up in memory.
            if (!_useDefaultWorld)
            {
                DestroyPhysicsBody();
                DestroyPhysicsShape();
            }
        }
        
        /// <summary>
        /// If debugging is enabled in editor, a set of logs will be sent to console just in case something was not set right.
        /// </summary>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void DebugPhysics()
        {
            if (!ControllerBody.isValid)
            {
                Debug.LogWarning($"The _controllerBody was not valid for ControllerBody2D component on game object named: {gameObject}", gameObject);
            }

            if (!PhysicsShape.isValid)
            {
                Debug.LogWarning(
                    $"The PhysicsShape was not valid for ControllerBody2D component on game object named: {gameObject}",
                    gameObject);
            }
            else
            {
                if (ControllerBody.bodyType == RigidbodyType2D.Dynamic && PhysicsShape.definition.density <= 0)
                    Debug.LogWarning(
                        $"The PhysicsShape's density value was set to be a zero or negative value while the PhysicsBody is RigidbodyType2D is set to Dynamic. This means gravity will not be applied to the PhysicsBody.",
                        gameObject);
            }
        }
        
    }
}
