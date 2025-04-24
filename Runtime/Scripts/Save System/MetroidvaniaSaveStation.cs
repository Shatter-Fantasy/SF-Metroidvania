using SF.Characters.Controllers;
using SF.SpawnModule;
using SF.StatModule;

namespace SF.DataManagement
{
    public class MetroidvaniaSaveStation : SaveStation
    {
        public override void Interact()
        {
            SaveSystem.CurrentSaveFileData.CurrentSaveStation = this;
            MetroidvaniaSaveManager.SaveGame();
        }

        public override void Interact(PlayerController controller)
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
