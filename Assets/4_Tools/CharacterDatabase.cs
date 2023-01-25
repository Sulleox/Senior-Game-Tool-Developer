using System.Collections.Generic;
using UnityEngine;

public class CharacterDatabase : ScriptableObject
{
    private List<CharacterEntry> m_characters;

    public void AddCharacter(string name, int price, GameObject prefab)
    {
        CharacterEntry newCharacter = new CharacterEntry(name, price, prefab);
        if (m_characters == null)
            m_characters = new List<CharacterEntry>();
        m_characters.Add(newCharacter);
    }
}
