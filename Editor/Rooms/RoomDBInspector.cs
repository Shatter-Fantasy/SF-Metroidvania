using SF.RoomModule;
using SF.UIElements.Utilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SFEditor.Rooms
{
    [CustomEditor(typeof(RoomDB))]
    public class RoomDBInspector : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            InspectorElement.FillDefaultInspector(root,serializedObject,this);
            root.AddChild(new Button(SetRoomDBInstance){ text = "Set Instance" });
            root.AddChild(new Button(LogRoomInstances){ text = "Log RoomDB Instances" });
            root.AddChild(new Button(SetRoomIDs){ text = "Set Room Prefab IDs" });
            return root;
        }

        private void LogRoomInstances()
        {
            Debug.Log($"RoomSystem.RoomDB value is: { RoomSystem.RoomDB } and the RoomDB.Instance value is {RoomDB.Instance}.");
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
            var roomDB = target as RoomDB;

            if (roomDB == null)
                return;
            
            foreach (Room room in roomDB.Rooms)
            {
                // These are the prefab assets inside of the project folder.
                room.RoomPrefab.GetComponent<RoomController>().RoomID = room.RoomID;
            }
        }
    }
}
