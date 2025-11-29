using System.Collections.Generic;

using SF.Characters;
using SF.Characters.Controllers;
using SF.Managers;
using SF.PhysicsLowLevel;
using UnityEngine;

namespace SF.CommandModule
{
    public enum CommandType
    {
        Cutscene,
        Character
    }
    public class CommandController : MonoBehaviour
    {
        [SerializeField] private CommandType _commandType;
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
                    characterCommand.ControllerBody2D = GetComponent<ControllerBody2D>();
                }    
            }
        }

        public void StartCommands()
        {
            if (_commandType == CommandType.Cutscene)
                GameManager.Instance.ControlState = GameControlState.Cutscenes;
                
            for (int i = 0; i < Commands.Count; i++)
            {
                _ = Commands[i].Use();
            }
            
            //if (_commandType == CommandType.Cutscene)
               // GameManager.Instance.ControlState = GameControlState.Player;
        }
    }
}