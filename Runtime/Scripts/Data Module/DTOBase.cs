using SF.ItemModule;
using UnityEngine;

namespace SF.DataModule
{
    /// <summary>
    /// This is an empty wrapper class for DTO based classes.
    /// 
    /// Example of this wrapper being utilized can be seen in the abstract DataView class that takes in a generic type that inherits from DTOBase.
    /// <seealso cref="SF.ItemModule.ItemPriceDTO"/>
    /// </summary>
    [System.Serializable]
    public class DTOBase 
    {
        public int ID = 0;
        public string Name;
        public string Description;
    }

    /// <summary>
    /// This is a DTObase that can be used as a scriptable object where needed.
    /// </summary>
    public class DTOAssetBase : ScriptableObject
    {
        // TODO: Replace the below with either ItemData or Item
        public int ID = 0;
        public int GUID;
        public string Name;
        public string Description;

        public DTOAssetBase() 
        {
           
        }
    }
}
