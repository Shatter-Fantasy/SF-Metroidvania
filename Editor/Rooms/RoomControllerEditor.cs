using SF.RoomModule;

using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

namespace SFEditor.Rooms
{
    [CustomEditor(typeof(RoomController), true)]
    public class RoomControllerEditor : Editor
    {
        /// <summary>
        /// Is the RoomController component on the same gameobject as a grid component for things like Tilemaps or custom grid layouts.
        /// </summary>
        /// <remarks>
        /// This is used to allow custom tools for Rooms to easily set up the connection points of rooms when they spawn.
        /// </remarks>
        private bool _isGrid;
        private RoomController _roomController;
        private Grid _grid;

        private BoundsIntField _selectionPositionField;
        public override VisualElement CreateInspectorGUI()
        {
            _roomController = target as RoomController;
            _roomController?.TryGetComponent(out _grid);
            _isGrid = _grid != null;
            
            VisualElement newInspector = new();
            InspectorElement.FillDefaultInspector(newInspector, serializedObject, this);
            
            if (_isGrid)
            {
                Foldout gridTools = new Foldout(){text = "Room Grid Tools"};
                _selectionPositionField = new BoundsIntField("Grid Position");
                
                gridTools.Add(
                    new Button(SetUpRoomObjects)
                    {
                        text = "Setup Room Objects",
                        tooltip =
                            "Sets up the needed objects like cameras, boundaries for a new room using the game object the room controller is on as the root object."
                    }
                );
                gridTools.Add(_selectionPositionField);
                
                newInspector.Add(gridTools);
            }
            
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

        private void OnEnable()
        {
            GridSelection.gridSelectionChanged += OnGridSelectionChanged;
        }
        private void OnDisable()
        {
            GridSelection.gridSelectionChanged -= OnGridSelectionChanged;
        }
        
        private void OnGridSelectionChanged()
        {
            if (_selectionPositionField != null)
            {
                _selectionPositionField.value = GridSelection.position;
            }
        }
    }
}
