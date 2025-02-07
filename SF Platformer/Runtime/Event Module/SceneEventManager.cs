using System.Collections;
using System.Collections.Generic;

using SF.Events;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace SF.Scenes
{
    public class SceneEventManager : MonoBehaviour
    {
		#region Scene Change Events
		[SerializeField] private SerializedEvent SceneLoadEvent;
		[SerializeField] private SerializedEvent SceneDeloadEvent;
		[SerializeField] private SerializedEvent SceneChangeEvent;
		#endregion


		public async Awaitable LoadScene(string sceneName, LoadSceneMode loadMode = LoadSceneMode.Single)
		{
			await SceneManager.LoadSceneAsync(sceneName, loadMode);
		}

		#region Scene Events
		private void OnSceneLoad(Scene scene, LoadSceneMode loadMode)
		{
			SceneLoadEvent?.Invoke();
		}
		private void OnSceneUnload(Scene scene)
		{
			SceneDeloadEvent?.Invoke();
		}
		private void OnSceneChanged(Scene previousScene, Scene newScene)
		{
			SceneChangeEvent?.Invoke();
		}
		#endregion

		private void OnEnable()
		{
			SceneManager.sceneLoaded += OnSceneLoad;
			SceneManager.sceneUnloaded += OnSceneUnload;
			SceneManager.activeSceneChanged += OnSceneChanged;
		}
		private void OnDisable()
		{
			SceneManager.sceneLoaded -= OnSceneLoad;
			SceneManager.sceneUnloaded -= OnSceneUnload;
			SceneManager.activeSceneChanged -= OnSceneChanged;
		}
	}
}
