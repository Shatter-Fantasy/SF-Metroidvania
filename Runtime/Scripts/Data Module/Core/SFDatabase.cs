using System.Collections.Generic;
using UnityEngine;

namespace SF.DataModule
{
    /// <summary>
    /// Wrapper class for SFDatabases to make it easier to register different database types.
    /// <see cref="DatabaseRegistry"/> for example uses.
    /// </summary>
    public abstract class SFDatabase : ScriptableObject
    {
        
        /// <summary>
        /// SFDatabases are registered by the <see cref="DatabaseRegistry"/> which is loaded during the player start up
        /// as part of the preloaded assets set in the project's PlayerSettings via SetPreloadedAssets.
        /// So there OnEnable runs when the runtime player starts and is guaranteed to run before anything scene related.
        /// </summary>
        public virtual void OnRegisterDatabase() { }

        public virtual void OnDeregisterDatabase(){ }
    }
    
    /// <summary>
    /// A generic database class for storing data about DTOBase classes or sub classes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SFDatabase<T> : SFDatabase where T : DTOAssetBase
    {
        [SerializeReference] public List<T> DataEntries = new List<T>();

        public virtual void AddData(T dataEntry)
        {
            if(dataEntry == null)
                return;

            DataEntries.Add(dataEntry);
        }

        public void RemoveData(T dataEntry)
        {
            if(dataEntry == null)
                return;

            DataEntries.Remove(dataEntry);
        }

        public T GetDataByID(int characterId)
        {
            return DataEntries.Find((T data) => data.ID == characterId);
        }
        
        public bool GetDataByID(int characterId, out T data)
        {
            data = DataEntries.Find((T data) => data.ID == characterId);

            return data != null;
        }

        public T this[int index]
        {
            get { return DataEntries[index]; }
        }
    }
}
