using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.CodeDom;
using UnityEditor.Animations;
using System.IO;

public class StoreCharactersImporterWindows : EditorWindow
{
    [MenuItem("Tools/Store Characters Importer")]
    [MenuItem("Assets/New Store Character")]
    public static void  ShowWindow () 
    {
        EditorWindow.GetWindow(typeof(StoreCharactersImporterWindows));
        CharacterDB.Init();
    }

    private static CharacterDatabase m_characterDB = null;

    public static CharacterDatabase CharacterDB
    {
        get
        {
            m_characterDB = AssetDatabase.LoadAssetAtPath<CharacterDatabase>(ToolsPaths.CHARACTERS_DATABASE_ASSET_PATH);
            if (m_characterDB == null)
            {
                m_characterDB = ScriptableObject.CreateInstance<CharacterDatabase>();
                AssetDatabase.CreateAsset(m_characterDB, ToolsPaths.CHARACTERS_DATABASE_ASSET_PATH);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            //Create character entries folder
            if (!Directory.Exists(ToolsPaths.CHARACTERS_SCRIPTABLES_FOLDER_PATH))
                Directory.CreateDirectory(ToolsPaths.CHARACTERS_SCRIPTABLES_FOLDER_PATH);

            return m_characterDB;
        }
    }

    private string m_characterName = "Name";
    private int m_characterPrice = 100;
    private int m_characterPriority = 10;
    private GameObject m_characterMesh;
    private Sprite m_characterIcon;
    private Texture2D m_characterTexture;
    private Material m_characterMaterial;
    private AnimatorController m_characterAnimator;
    private Avatar m_characterAvatar;

    private string m_characterMeshGUID = string.Empty;
    private GameObject m_characterPrefab = null;
    private CharacterEntry m_selectedCharacter = null;

    void OnGUI () 
    {
        m_characterName = EditorGUILayout.TextField(m_characterName);
        int.TryParse(EditorGUILayout.TextField(m_characterPrice.ToString()), out m_characterPrice);
        int.TryParse(EditorGUILayout.TextField(m_characterPriority.ToString()), out m_characterPriority);

        GUILayout.BeginHorizontal();
        m_characterIcon = ToolsUtils.SpriteField("Icon", m_characterIcon);
        m_characterTexture = ToolsUtils.TextureField("Texture", m_characterTexture);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        m_characterMesh = ToolsUtils.GameObjectField("FBX", m_characterMesh);
        //Idea : Check if the material can be get from the FBX 
        m_characterMaterial = ToolsUtils.MaterialField("Material", m_characterMaterial);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        m_characterAnimator = ToolsUtils.AnimatorControllerField("Controller", m_characterAnimator);
        m_characterAvatar = ToolsUtils.AvatarField("Avatar", m_characterAvatar);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (m_selectedCharacter == null)
        {
            if (GUILayout.Button("Create New"))
            {
                ChangeTextureType(m_characterTexture, TextureImporterType.Sprite);
                m_characterPrefab = GenerateCharacterPrefab();
                CharacterDB.AddCharacter(m_characterName, m_characterPrice, m_characterPrefab, m_characterPriority, m_characterIcon, m_characterMaterial, m_characterAvatar, m_characterAnimator);
            }

            if (GUILayout.Button("Load Character"))
                EditorGUIUtility.ShowObjectPicker<CharacterEntry>(null, false, "", 1);

            if (Event.current.commandName == "ObjectSelectorUpdated")
                if (EditorGUIUtility.GetObjectPickerControlID() == 1)
                {
                    m_selectedCharacter = (CharacterEntry) EditorGUIUtility.GetObjectPickerObject();
                    UpdateCharacterValues();
                }
        }
        else
        {
            if (GUILayout.Button("Update Existing"))
                CharacterDB.UpdateCharacter(m_characterName, m_characterPrice, m_characterPrefab, m_characterPriority, m_characterIcon, m_characterMaterial, m_characterAvatar, m_characterAnimator);

            if (GUILayout.Button("Unload Character"))
            {
                m_selectedCharacter = null;
                m_characterName = string.Empty;
            }
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Clear DB"))
            CharacterDB.CheckCharacters();
    }

    private void UpdateCharacterValues()
    {
        m_characterName = m_selectedCharacter.Name;
        m_characterPrice = m_selectedCharacter.Price;
        m_characterPriority = m_selectedCharacter.ShopPriority;
        m_characterPrefab = m_selectedCharacter.Prefab;
        m_characterIcon = m_selectedCharacter.Icon;
        m_characterMaterial =  m_selectedCharacter.Material;
        m_characterAvatar = m_selectedCharacter.Avatar;
        m_characterAnimator = m_selectedCharacter.Animator;
    }

    private GameObject GenerateCharacterPrefab()
    {
        if (m_characterIcon == null)
            m_characterIcon = CreateCharacterIcon();

        string meshPath = AssetDatabase.GetAssetPath(m_characterMesh);
        string prefabPath = $"{ToolsPaths.CHARACTER_PREFAB_PATH}/{m_characterName}.prefab";


        GameObject characterGameObject = Instantiate(m_characterMesh);
        Animator characterAnimator = ToolsUtils.GetOrAddAnimator(characterGameObject);
        characterAnimator.runtimeAnimatorController = m_characterAnimator;
        characterAnimator.avatar = m_characterAvatar;
        AddAndFitColliderToMesh(characterGameObject);

        SetMaterialAndTexture(characterGameObject, m_characterMaterial, m_characterTexture);
        GameObject characterPrefab = PrefabUtility.SaveAsPrefabAsset(characterGameObject, prefabPath);
        DestroyImmediate(characterGameObject);
        return characterPrefab;
    }

    private Sprite CreateCharacterIcon()
    {
        //Load a scene
        //Import Prefab in the scene
        //Take camera screenshot
        return null;
    }

    private void ChangeTextureType(Texture2D texture, TextureImporterType type)
    {
        string path = AssetDatabase.GetAssetPath(texture);
        TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(path);
        importer.textureType = type;
        importer.SaveAndReimport();
    }

    private void SetMaterialAndTexture(GameObject character, Material material, Texture2D texture)
    {
        SkinnedMeshRenderer meshRenderer = character.GetComponentInChildren<SkinnedMeshRenderer>();
        for (int i = 0; i < meshRenderer.materials.Length; i++)
            meshRenderer.materials[i] = m_characterMaterial;
    }

    private void AddAndFitColliderToMesh(GameObject character)
    {
        CapsuleCollider collider = character.AddComponent<CapsuleCollider>();
        SkinnedMeshRenderer meshRenderer = character.GetComponentInChildren<SkinnedMeshRenderer>();

        collider.center = meshRenderer.bounds.center;
        collider.radius = meshRenderer.bounds.size.x / 2;
        collider.height = meshRenderer.bounds.size.y;
    }
}
