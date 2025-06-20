using UnityEngine;
using SF.Characters;

namespace SF.CommandModule
{
    [System.Serializable]
    public class SetStatusEffectCommand : CharacterCommandNode
    {
        public StatusEffect StatusEffect;
        
        protected override async Awaitable DoAsyncCommand()
        {
            Controller2D.CharacterState.StatusEffect = StatusEffect;
            await Awaitable.EndOfFrameAsync();
        }
    }
}
