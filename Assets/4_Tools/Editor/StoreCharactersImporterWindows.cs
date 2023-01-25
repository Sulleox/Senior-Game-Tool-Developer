using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.CodeDom;
using UnityEditor.Animations;

public class StoreCharactersImporterWindows : EditorWindow
{
    [MenuItem("Tools/Store Characters Importer")]
    public static void  ShowWindow () 
    {
        EditorWindow.GetWindow(typeof(StoreCharactersImporterWindows));
    }

    private static CharacterDatabase m_characterDB = null;
    public static CharacterDatabase CharacterDB
    {
        get
        {
            m_characterDB = AssetDatabase.LoadAssetAtPath<CharacterDatabase>(CHARACTERS_DATABASE_PATH);
            if (m_characterDB == null)
            {
                m_characterDB = ScriptableObject.CreateInstance<CharacterDatabase>();
                AssetDatabase.CreateAsset(m_characterDB, CHARACTERS_DATABASE_PATH);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            return m_characterDB;
        }
    }

    private string m_characterName = "Name";
    private int m_characterPrice = 100;
    private int m_characterPriority = 10;
    private GameObject m_characterMesh;
    private Texture2D m_characterIcon;
    private Texture2D m_characterTexture;
    private Material m_characterMaterial;
    private AnimatorController m_animatorController;
    private Avatar m_characterAvatar;

    private string m_characterMeshGUID = string.Empty;
    private GameObject m_characterPrefab = null;

    private const string CHARACTER_PREFAB_PATH = "Assets\\2_Prefabs";
    private const string CHARACTERS_DATABASE_PATH = "Assets\\5_Database\\CharacterDatabase.asset";

    void OnGUI () 
    {
        m_characterName = EditorGUILayout.TextField(m_characterName);
        int.TryParse(EditorGUILayout.TextField(m_characterPrice.ToString()), out m_characterPrice);
        int.TryParse(EditorGUILayout.TextField(m_characterPriority.ToString()), out m_characterPriority);

        GUILayout.BeginHorizontal();

        m_characterIcon = ToolsUtils.TextureField("Icon", m_characterIcon);
        m_characterTexture = ToolsUtils.TextureField("Texture", m_characterTexture);

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        m_characterMesh = ToolsUtils.GameObjectField("FBX", m_characterMesh);
        //Idea : Check if the material can be get from the FBX 
        m_characterMaterial = ToolsUtils.MaterialField("Material", m_characterMaterial);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        m_animatorController = ToolsUtils.AnimatorControllerField("Controller", m_animatorController);
        //Idea : Check if the material can be get from the FBX 
        m_characterAvatar = ToolsUtils.AvatarField("Avatar", m_characterAvatar);
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Generate"))
        {
            ChangeTextureType(m_characterTexture, TextureImporterType.Sprite);
            m_characterPrefab = GenerateCharacterPrefab();
            AddToCharacterDataBase();
        }
    }

    private GameObject GenerateCharacterPrefab()
    {
        if (m_characterIcon == null)
        {
            m_characterIcon = CreateCharacterIcon();
        }
        string meshPath = AssetDatabase.GetAssetPath(m_characterMesh);
        string prefabPath = $"{CHARACTER_PREFAB_PATH}/{m_characterName}.prefab";


        GameObject characterRootGameObject = Instantiate(m_characterMesh);
        Animator characterAnimator = ToolsUtils.GetOrAddAnimator(characterRootGameObject);
        characterAnimator.runtimeAnimatorController = m_animatorController;
        characterAnimator.avatar = m_characterAvatar;
        characterRootGameObject.AddComponent<CapsuleCollider>();

        GameObject characterPrefab = PrefabUtility.SaveAsPrefabAsset(characterRootGameObject, prefabPath);
        DestroyImmediate(characterRootGameObject);
        return characterPrefab;
    }

    private void AddToCharacterDataBase()
    {
        CharacterDB.AddCharacter(m_characterName, m_characterPrice, m_characterPrefab);
    }

    private Texture2D CreateCharacterIcon()
    {
        //Load a scene
        //Import Prefab in the scene
        //Take camera screenshot
        return null;
    }

    private void AddCharacterToStore()
    {

    }

    private void ChangeTextureType(Texture2D texture, TextureImporterType type)
    {
        string path = AssetDatabase.GetAssetPath(texture);
        TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(path);
        importer.textureType = type;
        importer.SaveAndReimport();
    }
}
