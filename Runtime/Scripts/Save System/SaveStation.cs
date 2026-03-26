using UnityEngine;

namespace SF.DataManagement
{
    using Characters.Controllers;
    using Interactables;
    using RoomModule;
    using SpawnModule;
    using StatModule;
    
    /// <summary>
    /// A <see cref="SavePoint"/> that requires being interacted with to use.
    /// </summary>
    public class SaveStation : SavePoint, IInteractable<PlayerController>
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
            if(controller.TryGetComponent(out PlayerHealth health))
            {
                health.FullHeal();
                MetroidvaniaSaveManager.CurrentMetroidvaniaSaveData.PlayerHealth = health;
            }
            
            if(controller.TryGetComponent(out PlayerStats stats))
            {
                MetroidvaniaSaveManager.CurrentMetroidvaniaSaveData.PlayerStats = stats;
            }
            
            MetroidvaniaSaveManager.CurrentMetroidvaniaSaveData.SavedRoomID = RoomSystem.CurrentRoomID;
            SaveSystem.CurrentSaveFileData.CurrentSaveStation               = this;
            MetroidvaniaSaveManager.SaveGame();
        }
    }
}
