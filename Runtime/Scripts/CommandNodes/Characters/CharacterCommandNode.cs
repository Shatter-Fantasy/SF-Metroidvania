using SF.Characters;
using SF.Characters.Controllers;

using UnityEngine;

namespace SF.CommandModule
{
    public class CharacterCommandNode : CommandNode
    {
        [HideInInspector] public Controller2D Controller2D;
        [HideInInspector] public CharacterRenderer2D Character2D;

        public override Awaitable Use()
        {
            throw new System.NotImplementedException();
        }
    }
}
