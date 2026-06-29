using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

using TypeUtilities = SFEditor.Utilities.UnityEditorTypeUtilities;


namespace SFEditor.UIElements
{
    /// <summary>
    /// An editor only dropdown menu that displays options for c# types that meet the pass in predicate when calling the constructor.
    /// </summary>
    [UxmlElement]
    public partial class SFTypeDropdownMenu<TOption> : VisualElement where TOption : class
    {
        [NonSerialized] private List<Type> _optionsTypes;
        [NonSerialized] private List<TOption> _options = new List<TOption>();
        [NonSerialized] private PopupField<TOption> _typeDropdownMenu;
        
        
        public SFTypeDropdownMenu(){ }
        public SFTypeDropdownMenu(Predicate<Type> predicate)
        {
            Initialize(predicate);
        }

        private void Initialize(Predicate<Type> predicate)
        {
            _optionsTypes = TypeUtilities.GetTypesDerivedFrom(typeof(TOption),
                t => !t.IsAbstract && typeof(TOption).IsAssignableFrom(t));
            
            _optionsTypes.ForEach( optionType =>
            {
                _options.Add(Activator.CreateInstance(optionType) as TOption);
            });

            DropDownInit();
            Add(_typeDropdownMenu);
        }

        private void DropDownInit()
        {
            _typeDropdownMenu = new PopupField<TOption>("Options");
            _typeDropdownMenu.index = 0;

            for(int i = 0; i < _options.Count; i++)
            {
                _typeDropdownMenu.choices.Add(_options[i]);
            }
        }
    }
}
