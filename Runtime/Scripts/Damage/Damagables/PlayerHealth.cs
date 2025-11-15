using SF.DataManagement;

using SF.Events;

namespace SF.SpawnModule
{
    public class PlayerHealth : CharacterHealth, IDamagable, EventListener<SaveLoadEvent>
    {

        protected override void Kill()
        {
            base.Kill();

            LivesEvent.Trigger(LivesEventTypes.DecreaseLives, 1);
            RespawnEvent.Trigger(RespawnEventTypes.PlayerRespawn);
            RespawnEvent.Trigger(RespawnEventTypes.GameObjectRespawn);
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
            this.EventStartListening<SaveLoadEvent>();
        }

        protected override void OnDisable()
        {
            this.EventStopListening<SaveLoadEvent>();
        }
        
        public void OnEvent(SaveLoadEvent saveLoadEvent)
        {
            switch (saveLoadEvent.EventType)
            {
                case SaveLoadEventTypes.Loading:
                {
                    break;
                }
            }
        }
        
        public override void OnEvent(RespawnEvent respawnEvent)
        {
            switch (respawnEvent.EventType) 
            {
                case RespawnEventTypes.PlayerRespawn:
                    Respawn();
                    break;
            }
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            this.EventStopListening<SaveLoadEvent>();
        }
    }
}
