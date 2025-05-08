using SF.Characters.Controllers;
using SF.Managers;
using SF.RoomModule;
using UnityEngine;

namespace SF.Transitions
{
    /// <summary> Includes telling the <see cref="RoomSystem"/> to load/deload the rooms involved in the transition.
    /// </summary>
    public static class TransitionSystem
    {
        /// <summary>
        /// Does a room transition and sets up the logic to move the player to the target transition position. 
        /// </summary>
        /// <param name="roomTransitionLink"></param>
        public static void DoTransition(RoomTransitionLink roomTransitionLink)
        {
            // We make sure the room we are in when starting the transition has already been set as the current room.
            // This helps down the line for some safety and valid checking when getting the current rooms virtual camera to lower its priority. 
            RoomSystem.SetCurrentRoom(roomTransitionLink.CurrentRoomID);
            
            // Are we doing a simple enter/exit a building like shop or a tavern?
            if (roomTransitionLink.TransitionType == TransitionTypes.Local)
            {
                
                var newRoom = RoomSystem.LoadConnectedRoom(roomTransitionLink.EnteringRoomID);

                if (newRoom == null || newRoom.SpawnedInstance == null)
                {
                    Debug.LogWarning($"There was no room with the RoomID: {roomTransitionLink.EnteringRoomID} inside of the loaded RoomDB or it currently doesn't have it's spawned instance value set.");
                    return;
                }
               
                // This will need improved down line for possible performance improvements for larger rooms.
                if (!newRoom.TransitionsIDs.Contains(roomTransitionLink.EnteringTransitionID))
                {
                    Debug.LogWarning($"There was no RoomTransition with the ID: {roomTransitionLink.EnteringTransitionID} set inside of the room with the RoomID of{roomTransitionLink.EnteringRoomID}.");
                    return;
                }
                
                RoomController newRoomController = newRoom.SpawnedInstance.GetComponent<RoomController>();
                
                // Set up the new rooms cameras, camera boundary and sets the new room as the CurrentRoom at the end.
                newRoomController.MakeCurrentRoom();
                    
                // The below x.RoomTransitionLink.CurrentRoomTransitionID is the entering room transition we are trying to find to move to.
                // The RoomTransitionLink.EnteringTransitionID is the current room we are moving away from.
                var newRoomTransition = newRoomController.RoomTransitions.Find(
                    x => x.RoomTransitionLink.CurrentRoomTransitionID == roomTransitionLink.EnteringTransitionID);

                if (GameManager.PlayerSceneObject != null)
                {
                    GameManager.PlayerSceneObject.transform.position = newRoomTransition.transform.position;
                }
            }
        }
    }
}
