using SF.DetectionModule;
using UnityEditor;
using UnityEngine;

namespace SF.DetectionModule
{
    public class ArcHandle : IHandle
    {
        private HandleData _handleData;
        public ArcHandle(HandleData handleData)
        {
            _handleData = handleData;
        }

        public void DrawHandle(Transform target)
        {
            Handles.color = _handleData.HandleColor;
            Handles.DrawSolidArc(target.transform.position, _handleData.Normal, _handleData.Direction, _handleData.Angle, _handleData.Size);

            _handleData.Size = (float)Handles.ScaleValueHandle(_handleData.Size,
                target.transform.position + target.transform.forward * _handleData.Size,
                target.transform.rotation, 1, Handles.ConeHandleCap, 1);                
        }
    }
}
