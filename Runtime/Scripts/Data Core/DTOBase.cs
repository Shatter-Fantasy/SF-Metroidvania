using UnityEditor;

using UnityEngine;

namespace SF
{


    /// <summary>
    /// This is an empty wrapper class for DTO based classes.
    /// 
    /// Example of this wrapper being utilized can be seen in the abstract DataView class that takes in a generic type that inherits from DTOBase.
    /// <seealso cref="SF.Inventory.ItemGeneralDTO"/>
    /// <seealso cref="SF.Inventory.ItemPriceDTO"/>
    /// </summary>
    public class DTOBase 
    {
        public int ID = 0;
        public string GUID;
        public string Name;
        public string Description;
    }

    /// <summary>
    /// This is a DTObase that can be used as a scriptable object where needed.
    /// </summary>
    public class DTOAssetBase : ScriptableObject
    {
        public int ID = 0;
        public int GUID;
        public string Name;
        public string Description;

        public DTOAssetBase() 
        {
           
        }
    }

}
