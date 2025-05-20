using UnityEditor;
using SF.CommandModule;
using UnityEngine.UIElements;

using System.Collections.Generic;
using System;
using System.Linq;
using UnityEditor.UIElements;

namespace SF
{
    [CustomEditor(typeof(CommandController))]
    public class CommandControllerEditor : Editor
    {
        [NonSerialized] private List<Type> CommandTypes;
        [NonSerialized] private List<CommandNode> _commandNodes = new List<CommandNode>();
        [NonSerialized] private PopupField<CommandNode> _commandDropdownMenu;

        private CommandController _commandController;

        public override VisualElement CreateInspectorGUI()
        {
            CommandTypes =  TypeCache.GetTypesDerivedFrom<CommandNode>().ToList();

            CommandTypes.ForEach( commandType =>
            {
                _commandNodes.Add(Activator.CreateInstance(commandType) as CommandNode);
            });


            VisualElement root = new VisualElement();

            DropDownInit();
           _commandDropdownMenu.index = 0;
            root.Add(_commandDropdownMenu);
            InspectorElement.FillDefaultInspector(root,serializedObject,this);

            return root;
        }

        private void DropDownInit()
        {
            _commandController = (CommandController) serializedObject.targetObject;
            _commandDropdownMenu = new PopupField<CommandNode>("Options");
            _commandDropdownMenu.index = 0;

            for(int i = 0; i < _commandNodes.Count; i++)
            {
                _commandDropdownMenu.choices.Add(_commandNodes[i]);
            }

            _commandDropdownMenu.RegisterValueChangedCallback(evt =>
            {
                _commandController.Commands.Add(evt.newValue);
            });
        }
    }
}
