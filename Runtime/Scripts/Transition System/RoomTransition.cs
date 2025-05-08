using SF.Characters.Controllers;
using SF.Interactables;
using SF.Transitions;
using UnityEngine;

namespace SF.RoomModule
{
    /// <summary>
    /// The interactable object that tells the <see cref="TransitionSystem"/> to start a transition between two different rooms.
    /// </summary>
    public class RoomTransition : MonoBehaviour, IInteractable
    {
        [field:SerializeField]public InteractableMode InteractableMode { get; set; }
        public RoomTransitionLink RoomTransitionLink;
        
        public void Interact() { }

        public void Interact(PlayerController controller)
        {
            /* Step one: Load room we are transitioning to via the proper TransitionTypes.
             * Step two: Find the transition point we are transitioning specifically to via the SpawnedInstance of that room's RoomController. 
             * Step three: Move the player to the targeted transition point and move the camera as well via VirtualCamera.OnTeleport.
             */
            TransitionSystem.DoTransition(RoomTransitionLink);
        }
    }

    public enum TransitionTypes
    {
        RoomToRoom,
        FastTravel,
        NPCDialogue,
        ItemUse,
        Local // Think going from inside or outside a building. Example entering/exiting shops and the tavern.
    }
    
    
    /// <summary>
    /// Keeps track of the rooms involved in a possible transition and als keeps track of which part of the entering room to position.
    /// </summary>
    [System.Serializable]
    public struct RoomTransitionLink
    {
        /// <summary>
        /// The id of the room we are transitioning away from and exiting aka the current room we are in when starting a transition. <see cref="Room.RoomID"/>
        /// </summary>
        public int CurrentRoomID;
        /// <summary>
        /// What form of transition logic needs to be run for the relavent transition.
        /// </summary>
        public TransitionTypes TransitionType;
        /// <summary>
        /// The id of the room we are transitioning to and entering. <see cref="Room.RoomID"/>
        /// </summary>
        public int EnteringRoomID;
        /// <summary>
        /// The id of the transition in the room, we are transitioning away from and exiting aka the current room we are in when starting a transition.  <see cref="Room.RoomID"/>
        /// </summary>
        public int CurrentRoomTransitionID;
        /// <summary>
        /// The id of the transition in the room, we are transitioning to and entering. <see cref="Room.RoomID"/>
        /// </summary>
        public int EnteringTransitionID;
    }
}
