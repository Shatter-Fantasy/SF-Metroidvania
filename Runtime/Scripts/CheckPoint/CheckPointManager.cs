using SF.Events;

using UnityEngine;

namespace SF.SpawnModule
{
    public class CheckPointManager : MonoBehaviour, EventListener<CheckPointEvent>
    {
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
			if(StartingCheckPoint != null && CurrentCheckPoint == null)
				CurrentCheckPoint = StartingCheckPoint;
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
