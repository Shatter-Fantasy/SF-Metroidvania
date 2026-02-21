using UnityEngine;
using UnityEngine.UIElements;

namespace SF.UIModule
{
    using ItemModule;
    public class GameOverlayView : UIOverlayView
    {
        [SerializeField] private Timer _popTimer;
        
        [Header("Item Settings")]
        [SerializeField] private ItemDatabase _itemDatabase;
        [Header("Debugging")] public ItemDTO PickedUpTime;

        private Label _itemPickUpLabel;
        
        private void Awake()
        {
            _popTimer = new Timer(OnPopUpTimerCompleted);
        }

        private void OnEnable()
        {
            PlayerInventory.ItemPickedUpHandler += OnPickUpItem;
        }
        
        private void OnDisable()
        {
            PlayerInventory.ItemPickedUpHandler -= OnPickUpItem;
        }

        private void Start()
        {
            if (_overlayUXML == null)
                return;
            
            _overlayUXML.rootVisualElement.pickingMode = PickingMode.Ignore;
            
            _overlayContainer = _overlayUXML.rootVisualElement.Q<VisualElement>(name: "overlay-item__container");
            _itemPickUpLabel  = _overlayUXML.rootVisualElement.Q<Label>(name: "overlay-item__label");
        }
        
        private void OnPickUpItem(int itemID)
        {
            if (_itemDatabase == null)
                return;
            
            var itemDTO = _itemDatabase[itemID];
            if (itemDTO == null)
            {
#if UNITY_EDITOR
                Debug.Log($"There was no Item with the id: {itemID} inside of the item database.");
#endif
                return;
            }

            PickedUpTime                       = itemDTO;
            _itemPickUpLabel.text              = itemDTO?.Name;
            _overlayContainer.style.visibility = Visibility.Visible;

            _ = _popTimer.StartTimerAsync();
        }
        
        private void OnPopUpTimerCompleted()
        {
            _overlayContainer.style.visibility = Visibility.Hidden;
        }
    }
}
