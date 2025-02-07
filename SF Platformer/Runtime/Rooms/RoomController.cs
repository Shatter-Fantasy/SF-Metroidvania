using SF.Utilities;

using UnityEngine;

namespace SF.RoomModule
{
    public class RoomController : MonoBehaviour
    {

        private static Vector3 StartMousePosition;
        private static Vector3 EndMousePosition;
        Rect tRect = new Rect(100,0,250,250);
        Rect boundaryRect = new Rect(100,0,250,250);

        /*void OnGUI()
        {
           
            Event current = Event.current;

            switch(current.type)
            {
                case EventType.MouseDown:
                    {
                        StartMousePosition = current.mousePosition;
                        current.Use();
                        break;
                    }
                case EventType.MouseDrag:
                    {
                        EndMousePosition = current.mousePosition;
                        boundaryRect = RectByVectorEdges(StartMousePosition, EndMousePosition);
                        current.Use();
                        break;
                    }
                case EventType.MouseUp:
                    {
                        EndMousePosition = current.mousePosition;
                        boundaryRect = RectByVectorEdges(StartMousePosition, EndMousePosition);
                        current.Use();
                        break;
                    }
                case EventType.Repaint:
                    {
                        GL.Begin(GL.LINES);
                        GLUtilities.InitHandleMaterial();
                        GLUtilities.ApplyHandleMaterial();
                        GLUtilities.StartDrawing(Matrix4x4.identity, Color.red);
                        GLUtilities.DrawBox(boundaryRect);
                        GLUtilities.DrawBox(tRect);

                        GLUtilities.EndDrawing();
                        break;
                    }
            }
        }

        public Rect RectByVectorEdges(Vector2 leftCorner, Vector2 rightCorner)
        {
            var centerPoint = (leftCorner + rightCorner) / 2f;
            var size = new Vector2(
                    Mathf.Abs(leftCorner.x - rightCorner.x),
                    Mathf.Abs(leftCorner.y - rightCorner.y)
                );

            return new Rect(centerPoint, size);
        }*/

    }
}
