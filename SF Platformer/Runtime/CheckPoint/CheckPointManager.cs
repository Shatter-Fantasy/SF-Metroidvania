using SF.Events;

using UnityEngine;

namespace SF.SpawnModule
{
    public class CheckPointManager : MonoBehaviour, EventListener<CheckPointEvent>
    {
		/// <summary>
		/// Should the the player spawn at the starting check point at the start of playing a level if the starting checkpoint is not null.
		/// </summary>
		[SerializeField] private bool _useStartingCheckPoint = true;

        public CheckPoint StartingCheckPoint;
        public CheckPoint CurrentCheckPoint;

		private static CheckPointManager _instance;
		public static CheckPointManager Instance
		{
			get
			{
				if(_instance == null)
				{
					GameObject go = new GameObject("Check Point Manager", typeof(CheckPointManager));
					_instance = go.GetComponent<CheckPointManager>();
				}
				return _instance;
			}

			set
			{
				_instance = value;
			}
		}

        private void Awake()
        {
            Instance = this;
        }
        private void Start()
        {
			// If we have no starting check point set try to find one.
			if(StartingCheckPoint == null)
				StartingCheckPoint = FindFirstObjectByType<CheckPoint>();

			
			if(StartingCheckPoint != null && CurrentCheckPoint == null)
				CurrentCheckPoint = StartingCheckPoint;

			if(_useStartingCheckPoint && StartingCheckPoint != null)
                RespawnEvent.Trigger(RespawnEventTypes.PlayerRespawn);

        }
        public void OnEvent(CheckPointEvent checkPointEvent)
		{
			switch (checkPointEvent.EventType) 
			{
				case CheckPointEventTypes.ChangeCheckPoint:
					ChangeCheckPoint(checkPointEvent.CheckPoint as CheckPoint);
					break;
			}
		}

		private void ChangeCheckPoint(CheckPoint checkPoint)
		{
			CurrentCheckPoint = checkPoint;
		}

		private void OnEnable()
		{
			this.EventStartListening<CheckPointEvent>();
		}
		private void OnDisable()
		{
			this.EventStopListening<CheckPointEvent>();
		}
	}
}
