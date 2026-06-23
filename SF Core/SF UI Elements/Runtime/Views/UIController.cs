using UnityEngine;
using UnityEngine.UIElements;

namespace SF.UIModule
{
	/// <summary>
	/// The wrapper class for anything that controls the logic of an <see cref="UIView"/> or a class that inherits from <see cref="UIView"/>.
	/// <remarks>
	///		There can be multiple UIController using the same <see cref="UnityEngine.UIElements.UIDocument"/>.
	///		Example the main menu has sub <see cref="UIView"/> that display a specific menu like inventory, options, and ect.
	///		Depending on the submenu it might have its own UIController class for its logic.
	/// </remarks>
	/// </summary>
	public abstract class UIControllerBase : MonoBehaviour
	{
		public abstract void ShowView();
		public abstract void HideView();
	}
	
	/// <summary>
	/// The wrapper class for anything that controls the logic of an <see cref="UIView"/> or a class that inherits from <see cref="UIView"/>.
	/// <remarks>
	///		There can be multiple UIController using the same <see cref="UnityEngine.UIElements.UIDocument"/>.
	///		Example the main menu has sub <see cref="UIView"/> that display a specific menu like inventory, options, and ect.
	///		Depending on the submenu it might have its own UIController class for its logic.
	/// </remarks>
	/// </summary>
    public abstract class UIController : UIControllerBase
	{
		protected PanelRenderer _panelRenderer;
	    
		protected void Awake()
		{
			if(_panelRenderer == null)
			{
				// If _panelRenderer is null try to find one on the gameobject.
				if(!TryGetComponent(out _panelRenderer))
				{
#if UNITY_EDITOR
					Debug.LogWarning("There was no Main Menu UI Document found.", gameObject);
#endif 
					return;
				}
			} // End of null check and attempting to find a UI Document for Main Menu.
			
			
			// If the _panelRenderer was still null after trying to find one OnAwake is never called. 
			OnAwake();
		}


		/// <summary>
		/// Override this to add custom Awake logic to UIControllers.
		/// If a class inheriting from the UIController needs to delay the RegisterUIReloadCallback override the OnAwake.
		/// <example>
		///	Delaying an Inventory RegisterUIReloadCallback till after the inventory database or player inventory has been initiailized.
		/// </example>
		/// </summary>
		protected virtual void OnAwake() { }

		protected virtual void OnEnable()
		{
			_panelRenderer.RegisterUIReloadCallback(OnUIControllerUIReloaded);
		}
		
		protected virtual void OnDisable()
		{
			_panelRenderer.UnregisterUIReloadCallback(OnUIControllerUIReloaded);
		}
		
		/// <summary>
		/// Called when the <see cref="_panelRenderer"/> Panel Settings or properties change, or when the component is enabled.
		/// Important to prevent double callbacks. Unity also calls this once when the Panel Render is first created during the scene start up they are in.
		/// So Panel Render calls this once during scene start than again during OnEnable the normal OnEnable only at the first frame of the scene the Panel Render is in.
		/// So frame one of new scene loaded calls this twice. Be careful to not double any list or collection because of this. 
		/// </summary>
		/// <param name="panelRenderer"></param>
		/// <param name="rootElement"></param>
		protected abstract void OnUIControllerUIReloaded(PanelRenderer panelRenderer, VisualElement rootElement, int version);
	}
}
