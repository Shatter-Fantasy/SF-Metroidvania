using Unity.U2D.Physics;

namespace SF.U2D.Physics
{
    
    public interface ITriggerShapeCallback
    {
        void OnTriggerBegin2D(PhysicsEvents.TriggerBeginEvent beginEvent, SFShapeComponent callingShapeComponent);

        void OnTriggerEnd2D(PhysicsEvents.TriggerEndEvent endEvent, SFShapeComponent callingShapeComponent);
    }
    
    public interface ITrigger2DCallbackBase { }
    
    public interface ITriggerShapeBegin2DCallback : ITrigger2DCallbackBase
    {
        void OnTriggerBegin2D(PhysicsEvents.TriggerBeginEvent beginEvent, SFShapeComponent callingShapeComponent);
    }
    
    public interface ITriggerShapeEnd2DCallback : ITrigger2DCallbackBase
    {
        void OnTriggerEnd2D(PhysicsEvents.TriggerEndEvent endEvent, SFShapeComponent callingShapeComponent);
    }
}
