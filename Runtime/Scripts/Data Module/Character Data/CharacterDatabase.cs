using UnityEngine;

namespace SF.Characters.Data
{
    using DataModule;
    using StatModule;
    
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
