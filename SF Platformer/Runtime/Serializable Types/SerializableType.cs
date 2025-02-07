using System;

using UnityEngine;

namespace SF
{
    [Serializable]
    public class SerializableType : ISerializationCallbackReceiver
    {
		[SerializeField] string _assemblyQualifiedName = string.Empty;

		public Type Type { get; private set; }

		public void OnAfterDeserialize()
		{
			if(!TryGettype(_assemblyQualifiedName, out var type))
			{
				// Do a debug statement here for types not found.
				return;
			}
			Type = type;
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			_assemblyQualifiedName = Type?.AssemblyQualifiedName ?? _assemblyQualifiedName;
		}

		static bool TryGettype(string typeString, out Type type)
		{
			type = Type.GetType(typeString);
			return type != null || !string.IsNullOrEmpty(typeString);
		}
    }
}
