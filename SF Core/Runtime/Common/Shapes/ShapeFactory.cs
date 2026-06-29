using System;
using System.Collections.Generic;

using SF.Utilities.InteropServices;

using UnityEngine;
using UnityEngine.Rendering;

namespace SF.Utilities.Shapes
{

    /* Important knowledge about the Shape utilities.
     * These are made for performance reasons.
     * 
     * Things to remember just using structs, ref structs, and different span types won't be instant better performance.
     * If you are doing a lot of boxing and unboxing of data it can actually be worse performance when using them. 
     * 
     * Boxing is moving data from the stack to the heap
     * Unboxing is moving data from the heap to the stack.
     * 
     * Ref Struct information
     * Ref structs allocated on the stack.
     * Ref structs can not be moved to managed memory aka the heap.
     * Ref structs can not be boxed to System.ValueType or System.Object
     * https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/ref-struct
     
     public void Boxing()
     {
        int i = 10; // Line 1
        object o = i; // Line 2 shows an example of boxing
        int io = (int)o; // Line 3 show an example of unboxing.
     }
     * 
     * Notes to keep in mind when using Unity's Burst Compiler
     * Burst has basic support for getting static readonly data, 
     * but if you want to share a static mutable data between C# and HPC#, you have 
     * to use a SharedStatic<T> struct
     
        public abstract class MutableStaticTest
        {
        public static readonly SharedStatic<int> IntField = SharedStatic<int>.GetOrCreate<MutableStaticTest, IntFieldKey>();

        // Define a Key type to identify IntField
        private class IntFieldKey {}
        }
        
        C# and HPC# can then access this:
        
        // Write to a shared static 
        MutableStaticTest.IntField.Data = 5;
        // Read from a shared static
        var value = 1 + MutableStaticTest.IntField.Data;
     
      
     * This is done by doing the following.
     * 
     * Memory Performance.
     * 1. Use of structs and ref structs for stack allocation instead of heap allocation.
     * 2. Spans and ReadOnlySpans to allow for faster memory caching of the collection data types.
     *
     */



    /// <summary>
    /// The base class for all SF Shapes.
    /// </summary>
    /// <remarks>
    ///     This class is not marked as abstract so it can be instantiated than assigned to on the fly
    ///     while the initial instantiation can implement a constructor.
    ///     
    ///     It also will use a struct for shape data to allow for some performance and memory usages when copying it's path data into stuff like Unity's <see cref="UnityEngine.PhysicsShape2D"/> class.
    /// </remarks>
    public class ShapeFactory : MonoBehaviour
    {
        public List<Vector3> Points; // Vertices when doing Mesh or shader stuff with the tool

        public ShapeFactory()
        {
            Points = new List<Vector3>();
        }

        public ShapeFactory(List<Vector3> points)
        {
            Points = points ?? new List<Vector3>();
        }

        /// <summary>
        /// Returns true if a Span was able to be made or false it it failed.
        /// Also sends back a ref type of Span[Vector2] to use for any data you need it for.
        /// </summary>
        /// <param name="spanPoints"></param>
        /// <returns></returns>
        public bool TryGetPointAsSpan(ref Span<Vector3> spanPoints)
        {
            if(Points == null || Points.Count < 1)
                return false;

            spanPoints = CollectionsMarshal.AsSpan<Vector3>(Points);
            return true;
        }

        public Span<Vector3> GetPointsAsSpan()
        {
            if(Points == null || Points.Count < 1)
                return new Span<Vector3>();

            return CollectionsMarshal.AsSpan<Vector3>(Points);
        }


        private void DrawCamera(ScriptableRenderContext ctx, List<Camera> cameras)
        {
            if(Points == null || Points.Count < 1)
                return;

            GLUtilities.InitHandleMaterial();
            GLUtilities.ApplyHandleMaterial();
            GLUtilities.StartDrawing(Matrix4x4.Ortho(0,1,0,1,-1,100), GL.LINE_STRIP);
            GLUtilities.DrawLine(Vector2.zero,Vector2.one);
            GLUtilities.EndDrawing();
        }
    }

    public static class SFMeshUtilities
    {
        public static Mesh CreateMesh(Span<Vector3> vertices)
        {
            Mesh mesh = new Mesh
            {
                vertices = vertices.ToArray()
            };
            int[] triangles = new int[3];
            for(int i = 0; i < vertices.Length; i++ )
            {
                triangles[i] = i;
            }
            mesh.triangles = triangles;
            return mesh;
        }
    }

    public static class SFRendererUtilities
    {
        public static RenderTexture OutputTexture;

        public static Camera RenderCamera;
        public static Matrix4x4 ProjectionMatrix;
        public static float VirtualCameraSize = 1080;

        public static Material DebugMaterial;
        public static Color OutlineColor = Color.green;

        public readonly static List<Mesh> DebugMeshes = new List<Mesh>();

        private static void InitializeDebugMaterial()
        {
            DebugMaterial = new Material(Shader.Find("Unlit/Texture"))
            {
                color = OutlineColor
            };
        }

        public static Matrix4x4 SetOrthoProjectionMatrix()
        {
            if(RenderCamera == null)
                RenderCamera = Camera.main;

            RenderTextureDescriptor descriptor = new RenderTextureDescriptor(Screen.width,Screen.height);

            if(OutputTexture == null)
                OutputTexture = new RenderTexture(descriptor);

            ProjectionMatrix = Matrix4x4.Ortho(
                -VirtualCameraSize, VirtualCameraSize,
                -VirtualCameraSize, VirtualCameraSize,
                (RenderCamera != null) ? RenderCamera.nearClipPlane : 0,
                (RenderCamera != null) ? RenderCamera.farClipPlane : 0
            );

            return ProjectionMatrix;
        }

        public static void RenderMeshes(ScriptableRenderContext context, List<Camera> cameras)
        {
            if(DebugMaterial == null)
                InitializeDebugMaterial();

            //RenderParams renderParams = new RenderParams(DebugMaterial);
            CommandBuffer cmd = new CommandBuffer();
            cmd.name = "Debug Command Buffer";
            // -10f is camera distance
            cmd.SetViewMatrix(Matrix4x4.TRS(new Vector3(0, 0, -10f), Quaternion.identity, Vector3.one));
            cmd.SetProjectionMatrix(SetOrthoProjectionMatrix());

            cmd.SetRenderTarget(OutputTexture);


            for(int i = 0; i < DebugMeshes.Count; i++)
            {
                /*Graphics.RenderMesh(
                    renderParams,
                    Resources.GetBuiltinResource<Mesh>("Cube.fbx"),
                    0,
                    Matrix4x4.Translate(new Vector3(-5,0 + i,-5.0f))
                 );            
                */
                
                 cmd.DrawMesh(
                   DebugMeshes[i],
                   Matrix4x4.Translate(new Vector3(-5, 0 + i, -5.0f)),
                   DebugMaterial,
                   0,
                   0
                );              
            }

            Graphics.ExecuteCommandBuffer(cmd);
        }

        public static void AddDebugMesh(Mesh mesh)
        {
            DebugMeshes.Add(Resources.GetBuiltinResource<Mesh>("Cube.fbx"));
        }
    }


    /// <summary>
    /// Shape data struct that keeps track of the position of the single point and all the <see cref="ShapeEdge"/>
    /// it is a part of.
    /// </summary>
    public struct ShapePoint
    {
        public Vector3 Position;

        public List<ShapeEdge> ConnectedEdges;

        public ShapePoint(Vector3 position)
        {
            Position = position;
            ConnectedEdges = new List<ShapeEdge>();
        }

        public ShapePoint(Vector3 position, List<ShapeEdge> connectedEdges)
        {
            Position = position;
            ConnectedEdges = connectedEdges;
        }

        public static Vector3 ShapePointToVector3(ShapePoint shapePoint) => shapePoint;

        public static implicit operator Vector3(ShapePoint shapePoint) => shapePoint.Position;
        public static explicit operator ShapePoint(Vector3 position) => new ShapePoint(position);


    }

    /// <summary>
    /// This keeps tracks of all the connections between two points in a shape.
    /// Useful for mesh generation, rendering shape creation, and custom physics shape making.
    /// Example <see cref="UnityEngine.PhysicsShape2D"/>
    /// </summary>
    public struct ShapeEdge
    {
        public ShapePoint StartPoint;
        public ShapePoint EndPoint;
    }
}
