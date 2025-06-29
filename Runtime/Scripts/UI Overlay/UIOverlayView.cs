using SF.Events;
using SF.Inventory;
using SF.InventoryModule;
using UnityEngine;
using UnityEngine.UIElements;

namespace SF.UIModule
{
    public class UIOverlayView : MonoBehaviour, EventListener<ItemEvent>
    {
        [SerializeField] private UIDocument _overlayUXML;
        [SerializeField] private ItemDatabase _itemDatabase;
        
        [Header("Debugging")] public ItemDTO PickedUpTime;
        
        private VisualElement _overlayContainer;
        private Label _itemPickUpLabel;
        
        [SerializeField] private Timer _popTimer;
        /// <summary>
        /// This is a dev set max timer that will make sure user set timer values are not too high.
        /// </summary>
        [SerializeField] private float _maxTimer = 5;
        private void Awake()
        {
            _popTimer = new Timer(OnPopUpTimerCompleted);
        }
        
        private void Start()
        {
            if (_overlayUXML == null)
                return;
            _overlayUXML.rootVisualElement.pickingMode = PickingMode.Ignore;
            
            
            _overlayContainer = _overlayUXML.rootVisualElement.Q<VisualElement>(name: "overlay__view");
            _itemPickUpLabel = _overlayUXML.rootVisualElement.Q<Label>(name: "overlay-item__label");
        }
        
        private void OnPickUpItem(int itemID)
        {
            var itemDTO = _itemDatabase[itemID];
            PickedUpTime = itemDTO;
            _itemPickUpLabel.text = itemDTO.Name;
            _overlayContainer.style.visibility = Visibility.Visible;

            _ = _popTimer.StartTimerAsync();
        }
        
        private void OnPopUpTimerCompleted()
        {
            _overlayContainer.style.visibility = Visibility.Hidden;
        }
        
        public void OnEvent(ItemEvent itemEvent)
        {
            switch (itemEvent.EventType)
            {
                case ItemEventTypes.PickUp:
                { 
                    if (_itemDatabase == null)
                        break;
                    
                    OnPickUpItem(itemEvent.ItemId);
                    break;
                }
            }
        }

        private void OnEnable()
        {
            this.EventStartListening<ItemEvent>();
        }
        private void OnDisable()
        {
            this.EventStopListening<ItemEvent>();
        }
    }
}
