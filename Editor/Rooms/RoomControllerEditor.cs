using SF.Utilities;
using SF.RoomModule;

using UnityEditor;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

namespace SFEditor.Rooms
{

    public class RectHandleLogic
    {
        private static Vector2 StartMousePosition;
        private static Vector2 CurrentMousePosition;
        /// <summary>
        /// The current screen coordinates of the mouse positon.
        /// </summary>
        private static Vector2 CurrentScreenMousePosition;
        private static Vector2 StartPosition;

        public static void DoDraw(int id,
         Vector3 position,
         Quaternion rotation,
         float size)
        {
            Vector3 position2 = Handles.matrix.MultiplyPoint(position);

            Matrix4x4 matrix = Handles.matrix;
            Event current = Event.current;


            switch(current.GetTypeForControl(id))
            {
                case EventType.Layout:
                    {
                        Handles.matrix = Matrix4x4.identity;
                        Handles.matrix = matrix;
                        break;
                    }
                case EventType.Repaint:
                    {
                        Handles.matrix = Matrix4x4.identity;
                        Handles.matrix = matrix;
                        break;
                    }
                case EventType.MouseDown:
                    {
                        if(HandleUtility.nearestControl == id && current.button == 0)
                        {
                            GUIUtility.hotControl = id;
                            CurrentMousePosition = (CurrentScreenMousePosition 
                                = (StartMousePosition = current.mousePosition));

                            StartPosition = position;
                            current.Use();
                            EditorGUIUtility.SetWantsMouseJumping(1);
                        }
                        break;
                    }
                case EventType.MouseUp:
                    if(GUIUtility.hotControl == id && (current.button == 0 || current.button == 2))
                    {
                        Debug.Log(current.mousePosition);
                        GUIUtility.hotControl = 0;
                        current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(0);
                    }
                    break;
            }
        }

    }

    [CustomEditor(typeof(RoomController), true)]
    public class RoomControllerEditor : Editor
    {
        Rect boundaryRect = new Rect(0,0,10,10);
        Color handleFill = new Color(.5f, .5f, .5f, .5f);
        Vector3 rectPosition => (Vector3)boundaryRect.position;
        Vector3 snap = Vector3.one * 0.5f;

        int roomBoundaryID = "Room Controller Boundary".GetHashCode();

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
            GameObject levelBoundary = new GameObject("Level Boundary",
                    (typeof(BoxCollider2D))
            ) {
                name = "Level Boundary",
            };
            levelBoundary.transform.SetParent(room.transform);

            BoxCollider2D boundaryCollider = levelBoundary.GetComponent<BoxCollider2D>();
            boundaryCollider.isTrigger = true;
        }
    }
}
