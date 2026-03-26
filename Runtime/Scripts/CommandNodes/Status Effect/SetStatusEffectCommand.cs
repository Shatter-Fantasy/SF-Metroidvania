using UnityEngine;

namespace SF.CommandModule
{
    using Characters;
    [System.Serializable]
    public class SetStatusEffectCommand : CharacterCommandNode
    {
        public StatusEffect StatusEffect;
        
        protected override async Awaitable DoAsyncCommand()
        {
            ControllerBody2D.CharacterState.StatusEffect = StatusEffect;
            await Awaitable.EndOfFrameAsync();
        }
    }
}
