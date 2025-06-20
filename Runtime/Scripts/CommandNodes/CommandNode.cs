using UnityEngine;

namespace SF.CommandModule
{
    [System.Serializable]
    public abstract class CommandNode : ICommand
    {
        public string Name;
        /// <summary>
        /// Should the command be run async or not.
        /// </summary>
        [field:SerializeField] protected bool IsAsyncCommand { get; set; }

        [SerializeField] protected Timer _delayTimer;
        
        public async Awaitable Use()
        {
            if (!CanDoCommand())
                return;

            if (_delayTimer.Duration > 0)
            {
                _delayTimer = new Timer(_delayTimer.Duration, DelayCommand);
                await _delayTimer.StartTimerAsync();
            }
            else
            {
                if (IsAsyncCommand)
                    await DoAsyncCommand();
                else
                    DoCommand();
            }
        }

        protected abstract bool CanDoCommand();
        protected async void DelayCommand()
        {
            if (IsAsyncCommand)
                await DoAsyncCommand();
            else
                DoCommand();
        }

        protected abstract void DoCommand();
        protected abstract Awaitable DoAsyncCommand();
    }
}
