using UnityEngine;

namespace SF.RoomModule.RegionModule
{
    using DataModule;
    using static Managers.GameDefaultExecutionOrders;
    
    [DefaultExecutionOrder(DatabaseExecutionOrder)]
    [CreateAssetMenu(fileName = "RegionDatabase", menuName = "SF/Regions/Region Database")]
    public class RegionDatabase : SFAssetDatabase<RegionDataAsset>
    {
        public RegionDatabase()
        {
            DatabaseLoadOrder = 0;
        }
        public override void OnRegisterDatabase()
        {
            RegionSystem.RegionDatabase = this;
        }
        
        public override void OnDeregisterDatabase()
        {
            RegionSystem.RegionDatabase = null;
        }
    }
}
