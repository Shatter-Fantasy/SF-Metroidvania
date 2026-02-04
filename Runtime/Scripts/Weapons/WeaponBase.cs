using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;
using UnityEngine.Serialization;

namespace SF.Weapons
{
    using Characters;
    using CombatModule;
    using PhysicsLowLevel;
    
    public enum AttackState
    {
        NotAttacking,
        Attacking,
        Charging, // Can be for spells, holding a bow pull, and ect.
        Aiming // Aiming bow or some projectile.
    }
    
    public abstract class WeaponBase : MonoBehaviour, IWeapon
    {
        public int WeaponDamage = 1;
        
       
        [SerializeField] protected Vector2 _knockBackForce;
        [SerializeField] protected CharacterRenderer2D _character2D;
        [SerializeField] protected ControllerBody2D _controllerBody2D;
        
        [SerializeField] protected PhysicsQuery.QueryFilter _filter;
        
        [SerializeField] protected Timer _attackTimer;
        
        public bool OnCooldown { get; protected set;}
        
        public System.Action UseCompleted;
        
        public virtual void Initialize(ControllerBody2D controllerBody2D = null)
        {
            _controllerBody2D = controllerBody2D;
            
            if(_controllerBody2D != null)
                _controllerBody2D.OnDirectionChanged += OnDirectionChange;
        }
        
        public virtual void Use()
        {

        }

        /// <summary>
        /// Mainly used in subclasses for things needing updated when character changes directions.
        /// Flipping melee hit boxes when characters change direction
        /// Changing projectile firing directions for range weapons or mixed style weapons. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="newDirection"></param>
        protected virtual void OnDirectionChange(object sender, Vector2 newDirection) { }
    }
}
