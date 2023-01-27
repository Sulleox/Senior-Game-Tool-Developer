using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        EditorWindow window = EditorWindow.GetWindow(typeof(CharactersFinder));
        window.minSize = new Vector2(250f, 200f);
    }

    //Would be better as a Dictionnary
    private List<GameObject> m_possibleCharacters = new List<GameObject>();
    private List<string> m_possibleCharactersNames = new List<string>();
    private int m_selectedIndex;

    void OnGUI()
    {
        if (GUILayout.Button("Scan project"))
        {
            m_possibleCharacters = SearchForPossibleCharacterPrefabs();
            m_possibleCharactersNames.Clear();
            if (m_possibleCharacters.Count > 0)
            {
                foreach(GameObject gameObject in m_possibleCharacters)
                {
                    m_possibleCharactersNames.Add(gameObject.name);
                }
            }
        }

        if (m_possibleCharacters.Count > 0)
        {
            m_selectedIndex = EditorGUILayout.Popup(m_selectedIndex, m_possibleCharactersNames.ToArray());
            if (GUILayout.Button("Open in Character Importer"))
            {
                StoreCharactersImporterWindows storeCharactersImporter = GetWindow<StoreCharactersImporterWindows>();
                StoreCharactersImporterWindows.ShowWindow();
                storeCharactersImporter.LoadPossibleCharacter(m_possibleCharacters[m_selectedIndex]);
            }
        }

    }

    public List<GameObject> SearchForPossibleCharacterPrefabs()
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
        return result;
    }

    private bool IsAPossibleNewCharacter(GameObject prefab)
    {
        bool hasSkinnedMeshRenderer = prefab.GetComponentInChildren<SkinnedMeshRenderer>() != null;
        bool isInDataBase = ToolsUtils.CharacterDB.GetCharacter(prefab) != null;
        return hasSkinnedMeshRenderer && !isInDataBase;
    }
}
