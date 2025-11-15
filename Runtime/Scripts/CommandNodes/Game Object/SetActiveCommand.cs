using UnityEngine;

namespace SF.CommandModule
{
    [System.Serializable]
    [CommandMenu("GameObject/Activate")]
    public class SetActiveCommand : CommandNode, ICommand
    {
        [SerializeField] private bool _setActive = false;
        [SerializeField] private GameObject _gameObject;
        public SetActiveCommand() { }
        public SetActiveCommand( bool setActive = false)
        {
            _setActive = setActive;
        }

        protected override bool CanDoCommand()
        {
            return _gameObject != null;
        }

        protected override void DoCommand()
        {
        }

        protected override async Awaitable DoAsyncCommand()
        {
            _gameObject.SetActive(_setActive);

            // TODO: This is a temp thing till I fully implement the both the aynsc
            // and non-async versions.
            await Awaitable.MainThreadAsync();
        }
    }
}
