using System.Collections.Generic;

using SF.Characters;
using SF.Characters.Controllers;

using UnityEngine;

namespace SF.CommandModule
{
    public class CommandController : MonoBehaviour
    {
        [SerializeField] private bool DoStart = false;
        [SerializeReference] public List<CommandNode> Commands = new List<CommandNode>();

        private void Start()
        {
            OnStart();

            if (DoStart)
                StartCommands();
        }

        protected virtual void OnStart()
        {
            foreach (var cmd in Commands)
            {
                if(cmd is CharacterCommandNode characterCommand)
                {
                    characterCommand.Character2D = GetComponent<CharacterRenderer2D>();
                    characterCommand.Controller2D = GetComponent<Controller2D>();
                }    
            }
        }

        public async void StartCommands()
        {
            for (int i = 0; i < Commands.Count; i++)
            {
                await Commands[i].Use();
            }
        }
    }
}
