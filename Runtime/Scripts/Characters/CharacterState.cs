using System;
using UnityEngine;

namespace SF.Characters
{
	public enum CharacterType
	{
		Player,
		Enemy,
		AI
	}
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
		Paused = 512
	}
	public enum CharacterStatus
	{
		Alive,
		Dead,
	}

    public enum StatusEffect
    {
        Normal,
        Beserk,
		Weakened,
		Bleeding,
		Confused
    }

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
		
		public CharacterStatus CharacterStatus;

		[UnityEngine.SerializeField] private StatusEffect _statusEffect;
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
