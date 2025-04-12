using SF.DialogueModule;
using SF.Events;
using SF.Inventory;
using SF.InventoryModule;
using UnityEngine;
using UnityEngine.UIElements;

namespace SF.UIModule
{
    public class UIOverlayView : MonoBehaviour, EventListener<ItemEvent>, EventListener<DialogueEvent>
    {
        [SerializeField] private UIDocument _overlayUXML;
        [SerializeField] private ItemDatabase _itemDatabase;


        [Header("Debugging")] public ItemDTO PickedUpTime;
        
        private VisualElement _dialogueContainer;
        private Label _dialogueLabel;
        
        private VisualElement _itemContainer;
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
            
            _dialogueContainer = _overlayUXML.rootVisualElement.Q<VisualElement>(name: "overlay-dialogue__container");
            _dialogueLabel = _overlayUXML.rootVisualElement.Q<Label>(name: "overlay-dialogue__label");
            
            _itemContainer = _overlayUXML.rootVisualElement.Q<VisualElement>(name: "overlay-item__container");
            _itemPickUpLabel = _overlayUXML.rootVisualElement.Q<Label>(name: "overlay-item__label");
        }
        
        private void OnPickUpItem(int itemID)
        {
            var itemDTO = _itemDatabase[itemID];
            PickedUpTime = itemDTO;
            _itemPickUpLabel.text = itemDTO.Name;
            _itemContainer.style.visibility = Visibility.Visible;

            _popTimer.StartTimerAsync();
        }
        
        private void OnDialogueAppear(string dialogue)
        {
            
            // Close misc pop up boxes for things like item pick-ups or smaller quest notifications when entering the dialogue conversation.
            _itemContainer.style.visibility = Visibility.Hidden;
            
            _dialogueContainer.style.visibility = Visibility.Visible;
            _dialogueLabel.text = dialogue;
            _popTimer.StartTimerAsync();
        }
        
        
        private void OnPopUpTimerCompleted()
        {
            _itemContainer.style.visibility = Visibility.Hidden;
            _dialogueContainer.style.visibility = Visibility.Hidden;
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

        public void OnEvent(DialogueEvent dialogueEvent)
        {
            switch (dialogueEvent.EventType)
            {
                case DialogueEventTypes.DialoguePopup:
                { 
                    OnDialogueAppear(dialogueEvent.Dialogue);
                    break;
                }
            }
        }

        private void OnEnable()
        {
            this.EventStartListening<ItemEvent>();
            this.EventStartListening<DialogueEvent>();
        }
        private void OnDisable()
        {
            this.EventStopListening<ItemEvent>();
            this.EventStopListening<DialogueEvent>();
        }
    }
}
