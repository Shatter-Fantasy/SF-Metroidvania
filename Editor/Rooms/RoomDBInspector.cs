using System.Collections.Generic;
using SF.RoomModule;
using SF.UIElements.Utilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SFEditor.Rooms
{
    [CustomEditor(typeof(RoomDB))]
    public class RoomDBInspector : Editor
    {
        private VisualElement _root;
        private ListView _roomListView;
        private RoomDB _roomDB;
        
        public override VisualElement CreateInspectorGUI()
        {
            _roomDB = target as RoomDB;
            _root = new VisualElement() {name = "room-db--root"};
            InspectorElement.FillDefaultInspector(_root,serializedObject,this);


            _root.AddChild(new Button(SetRoomDBInstance){ text = "Set Instance" });
            _root.AddChild(new Button(SetRoomIDs){ text = "Set Room Prefab IDs" });
            
            _root.RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            
            return _root;
        }

        private void OnAttachToPanel(AttachToPanelEvent evt)
        {
            this.
            // Unity auto assigns a name to the list-view based on the name of the variable.
            _roomListView = _root.Q<ListView>("unity-list-Rooms");
            if (_roomListView != null)
            {
                _roomListView.itemsAdded += OnRoomAdded;
            }
        }

        private void OnRoomAdded(IEnumerable<int> roomsAdded)
        {
            foreach (int roomIndex in roomsAdded)
            {
                _roomDB[roomIndex].RoomID = roomIndex;
            }
        }

        private void SetRoomDBInstance()
        {
            RoomDB roomDB = target as RoomDB;
            if (roomDB == null)
                return;
                
            RoomDB.Instance = roomDB;
            RoomSystem.RoomDB = roomDB;
        }
        
        private void SetRoomIDs()
        {
            _roomDB.ResetRoomIds();
        }
    }
}
