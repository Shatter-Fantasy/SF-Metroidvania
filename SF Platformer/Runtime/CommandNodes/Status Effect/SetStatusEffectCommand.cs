using UnityEngine;

using SF.Characters;

namespace SF.CommandModule
{

    [System.Serializable]
    public class SetStatusEffectCommand : CharacterCommandNode
    {
        public StatusEffect StatusEffect;

        public override async Awaitable Use()
        {
            if(Controller2D != null)
                Controller2D.CharacterState.StatusEffect = StatusEffect;
            await Awaitable.EndOfFrameAsync();
        }
    }
}
