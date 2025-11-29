using UnityEngine;
using UnityEngine.Serialization;

namespace SF.RoomModule
{
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
    public class RoomTransition
    {
        /// <summary>
        /// The id of the room we are transitioning away from and exiting aka the current room we are in when starting a transition. <see cref="Room.RoomID"/>
        /// </summary>
        [FormerlySerializedAs("RoomID")] public int LeavingRoomID;
        /// <summary>
        /// What form of transition logic needs to be run for the relavent transition.
        /// </summary>
        public TransitionTypes TransitionType;
        /// <summary>
        /// The position of the current room that should be linked and aligned to the other rooms transition position.
        /// </summary>
        public Vector2 LinkPosition;
        /// <summary>
        /// The id of the room we are transitioning to and entering. <see cref="Room.RoomID"/>
        /// </summary>
        [Header("Data Ids")]
        public int EnteringRoomID;
        /// <summary>
        /// The id of the transition in the room, we are transitioning to and entering. <see cref="Room.RoomID"/>
        /// </summary>
        public int EnteringTransitionID;
    }
}
