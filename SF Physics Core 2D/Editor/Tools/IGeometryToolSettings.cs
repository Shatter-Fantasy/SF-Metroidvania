using UnityEngine;

namespace SFEditor.U2D.Physics
{
    /// <summary>
    /// Settings for the scene tool used in low level physics <see cref="SF.U2D.Physics.SFShapeComponent"/>
    /// in scene mode.
    /// </summary>
    public interface IGeometryToolSettings
    {
        public Color GrabHandleColor { get; set; }
    }
}
