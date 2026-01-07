using UnityEngine;

namespace SFEditor.PhysicsLowLevel
{
    /// <summary>
    /// Settings for the scene tool used in low level physics <see cref="SF.PhysicsLowLevel.SFShapeComponent"/>
    /// in scene mode.
    /// </summary>
    public interface IGeometryToolSettings
    {
        public Color GrabHandleColor { get; set; }
    }
}
