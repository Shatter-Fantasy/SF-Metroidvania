using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class EditorWindowBase : EditorWindow
{
    [SerializeField] protected VisualTreeAsset _visualTreeAsset = default;
    protected VisualElement _mainContainer;
    protected VisualElement _rootElement;
    protected StyleSheet _commonStyleSheet;

    protected bool _hasInitialized = false;

    /// <summary>
    /// Initialize the basic assets needed for all Editor Windows using the UI Toolkit.
    /// </summary> 
    /// Call this first in CreateGUI in classes inheriting this. 
    protected void InitWindowAssets()
    {
		if(_visualTreeAsset == null)
		{
			Debug.LogError("The editor window for " + name + " has no visual tree asset assigned to it.");
			return;
		}

		_rootElement = rootVisualElement;
		_mainContainer = _visualTreeAsset.Instantiate();
		_rootElement.Add(_mainContainer);

        if(_commonStyleSheet != null)
            _rootElement.styleSheets.Add(_commonStyleSheet);

        InitStyleSheets();

        _hasInitialized = true;
	}

    protected virtual void InitStyleSheets(){}

}
