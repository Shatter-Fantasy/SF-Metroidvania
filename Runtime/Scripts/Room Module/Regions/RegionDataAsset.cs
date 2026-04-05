using System.Collections.Generic;
using UnityEngine;

namespace SF.RoomModule.RegionModule
{
    using SF.DataModule;
    
    [CreateAssetMenu(menuName = "SF/Regions/Region Data", fileName = "Region Data")]
    public class RegionDataAsset : DTOAssetBase
    {
        /// <summary>
        /// The scene index for the region.
        /// </summary>
        public int SceneIndex; 
        
        /// <summary>
        /// The ids of the rooms that act like transitions to and from a region.
        /// </summary>
        public List<int> TransitionRoomIDs = new List<int>();
    }
}
