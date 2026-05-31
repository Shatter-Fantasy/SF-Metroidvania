using System;

using SF.Utilities;

using UnityEditor;

using UnityEngine;

namespace SFEditor.Utilities
{
    public enum OutlineRenderMode
    {
        Solid,
        Dotted,
    }
    public enum FillRenderMode
    {
        Solid,
        TransparentFill,
    }

    public static class HandlesUtilities
    {
        /* Circle Geomentry Formulas
        // Circle Area = PI * Radius * Radius
        // Area of a sector of a circle = (Theta Angle / 2 * PI) * (PI * Radius * Radius)
        // Area of a segment of a circle when Theta angle is in Radians = (Theta Angle - Sin(Theta Angle) /2 ) * Radius * Radius 
        // Area of a segment of a circle when Theta angle is in degrees = ((Theta Angle * PI) / 360) * Radius * Radius
        */
        
        public static void DrawCircle2D(Vector2 center,float angle, float radius, int amountOfPoints)
        {

            Handles.DrawWireCube(center, new Vector3(radius, radius, 1));

            if(Event.current.type != EventType.Repaint)
                return;

            Handles.color = Color.red;

            GLUtilities.ApplyHandleMaterial();
            GLUtilities.StartDrawing(Handles.matrix, GL.LINES);

            for(int i = 1; i < amountOfPoints; i++)
            {
                GL.Vertex(i * Vector2.one);
                GL.Vertex(new Vector2(MathF.Sin(angle) * radius, MathF.Cos(angle)) * radius);
            }

            GLUtilities.EndDrawing();
        }

        public static void DrawArc2D(Vector2 startingPoint, float angle, float radius)
        {

        }
        public static void GetCircleSectorPoints(out Vector2[] points)
        {
            points = new Vector2[4];

        }
    }
}
