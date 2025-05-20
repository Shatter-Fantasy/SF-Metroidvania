using System;

using SF.Characters.Controllers;

using UnityEngine;

namespace SF.AIModule
{
	/// <summary>
	/// A the base class for components that allow giving AI the ability to the actions.
	/// These can be controllerd by an AIBrain component or if you only need one action for a 
	/// character you can just use a standalone AIAction.
	/// </summary>
	[System.Serializable]
	public abstract class AIActionBase : MonoBehaviour, IAIAction
	{
		public bool IsEnabled = true;
        [NonSerialized] public bool IsControlledByAIBrain = false;

        protected Controller2D Controller2D;

		public abstract void Init();
		public abstract void DoAction();
    }
}
