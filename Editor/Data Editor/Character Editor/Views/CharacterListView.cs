using SFEditor.Data;
using SF.Characters.Data;

using UnityEngine.UIElements;

namespace SFEditor.Characters.Data
{
    [UxmlElement]
    public partial class CharacterListView : DataListView<CharacterDatabase, CharacterDTO>, INotifyValueChanged<CharacterDatabase>
    {
        protected override string _dataFilePath => "Assets/Data/Character/";

        public CharacterListView() { }
        public CharacterListView(CharacterDatabase characterDatabase) : base(characterDatabase) { }
    }
}
