using UnityEngine;

using SF.Transitions;
using Unity.Cinemachine;

namespace SF
{
    public class RoomTransition : MonoBehaviour, ICameraTransition
    {
        public CinemachineCamera TransitioningCamera;

        public void DoCameraTransition()
        {
            
        }

        public void DoTransition()
        {
           
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            DoTransition();
        }
    }
}
