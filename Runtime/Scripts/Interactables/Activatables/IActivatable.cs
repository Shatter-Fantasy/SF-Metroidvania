using UnityEngine;

namespace SF
{
    public interface IActivatable
    {
        public bool Activated{ get; set; }
    }

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