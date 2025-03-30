using SF.Characters.Controllers;
using SF.Interactables;
using SF.SpawnModule;

using UnityEngine;

namespace SF.DataManagement
{
    public class SaveStation : CheckPoint, IInteractable
    {
        [field:SerializeField] public InteractableMode InteractableMode { get; set; }

        public virtual void Interact()
        {
            SaveSystem.CurrentSaveFileData.CurrentSaveStation = this;
        }

        public virtual void Interact(PlayerController controller)
        {
            if(controller.TryGetComponent<PlayerHealth>(out PlayerHealth health))
            {
                health.FullHeal();
            }
            SaveSystem.CurrentSaveFileData.CurrentSaveStation = this;
        }
    }
}
