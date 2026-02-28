using UnityEngine;

namespace SF.DataModule
{
    /// <summary>
    /// This is a DTOBase that can be used for normal class object where needed.
    /// </summary>
    public class DTOBase
    {
        
    }
    
    /// <summary>
    /// This is a DTOBase that can be used as a scriptable object where needed.
    /// </summary>
    public class DTOAssetBase : ScriptableObject
    {
        // TODO: Replace the below with either ItemData or Item
        public int ID = 0;
        public string Name;
        public string Description;

        public DTOAssetBase() 
        {
           
        }
    }
}
