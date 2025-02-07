using System.Collections.Generic;

using UnityEngine;

namespace SF.Characters
{
    [CreateAssetMenu(menuName = "SF/Data/Character Database", fileName = "Character Database")]
    public class CharacterDB : ScriptableObject
    {
        // We want all data to be retrived by the database functions so we can do custom error checking on them.
        // CharacterDatas will be private.
        [SerializeField] private List<CharacterData> CharacterDatas = new List<CharacterData>();

        public CharacterData GetCharacterDataByID(int characterId)
        {
            return CharacterDatas.Find((data) => data.CharacterID == characterId);
        }

        public CharacterData GetCharacterDataByIndex(int index)
        {
            return CharacterDatas[index];
        }
    }
}
