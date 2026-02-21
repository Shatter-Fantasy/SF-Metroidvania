using UnityEngine;

namespace SF.Activatable
{
    public interface IActivatable
    {
        public bool Activated{ get; set; }
    }

    /// <summary>
    /// Base wrapper clas for allowing <see cref="IActivatable"/> to be shown in inspector.
    /// Implement this class to allow <see cref="Interactables.InteractableSwitch"/> or other
    /// <see cref="Interactables"/> class to active something.
    /// </summary>
    public abstract class ActivatableWrapper : MonoBehaviour, IActivatable
    {
        [field: SerializeField]
        private bool _activated;
        public bool Activated
        {
            get => _activated;
            set
            {
                // If we are activating while currently not already active.
                if(value == true && !_activated)
                {
                    OnActivation();
                }
                // If we are deactivating it while it is currently activated
                else if(value == false && _activated)
                {
                    OnDeactivate();
                }
                _activated = value;
            }
        }

        protected virtual void OnActivation()
        {

        }
        protected virtual void OnDeactivate()
        {

        }
    }
}