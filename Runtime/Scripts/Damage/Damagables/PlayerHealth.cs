using SF.DataManagement;

using SF.Events;

namespace SF.SpawnModule
{
    public class PlayerHealth : CharacterHealth, IDamagable
    {

        protected override void Kill()
        {
            base.Kill();

            LivesEvent.Trigger(LivesEventTypes.DecreaseLives, 1);
            SpawnSystem.RespawnPlayer();
        }

        public override void Respawn()
        {
            // TODO: Make the PlayerRespawn event pass in the check points position to
            // use it to move the player.
            if(CheckPointManager.Instance == null)
                return;

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
