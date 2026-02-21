using UnityEngine;

namespace SF.CommandModule
{
    using Characters;
    using PhysicsLowLevel;
    
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
