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
            // TODO: Remove CheckPointManager and use the SpawnSystem.
            if(CheckPointManager.Instance == null)
                return;
            
            // TODO: Remove CheckPointManager and use the SpawnSystem.
            if(CheckPointManager.Instance.CurrentCheckPoint != null)
                transform.position = CheckPointManager.Instance.CurrentCheckPoint.transform.position;

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
