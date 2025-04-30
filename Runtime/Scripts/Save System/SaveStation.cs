using SF.Characters.Controllers;
using SF.Interactables;
using SF.SpawnModule;
using SF.StatModule;
using UnityEngine;

namespace SF.DataManagement
{
    
    public class SaveStation : CheckPoint, IInteractable
    {
        /// <summary>
        /// The room id that the save room is in.
        /// </summary>
        public int RoomID;
        [field:SerializeField] public InteractableMode InteractableMode { get; set; }
        
        public virtual void Interact()
        {
            SaveSystem.CurrentSaveFileData.CurrentSaveStation = this;
            MetroidvaniaSaveManager.SaveGame();
        }

        public virtual void Interact(PlayerController controller)
        {
            if(controller.TryGetComponent<PlayerHealth>(out PlayerHealth health))
            {
                health.FullHeal();
                MetroidvaniaSaveManager.CurrentMetroidvaniaSaveData.PlayerHealth = health;
            }
            
            if(controller.TryGetComponent<PlayerStats>(out PlayerStats stats))
            {
                MetroidvaniaSaveManager.CurrentMetroidvaniaSaveData.PlayerStats = stats;
            }
            
            SaveSystem.CurrentSaveFileData.CurrentSaveStation = this;
            MetroidvaniaSaveManager.SaveGame();
        }
    }
}
