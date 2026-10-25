#if !SF_DATABASES
    using System;
    using UnityEngine;

namespace SF.DataModule
{
    /// <summary>
    /// This is a DTOBase that can be used for normal class object where needed.
    /// </summary>
    [Serializable]
    public class DTOBase
    {
        public int ID = 0;
    }
    
    /// <summary>
    /// This is a DTOBase that can be used as a scriptable object where needed.
    /// </summary>
    public class DTOAssetBase : ScriptableObject
    {
        // TODO: Replace the below with either ItemData or Item
        public int ID = 0;
        public string Description;

        public DTOAssetBase() 
        {
           
        }
    }
}
#endif