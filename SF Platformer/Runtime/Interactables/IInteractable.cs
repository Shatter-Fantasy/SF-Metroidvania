using System;

using SF.Characters.Controllers;

using UnityEngine;

namespace SF.Interactables
{
	public enum InteractableMode
	{
		Collision,
		Input,
		RayCast // Used to do interaction during ray cast checks only.
	}
	/// <summary>
	/// Allows any object to interact with the component implementing this interface.
	/// </summary>
	public interface IInteractable
	{
		InteractableMode InteractableMode { get; set; }

        // TODO: There has to be a better way than just having two functions.
        // Need to look into a cleaner code design.

		void Interact();


        /// <summary>
        /// Allows for player only interactions.
        /// </summary>
        /// <param name="controller"></param>
        void Interact(PlayerController controller);
	}

    /// <summary>
    /// Allows any object to interact with the component implementing this interface.
    /// Also adds the ability to pass in Components if the interactable object needs a reference.
    /// </summary>
    public interface IInteractableComponent : IInteractable
	{
        /// <summary>
        /// Tells an interactable object to began the interaction command 
        /// and also allows for passing in any type of value that might be needed.
        /// </summary>
        /// <remarks>
        ///		Random example for why we allow passing in any component:
        ///		
        ///		You could have a health script passed into an interaction for objects that heal 
        ///		the player on interacting with them. Think a save station that fully heals you when used.
        /// </remarks>
        /// <typeparam name="T">Any value needing to be passed and used during the interaction.</typeparam>
        /// <param name="interactingObject"></param>
        void Interact<T>(T interactingObject = null) where T : Component;
    }
}