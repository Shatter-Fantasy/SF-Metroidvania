using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace SF.UIModule
{
	/// <summary>
	/// Initializes the uxml/visual element assets for a set of UI elements that make up a UI menu/view
	/// and registers the needed events in them. 
	/// </summary>
	/// <remarks>
	/// Multiple views are able to be on screen at once. Each UI View can have UI views in them as sub views.
	/// Each UIView can be seen as an organized set of UI elements that focus on a certain feature or function.
	/// Examples: Each view of the setting menu, the main menu contains several UI views, title screen, and overlays.
	///
	/// Each UI View has an <see cref="UIController"/> for logic that contains the events the UIViews register listens for.
	/// Note each <see cref="UIController"/> can control multiple UIViews. Think about the main menu and how it has logic for the side
	/// buttons to open different sub views like inventory, settings, and journal.
	/// </remarks>
	[Serializable]
    public class UIView : IUIView, IDisposable
    {
	    [SerializeField] protected bool _hideOnAwake = true;
	    
	    // Properties
	    public VisualElement RootElement { get; protected set; }
	    public bool IsHidden => RootElement?.style.display == DisplayStyle.None;
	    
	    /// <summary> The UI Controller that has the logic for the UI View. </summary>
	    protected UIController _uiController;

	    /// <summary>
	    /// If the UView's uxml structure is being instantiated via Templates inside an uxml file.
	    /// If this is set to true the parent of the passed in root object will be used for show/hide
	    /// by declaring the <see cref="TemplateContainer"/> in the initialize as the root object
	    /// </summary>
	    /// <remarks>
	    ///	When <see cref="TemplateContainer"/> are used it is sometimes better to set the <see cref="TemplateContainer"/> as the root object.
	    /// </remarks>
	    [NonSerialized] public bool IsInstancedUXMLTemplate = false;
	    public virtual void Initialize(VisualElement rootElement, UIController uiController = null)
	    {
		    if(uiController != null)
			    _uiController = uiController;
		    
		    RootElement = rootElement;
		    
		    /*	Might be able to use Template instances inside the UXML instead.
				Basically the UIDocument component has the root UIViews VisualTreeAset and
				inside of it's uxml structure the templates that contain the structure for sub views are in it.
				We use the Query system to get the roots for each UIView and go from there.
		     
		    if(_mainViewTreeAsset == null || RootElement == null)
			    return;
		    _mainViewTreeAsset.CloneTree(RootElement);
		    */

		    if(_hideOnAwake)
		    {
			    Hide();
		    }
		    
		    SetVisualElements();
		    RegisterEventCallbacks();
	    }
	    
	    /// <summary>
	    /// Sets up the VisualElements for the UI. Override to customize.
	    /// </summary>
	    protected virtual void  SetVisualElements()
	    {
  
	    }
	    
	    /// <summary>
	    /// Registers callbacks for buttons in the UI. Override to customize.
	    /// </summary>
	    protected virtual void RegisterEventCallbacks()
	    {

	    }
	    
	    /// <summary>
	    /// Shows the defined root element of the <see cref="UIView"/>.
	    /// This sets the root element's style display to flex.
	    /// </summary>
	    public virtual void Show()
	    {
		    if(RootElement != null)
				RootElement.style.display = DisplayStyle.Flex;
	    }

	    /// <summary>
	    /// Hides the defined root element of the <see cref="UIView"/>.
	    /// /// This sets the root element's style display to none.
	    /// </summary>
	    public virtual void Hide()
	    {
		    if(RootElement != null)
			    RootElement.style.display = DisplayStyle.None;
	    }

	    /// <summary>
	    /// Unregisters any callbacks or event handlers.
	    /// </summary>
	    public virtual void Dispose()
	    {
		    
	    }
    }
}
