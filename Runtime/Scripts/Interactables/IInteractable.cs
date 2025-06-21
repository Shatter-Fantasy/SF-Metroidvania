using System;

using SF.Characters.Controllers;

using UnityEngine;

namespace SF.Interactables
{
	public enum InteractableMode
	{
		Collision,
		Input,
		RayCast, // Used to do interaction during ray cast checks only.
		ItemUse
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
	}
	
	/// <summary>
	/// Allows any object to interact with the component implementing this interface,
	/// while also passing in any type of data that might need to be read during the interaction.
	/// See <see cref="SF.DataManagement.SaveStation.Interact(PlayerController)"/> for an example.
	/// </summary>
	/// <typeparam name="T">The data that needs to be passed in during an interaction.</typeparam>
	public interface IInteractable<in T> : IInteractable
	{
		void Interact(T interactingComponent);
	}
}