using SF.CommandModule;
using UnityEngine;

namespace SF
{
    public class SetActiveCommand : CommandNode, ICommand
    {
        [SerializeField] private bool _setActive = false;
        [SerializeField] private GameObject _gameObject;
        public SetActiveCommand() { }
        public SetActiveCommand( bool setActive = false)
        {
            _setActive = setActive;
        }
        public override async Awaitable Use()
        {
            _gameObject.SetActive(_setActive);

            // TODO: This is a temp thing till I fully implement the both the aynsc
            // and non-async versions.
            await Awaitable.MainThreadAsync();
        }
    }
}
