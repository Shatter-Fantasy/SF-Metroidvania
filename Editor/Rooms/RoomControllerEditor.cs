using SF.RoomModule;

using UnityEditor;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

namespace SFEditor.Rooms
{
    [CustomEditor(typeof(RoomController), true)]
    public class RoomControllerEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement newInspector = new();
            InspectorElement.FillDefaultInspector(newInspector, serializedObject, this);
            newInspector.Add(
                    new Button(SetUpRoomObjects)
                    {
                        text = "Setup Room Objects",
                        tooltip = "Sets up the needed objects like cameras, boundaries for a new room using the game object the room controller is on as the root object."
                    }
                );
            return newInspector;
        }

        /// <summary>
        /// Creates the Cinemachine camera for the room and sets it up.
        /// </summary>
        private void SetUpRoomObjects()
        {
            RoomController room = target as RoomController;
            Debug.Log("Set up room objects function needs reworked to match the updated room controller.");
        }
    }
}
