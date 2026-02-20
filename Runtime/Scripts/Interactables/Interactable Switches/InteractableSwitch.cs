using System.Collections.Generic;
using UnityEngine;

namespace SF.Interactables
{
    using Activatable;
    using Characters.Controllers;
    using InputModule;

    public class InteractableSwitch : MonoBehaviour, IInteractable
    {
        public List<ActivatableWrapper> Activatables = new List<ActivatableWrapper>();

        [SerializeField] private bool _oneTimeUse = false;
        private bool _wasUsed = false;

        [field: SerializeField] public InteractableMode InteractableMode { get; set; }


        public void Interact()
        {
            if(InteractableMode != InteractableMode.Input && !SFInputManager.Controls.Player.Interact.WasPressedThisFrame())
                return;

            if(_oneTimeUse && _wasUsed)
                return;

            if(_oneTimeUse)
                _wasUsed = true;

            for(int i = 0; i < Activatables.Count; i++)
            {
                if(Activatables[i] == null)
                    continue;

                Activatables[i].Activated = !Activatables[i].Activated;
            }
        }

        public void Interact(PlayerController controller)
        {
            Interact();
        }
    }
}