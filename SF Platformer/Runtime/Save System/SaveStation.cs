using SF.Characters.Controllers;
using SF.Interactables;
using SF.SpawnModule;

using UnityEngine;

namespace SF.DataManagement
{
    public class SaveStation : CheckPoint, IInteractable, ICheckPoint
    {
        [field:SerializeField] public InteractableMode InteractableMode { get; set; }

        public void Interact()
        {
            SaveSystem.CurrentSaveFileData.CurrentSaveStation = this;
            SaveSystem.SaveDataFile();
        }

        public void Interact(PlayerController controller)
        {
            SaveSystem.CurrentSaveFileData.CurrentSaveStation = this;
            SaveSystem.SaveDataFile();
            if(controller.TryGetComponent<PlayerHealth>(out PlayerHealth health))
            {
                health.FullHeal();
            }
        }
    }
}
