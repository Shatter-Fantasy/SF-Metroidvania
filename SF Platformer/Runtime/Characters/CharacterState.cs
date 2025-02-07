using System;

namespace SF.Characters
{
	public enum CharacterType
	{
		Player,
		Enemy,
		AI
	}
	[Flags]
	public enum MovementState
	{
		None,
		Idle,
		Crouching,
		Walking,
		Running,
		Jumping,
		Falling, 
		Gliding,
		Climbing,
		ClimbingIdle,
	}
	public enum CharacterStatus
	{
		Alive,
		Dead
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
		public MovementState MovementState;
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
	}
}
