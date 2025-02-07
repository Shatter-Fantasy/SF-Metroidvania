using UnityEngine;

namespace SF.AbilityModule.Characters
{
    public abstract class CharacterAbility : ICharacterAbility
    {
		public bool IsEnabled = true;

		[System.NonSerialized] protected GameObject _owner;
	}
}
