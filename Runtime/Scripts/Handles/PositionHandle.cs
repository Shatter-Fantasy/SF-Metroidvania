using UnityEditor;
using UnityEngine;

namespace SF.HandleModule
{

    public class PositionHandle : MonoBehaviour
    {
        public Vector3 StartPosition;
        public Vector3 EndPosition;


#if UNITY_EDITOR
        [MenuItem("CONTEXT/Position Handle/Set start position")]
        private static void SetStartPositionToCurrentPosition(MenuCommand command)
        {
            var t = (PositionHandle)command.context;
            t.StartPosition = t.transform.position;
        }
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(PositionHandle))]
    public class PositionHandleEditor : Editor
    {
        public void OnSceneGUI()
        {
            var t = target as PositionHandle;
            var tr = t.transform;

            // display an orange disc where the object is
            var color = new Color(1, 0.8f, 0.4f, 1);
            Handles.color = color;

            Vector3 newStartPosition =  Handles.PositionHandle(t.StartPosition, Quaternion.identity);
            Vector3 newEndingPosition = Handles.PositionHandle(t.EndPosition, Quaternion.identity);

            if(EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(t, "Update Positions");
                t.StartPosition = newStartPosition;
                t.EndPosition = newEndingPosition;
            }

            GUI.color = color;

            Handles.DrawLine(t.StartPosition, t.EndPosition);
            Handles.Label(t.StartPosition, "Start Position");
            Handles.Label(t.EndPosition, "End Position");
        }
    }
#endif
}
