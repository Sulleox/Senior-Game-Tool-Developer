using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CharacterDatabase : ScriptableObject
{
    public List<CharacterEntry> m_characters = new List<CharacterEntry>();

    public void CheckCharacters()
    {
        List<CharacterEntry> missingPrefabsCharacters = new List<CharacterEntry>();
        foreach(CharacterEntry character in m_characters)
        {
            if(character == null || character.Prefab == null)
            {
                missingPrefabsCharacters.Add(character);
            }
        }

        foreach(CharacterEntry missingPrefabEntry in missingPrefabsCharacters)
        {
            Debug.Log($"Character {missingPrefabEntry.Name} is missing is prefab, removing it from DB");
            m_characters.Remove(missingPrefabEntry);
            EditorUtility.SetDirty(this);
        }
    }

    public void AddCharacter(string characterName, int price, GameObject prefab, int priority, Sprite icon)
    {
        CharacterEntry newCharacterEntry = CreateCharacterEntry(characterName, price, prefab, priority, icon);
        m_characters.Add(newCharacterEntry);
        EditorUtility.SetDirty(this);
    }

    public CharacterEntry CreateCharacterEntry(string characterName, int price, GameObject prefab, int priority, Sprite icon)
    {
        CharacterEntry newCharacter = ScriptableObject.CreateInstance<CharacterEntry>();
        newCharacter.Price = price;
        newCharacter.Prefab = prefab;
        newCharacter.Icon = icon;
        newCharacter.ShopPriority = priority;

        int iterator = 1;
        string tempCharacterName = characterName;
        while (m_characters.Any(x=> x.Name == tempCharacterName))
        {
            tempCharacterName = $"{characterName}({iterator})";
            iterator++;
        }
        newCharacter.Name = tempCharacterName;

        AssetDatabase.CreateAsset(newCharacter, Path.Combine(ToolsPaths.CHARACTERS_SCRIPTABLES_FOLDER_PATH, $"{newCharacter.Name}.asset"));
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return newCharacter;
    }
}
