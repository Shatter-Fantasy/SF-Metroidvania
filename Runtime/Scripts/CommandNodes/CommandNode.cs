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

        public abstract Awaitable Use();
    }
}
