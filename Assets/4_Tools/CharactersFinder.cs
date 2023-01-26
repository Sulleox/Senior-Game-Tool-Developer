using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;


public class CharactersFinder : EditorWindow
{
    [MenuItem("Tools/Store Characters Finder")]
    [MenuItem("Assets/Open Store Characters Finder")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(CharactersFinder));
        m_characterDB = AssetDatabase.LoadAssetAtPath<CharacterDatabase>(ToolsPaths.CHARACTERS_DATABASE_ASSET_PATH);
    }

    private static CharacterDatabase m_characterDB = null;

    private GameObject[] m_possibleCharacters;
    private List<string> m_possibleCharactersNames;
    private int m_selectedIndex;

    void OnGUI()
    {
        if (GUILayout.Button("Scan project"))
        {
            m_possibleCharacters = SearchForPossibleCharacterPrefabs();
            m_possibleCharactersNames.Clear();
            if (m_possibleCharacters.Length > 0)
            {
                foreach(GameObject gameObject in m_possibleCharacters)
                {
                    m_possibleCharactersNames.Add(gameObject.name);
                }
            }
        }

        if (m_possibleCharacters.Length > 0)
        {
            m_selectedIndex = EditorGUILayout.Popup(m_selectedIndex, m_possibleCharactersNames.ToArray());
        }
    }

    public GameObject[] SearchForPossibleCharacterPrefabs()
    {
        string[] assets = AssetDatabase.GetAllAssetPaths();
        List<GameObject> result = new List<GameObject>();
        foreach (string asset in assets)
        {
            if (asset.Contains(".prefab"))
            {
                GameObject prefab = (GameObject) AssetDatabase.LoadAssetAtPath<GameObject>(asset);
                if(IsAPossibleNewCharacter(prefab))
                {
                    result.Add(prefab);
                }
            }
        }
        Debug.Log($"Found {result.Count} possible characters");
        return result.ToArray();
    }

    private bool IsAPossibleNewCharacter(GameObject prefab)
    {
        bool hasSkinnedMeshRenderer = prefab.GetComponentInChildren<SkinnedMeshRenderer>() != null;
        bool isInDataBase = false;
        if (m_characterDB != null)
        {
            isInDataBase = m_characterDB.GetCharacter(prefab) != null;
        }
        return hasSkinnedMeshRenderer && !isInDataBase;
    }
}
