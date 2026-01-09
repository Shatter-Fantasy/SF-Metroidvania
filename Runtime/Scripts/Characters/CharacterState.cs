using System;
using UnityEngine;

namespace SF.Characters
{
	using Weapons;
	
	/// <summary>
	/// The current movement state of the Character.
	///  This is used to tell what animation to play, when physics need to apply gravity, and what movement speed value is used when a character is moved. 
	/// </summary>
	[Flags]
	public enum MovementState : int
	{
		None = 0,
		Idle = 1,
		Crouching = 2,
		Walking = 4,
		Running = 8,
		Jumping = 16,
		Falling = 32, 
		Gliding = 64,
		Climbing = 128,
		ClimbingIdle = 256,
		Paused = 512,
		Attacking = 1024
	}
	
	/// <summary>
	/// The current status of the character's lifespan.
	/// </summary>
	public enum CharacterStatus
	{
		Alive,
		Dead,
		Respawning,
		KnockedOut
	}

	/// <summary>
	/// The characters combat status. 
	/// </summary>
    public enum StatusEffect : int
    {
        Normal = 0,
        Berserk = 1,
		Weakened = 2,
		Bleeding = 4,
		Confused = 8
    }
    
    /// <summary>
    /// The type of character being rendered. Allows different character types will have different vfx logic.
    /// </summary>
    public enum CharacterTypes { Player, Ally, Enemy, NPC}
    
    /// <summary>
    /// Keeps track of all the characters current states for <see cref="MovementState"/>, <see cref="CharacterStatus"/>,
    /// and <see cref="StatusEffect"/>.
    ///
    /// It also has callbacks that can registered to when any form of state change for the character has happened.
    /// </summary>
    [Serializable]
	public class CharacterState
	{
		private MovementState _previousMovementState;
		[SerializeField] private MovementState _currentMovementState;
		public MovementState CurrentMovementState
		{
			get => _currentMovementState;
			set
			{

				if (_currentMovementState == value)
					return;
				
				_previousMovementState = _currentMovementState;
				_currentMovementState = value;
				
			
			}
		}

		public Action OnMovementStateChanged;

		[SerializeField] private AttackState _attackState;

		public AttackState AttackState
		{
			get => _attackState;
			set
			{
				if (_attackState == value)
					return;
				
				AttackStateChangedHandler?.Invoke(value);
				_attackState = value;
			}
		}
		
		public Action<AttackState> AttackStateChangedHandler;
		
		[SerializeField]
		private CharacterStatus _characterStatus;

		public CharacterStatus CharacterStatus
		{
			get => _characterStatus;
			set
			{
				// If we were alive and now dead, invoke on death event.
				if (value == CharacterStatus.Dead && _characterStatus == CharacterStatus.Alive)
				{
					OnDeathHandler?.Invoke();
				}

				_characterStatus = value;
			}
		}
		
		public Action OnDeathHandler;
		
		[SerializeField] private StatusEffect _statusEffect;
		public StatusEffect StatusEffect
		{
			get	{ return _statusEffect;	}
			set
			{
				if(value != _statusEffect)
					StatusEffectChanged?.Invoke(value);
				_statusEffect = value;
			}
		}

		public Action<StatusEffect> StatusEffectChanged;


		public void RestoreMovementState()
		{
			_currentMovementState = _previousMovementState;
		}
	}
}
