using System;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace SF.Pathfinding
{
	public enum TrigFunctionType
	{
		Sin,
		Cos,
		Tan
	}

	public class MathGraphMovement : MonoBehaviour
    {
		public Vector2 Speed = new Vector2(1,2);
		public TrigFunctionType TrigMovementType;
		public float Amplitude;

		private Transform _transform;
		private Vector3 _startingPos;

		public float YOffSet = 0;
		private void Awake()
		{	
			_transform = GetComponent<Transform>();
			_startingPos = _transform.position;
		}

		private void Update()
		{
			switch(TrigMovementType) 
			{
				case TrigFunctionType.Cos:
				{
						break;
				}
				case TrigFunctionType.Sin:
				{



						// y = sin(Angle) * Amplitude + Y Amplitude Offset
						YOffSet = Mathf.Sin(Time.time) * Amplitude;
						Vector3 targetPos = new Vector3(_startingPos.x, _startingPos.y + YOffSet, _startingPos.z);
						transform.position = Vector3.Lerp(_startingPos, targetPos, Mathf.Abs(YOffSet) * Speed.y);
						break;
				}
			}

		}
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
		{
			Handles.color = Color.yellow;
			Vector3 tangentStart = new Vector3(transform.position.x, transform.position.y + Amplitude,transform.position.z);
			Vector3 tangentEnd = new Vector3(transform.position.x, transform.position.y - Amplitude,transform.position.z);

			Handles.DrawBezier(transform.position, transform.up, tangentStart, tangentEnd,Color.yellow,null,5);
		}
#endif
	}
}
