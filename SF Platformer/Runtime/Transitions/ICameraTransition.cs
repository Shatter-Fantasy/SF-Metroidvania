using Unity.Cinemachine;

namespace SF.Transitions
{
    public interface ICameraTransition : ITransition
    {
        void DoCameraTransition();
    }
}
