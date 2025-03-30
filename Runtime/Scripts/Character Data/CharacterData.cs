using UnityEngine;

namespace SF.Characters.Data
{
    /// <summary>
    /// Contains the data of the character used for databases, spawning, and more.
    /// </summary>
    public class CharacterData : MonoBehaviour
    {
        /// <summary>
        /// The ID of the character in the database.
        /// </summary>
        public int CharacterID;
        
        public virtual void SetData(CharacterDTO dto)
        {
            CharacterID = dto.ID;
        }
    }
}
