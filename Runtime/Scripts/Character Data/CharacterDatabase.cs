using System.Collections.Generic;
using UnityEngine;

using SF.DataModule;
using SF.StatModule;

namespace SF.Characters.Data
{
    /// <summary>
    /// A generic database class for storing data about DTOBase classes or sub classes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SFDatabase<T> : ScriptableObject where T : DTOAssetBase
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


    [CreateAssetMenu(fileName = "Character Database", menuName = "SF/Data/Character Editor Database")]
    public class CharacterDatabase : SFDatabase<CharacterDTO>
    {
        /// <summary>
        /// The database with the default stat types and declarations in it.
        /// </summary>
        public StatSetDatabase StatDatabase;

        public override void AddData(CharacterDTO dataEntry)
        {
            if(StatDatabase != null)
            {
                dataEntry.Stats = StatDatabase.DefaultStatTypes;
            }          
            base.AddData(dataEntry);
        }
    }
}
