using UnityEngine;

namespace SF.Managers
{
    using RoomModule;
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
    }
    
    /// <summary>
    /// Declares the default order for important game features other than physics.
    /// for Physics DefaultExecutionOrder see <see cref="PhysicsLowLevel.LowLevelPhysicsExecutionOrder"/>
    /// </summary>
    public static class GameDefaultExecutionOrders
    {
        public const int DatabaseExecutionOrder = -10;
    }
}
