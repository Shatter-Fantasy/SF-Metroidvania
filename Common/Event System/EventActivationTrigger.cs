using UnityEngine;
using UnityEngine.SceneManagement;

namespace SF.Events
{
    public enum EventTriggerType
    {
        OnCollisionEnter,
        OnCollisionExit,
        OnCollisionStay,
        OnCollisionEnter2D,
        OnCollisionExit2D,
        OnCollisionStay2D,
    }

    public class EventActivationTrigger : MonoBehaviour
    {
        public Scene ActiveScene;
        public string SceneName;

        [Header("Event Activation Options")]
        public EventTriggerType EventTriggerType;
        public LocalPhysicsMode PhysicsMode;
    }
}