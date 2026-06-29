using Unity.U2D.Physics;

namespace SF.U2D.Physics
{
    public interface IContact2DCallbackBase { }
    
    public interface IContactShapeBegin2DCallback : IContact2DCallbackBase
    {
        void OnContactBegin2D(PhysicsEvents.ContactBeginEvent beginEvent, SFShapeComponent callingShapeComponent);
    }
    
    public interface IContactShapeEnd2DCallback : IContact2DCallbackBase
    {
        void OnContactEnd2D(PhysicsEvents.ContactEndEvent endEvent, SFShapeComponent callingShapeComponent);
    }
}
