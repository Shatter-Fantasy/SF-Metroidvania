using System.Collections.Generic;

using SF.Characters.Controllers;
using SF.InputModule;
using SF.Interactables;

using UnityEngine;

namespace SF
{
    public class InteractableSwitch : MonoBehaviour, IInteractable
    {

        public List<ActivablteWrapper> Activatables = new List<ActivablteWrapper>();

        [SerializeField] private bool _oneTimeUse = false;
        private bool _wasUsed = false;

        [field: SerializeField] public InteractableMode InteractableMode { get; set; }


        public void Interact()
        {
            if(InteractableMode != InteractableMode.Input && !InputManager.Controls.Player.Interact.WasPressedThisFrame())
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