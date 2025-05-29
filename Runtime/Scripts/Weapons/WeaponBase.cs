using System.Collections.Generic;

using SF.Characters;
using SF.Characters.Controllers;
using SF.CombatModule;

using UnityEngine;

namespace SF.Weapons
{
    public abstract class WeaponBase : MonoBehaviour, IWeapon
    {
        public int WeaponDamage = 1;

        [SerializeField] protected ComboType ComboType;
        public List<ComboAttack> ComboAttacks = new();
        [SerializeField] protected Vector2 _knockBackForce;
        [SerializeField] protected CharacterRenderer2D _character2D;
        [SerializeField] protected Controller2D _controller2D;
        [SerializeField] protected ContactFilter2D _hitBoxFilter;

        [SerializeField] protected Timer _attackTimer;
        
        public bool OnCooldown { get; protected set;}
        public System.Action UseCompleted;
        
        public virtual void Initialize(Controller2D controller2D = null)
        {
            _controller2D = controller2D;
            
            
            if(_controller2D != null)
                _controller2D.OnDirectionChanged += OnDirectionChange;
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
