using UnityEngine;

namespace SF.Managers
{
    public enum GameLoadingMode
    {
        NewGame, LoadGame, Continue
    }
    
    [CreateAssetMenu(fileName = "SF GameLoader SO", menuName = "SF/Managers/GameLoader SO")]
    public class GameLoaderSO : ScriptableObject
    {

        /// <summary>
        /// The index of the scene for a new game inside the build profile list.
        /// </summary>
        [Header("Scene Loading Data")]
        [field: SerializeField] public int NewGameSceneIndex { get; private set; } = 1;
        /// <summary>
        /// Is set to true when a new game is being initialized.
        /// <remarks>
        /// This is used in places like SceneManager.loadedScene event callbacks to see if we are loading a new game first playable scene or not.
        /// </remarks>
        /// </summary>
        public bool SettingUpNewGame = false;

        [Header("Room Data")]
        public int StartingRoomID = 0;

        /// <summary>
        /// The Game Loader prefab that will be spawned during the game runtime initialization if one is not in the starting scene.
        /// </summary>
        /// <remarks>
        /// Important note: Because this is a static variable that doesn't appear in inspector, we set it in an OnValidate that only runs in Unity Editor.
        /// There is a  _gameLoaderObj wrapped in an #if UNITY_EDITOR that is stripped when building and going into runtime.
        /// </remarks>
        private static GameLoader _gameLoaderInstance; 
        
        /*
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void SetupGameManagers()
        {
            // If the game was already initialized nothing is needed to be done.
            if (GameLoader.WasGameInitialized)
                return;
            
        }
        */
    }
}
