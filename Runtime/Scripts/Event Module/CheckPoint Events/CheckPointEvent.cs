using SF.DataManagement;
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
        // SavePoint is the base class that CheckPoint inherits from.
        public SavePoint CheckPoint;
        
		public CheckPointEvent(CheckPointEventTypes eventType, CheckPoint checkPoint)
		{
			EventType = eventType;
			CheckPoint = checkPoint;
		}
		
		/// <summary>
		/// Use this when needing to pass in the base class SavePoint that CheckPoint inherit froms.
		/// </summary>
		/// <param name="eventType"></param>
		/// <param name="savePoint"></param>
		public CheckPointEvent(CheckPointEventTypes eventType, SavePoint savePoint)
		{
			EventType = eventType;
			CheckPoint = savePoint;
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
