using UnityEngine;

namespace SF.HandleModule
{
    [System.Serializable]
    public class HandleData
    {
        public float Size = 3;
        // This is the direction the handle shape is pointing toward.
        public Vector3 Normal = Vector3.forward;
        public Vector3 Direction = Vector3.right;
        public float Angle = 90;
        public Color HandleColor;

        public HandleData(float size, Vector3 normal, Vector3 direction,float angle, Color handleColor)
        {
            Size = size;
            Normal = normal;
            Direction = direction;
            Angle = angle;
            HandleColor = handleColor;
        }
    }
}
