using SF.Events;

namespace SF.SpawnModule
{
    public enum CheckPointEventTypes
    {
        ChangeCheckPoint,
        ResetCheckPoint
    }

    public struct CheckPointEvent
    {
        public CheckPointEventTypes EventType;
        public CheckPoint CheckPoint;

		public CheckPointEvent(CheckPointEventTypes eventType, CheckPoint checkPoint)
		{
			EventType = eventType;
			CheckPoint = checkPoint;
		}

        static CheckPointEvent checkPointEvent;

        public static void Trigger(CheckPointEventTypes eventType)
        {
            checkPointEvent.EventType = eventType;
            EventManager.TriggerEvent(checkPointEvent);
        }

		public static void Trigger(CheckPointEventTypes eventType, CheckPoint checkPoint)
		{
			checkPointEvent.EventType = eventType;
            checkPointEvent.CheckPoint = checkPoint;
			EventManager.TriggerEvent(checkPointEvent);
		}
	}
}
