using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SF
{
	public static class LoggingSystem 
	{
		public static bool DoLogging = true;

		[System.Diagnostics.Conditional("UNITY_EDITOR")]
		public static void LogMessage(string message, Object context, LogType logType = LogType.Log)
		{
#if UNITY_EDITOR
			if(!DoLogging) return;

			Debug.Log(message, context);
#endif
		}

		[System.Diagnostics.Conditional("UNITY_EDITOR")]
		public static void LogNullObject(Object context, Type nullObject, LogType logType = LogType.Log)
		{
#if UNITY_EDITOR
			if(!DoLogging) return;

			Debug.Log($"The {nullObject} passed on the type {context} was null.", context);
#endif
		}
	}
}
