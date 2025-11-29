using SF.Characters;
using SF.Characters.Controllers;
using SF.PhysicsLowLevel;
using UnityEngine;

namespace SF.CommandModule
{
    public class CharacterCommandNode : CommandNode
    {
        [HideInInspector] public ControllerBody2D ControllerBody2D;
        [HideInInspector] public CharacterRenderer2D Character2D;

        protected override bool CanDoCommand()
        {
            return ControllerBody2D != null && Character2D != null;
        }

        protected override void DoCommand() { }

        protected override Awaitable DoAsyncCommand()
        {
            return null; 
        }
    }
}
