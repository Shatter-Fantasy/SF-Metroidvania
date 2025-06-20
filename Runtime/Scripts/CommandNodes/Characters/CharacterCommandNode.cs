using SF.Characters;
using SF.Characters.Controllers;

using UnityEngine;

namespace SF.CommandModule
{
    public class CharacterCommandNode : CommandNode
    {
        [HideInInspector] public Controller2D Controller2D;
        [HideInInspector] public CharacterRenderer2D Character2D;

        protected override bool CanDoCommand()
        {
            return Controller2D != null && Character2D != null;
        }

        protected override void DoCommand() { }

        protected override Awaitable DoAsyncCommand()
        {
            return null; 
        }
    }
}
