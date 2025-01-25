using System.Collections.Generic;

using UnityEngine;

using SF.Characters.Data;
using SF;

namespace SFEditor.Characters.Data
{
    /// <summary>
    /// A generic database class for storing data about DTOBase classes or sub classes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SFDatabase<T> : ScriptableObject where T : DTOAssetBase
    {
        [SerializeReference] public List<T> DataEntries = new List<T>();

        public void AddData(T dataEntry)
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

        public T this[int index]
        {
            get { return DataEntries[index]; }
        }
    }


    [CreateAssetMenu(fileName = "Character Database", menuName = "SF/Data/Character Editor Database")]
    public class CharacterDatabase : SFDatabase<CharacterDTO>
    {
    }
}
