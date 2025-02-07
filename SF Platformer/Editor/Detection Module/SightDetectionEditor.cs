using UnityEditor;

namespace SF.DetectionModule
{
    public class SightDetectionEditor : Editor
    {
        /*
        void OnSceneGUI()
        {

            SightDetectionDecision myObj = (SightDetectionDecision)target;
            float radian = myObj.StartingAngle * Mathf.Deg2Rad;
            Handles.color = myObj.ToolHandleColor;
            Handles.DrawSolidArc(myObj.transform.position, myObj.transform.forward, new Vector3(Mathf.Cos(radian),Mathf.Sin(radian)) , myObj.FieldOfView, myObj.DetectionSize);

            EditorGUI.BeginChangeCheck();
            float newSize = Handles.ScaleValueHandle(myObj.DetectionSize,
                myObj.transform.position + myObj.transform.right,
                myObj.transform.rotation, 2, Handles.ConeHandleCap, 1);

            if(EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Updated Size");
                myObj.DetectionSize = newSize;
            }

            float newFOV = Handles.ScaleValueHandle(myObj.FieldOfView,
                myObj.transform.position - myObj.transform.right,
                myObj.transform.rotation, 4, Handles.CircleHandleCap, 1);

            if(EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Updated FOV");
                myObj.FieldOfView = newFOV;
            }
        }
        */
    }
}
