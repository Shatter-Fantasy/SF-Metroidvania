using UnityEngine;
using UnityEngine.UIElements;

namespace SF.UIModule
{ 
    /// <summary>
    /// Base class for any overlay that needs to be displayed for runtime.
    /// </summary>
    public class UIOverlayView : MonoBehaviour
    {
        [SerializeField] protected UIDocument _overlayUXML;
        protected VisualElement _overlayContainer;

        private void Start()
        {
            if (_overlayUXML == null)
                return;
            
            _overlayUXML.rootVisualElement.pickingMode = PickingMode.Ignore;
            
            _overlayContainer = _overlayUXML.rootVisualElement.Q<VisualElement>(name: "overlay-item__container");
        }
    }
}
