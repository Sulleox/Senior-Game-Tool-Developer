using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CharacterDatabase : ScriptableObject
{
    public List<CharacterEntry> m_characters = new List<CharacterEntry>();
    private HashSet<int> m_Ids = new HashSet<int>();

    public void Init()
    {
        CheckCharacters();
        foreach (CharacterEntry entry in m_characters)
        {
            m_Ids.Add(entry.Id);
        }
    }

    public void CheckCharacters()
    {
        m_characters.RemoveAll(x => x == null);

        //TO DO : Add a dialog to choose or show what happens to the characters
        m_characters.RemoveAll(x => x.Prefab == null);
    }

    public void AddCharacter(string characterName, int price, GameObject prefab, int order, Sprite icon, Material material, Avatar avatar, UnityEditor.Animations.AnimatorController controller)
    {
        CharacterEntry newCharacterEntry = CreateCharacterEntry(characterName, price, prefab, order, icon, material, avatar, controller);
        m_characters.Add(newCharacterEntry);
        EditorUtility.SetDirty(this);
    }

    public CharacterEntry GetCharacter(GameObject prefab)
    {
        return m_characters.Find(x => AssetDatabase.GetAssetPath(x.Prefab) == AssetDatabase.GetAssetPath(prefab));
    }

    public void UpdateCharacter(string characterName, int price, GameObject prefab, int order, Sprite icon, Material material, Avatar avatar, UnityEditor.Animations.AnimatorController controller)
    {
        CharacterEntry character = m_characters.Find(x => x.Name == characterName);
        character.Price = price;
        character.Prefab = prefab;
        character.Icon = icon;
        character.ShopOrder = order;
        character.Material = material;
        character.Avatar = avatar;
        character.Animator = controller;
    }

    public CharacterEntry CreateCharacterEntry(string characterName, int price, GameObject prefab, int order, Sprite icon, Material material, Avatar avatar, UnityEditor.Animations.AnimatorController controller)
    {
        CharacterEntry newCharacter = ScriptableObject.CreateInstance<CharacterEntry>();
        newCharacter.Price = price;
        newCharacter.Prefab = prefab;
        newCharacter.Icon = icon;
        newCharacter.ShopOrder = order;
        newCharacter.Id = GetCharacterUniqueId();
        newCharacter.Name = GetCharacterUniqueName(characterName);
        newCharacter.Material = material;
        newCharacter.Avatar = avatar;
        newCharacter.Animator = controller;

        AssetDatabase.CreateAsset(newCharacter, Path.Combine(ToolsPaths.CHARACTERS_SCRIPTABLES_FOLDER_PATH, $"{newCharacter.Name}.asset"));
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return newCharacter;
    }

    private int GetCharacterUniqueId()
    {
        int randomId = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        while (m_Ids.Contains(randomId))
        {
            randomId = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        }
        return randomId;
    }

    private string GetCharacterUniqueName(string characterName)
    {
        int iterator = 1;
        string tempCharacterName = characterName;
        while (m_characters.Any(x => x.Name == tempCharacterName))
        {
            tempCharacterName = $"{characterName}({iterator})";
            iterator++;
        }
        return tempCharacterName;
    }
}
