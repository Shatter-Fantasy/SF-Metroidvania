using SF.DataManagement;

namespace SF.SpawnModule
{
    public class PlayerHealth : CharacterHealth
    {

        protected override void Kill()
        {
            base.Kill();
            
            SpawnSystem.RespawnPlayer();
        }

        public override void Respawn()
        {
            if(MetroidvaniaSaveManager.CurrentSavePoint == null)
                return;
            
            transform.position = MetroidvaniaSaveManager.CurrentSavePoint.transform.position;
            base.Respawn();
        }

        public override void Despawn()
        {
            // This empty ovveride prevents the base health script from deactivating the player.
            // Only needed for a bit before the next update to the player spawn system is done.
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            SpawnSystem.PlayerRespawnHandler += Respawn;
        }

        protected override void OnDisable()
        {
            // Might need to move this to the OnDestroy if we disable the player during respawning.
            SpawnSystem.PlayerRespawnHandler -= Respawn;
        }
    }
}
