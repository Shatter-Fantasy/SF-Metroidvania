using UnityEngine;

namespace SF
{
    public class InputManager : MonoBehaviour
    {
		public static Controls Controls { get; private set; }

		private static InputManager _instance;
		public static InputManager Instance
		{
			get
			{
				if(_instance == null)
					_instance = FindFirstObjectByType<InputManager>();
				return _instance;
			}
		}

		#region Starting Lifecycle Functions
		private void Awake()
		{
			if(Instance != null && Instance != this)
			{
				Destroy(this);
				return;
			}
		}
		#endregion
	}
}
